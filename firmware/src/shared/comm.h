// =============================================================================
// comm.h — Module M7: Command Parser, EEPROM & Telemetry
// Giao tiếp Serial/Bluetooth HC-05, parse lệnh \n, gửi telemetry D:...
// =============================================================================
#pragma once
#include <Arduino.h>
#include "pid.h"

// Dữ liệu lệnh di chuyển nhận được từ M:speed,angle\n
struct MoveCommand {
    int16_t speed;    // [-255, +255]
    uint8_t angle;    // [SERVO_MIN_DEG, SERVO_MAX_DEG]
    bool    fresh;    // true khi vừa nhận lệnh M: mới (chưa được xử lý)
};

// Khởi tạo Serial với baud rate đúng (phải khớp HC-05 và controller_app)
void comm_init();

// Đọc từng ký tự từ Serial vào buffer tĩnh, không chặn vòng lặp
// Khi nhận đủ \n, phân tích lệnh và cập nhật state nội bộ
// Gọi mỗi chu kỳ loop()
void comm_tick();

// Lấy lệnh di chuyển mới nhất (M:speed,angle\n)
// Trả về struct với fresh=true nếu có lệnh mới chưa xử lý
MoveCommand comm_getMoveCommand();

// Xác nhận đã xử lý lệnh di chuyển (xóa cờ fresh)
void comm_clearMoveCommand();

// Lấy thời điểm cuối cùng nhận được lệnh M: (millis), dùng cho watchdog
uint32_t comm_getLastCommandMs();

// Phát bản tin telemetry D:angle,gyroRate,pwm\n lên HC-05
// format: "D:-0.45,1.20,85\n" — số thực dùng dấu chấm thập phân
void comm_sendTelemetry(float angle, float gyroRate, int16_t pwm);

// Phát thông điệp phản hồi hệ thống (OK, CAL_DONE, SAVE_DONE, ERROR, v.v.)
// Tự động thêm \n vào cuối
void comm_sendAck(const char* msg);

// Kiểm tra cờ lệnh CAL (hiệu chuẩn) và xóa sau khi đọc
bool comm_popCalFlag();

// Kiểm tra cờ lệnh SAVE (lưu EEPROM) và xóa sau khi đọc
bool comm_popSaveFlag();

// Lấy tham số PID mới nhất nhận được từ lệnh P:kp,ki,kd\n
// Trả về false nếu chưa có lệnh P: mới
bool comm_popPidParams(PidParams* out);
