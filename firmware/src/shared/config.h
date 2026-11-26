// =============================================================================
// config.h — Hằng số cấu hình chung cho toàn bộ firmware
// Áp dụng cho cả chương trình chính lẫn các chương trình test.
// =============================================================================
#pragma once

// --- Chân GPIO / Pin Mapping (Arduino Uno R3) ---
// Giao tiếp I2C (phần cứng cố định, không thay đổi):
//   SDA = A4,  SCL = A5

// Nidec 24H055M020 — Bánh đà (Reaction Wheel)
// Logic điều khiển Nidec 24H: PWM thấp = tốc độ cao, PWM=255 = dừng (đặc tính nghịch)
// BRAKE = LOW  → nhả phanh (motor quay tự do)
// BRAKE = HIGH → hãm phanh điện từ
#define PIN_NIDEC_FG        2   // Feedback xung tốc độ (ngắt ngoài INT0, chưa dùng trong PID cơ bản)
#define PIN_NIDEC_DIR       4   // Chiều quay: HIGH = tiến, LOW = lùi
#define PIN_NIDEC_BRAKE     5   // Hãm phanh: HIGH = phanh, LOW = nhả
#define PIN_NIDEC_PWM       6   // PWM điều tốc (Timer0 OC0A) — nghịch: 255=dừng, 0=full speed

// L298N — Bánh sau (Drive Wheel, Motor giảm tốc)
// IN1/IN2 xác định chiều, ENA = PWM tốc độ
#define PIN_L298N_ENA       3   // PWM tốc độ bánh sau (Timer2 OC2B)
#define PIN_L298N_IN1       7   // Chiều quay H-Bridge In1
#define PIN_L298N_IN2       8   // Chiều quay H-Bridge In2

// Servo — Bánh trước (Steering)
// Nguồn 5V riêng qua L7805, nối chung GND với toàn hệ thống
#define PIN_SERVO           9   // Servo PWM (Timer1 OC1A, dùng thư viện Servo.h)

// HC-05 Bluetooth — Cổng Serial phần cứng D0 (RX) / D1 (TX)
// Baud rate phải khớp với cài đặt HC-05 và ứng dụng controller_app
// KHÔNG dùng SoftwareSerial để tránh xung đột interrupt và jitter PWM
#define HC05_BAUD_RATE      38400

// --- MPU6050 I2C ---
#define MPU6050_I2C_ADDR    0x68  // AD0 = LOW (mặc định)

// --- Chu kỳ vòng lặp ---
#define PID_LOOP_INTERVAL_US    5000UL   // Vòng lặp PID cân bằng: 5ms = 200Hz
#define TELEMETRY_INTERVAL_MS   50UL     // Gửi telemetry: 50ms = 20Hz
#define CMD_WATCHDOG_MS         500UL    // Ngưỡng mất kết nối: 500ms không nhận lệnh M: → dừng

// --- PID Defaults (sẽ bị ghi đè từ EEPROM nếu có dữ liệu hợp lệ) ---
#define PID_DEFAULT_KP      35.0f
#define PID_DEFAULT_KI       0.6f
#define PID_DEFAULT_KD       1.2f

// --- Ngưỡng an toàn ---
#define TILT_FAILSAFE_DEG   35.0f    // Góc nghiêng vượt ±35° → Fail-Safe ngắt toàn bộ
#define INTEGRAL_CLAMP      150.0f   // Giới hạn tích phân chống windup

// --- Giới hạn PWM đầu ra ---
// Nidec 24H: PWM logic nghịch chiều, giá trị "nghỉ" = 255
// PID output được ánh xạ: 0 → Nidec PWM 255 (dừng), ±max → 0/255 đảo chiều
#define NIDEC_PWM_STOP      255   // Dừng hoàn toàn (đặc tính Nidec 24H)
#define NIDEC_PWM_MAX_OUT   200   // Giới hạn PWM tối đa đầu ra PID (tránh quá dòng)

// L298N bánh sau
#define L298N_MAX_DUTY      180   // Giới hạn duty cycle tối đa (~70%) bảo vệ motor ở nguồn 3S 11.1V

// --- Servo lái ---
#define SERVO_CENTER_DEG    90    // Góc giữa mặc định (không có Trim)
#define SERVO_MIN_DEG       45    // Giới hạn cơ khí trái
#define SERVO_MAX_DEG      135    // Giới hạn cơ khí phải

// --- Bộ lọc Complementary Filter ---
// Chọn tau để alpha = tau / (tau + dt) xấp xỉ 0.98 tại chu kỳ PID 5ms.
#define COMP_FILTER_TIME_CONSTANT_S 0.245f

// --- EEPROM ---
#define EEPROM_VALID_MAGIC_V1 0xA5
#define EEPROM_VALID_MAGIC    0xA6
#define EEPROM_ADDR_MAGIC      0
#define EEPROM_ADDR_KP         1
#define EEPROM_ADDR_KI         5
#define EEPROM_ADDR_KD         9
#define EEPROM_ADDR_OFFSET    13
#define EEPROM_ADDR_GYRO_BIAS 17

// --- Buffer lệnh Serial ---
#define CMD_BUFFER_SIZE     48    // Tối đa 48 byte/dòng lệnh (đủ cho "P:200.00,50.00,50.00\n")

// --- Điều khiển di chuyển (lệnh M:) ---
// speed: [-255, +255], angle: [SERVO_MIN_DEG, SERVO_MAX_DEG]
#define MOVE_SPEED_MAX      255
#define MOVE_SPEED_MIN     -255
