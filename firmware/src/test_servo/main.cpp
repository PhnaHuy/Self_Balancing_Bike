// =============================================================================
// test_servo/main.cpp — Servo steering test with Controller App support
//
// The firmware accepts the app protocol M:<speed>,<angle>, ignores speed,
// and only moves the servo. No drive or reaction-wheel motor is controlled.
//
// Serial commands (38400 baud, newline):
//   M:0,60 / M:0,120 -> app-compatible left/right steering
//   LEFT / RIGHT / CENTER / ANGLE:<deg> / STATUS
// =============================================================================
#include <Arduino.h>
#include <Servo.h>
#include "../shared/config.h"

#define RX_BUFFER_SIZE 48
#define TEST_TURN_DELTA_DEG 30

static Servo g_servo;
static char g_rxBuffer[RX_BUFFER_SIZE];
static uint8_t g_rxIndex = 0;
static uint8_t g_currentAngle = SERVO_CENTER_DEG;

static void processLine(char* line);
static void setServoAngle(long angle);
static bool parseLong(char* text, long* value);

void setup() {
    Serial.begin(HC05_BAUD_RATE);
    g_servo.attach(PIN_SERVO);
    g_servo.write(g_currentAngle);

    Serial.println(F("SERVO TEST READY"));
    Serial.print(F("BAUD:"));
    Serial.println(HC05_BAUD_RATE);
    Serial.println(F("APP: M:<speed>,<angle> (speed ignored)"));
    Serial.println(F("SERIAL: LEFT | RIGHT | CENTER | ANGLE:<deg> | STATUS"));
}

void loop() {
    while (Serial.available() > 0) {
        char c = (char)Serial.read();
        if (c == '\r') continue;

        if (c == '\n') {
            g_rxBuffer[g_rxIndex] = '\0';
            if (g_rxIndex > 0) {
                processLine(g_rxBuffer);
            }
            g_rxIndex = 0;
        } else if (g_rxIndex < RX_BUFFER_SIZE - 1) {
            g_rxBuffer[g_rxIndex++] = c;
        } else {
            g_rxIndex = 0;
            Serial.println(F("ERROR:COMMAND_TOO_LONG"));
        }
    }
}

static void processLine(char* line) {
    if (strncmp(line, "M:", 2) == 0) {
        char* comma = strchr(line + 2, ',');
        if (comma == nullptr) {
            Serial.println(F("ERROR:EXPECTED_M_SPEED_ANGLE"));
            return;
        }

        *comma = '\0';
        long ignoredSpeed;
        long angle;
        if (!parseLong(line + 2, &ignoredSpeed) || !parseLong(comma + 1, &angle)
            || ignoredSpeed < MOVE_SPEED_MIN || ignoredSpeed > MOVE_SPEED_MAX) {
            Serial.println(F("ERROR:INVALID_M_COMMAND"));
            return;
        }

        setServoAngle(angle);
        return;
    }

    if (strcmp(line, "LEFT") == 0) {
        setServoAngle(SERVO_CENTER_DEG - TEST_TURN_DELTA_DEG);
    } else if (strcmp(line, "RIGHT") == 0) {
        setServoAngle(SERVO_CENTER_DEG + TEST_TURN_DELTA_DEG);
    } else if (strcmp(line, "CENTER") == 0) {
        setServoAngle(SERVO_CENTER_DEG);
    } else if (strncmp(line, "ANGLE:", 6) == 0) {
        long angle;
        if (!parseLong(line + 6, &angle)) {
            Serial.println(F("ERROR:INVALID_ANGLE"));
            return;
        }
        setServoAngle(angle);
    } else if (strcmp(line, "STATUS") == 0) {
        Serial.print(F("STATUS:SERVO_ANGLE="));
        Serial.println(g_currentAngle);
    } else {
        Serial.print(F("ERROR:UNKNOWN_COMMAND:"));
        Serial.println(line);
    }
}

static void setServoAngle(long angle) {
    uint8_t requestedAngle = (uint8_t)constrain(angle, SERVO_MIN_DEG, SERVO_MAX_DEG);
    if (requestedAngle == g_currentAngle) return;

    g_currentAngle = requestedAngle;
    g_servo.write(g_currentAngle);
    Serial.print(F("SERVO:ANGLE="));
    Serial.println(g_currentAngle);
}

static bool parseLong(char* text, long* value) {
    if (text == nullptr || *text == '\0') return false;

    char* end = nullptr;
    long parsed = strtol(text, &end, 10);
    if (end == text || *end != '\0') return false;

    *value = parsed;
    return true;
}
