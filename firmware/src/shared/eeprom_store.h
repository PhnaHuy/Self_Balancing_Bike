// =============================================================================
// eeprom_store.h — Module M7: Lưu/đọc cấu hình từ EEPROM
// Ghi: Kp, Ki, Kd, Angle_Offset kèm byte magic kiểm tra tính hợp lệ
// =============================================================================
#pragma once
#include <Arduino.h>
#include "pid.h"

// Cấu trúc lưu trữ EEPROM
struct EepromConfig {
    PidParams pid;
    float     angleOffset;
};

// Đọc cấu hình từ EEPROM
// Trả về true nếu dữ liệu hợp lệ (magic byte đúng), false nếu EEPROM chưa được ghi
bool eeprom_load(EepromConfig* cfg);

// Ghi cấu hình vào EEPROM cùng byte magic
void eeprom_save(const EepromConfig& cfg);
