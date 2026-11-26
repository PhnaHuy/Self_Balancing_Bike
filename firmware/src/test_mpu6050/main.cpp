// =============================================================================
// test_mpu6050/main.cpp — Test cảm biến MPU6050
// Kiểm tra: kết nối I2C, đọc dữ liệu thô, in góc nghiêng ước lượng ra Serial
//
// Cách dùng (Serial Monitor ở 9600 baud, nhớ set "No line ending" hoặc "Newline"):
//   Mỗi 100ms: in dòng "RAW: ax,ay,az,gx,gy,gz | ANGLE: X.XX° | GYRO: X.XX°/s"
//   Gõ "FLIP" → đảo chiều trục gia tốc (để xác định hướng lắp cảm biến)
//   Gõ "CAL"  → lấy 50 mẫu trung bình làm Angle_Offset và in ra
//   Gõ "COMP" → chuyển sang chế độ chỉ in góc Complementary Filter (ít nhiễu hơn)
//   Gõ "RAW"  → quay lại chế độ in đầy đủ
//
// MỤC ĐÍCH: Xác nhận trục và dấu góc nghiêng trước khi tích hợp vào PID chính
// =============================================================================
#include <Arduino.h>
#include <Wire.h>
#include <MPU6050.h>

#define SAMPLE_INTERVAL_MS  100UL
#define COMP_ALPHA          0.98f
#define ACCEL_SCALE_G       16384.0f
#define GYRO_SCALE_DPS      131.0f
#define MPU6050_I2C_ADDR    0x68

static MPU6050  g_mpu(MPU6050_I2C_ADDR);
static bool     g_axisFlipped  = false;
static bool     g_compMode     = false;
static float    g_compAngle    = 0.0f;
static float    g_angleOffset  = 0.0f;
static uint32_t g_lastSampleMs = 0;
static uint32_t g_lastUs       = 0;

static char g_rxBuf[32];
static uint8_t g_rxIdx = 0;

static void processCmd(char* cmd);
static float doCalibrate(uint8_t numSamples);

void setup() {
    Serial.begin(9600);
    Wire.begin();
    delay(100);                 // Chờ bộ dao động nội (PLL) và mạch reset MPU6050 ổn định
    Wire.setClock(100000UL);    // Hạ xuống 100kHz để tránh méo xung trên dây cắm thực tế

    Serial.println(F("MPU6050 TEST"));

    // Kiểm tra ping I2C cơ bản tại địa chỉ 0x68
    Wire.beginTransmission(MPU6050_I2C_ADDR);
    if (Wire.endTransmission() != 0) {
        Serial.println(F("ERROR: I2C 0x68 NOT RESPONDING! Check wiring: SDA=A4, SCL=A5, VCC=3.3V/5V"));
        while (true) delay(1000);
    }

    g_mpu.initialize();

    // Đọc và in giá trị WHO_AM_I để debug chip chính hãng vs clone/tương thích
    uint8_t devId = g_mpu.getDeviceID();
    Serial.print(F("WHO_AM_I (DeviceID): 0x"));
    Serial.println(devId, HEX);

    if (!g_mpu.testConnection()) {
        // Nhiều chip MPU6050 lô mới/clone trả về WHO_AM_I khác 0x68 (0x70, 0x72, 0x98...)
        // nhưng vẫn hoạt động bình thường → chỉ cảnh báo và tiếp tục chạy
        Serial.println(F("WARNING: testConnection() failed (non-0x68 WHO_AM_I), continuing anyway..."));
    }

    g_mpu.setFullScaleAccelRange(MPU6050_ACCEL_FS_2);
    g_mpu.setFullScaleGyroRange(MPU6050_GYRO_FS_250);
    g_mpu.setDLPFMode(MPU6050_DLPF_BW_42);

    Serial.println(F("MPU6050 OK | CMD: CAL | FLIP | COMP | RAW"));
    g_lastSampleMs = millis();
    g_lastUs       = micros();
}

void loop() {
    // --- Nhận lệnh ---
    while (Serial.available() > 0) {
        char c = (char)Serial.read();
        if (c == '\r') continue;
        if (c == '\n') {
            g_rxBuf[g_rxIdx] = '\0';
            if (g_rxIdx > 0) processCmd(g_rxBuf);
            g_rxIdx = 0;
        } else if (g_rxIdx < 30) {
            g_rxBuf[g_rxIdx++] = c;
        } else {
            g_rxIdx = 0;
        }
    }

    // --- Lấy mẫu và in dữ liệu ---
    uint32_t nowMs = millis();
    if ((nowMs - g_lastSampleMs) >= SAMPLE_INTERVAL_MS) {
        uint32_t nowUs = micros();
        float dt_s = (float)(nowUs - g_lastUs) / 1e6f;
        g_lastUs       = nowUs;
        g_lastSampleMs = nowMs;

        int16_t ax, ay, az, gx, gy, gz;
        g_mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);

        // Đảo trục nếu được yêu cầu (kiểm tra hướng lắp)
        if (g_axisFlipped) ax = -ax;

        float gyroRate = (float)gy / GYRO_SCALE_DPS;
        float accelAngle = atan2f((float)ax, (float)az) * (180.0f / M_PI);

        // Complementary Filter
        g_compAngle = COMP_ALPHA * (g_compAngle + gyroRate * dt_s)
                    + (1.0f - COMP_ALPHA) * accelAngle;
        float displayAngle = g_compAngle - g_angleOffset;

        if (g_compMode) {
            // Chế độ compact: chỉ in góc (dùng khi quan sát đồ thị)
            char buf[32];
            dtostrf(displayAngle, 6, 2, buf);
            Serial.print(F("ANGLE:"));
            Serial.print(buf);
            Serial.print(F("  GYRO:"));
            dtostrf(gyroRate, 6, 2, buf);
            Serial.println(buf);
        } else {
            // Chế độ đầy đủ: in RAW + ước lượng
            Serial.print(F("RAW:"));
            Serial.print(ax); Serial.print(',');
            Serial.print(ay); Serial.print(',');
            Serial.print(az); Serial.print(',');
            Serial.print(gx); Serial.print(',');
            Serial.print(gy); Serial.print(',');
            Serial.print(gz);
            Serial.print(F(" | ANGLE:"));
            char buf[12];
            dtostrf(displayAngle, 6, 2, buf);
            Serial.print(buf);
            Serial.print(F("deg | GYRO:"));
            dtostrf(gyroRate, 6, 2, buf);
            Serial.print(buf);
            Serial.println(F("dps"));
        }
    }
}

static void processCmd(char* cmd) {
    if (strcmp(cmd, "CAL") == 0) {
        Serial.println(F("CAL: Hold sensor UPRIGHT and still..."));
        g_angleOffset = doCalibrate(50);
        Serial.print(F("CAL_DONE: Offset="));
        char buf[12];
        dtostrf(g_angleOffset, 6, 2, buf);
        Serial.println(buf);
    } else if (strcmp(cmd, "FLIP") == 0) {
        g_axisFlipped = !g_axisFlipped;
        g_compAngle   = 0.0f;
        Serial.print(F("AXIS_FLIP: "));
        Serial.println(g_axisFlipped ? F("ON") : F("OFF"));
    } else if (strcmp(cmd, "COMP") == 0) {
        g_compMode = true;
        Serial.println(F("Mode: COMPACT (angle only)"));
    } else if (strcmp(cmd, "RAW") == 0) {
        g_compMode = false;
        Serial.println(F("Mode: FULL RAW"));
    }
}

static float doCalibrate(uint8_t numSamples) {
    int16_t ax, ay, az, gx, gy, gz;
    // Warm-up
    for (uint8_t i = 0; i < 20; i++) {
        g_mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
        delay(5);
    }
    float sum = 0.0f;
    for (uint8_t i = 0; i < numSamples; i++) {
        g_mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
        if (g_axisFlipped) ax = -ax;
        sum += atan2f((float)ax, (float)az) * (180.0f / M_PI);
        delay(5);
    }
    g_compAngle = 0.0f;  // Reset filter sau calibration
    return sum / (float)numSamples;
}
