// =============================================================================
// actuators.cpp — Module M6: Actuator & Power Drive Module
// =============================================================================
#include "actuators.h"
#include "config.h"
#include "nidec_pfm.h"
#include <Servo.h>

static Servo   g_servo;
static int16_t g_nidecFrequencyHz = 0;
static bool    g_nidecDirection = true;
static bool    g_nidecDirectionPending = false;
static uint32_t g_nidecDirectionChangedUs = 0;

void actuators_init() {
    // --- Nidec 24H055M020 ---
    pinMode(PIN_NIDEC_DIR,   OUTPUT);
    pinMode(PIN_NIDEC_BRAKE, OUTPUT);
    nidec_pfm_init();
    digitalWrite(PIN_NIDEC_DIR, HIGH);
    g_nidecDirection = true;

    // Trạng thái an toàn ban đầu: hãm phanh, dừng
    nidec_brake();

    // --- L298N bánh sau ---
    pinMode(PIN_L298N_ENA, OUTPUT);
    pinMode(PIN_L298N_IN1, OUTPUT);
    pinMode(PIN_L298N_IN2, OUTPUT);
    l298n_setSpeed(0);

    // --- Servo lái ---
    // Nguồn 5V từ L7805 riêng biệt; nối GND chung với Arduino là bắt buộc
    g_servo.attach(PIN_SERVO);
    servo_center();
}

// =============================================================================
// Nidec 24H055M020
// Tần số PFM quyết định tốc độ; duty cycle được cố định 50% bởi bộ định thời.
// BRAKE HIGH = hãm điện từ, BRAKE LOW = nhả
// =============================================================================
void nidec_setControlOutput(int16_t controlOutput) {
    controlOutput = (int16_t)constrain(controlOutput, -NIDEC_CONTROL_OUTPUT_MAX, NIDEC_CONTROL_OUTPUT_MAX);

    // Nhả phanh để motor quay
    digitalWrite(PIN_NIDEC_BRAKE, LOW);

    if (controlOutput == 0) {
        nidec_pfm_stop();
        g_nidecFrequencyHz = 0;
        g_nidecDirectionPending = false;
        return;
    }

    bool requestedDirection = controlOutput > 0;
    if (requestedDirection != g_nidecDirection) {
        nidec_pfm_stop();
        g_nidecFrequencyHz = 0;
        digitalWrite(PIN_NIDEC_DIR, requestedDirection ? HIGH : LOW);
        g_nidecDirection = requestedDirection;
        g_nidecDirectionPending = true;
        g_nidecDirectionChangedUs = micros();
        return;
    }

    if (g_nidecDirectionPending) {
        if ((uint32_t)(micros() - g_nidecDirectionChangedUs) < NIDEC_DIRECTION_DEADTIME_US) {
            return;
        }
        g_nidecDirectionPending = false;
    }

    uint16_t frequencyHz = ((uint32_t)abs(controlOutput) * NIDEC_MAX_FREQUENCY_HZ
                          + NIDEC_CONTROL_OUTPUT_MAX / 2)
                         / NIDEC_CONTROL_OUTPUT_MAX;
    uint16_t actualFrequencyHz = nidec_pfm_setFrequency(frequencyHz);
    g_nidecFrequencyHz = requestedDirection ? actualFrequencyHz : -(int16_t)actualFrequencyHz;
}

void nidec_brake() {
    // Hãm phanh điện từ tức thì, đồng thời ngắt xung PFM.
    digitalWrite(PIN_NIDEC_BRAKE, HIGH);
    nidec_pfm_stop();
    g_nidecFrequencyHz = 0;
    g_nidecDirectionPending = false;
}

void nidec_coast() {
    // Nhả phanh và ngắt xung PFM.
    digitalWrite(PIN_NIDEC_BRAKE, LOW);
    nidec_pfm_stop();
    g_nidecFrequencyHz = 0;
    g_nidecDirectionPending = false;
}

int16_t nidec_getFrequencyHz() {
    return g_nidecFrequencyHz;
}

// =============================================================================
// L298N — Bánh sau
// ENA = PWM tốc độ trên Timer0, IN1/IN2 = chiều quay
// Giới hạn duty cycle tối đa để bảo vệ motor ở nguồn 3S 11.1–12.6V
// =============================================================================
void l298n_setSpeed(int16_t speed) {
    // Clamp tốc độ trong giới hạn an toàn
    speed = (int16_t)constrain(speed, -L298N_MAX_DUTY, L298N_MAX_DUTY);

    if (speed == 0) {
        analogWrite(PIN_L298N_ENA, 0);
        digitalWrite(PIN_L298N_IN1, LOW);
        digitalWrite(PIN_L298N_IN2, LOW);
    } else if (speed > 0) {
        // Tiến
        digitalWrite(PIN_L298N_IN1, HIGH);
        digitalWrite(PIN_L298N_IN2, LOW);
        analogWrite(PIN_L298N_ENA, (uint8_t)speed);
    } else {
        // Lùi
        digitalWrite(PIN_L298N_IN1, LOW);
        digitalWrite(PIN_L298N_IN2, HIGH);
        analogWrite(PIN_L298N_ENA, (uint8_t)(-speed));
    }
}

// =============================================================================
// Servo — Bánh trước (Lái)
// Nguồn 5V riêng biệt qua L7805 (kèm tụ 0.1µF + 0.33µF và tản nhiệt).
// Lý do tách nguồn: Servo có dòng khởi động lên đến 500mA–1A,
// nếu dùng chung nguồn 5V với Arduino có thể gây reset vi điều khiển.
// =============================================================================
void servo_setAngle(uint8_t angle) {
    // Kẹp trong giới hạn cơ khí để bảo vệ cơ cấu lái
    uint8_t safeAngle = (uint8_t)constrain(angle, SERVO_MIN_DEG, SERVO_MAX_DEG);
    g_servo.write(safeAngle);
}

void servo_center() {
    servo_setAngle(SERVO_CENTER_DEG);
}

// =============================================================================
// Dừng khẩn cấp — ngắt tất cả cơ cấu chấp hành ngay lập tức
// =============================================================================
void actuators_stopAll() {
    nidec_brake();
    l298n_setSpeed(0);
    servo_center();
}
