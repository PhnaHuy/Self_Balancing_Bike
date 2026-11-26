// =============================================================================
// imu.cpp — Module M4: IMU Sensing & Angle Estimation
// MPU6050 đọc qua thư viện electroniccats/MPU6050
// Complementary Filter: angle = alpha*(angle + gyro*dt) + (1-alpha)*accel_angle
// =============================================================================
#include "imu.h"
#include "config.h"
#include <Wire.h>
#include <MPU6050.h>

static MPU6050  g_mpu(MPU6050_I2C_ADDR);
static ImuData  g_data     = {0.0f, 0.0f, false};
static float    g_offset   = 0.0f;
static bool     g_firstRun = true;

// Hằng số trục: điều chỉnh nếu hướng lắp cảm biến trên khung xe khác
// Trục Y của gia tốc kế dùng để tính góc nghiêng mặt phẳng đứng
// (kiểm chứng qua test_mpu6050 trước khi gắn vào PID)
#define ACCEL_SCALE_G       16384.0f   // ±2g range: 16384 LSB/g
#define GYRO_SCALE_DPS      131.0f     // ±250°/s range: 131 LSB/(°/s)

bool imu_init() {
    Wire.begin();
    delay(100);                 // Chờ bộ dao động nội (PLL) và mạch reset MPU6050 ổn định
    Wire.setClock(100000UL);    // 100kHz để ổn định tín hiệu I2C trên dây cắm thực tế

    // Kiểm tra ping I2C vật lý tại địa chỉ 0x68
    Wire.beginTransmission(MPU6050_I2C_ADDR);
    if (Wire.endTransmission() != 0) {
        return false;
    }

    g_mpu.initialize();

    // Bỏ qua kiểm tra cứng WHO_AM_I (testConnection) để tương thích các chip clone
    // (0x70, 0x72, 0x98...) vẫn giao tiếp bình thường tại địa chỉ I2C 0x68
    if (!g_mpu.testConnection()) {
        Serial.print(F("WARN:MPU6050_WHO_AM_I=0x"));
        Serial.println(g_mpu.getDeviceID(), HEX);
    }

    // Cấu hình dải đo an toàn cho xe cân bằng
    g_mpu.setFullScaleAccelRange(MPU6050_ACCEL_FS_2);    // ±2g
    g_mpu.setFullScaleGyroRange(MPU6050_GYRO_FS_250);    // ±250°/s
    g_mpu.setDLPFMode(MPU6050_DLPF_BW_42);               // Digital Low-Pass Filter 42Hz

    g_data.valid = true;
    g_firstRun   = true;
    return true;
}

void imu_update(uint32_t dt_us) {
    if (!g_data.valid) return;

    int16_t ax, ay, az, gx, gy, gz;
    g_mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);

    // Vận tốc góc quanh trục liên quan đến nghiêng (trục X IMU = trục pitch xe)
    // Dấu: điều chỉnh nếu chiều nghiêng bị đảo
    float gyroRateDps = (float)gy / GYRO_SCALE_DPS;   // °/s

    // Góc ước lượng từ Accelerometer (atan2 cho góc không bị giới hạn 0-90°)
    float accelAngle = atan2f((float)ax, (float)az) * (180.0f / M_PI);

    float dt_s = (float)dt_us / 1e6f;

    if (g_firstRun) {
        // Lần đầu: khởi tạo góc từ accelerometer (tránh giật khởi động)
        g_data.angle   = accelAngle - g_offset;
        g_firstRun     = false;
    } else {
        // Complementary Filter:
        //   - Gyro: tích phân chính xác ngắn hạn, nhưng tích lũy drift dài hạn
        //   - Accel: đúng dài hạn nhưng nhiều nhiễu (rung bánh đà)
        float rawAngle = COMP_FILTER_ALPHA * (g_data.angle + g_offset + gyroRateDps * dt_s)
                       + (1.0f - COMP_FILTER_ALPHA) * accelAngle;
        g_data.angle   = rawAngle - g_offset;
    }

    g_data.gyroRate = gyroRateDps;
}

ImuData imu_getData() {
    return g_data;
}

float imu_calibrate(uint8_t numSamples) {
    // Warm-up: đọc vài mẫu để bộ lọc DLPF ổn định
    for (uint8_t i = 0; i < 20; i++) {
        g_mpu.getMotion6(nullptr, nullptr, nullptr, nullptr, nullptr, nullptr);
        delayMicroseconds(5000);  // OK dùng delay trong hàm calibrate vì không trong vòng lặp chính
    }

    float sumAngle = 0.0f;
    int16_t ax, ay, az, gx, gy, gz;
    for (uint8_t i = 0; i < numSamples; i++) {
        g_mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
        sumAngle += atan2f((float)ax, (float)az) * (180.0f / M_PI);
        delayMicroseconds(5000);
    }

    g_offset   = sumAngle / (float)numSamples;
    g_firstRun = true;  // reset filter sau calibration
    return g_offset;
}

void imu_setOffset(float offset) {
    g_offset   = offset;
    g_firstRun = true;
}

float imu_getOffset() {
    return g_offset;
}
