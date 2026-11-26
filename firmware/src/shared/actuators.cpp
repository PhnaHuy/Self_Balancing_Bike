// =============================================================================
// actuators.cpp — Module M6: Actuator & Power Drive Module
// =============================================================================
#include "actuators.h"
#include "config.h"
#include <Servo.h>

static Servo   g_servo;
static int16_t g_nidecLastPwm = NIDEC_PWM_STOP;

void actuators_init() {
    // --- Nidec 24H055M020 ---
    pinMode(PIN_NIDEC_DIR,   OUTPUT);
    pinMode(PIN_NIDEC_BRAKE, OUTPUT);
    pinMode(PIN_NIDEC_PWM,   OUTPUT);

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
// Logic PWM nghịch: giá trị PWM nhỏ → tốc độ cao; 255 → dừng
// DIR HIGH = quay một chiều, LOW = chiều ngược
// BRAKE HIGH = hãm điện từ, BRAKE LOW = nhả
// =============================================================================
void nidec_setSpeed(int16_t targetSpeed) {
    // Clamp đầu vào
    if      (targetSpeed >  (int16_t)NIDEC_PWM_MAX_OUT) targetSpeed =  (int16_t)NIDEC_PWM_MAX_OUT;
    else if (targetSpeed < -(int16_t)NIDEC_PWM_MAX_OUT) targetSpeed = -(int16_t)NIDEC_PWM_MAX_OUT;

    // Nhả phanh để motor quay
    digitalWrite(PIN_NIDEC_BRAKE, LOW);

    if (targetSpeed == 0) {
        // Dừng: PWM = STOP (255 = không xung → motor tự giữ theo đặc tính Nidec)
        analogWrite(PIN_NIDEC_PWM, NIDEC_PWM_STOP);
        g_nidecLastPwm = NIDEC_PWM_STOP;
        return;
    }

    // Ánh xạ |targetSpeed| → PWM thực tế (nghịch chiều)
    // targetSpeed = NIDEC_PWM_MAX_OUT → PWM thực = 255 - NIDEC_PWM_MAX_OUT (gần 0 = full speed)
    uint8_t pwmValue = (uint8_t)(NIDEC_PWM_STOP - constrain(abs(targetSpeed), 0, NIDEC_PWM_MAX_OUT));

    if (targetSpeed > 0) {
        digitalWrite(PIN_NIDEC_DIR, HIGH);
    } else {
        digitalWrite(PIN_NIDEC_DIR, LOW);
    }

    analogWrite(PIN_NIDEC_PWM, pwmValue);
    g_nidecLastPwm = pwmValue;
}

void nidec_brake() {
    // Hãm phanh điện từ tức thì (BRAKE = HIGH)
    // Áp dụng khi fail-safe hoặc dừng khẩn cấp — không để coast khi xe đang nghiêng
    digitalWrite(PIN_NIDEC_BRAKE, HIGH);
    analogWrite(PIN_NIDEC_PWM, NIDEC_PWM_STOP);
    g_nidecLastPwm = NIDEC_PWM_STOP;
}

void nidec_coast() {
    // Nhả phanh, motor quay tự do — CHỈ dùng khi tắt nguồn an toàn
    digitalWrite(PIN_NIDEC_BRAKE, LOW);
    analogWrite(PIN_NIDEC_PWM, NIDEC_PWM_STOP);
    g_nidecLastPwm = NIDEC_PWM_STOP;
}

int16_t nidec_getLastPwm() {
    return g_nidecLastPwm;
}

// =============================================================================
// L298N — Bánh sau
// ENA = PWM tốc độ, IN1/IN2 = chiều quay
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
