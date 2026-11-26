// =============================================================================
// test_nidec/main.cpp — PFM test for Nidec 24H055M020
//
// SAFETY:
//   1. Secure the wheel on a stand and keep clear of rotating parts.
//   2. Verify 12V supply polarity and the DIR/BRAKE/PFM wiring.
//   3. Startup holds BRAKE=HIGH; test speed is limited to 1500 RPM.
//
// PFM: fixed 50% duty, frequency controls speed (RPM ≈ 0.15 * Hz).
// No FG/HU/HV/HW feedback is used.
//
// Serial commands (9600 baud, newline):
//   FW:150 / BW:300  -> forward/reverse, RPM
//   BRAKE             -> electromagnetic brake
//   COAST             -> release brake, no PFM pulses
//   STOP              -> stop pulses without braking
//   RAMP              -> gradual RPM test
//   STATUS            -> print command frequency and estimated RPM
// =============================================================================
#include <Arduino.h>
#include "../shared/nidec_pfm.h"

#define RAMP_STEP_RPM         150
#define RAMP_STEP_MS          100UL
#define DIRECTION_DEADTIME_MS 5UL

static int16_t g_currentRpm = 0;
static int16_t g_currentFrequencyHz = 0;
static bool g_directionForward = true;
static bool g_braking = true;
static char g_rxBuf[32];
static uint8_t g_rxIdx = 0;

static void nidecSetRpm(int16_t rpm);
static void nidecBrake();
static void nidecCoast();
static void runRampTest();
static void printStatus();
static void processCmd(char* cmd);
static void printOutput(const __FlashStringHelper* prefix);

void setup() {
    Serial.begin(9600);
    pinMode(PIN_NIDEC_DIR, OUTPUT);
    pinMode(PIN_NIDEC_BRAKE, OUTPUT);
    nidec_pfm_init();
    digitalWrite(PIN_NIDEC_DIR, HIGH);
    nidecBrake();

    Serial.println(F("NIDEC 24H PFM TEST — SECURE WHEEL ON STAND"));
    Serial.println(F("PFM 50% duty; FW:<rpm> | BW:<rpm> | BRAKE | COAST | STOP | RAMP | STATUS"));
    Serial.print(F("MAX_RPM: "));
    Serial.println(NIDEC_MAX_RPM);
}

void loop() {
    while (Serial.available() > 0) {
        char c = (char)Serial.read();
        if (c == '\r') continue;
        if (c == '\n') {
            g_rxBuf[g_rxIdx] = '\0';
            if (g_rxIdx > 0) processCmd(g_rxBuf);
            g_rxIdx = 0;
        } else if (g_rxIdx < sizeof(g_rxBuf) - 1) {
            g_rxBuf[g_rxIdx++] = c;
        } else {
            g_rxIdx = 0;
        }
    }
}

static void nidecSetRpm(int16_t rpm) {
    rpm = (int16_t)constrain(rpm, -NIDEC_MAX_RPM, NIDEC_MAX_RPM);
    if (rpm == 0) {
        nidec_pfm_stop();
        digitalWrite(PIN_NIDEC_BRAKE, LOW);
        g_currentRpm = 0;
        g_currentFrequencyHz = 0;
        g_braking = false;
        return;
    }

    bool requestedDirectionForward = rpm > 0;
    nidec_pfm_stop();
    digitalWrite(PIN_NIDEC_BRAKE, LOW);
    if (requestedDirectionForward != g_directionForward) {
        digitalWrite(PIN_NIDEC_DIR, requestedDirectionForward ? HIGH : LOW);
        g_directionForward = requestedDirectionForward;
        delay(DIRECTION_DEADTIME_MS);
    }

    uint16_t requestedFrequencyHz = ((uint32_t)abs(rpm) * NIDEC_MAX_FREQUENCY_HZ
                                   + NIDEC_MAX_RPM / 2)
                                  / NIDEC_MAX_RPM;
    uint16_t actualFrequencyHz = nidec_pfm_setFrequency(requestedFrequencyHz);
    g_currentRpm = rpm;
    g_currentFrequencyHz = requestedDirectionForward
        ? (int16_t)actualFrequencyHz
        : -(int16_t)actualFrequencyHz;
    g_braking = false;
    printOutput(F("NIDEC: "));
}

static void nidecBrake() {
    nidec_pfm_stop();
    digitalWrite(PIN_NIDEC_BRAKE, HIGH);
    g_currentRpm = 0;
    g_currentFrequencyHz = 0;
    g_braking = true;
    Serial.println(F("NIDEC: BRAKE ON (PFM off)"));
}

static void nidecCoast() {
    nidec_pfm_stop();
    digitalWrite(PIN_NIDEC_BRAKE, LOW);
    g_currentRpm = 0;
    g_currentFrequencyHz = 0;
    g_braking = false;
    Serial.println(F("NIDEC: COAST (PFM off)"));
}

static void runRampTest() {
    Serial.println(F("RAMP: 0 -> 1500 -> 0 RPM, then reverse"));
    for (int16_t rpm = 0; rpm <= NIDEC_MAX_RPM; rpm += RAMP_STEP_RPM) {
        nidecSetRpm(rpm);
        delay(RAMP_STEP_MS);
    }
    for (int16_t rpm = NIDEC_MAX_RPM; rpm >= 0; rpm -= RAMP_STEP_RPM) {
        nidecSetRpm(rpm);
        delay(RAMP_STEP_MS);
    }
    for (int16_t rpm = 0; rpm >= -NIDEC_MAX_RPM; rpm -= RAMP_STEP_RPM) {
        nidecSetRpm(rpm);
        delay(RAMP_STEP_MS);
    }
    for (int16_t rpm = -NIDEC_MAX_RPM; rpm <= 0; rpm += RAMP_STEP_RPM) {
        nidecSetRpm(rpm);
        delay(RAMP_STEP_MS);
    }
    nidecBrake();
    Serial.println(F("RAMP DONE — BRAKE ON"));
}

static void printOutput(const __FlashStringHelper* prefix) {
    Serial.print(prefix);
    Serial.print(F("rpm="));
    Serial.print(g_currentRpm);
    Serial.print(F(" freq="));
    Serial.print(g_currentFrequencyHz);
    Serial.println(F("Hz duty=50%"));
}

static void printStatus() {
    Serial.print(F("STATUS: rpm="));
    Serial.print(g_currentRpm);
    Serial.print(F(" freq="));
    Serial.print(g_currentFrequencyHz);
    Serial.print(F("Hz brake="));
    Serial.print(g_braking ? F("ON") : F("OFF"));
    Serial.print(F(" dir="));
    Serial.println(g_directionForward ? F("FW") : F("BW"));
}

static void processCmd(char* cmd) {
    if (strncmp(cmd, "FW:", 3) == 0) {
        int16_t rpm = (int16_t)constrain(atoi(cmd + 3), 0, NIDEC_MAX_RPM);
        nidecSetRpm(rpm);
    } else if (strncmp(cmd, "BW:", 3) == 0) {
        int16_t rpm = (int16_t)constrain(atoi(cmd + 3), 0, NIDEC_MAX_RPM);
        nidecSetRpm(-rpm);
    } else if (strcmp(cmd, "BRAKE") == 0) {
        nidecBrake();
    } else if (strcmp(cmd, "COAST") == 0) {
        nidecCoast();
    } else if (strcmp(cmd, "STOP") == 0) {
        nidecSetRpm(0);
        Serial.println(F("NIDEC: STOP (PFM off, no brake)"));
    } else if (strcmp(cmd, "RAMP") == 0) {
        runRampTest();
    } else if (strcmp(cmd, "STATUS") == 0) {
        printStatus();
    } else {
        Serial.print(F("UNKNOWN CMD: "));
        Serial.println(cmd);
    }
}
