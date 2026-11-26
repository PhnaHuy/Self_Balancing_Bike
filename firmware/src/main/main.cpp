// =============================================================================
// main.cpp — Chương trình chính tích hợp đầy đủ M4–M7
// Self-Balancing Bike Firmware — Arduino Uno R3 (ATmega328P)
//
// Vòng lặp điều khiển: 5ms (200Hz) — không có delay() chặn
// Telemetry: 50ms (20Hz) qua HC-05 Bluetooth
// Giao thức: M:speed,angle\n | P:kp,ki,kd\n | CAL\n | SAVE\n | D:angle,gyro,pwm\n
//
// Sơ đồ chân (tham chiếu config.h):
//   MPU6050 : SDA=A4, SCL=A5 (I2C phần cứng)
//   HC-05   : TX→D0(RX), RX→D1(TX) (UART phần cứng)
//   Nidec   : DIR=D4, BRAKE=D5, PWM=D6 (Timer0)
//   L298N   : ENA=D3(PWM/Timer2), IN1=D7, IN2=D8
//   Servo   : D9 (Timer1, thư viện Servo.h)
//   Nidec FG: D2 (INT0, chưa kích hoạt trong phiên bản cơ bản)
// =============================================================================
#include <Arduino.h>
#include "../shared/config.h"
#include "../shared/imu.h"
#include "../shared/pid.h"
#include "../shared/actuators.h"
#include "../shared/comm.h"
#include "../shared/eeprom_store.h"

// --- Biến trạng thái thời gian ---
static uint32_t g_lastPidUs    = 0;   // Mốc thời gian vòng PID cuối (microseconds)
static uint32_t g_lastTeleMs   = 0;   // Mốc thời gian gửi telemetry cuối (milliseconds)

// --- Trạng thái hệ thống ---
static bool     g_failsafeActive = false;   // Cờ Fail-Safe (góc vượt ngưỡng hoặc mất tín hiệu)
static int16_t  g_moveSpeed      = 0;       // Tốc độ bánh sau yêu cầu từ lệnh M:
static uint8_t  g_steerAngle     = SERVO_CENTER_DEG; // Góc lái yêu cầu từ lệnh M:

// Tiến hành calibration và phản hồi ack
static void doCalibration();
// Tiến hành lưu EEPROM và phản hồi ack
static void doSaveEeprom();
// Kiểm tra và xử lý tham số PID mới
static void applyNewPidIfAny();
// Kiểm tra watchdog mất kết nối
static void checkCommandWatchdog(uint32_t nowMs);
// Tính setpoint điều chỉnh khi xe di chuyển (lean-forward control)
static float computeSetpoint(int16_t moveSpeed);

// =============================================================================
void setup() {
    // --- Khởi tạo cơ cấu chấp hành trước (trạng thái dừng an toàn) ---
    actuators_init();

    // --- Khởi tạo giao tiếp Serial / HC-05 ---
    comm_init();

    // --- Khởi tạo MPU6050 ---
    if (!imu_init()) {
        // Không tìm thấy cảm biến → báo lỗi lên app và vòng lặp vô hạn
        // (không được chạy PID mù khi không có dữ liệu góc)
        comm_sendAck("ERROR:MPU6050_NOT_FOUND");
        while (true) {
            // Giữ trạng thái dừng an toàn vĩnh viễn
            actuators_stopAll();
            delay(1000);
        }
    }

    // --- Nạp cấu hình từ EEPROM ---
    EepromConfig cfg;
    if (eeprom_load(&cfg)) {
        pid_init(cfg.pid);
        imu_setOffset(cfg.angleOffset);
        imu_setGyroBias(cfg.gyroBiasDps);
        comm_sendAck("OK:EEPROM_LOADED");
    } else {
        // Dùng thông số mặc định từ config.h
        PidParams defaults = {PID_DEFAULT_KP, PID_DEFAULT_KI, PID_DEFAULT_KD};
        pid_init(defaults);
        comm_sendAck("OK:DEFAULT_PARAMS");
    }

    g_lastPidUs  = micros();
    g_lastTeleMs = millis();
    comm_sendAck("OK:READY");
}

// =============================================================================
void loop() {
    // --- 1. Nhận và phân tích lệnh Serial (không chặn) ---
    comm_tick();

    // --- 2. Áp dụng tham số PID mới (nếu có lệnh P:) ---
    applyNewPidIfAny();

    // --- 3. Xử lý lệnh CAL ---
    if (comm_popCalFlag()) {
        doCalibration();
    }

    // --- 4. Xử lý lệnh SAVE ---
    if (comm_popSaveFlag()) {
        doSaveEeprom();
    }

    // --- 5. Nhận lệnh di chuyển M: ---
    MoveCommand mc = comm_getMoveCommand();
    if (mc.fresh) {
        g_moveSpeed  = mc.speed;
        g_steerAngle = mc.angle;
        comm_clearMoveCommand();
    }

    // Cập nhật mốc thời gian sau các lệnh CAL/EEPROM có thể chặn vòng lặp.
    uint32_t nowUs = micros();
    uint32_t nowMs = millis();

    // --- 6. Áp dụng góc lái (Servo) ngay khi có lệnh M: (không phụ thuộc chu kỳ PID) ---
    servo_setAngle(g_steerAngle);

    // --- 7. Kiểm tra watchdog mất tín hiệu điều khiển ---
    checkCommandWatchdog(nowMs);

    // --- 8. Vòng lặp PID cân bằng — chỉ chạy đúng chu kỳ 5ms ---
    uint32_t elapsedUs = nowUs - g_lastPidUs;
    if (elapsedUs >= PID_LOOP_INTERVAL_US) {
        g_lastPidUs = nowUs;
        float dt_s  = (float)elapsedUs / 1e6f;

        // 8a. Cập nhật ước lượng góc nghiêng (Complementary Filter)
        imu_update(elapsedUs);
        ImuData imu = imu_getData();

        // 8b. Kiểm tra Fail-Safe: góc nghiêng vượt ngưỡng ±35°
        if (fabsf(imu.angle) > TILT_FAILSAFE_DEG) {
            if (!g_failsafeActive) {
                g_failsafeActive = true;
                pid_reset();
                actuators_stopAll();
                comm_sendAck("WARN:TILT_FAILSAFE");
            }
        }

        // Phục hồi sau fail-safe khi góc quay về < 20° (trễ để tránh dao động ON/OFF)
        if (g_failsafeActive && fabsf(imu.angle) < 20.0f) {
            g_failsafeActive = false;
            pid_reset();
            comm_sendAck("OK:FAILSAFE_CLEARED");
        }

        if (!g_failsafeActive) {
            // 8c. Tính setpoint: lean-forward khi yêu cầu di chuyển
            float setpoint = computeSetpoint(g_moveSpeed);

            // 8d. Tính đầu ra PID
            float pidOut = pid_compute(setpoint, imu.angle, imu.gyroRate, dt_s);

            // 8e. Đặt tốc độ bánh đà theo đầu ra PID
            nidec_setSpeed((int16_t)pidOut);

            // 8f. Điều khiển bánh sau theo lệnh di chuyển
            l298n_setSpeed(g_moveSpeed);
        }
    }

    // --- 9. Gửi telemetry mỗi 50ms ---
    if ((nowMs - g_lastTeleMs) >= TELEMETRY_INTERVAL_MS) {
        g_lastTeleMs = nowMs;
        ImuData imu  = imu_getData();
        comm_sendTelemetry(imu.angle, imu.gyroRate, nidec_getLastPwm());
    }
}

// =============================================================================
// Các hàm hỗ trợ
// =============================================================================
static void doCalibration() {
    // Tạm dừng PID và cơ cấu chấp hành trong thời gian lấy mẫu
    actuators_stopAll();
    comm_sendAck("CAL:SAMPLING");

    // Lấy 50 mẫu (~250ms) để ước lượng offset góc và gyro bias
    imu_calibrate(50);
    comm_sendAck("OK:CAL_DONE");

    pid_reset();
    g_failsafeActive = false;
}

static void doSaveEeprom() {
    PidParams p   = pid_getParams();
    float     off = imu_getOffset();
    float     gyroBias = imu_getGyroBias();
    EepromConfig cfg = { p, off, gyroBias };
    eeprom_save(cfg);
    comm_sendAck("OK:SAVE_DONE");
}

static void applyNewPidIfAny() {
    PidParams newPid;
    if (comm_popPidParams(&newPid)) {
        pid_setParams(newPid);
        comm_sendAck("OK:PID_UPDATED");
    }
}

static void checkCommandWatchdog(uint32_t nowMs) {
    // Nếu không nhận lệnh M: quá CMD_WATCHDOG_MS ms → dừng bánh sau
    // Bánh đà vẫn chạy PID để giữ cân bằng
    if ((nowMs - comm_getLastCommandMs()) > CMD_WATCHDOG_MS) {
        if (g_moveSpeed != 0) {
            g_moveSpeed  = 0;
            g_steerAngle = SERVO_CENTER_DEG;
            l298n_setSpeed(0);
            servo_setAngle(SERVO_CENTER_DEG);
        }
    }
}

// Điều chỉnh setpoint khi di chuyển: nghiêng nhẹ về trước (tiến) / sau (lùi)
// để tạo lực đẩy bánh sau (lean-forward locomotion)
// Hệ số nghiêng được tinh chỉnh thực nghiệm trên giá đỡ
static float computeSetpoint(int16_t moveSpeed) {
    const float LEAN_FACTOR = 0.015f;  // Độ nghiêng mỗi đơn vị speed (cần tinh chỉnh)
    return (float)moveSpeed * LEAN_FACTOR;
}
