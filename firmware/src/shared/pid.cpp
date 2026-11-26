// =============================================================================
// pid.cpp — Module M5: Balance Control & Safety
// D-term dùng gyroRate trực tiếp (không vi phân measurement) → tránh nhiễu giật cục
// Anti-Windup: clamp integral trong khoảng [-INTEGRAL_CLAMP, +INTEGRAL_CLAMP]
// =============================================================================
#include "pid.h"
#include "config.h"

static PidParams g_params = { PID_DEFAULT_KP, PID_DEFAULT_KI, PID_DEFAULT_KD };
static PidState  g_state  = { 0.0f, 0.0f, 0.0f };

void pid_init(PidParams params) {
    g_params = params;
    pid_reset();
}

void pid_setParams(PidParams params) {
    // Chỉ cập nhật tham số, không reset tích phân (tránh giật khi tune nhanh)
    g_params = params;
}

PidParams pid_getParams() {
    return g_params;
}

float pid_compute(float setpoint, float measurement, float gyroRate, float dt_s) {
    // Tính sai số
    float error = setpoint - measurement;

    // P-term
    float p = g_params.kp * error;

    // I-term: tích phân có anti-windup
    g_state.integral += error * dt_s;
    // Clamp tích phân: ngăn bão hòa khi xe nghiêng lâu hoặc bị giữ
    if      (g_state.integral >  INTEGRAL_CLAMP) g_state.integral =  INTEGRAL_CLAMP;
    else if (g_state.integral < -INTEGRAL_CLAMP) g_state.integral = -INTEGRAL_CLAMP;
    float i = g_params.ki * g_state.integral;

    // D-term: dùng gyroRate đo trực tiếp từ IMU thay vì vi phân error
    // Lý do: gyroRate đã được DLPF lọc sẵn, còn d(error)/dt sẽ bị khuếch đại nhiễu từ bánh đà
    float d = g_params.kd * (-gyroRate);  // Âm vì chiều quay gyro ngược chiều hiệu chỉnh

    float output = p + i + d;

    // Clamp demand sent to the Nidec frequency controller.
    if      (output >  (float)NIDEC_CONTROL_OUTPUT_MAX) output =  (float)NIDEC_CONTROL_OUTPUT_MAX;
    else if (output < -(float)NIDEC_CONTROL_OUTPUT_MAX) output = -(float)NIDEC_CONTROL_OUTPUT_MAX;

    g_state.output   = output;
    g_state.prevError = error;
    return output;
}

void pid_reset() {
    g_state.integral  = 0.0f;
    g_state.prevError = 0.0f;
    g_state.output    = 0.0f;
}

PidState pid_getState() {
    return g_state;
}
