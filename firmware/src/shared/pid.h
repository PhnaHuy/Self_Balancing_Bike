// =============================================================================
// pid.h — Module M5: Balance Control & Safety Module
// PID Controller với Anti-Windup và Fail-Safe
// =============================================================================
#pragma once
#include <Arduino.h>

// Tham số PID (có thể cập nhật động từ lệnh P:... qua HC-05)
struct PidParams {
    float kp;
    float ki;
    float kd;
};

// Trạng thái nội bộ của bộ điều khiển PID
struct PidState {
    float integral;    // Tích phân (đã clamp chống windup)
    float prevError;   // Sai số vòng lặp trước (để tính đạo hàm)
    float output;      // Đầu ra tính được lần cuối
};

// Khởi tạo PID với bộ tham số ban đầu
void pid_init(PidParams params);

// Cập nhật tham số PID (nhận từ lệnh P:kp,ki,kd\n)
void pid_setParams(PidParams params);

// Lấy tham số PID hiện tại
PidParams pid_getParams();

// Tính đầu ra PID
// setpoint: góc mục tiêu (thường = 0.0, hiệu chỉnh thêm khi tiến/lùi)
// measurement: góc nghiêng thực tế từ IMU
// gyroRate: vận tốc góc từ IMU (dùng cho D-term trực tiếp, tránh jitter vi phân)
// dt_s: bước thời gian thực tế (giây)
// Trả về: lệnh điều khiển đã clamp [-NIDEC_CONTROL_OUTPUT_MAX, +NIDEC_CONTROL_OUTPUT_MAX]
float pid_compute(float setpoint, float measurement, float gyroRate, float dt_s);

// Reset tích phân và trạng thái (khi fail-safe hoặc khởi động lại)
void pid_reset();

// Lấy trạng thái nội bộ (dùng để gửi telemetry hoặc debug)
PidState pid_getState();
