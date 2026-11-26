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
static float    g_gyroBiasDps = 0.0f;
static bool     g_firstRun = true;

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

    // Vận tốc góc quanh trục nghiêng của IMU (gyro Y)
    float gyroRateDps = (float)gy / GYRO_SCALE_DPS - g_gyroBiasDps;

    // Góc ước lượng từ Accelerometer (atan2 cho góc không bị giới hạn 0-90°)
    float accelAngle = atan2f((float)ax, (float)az) * (180.0f / M_PI);

    float dt_s = (float)dt_us / 1e6f;

    if (g_firstRun) {
        // Lần đầu: khởi tạo góc từ accelerometer (tránh giật khởi động)
        g_data.angle   = accelAngle - g_offset;
        g_firstRun     = false;
    } else {
        float alpha = COMP_FILTER_TIME_CONSTANT_S / (COMP_FILTER_TIME_CONSTANT_S + dt_s);
        float accelAngleCorrected = accelAngle - g_offset;
        g_data.angle = alpha * (g_data.angle + gyroRateDps * dt_s)
                     + (1.0f - alpha) * accelAngleCorrected;
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
    float sumGyroRate = 0.0f;
    int16_t ax, ay, az, gx, gy, gz;
    for (uint8_t i = 0; i < numSamples; i++) {
        g_mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
        sumAngle += atan2f((float)ax, (float)az) * (180.0f / M_PI);
        sumGyroRate += (float)gy / GYRO_SCALE_DPS;
        delayMicroseconds(5000);
    }

    g_offset   = sumAngle / (float)numSamples;
    g_gyroBiasDps = sumGyroRate / (float)numSamples;
    g_data.angle = 0.0f;
    g_data.gyroRate = 0.0f;
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

void imu_setGyroBias(float biasDps) {
    g_gyroBiasDps = biasDps;
}

float imu_getGyroBias() {
    return g_gyroBiasDps;
}
