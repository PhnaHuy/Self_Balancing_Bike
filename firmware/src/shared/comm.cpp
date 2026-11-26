// =============================================================================
// comm.cpp — Module M7: Command Parser, EEPROM & Telemetry
// Dùng buffer tĩnh cố định, tránh hoàn toàn String động (tiết kiệm SRAM)
// Tất cả parse số dùng atof()/atoi() — không dùng sscanf() để tiết kiệm Flash
// =============================================================================
#include "comm.h"
#include "config.h"

// Buffer tĩnh nhận lệnh — không dùng String để tránh phân mảnh heap SRAM
static char     g_rxBuf[CMD_BUFFER_SIZE];
static uint8_t  g_rxIdx    = 0;

// Trạng thái lệnh nhận được (cờ)
static MoveCommand g_moveCmd    = {0, SERVO_CENTER_DEG, false};
static bool        g_calFlag    = false;
static bool        g_saveFlag   = false;
static bool        g_pidFlag    = false;
static PidParams   g_pidNew     = {PID_DEFAULT_KP, PID_DEFAULT_KI, PID_DEFAULT_KD};
static uint32_t    g_lastCmdMs  = 0;

// Hàm nội bộ: xử lý dòng lệnh hoàn chỉnh (đã loại \r\n)
static void processLine(char* line, uint8_t len);

void comm_init() {
    Serial.begin(HC05_BAUD_RATE);
    // Flush buffer khởi động
    while (Serial.available()) Serial.read();
    g_lastCmdMs = millis();
}

// Gọi mỗi chu kỳ loop() — không chặn, chỉ đọc số byte khả dụng
void comm_tick() {
    while (Serial.available() > 0) {
        char c = (char)Serial.read();

        if (c == '\r') continue;  // Bỏ qua CR (Windows line ending)

        if (c == '\n') {
            // Kết thúc dòng lệnh — xử lý buffer hiện tại
            g_rxBuf[g_rxIdx] = '\0';
            if (g_rxIdx > 0) {
                processLine(g_rxBuf, g_rxIdx);
            }
            g_rxIdx = 0;
        } else {
            if (g_rxIdx < CMD_BUFFER_SIZE - 1) {
                g_rxBuf[g_rxIdx++] = c;
            }
            // Nếu buffer đầy mà chưa có \n: có thể là dữ liệu rác → reset
            else {
                g_rxIdx = 0;
            }
        }
    }
}

static void processLine(char* line, uint8_t len) {
    // --- Lệnh M:speed,angle --- di chuyển & lái
    if (line[0] == 'M' && line[1] == ':') {
        char* p1 = line + 2;
        char* p2 = strchr(p1, ',');
        if (p2 != nullptr) {
            *p2 = '\0';
            int16_t spd   = (int16_t)atoi(p1);
            uint8_t angle = (uint8_t)constrain(atoi(p2 + 1), SERVO_MIN_DEG, SERVO_MAX_DEG);
            spd = (int16_t)constrain(spd, MOVE_SPEED_MIN, MOVE_SPEED_MAX);
            g_moveCmd = {spd, angle, true};
            g_lastCmdMs = millis();
        }
    }
    // --- Lệnh P:kp,ki,kd --- cập nhật PID
    else if (line[0] == 'P' && line[1] == ':') {
        char* p1 = line + 2;
        char* p2 = strchr(p1, ',');
        if (p2 != nullptr) {
            *p2 = '\0';
            char* p3 = strchr(p2 + 1, ',');
            if (p3 != nullptr) {
                *p3 = '\0';
                g_pidNew.kp = (float)atof(p1);
                g_pidNew.ki = (float)atof(p2 + 1);
                g_pidNew.kd = (float)atof(p3 + 1);
                // Clamp giá trị hợp lý để tránh nạp tham số vô lý gây mất kiểm soát
                g_pidNew.kp = constrain(g_pidNew.kp, 0.0f, 500.0f);
                g_pidNew.ki = constrain(g_pidNew.ki, 0.0f, 100.0f);
                g_pidNew.kd = constrain(g_pidNew.kd, 0.0f, 100.0f);
                g_pidFlag = true;
            }
        }
    }
    // --- Lệnh CAL --- hiệu chuẩn điểm cân bằng
    else if (strcmp(line, "CAL") == 0) {
        g_calFlag = true;
    }
    // --- Lệnh SAVE --- lưu EEPROM
    else if (strcmp(line, "SAVE") == 0) {
        g_saveFlag = true;
    }
    // Lệnh không nhận ra: bỏ qua (tránh crash hoặc động đến stack)
}

MoveCommand comm_getMoveCommand() {
    return g_moveCmd;
}

void comm_clearMoveCommand() {
    g_moveCmd.fresh = false;
}

uint32_t comm_getLastCommandMs() {
    return g_lastCmdMs;
}

// Phát telemetry D:angle,gyroRate,nidecFrequencyHz\n
// Dùng dtostrf() thay cho Serial.print(float) để kiểm soát độ chính xác và tốc độ.
void comm_sendTelemetry(float angle, float gyroRate, int16_t nidecFrequencyHz) {
    char buf[48];
    char s_angle[8], s_gyro[8];
    dtostrf(angle,    5, 2, s_angle);
    dtostrf(gyroRate, 5, 2, s_gyro);
    // Dùng snprintf trên Arduino để tránh tràn buffer
    snprintf(buf, sizeof(buf), "D:%s,%s,%d\n", s_angle, s_gyro, (int)nidecFrequencyHz);
    Serial.print(buf);
}

void comm_sendAck(const char* msg) {
    Serial.print(msg);
    Serial.print('\n');
}

bool comm_popCalFlag() {
    if (g_calFlag) {
        g_calFlag = false;
        return true;
    }
    return false;
}

bool comm_popSaveFlag() {
    if (g_saveFlag) {
        g_saveFlag = false;
        return true;
    }
    return false;
}

bool comm_popPidParams(PidParams* out) {
    if (g_pidFlag) {
        *out = g_pidNew;
        g_pidFlag = false;
        return true;
    }
    return false;
}
