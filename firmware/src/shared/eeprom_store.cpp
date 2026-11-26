// =============================================================================
// eeprom_store.cpp — Module M7: EEPROM Read/Write
// Dùng EEPROM.update() thay EEPROM.write() để bảo vệ tuổi thọ EEPROM
// (ATmega328P: ~100.000 chu kỳ ghi/xóa mỗi địa chỉ)
// =============================================================================
#include "eeprom_store.h"
#include "config.h"
#include <EEPROM.h>

// Hàm nội bộ ghi float vào EEPROM tại địa chỉ addr
static void writeFloat(int addr, float val) {
    EEPROM.update(addr,     ((uint8_t*)&val)[0]);
    EEPROM.update(addr + 1, ((uint8_t*)&val)[1]);
    EEPROM.update(addr + 2, ((uint8_t*)&val)[2]);
    EEPROM.update(addr + 3, ((uint8_t*)&val)[3]);
}

// Hàm nội bộ đọc float từ EEPROM tại địa chỉ addr
static float readFloat(int addr) {
    float val;
    ((uint8_t*)&val)[0] = EEPROM.read(addr);
    ((uint8_t*)&val)[1] = EEPROM.read(addr + 1);
    ((uint8_t*)&val)[2] = EEPROM.read(addr + 2);
    ((uint8_t*)&val)[3] = EEPROM.read(addr + 3);
    return val;
}

bool eeprom_load(EepromConfig* cfg) {
    if (EEPROM.read(EEPROM_ADDR_MAGIC) != EEPROM_VALID_MAGIC) {
        return false;  // EEPROM chưa được ghi lần nào, dùng default
    }
    cfg->pid.kp      = readFloat(EEPROM_ADDR_KP);
    cfg->pid.ki      = readFloat(EEPROM_ADDR_KI);
    cfg->pid.kd      = readFloat(EEPROM_ADDR_KD);
    cfg->angleOffset = readFloat(EEPROM_ADDR_OFFSET);
    return true;
}

void eeprom_save(const EepromConfig& cfg) {
    writeFloat(EEPROM_ADDR_KP,     cfg.pid.kp);
    writeFloat(EEPROM_ADDR_KI,     cfg.pid.ki);
    writeFloat(EEPROM_ADDR_KD,     cfg.pid.kd);
    writeFloat(EEPROM_ADDR_OFFSET, cfg.angleOffset);
    // Ghi magic byte cuối cùng (đảm bảo dữ liệu hoàn chỉnh trước khi đánh dấu valid)
    EEPROM.update(EEPROM_ADDR_MAGIC, EEPROM_VALID_MAGIC);
}
