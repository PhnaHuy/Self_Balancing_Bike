// =============================================================================
// actuators.h — Module M6: Actuator & Power Drive Module
// Điều khiển: Nidec 24H055M020 (bánh đà), L298N + motor giảm tốc (bánh sau), Servo (lái)
// =============================================================================
#pragma once
#include <Arduino.h>

// Khởi tạo tất cả các chân điều khiển cơ cấu chấp hành
// Phải gọi trong setup() TRƯỚC khi cấp nguồn motor
void actuators_init();

// --- Nidec 24H055M020 (Bánh đà / Reaction Wheel) ---
// Điều khiển bằng tín hiệu PWM + DIR + BRAKE
// Logic Nidec 24H: PWM 255 = DỪNG, PWM 0 = Full Speed
//   Vì vậy: setSpeed âm = quay chiều thuận, dương = quay chiều ngược
// targetSpeed: [-NIDEC_PWM_MAX_OUT, +NIDEC_PWM_MAX_OUT]
void nidec_setSpeed(int16_t targetSpeed);

// Hãm phanh điện từ tức thì (BRAKE=HIGH, PWM=255)
// Dùng khi fail-safe hoặc dừng khẩn cấp
void nidec_brake();

// Nhả phanh, motor quay tự do (BRAKE=LOW, PWM=255)
void nidec_coast();

// Lấy giá trị PWM thực đang xuất ra (0–255, ánh xạ từ targetSpeed)
int16_t nidec_getLastPwm();

// --- L298N (Bánh sau / Drive Wheel) ---
// speed: [-L298N_MAX_DUTY, +L298N_MAX_DUTY], 0 = dừng
void l298n_setSpeed(int16_t speed);

// --- Servo (Bánh trước / Steering) ---
// angle: góc tuyệt đối [SERVO_MIN_DEG, SERVO_MAX_DEG]
// trimDeg: bù lệch tâm cơ học (cộng thêm vào angle trung tâm)
void servo_setAngle(uint8_t angle);

// Khởi tạo Servo và trả về vị trí trung tâm
void servo_center();

// Dừng toàn bộ cơ cấu chấp hành (fail-safe)
void actuators_stopAll();
