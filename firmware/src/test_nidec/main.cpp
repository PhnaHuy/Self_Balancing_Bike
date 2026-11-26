// =============================================================================
// test_nidec/main.cpp — Test động cơ bánh đà Nidec 24H055M020
//
// QUAN TRỌNG — AN TOÀN TRƯỚC KHI CHẠY:
//   1. ĐẶT XE TRÊN GIÁ ĐỠ — bánh đà KHÔNG được chạm đất hoặc người!
//   2. Kiểm tra cấp nguồn 12V (3S 11.1–12.6V) đúng cực cho Nidec trước khi thử.
//   3. Dây DIR, BRAKE, PWM phải đúng như config.h (D4, D5, D6).
//   4. Khởi động bắt đầu ở BRAKE=ON để tránh bánh đà quay bất ngờ.
//   5. KHÔNG để tay hoặc dị vật gần bánh đà khi đang cấp nguồn.
//
// Logic PWM Nidec 24H055M020:
//   - BRAKE = HIGH → hãm phanh điện từ (phải BRAKE=LOW trước khi quay)
//   - BRAKE = LOW  → nhả phanh, motor sẵn sàng nhận PWM
//   - PWM = 255    → DỪNG (không xung → không tốc độ)
//   - PWM = 0      → Full speed (tốc độ tối đa)
//   - DIR = HIGH   → chiều quay A
//   - DIR = LOW    → chiều quay B (đảo chiều)
//
// Cách dùng (Serial Monitor 9600 baud, Newline):
//   "FW:50"   → quay tới, tốc độ 50 (PWM thực = 255-50 = 205)
//   "FW:150"  → quay tới, tốc độ 150 (PWM thực = 105)
//   "BW:100"  → quay lui, tốc độ 100
//   "BRAKE"   → hãm phanh điện từ
//   "COAST"   → nhả phanh, coast
//   "STOP"    → dừng nhẹ nhàng (PWM=255, BRAKE=LOW)
//   "RAMP"    → chạy ramp test tự động từ 0 đến max và ngược lại
//   "STATUS"  → in trạng thái hiện tại
// =============================================================================
#include <Arduino.h>

// Chân GPIO theo đúng config.h
#define PIN_NIDEC_FG        2
#define PIN_NIDEC_DIR       4
#define PIN_NIDEC_BRAKE     5
#define PIN_NIDEC_PWM       6

// Giới hạn an toàn khi test
#define TEST_MAX_SPEED      180   // Không vượt quá 180/255 khi test đơn thuần
#define RAMP_STEP_MS        30UL  // Mỗi bước ramp chờ 30ms

// Trạng thái nội bộ
static int16_t g_currentSpeed = 0;   // -255 đến +255 (âm = chiều ngược)
static bool    g_braking      = true;
static char    g_rxBuf[32];
static uint8_t g_rxIdx = 0;

static void nidecSetSpeed(int16_t speed);
static void nidecBrake();
static void nidecCoast();
static void runRampTest();
static void printStatus();
static void processCmd(char* cmd);

void setup() {
    Serial.begin(9600);

    pinMode(PIN_NIDEC_DIR,   OUTPUT);
    pinMode(PIN_NIDEC_BRAKE, OUTPUT);
    pinMode(PIN_NIDEC_PWM,   OUTPUT);
    // Trạng thái an toàn ban đầu: hãm phanh, dừng
    nidecBrake();

    Serial.println(F("NIDEC 24H TEST — ENSURE WHEEL ON STAND!"));
    Serial.println(F("CMD: FW:<0-180> | BW:<0-180> | BRAKE | COAST | STOP | RAMP | STATUS"));
    Serial.print(F("MAX_SPEED_LIMIT: "));
    Serial.println(TEST_MAX_SPEED);
}

void loop() {
    // Nhận lệnh từ Serial Monitor
    while (Serial.available() > 0) {
        char c = (char)Serial.read();
        if (c == '\r') continue;
        if (c == '\n') {
            g_rxBuf[g_rxIdx] = '\0';
            if (g_rxIdx > 0) processCmd(g_rxBuf);
            g_rxIdx = 0;
        } else if (g_rxIdx < 30) {
            g_rxBuf[g_rxIdx++] = c;
        } else {
            g_rxIdx = 0;
        }
    }
}

// speed: -TEST_MAX_SPEED đến +TEST_MAX_SPEED
// Dương = chiều DIR HIGH, Âm = chiều DIR LOW
static void nidecSetSpeed(int16_t speed) {
    speed = (int16_t)constrain(speed, -TEST_MAX_SPEED, TEST_MAX_SPEED);

    // Nhả phanh trước khi đặt tốc độ
    digitalWrite(PIN_NIDEC_BRAKE, LOW);
    g_braking = false;

    if (speed == 0) {
        analogWrite(PIN_NIDEC_PWM, 255);  // PWM=255 → dừng (Nidec nghịch)
        g_currentSpeed = 0;
        return;
    }

    if (speed > 0) {
        digitalWrite(PIN_NIDEC_DIR, HIGH);
    } else {
        digitalWrite(PIN_NIDEC_DIR, LOW);
    }
    uint8_t pwmVal = (uint8_t)(255 - constrain(abs(speed), 0, TEST_MAX_SPEED));
    analogWrite(PIN_NIDEC_PWM, pwmVal);
    g_currentSpeed = speed;

    Serial.print(F("NIDEC: speed="));
    Serial.print(speed);
    Serial.print(F(" dir="));
    Serial.print(speed > 0 ? F("FW") : F("BW"));
    Serial.print(F(" pwm_raw="));
    Serial.println(pwmVal);
}

static void nidecBrake() {
    digitalWrite(PIN_NIDEC_BRAKE, HIGH);
    analogWrite(PIN_NIDEC_PWM, 255);
    g_currentSpeed = 0;
    g_braking      = true;
    Serial.println(F("NIDEC: BRAKE ON (electromagnetic hold)"));
}

static void nidecCoast() {
    digitalWrite(PIN_NIDEC_BRAKE, LOW);
    analogWrite(PIN_NIDEC_PWM, 255);
    g_currentSpeed = 0;
    g_braking      = false;
    Serial.println(F("NIDEC: COAST (free spinning)"));
}

// Ramp test tự động: tăng dần từ 0 đến max, giảm về 0, đảo chiều, lặp lại
static void runRampTest() {
    Serial.println(F("RAMP TEST: 0 → MAX → 0 → -MAX → 0 (FW then BW)"));

    // Tiến
    for (int16_t s = 0; s <= TEST_MAX_SPEED; s += 10) {
        nidecSetSpeed(s);
        delay(RAMP_STEP_MS);
    }
    for (int16_t s = TEST_MAX_SPEED; s >= 0; s -= 10) {
        nidecSetSpeed(s);
        delay(RAMP_STEP_MS);
    }
    // Lùi
    for (int16_t s = 0; s >= -TEST_MAX_SPEED; s -= 10) {
        nidecSetSpeed(s);
        delay(RAMP_STEP_MS);
    }
    for (int16_t s = -TEST_MAX_SPEED; s <= 0; s += 10) {
        nidecSetSpeed(s);
        delay(RAMP_STEP_MS);
    }
    nidecBrake();
    Serial.println(F("RAMP TEST DONE — BRAKE ON"));
}

static void printStatus() {
    Serial.print(F("STATUS: speed="));
    Serial.print(g_currentSpeed);
    Serial.print(F(" brake="));
    Serial.print(g_braking ? F("ON") : F("OFF"));
    Serial.print(F(" dir="));
    Serial.println(digitalRead(PIN_NIDEC_DIR) ? F("FW") : F("BW"));
}

static void processCmd(char* cmd) {
    if (strncmp(cmd, "FW:", 3) == 0) {
        int16_t spd = (int16_t)atoi(cmd + 3);
        nidecSetSpeed(constrain(spd, 0, TEST_MAX_SPEED));
    } else if (strncmp(cmd, "BW:", 3) == 0) {
        int16_t spd = (int16_t)atoi(cmd + 3);
        nidecSetSpeed(-constrain(spd, 0, TEST_MAX_SPEED));
    } else if (strcmp(cmd, "BRAKE") == 0) {
        nidecBrake();
    } else if (strcmp(cmd, "COAST") == 0) {
        nidecCoast();
    } else if (strcmp(cmd, "STOP") == 0) {
        nidecSetSpeed(0);
        Serial.println(F("NIDEC: STOP (soft, no brake)"));
    } else if (strcmp(cmd, "RAMP") == 0) {
        runRampTest();
    } else if (strcmp(cmd, "STATUS") == 0) {
        printStatus();
    } else {
        Serial.print(F("UNKNOWN CMD: "));
        Serial.println(cmd);
    }
}
