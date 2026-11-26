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
// Điều khiển bằng PFM 50% duty + DIR + BRAKE.
// controlOutput [-NIDEC_CONTROL_OUTPUT_MAX, +NIDEC_CONTROL_OUTPUT_MAX] ánh xạ sang 0..10kHz.
void nidec_setControlOutput(int16_t controlOutput);

// Hãm phanh điện từ tức thì (BRAKE=HIGH, không phát xung PFM)
// Dùng khi fail-safe hoặc dừng khẩn cấp
void nidec_brake();

// Nhả phanh, motor quay tự do (BRAKE=LOW, không phát xung PFM)
void nidec_coast();

// Lấy tần số PFM có dấu hiện đang yêu cầu (Hz; dấu biểu thị chiều quay)
int16_t nidec_getFrequencyHz();

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
