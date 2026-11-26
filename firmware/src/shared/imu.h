// =============================================================================
// imu.h — Module M4: IMU Sensing & Angle Estimation
// MPU6050 qua I2C (A4=SDA, A5=SCL)
// Sử dụng Complementary Filter kết hợp Accelerometer + Gyroscope
// =============================================================================
#pragma once
#include <Arduino.h>

// Kết quả ước lượng góc nghiêng và vận tốc góc
struct ImuData {
    float angle;      // Góc nghiêng hiện tại (độ, âm = ngã về trước)
    float gyroRate;   // Vận tốc góc từ gyroscope (độ/giây)
    bool  valid;      // false nếu cảm biến chưa sẵn sàng
};

// Khởi tạo MPU6050, kiểm tra kết nối I2C
// Trả về true nếu thành công, false nếu không tìm thấy cảm biến
bool imu_init();

// Đọc dữ liệu và cập nhật ước lượng góc nghiêng
// Phải gọi theo chu kỳ cố định (dt_us = khoảng thời gian tính bằng microseconds)
void imu_update(uint32_t dt_us);

// Lấy kết quả ước lượng góc hiện tại
ImuData imu_getData();

// Hiệu chuẩn điểm cân bằng: lấy trung bình N mẫu góc hiện tại làm Angle_Offset
// Gọi khi xe được giữ đứng thẳng trên giá đỡ
// Trả về giá trị Angle_Offset vừa tính
float imu_calibrate(uint8_t numSamples);

// Nạp giá trị Angle_Offset từ ngoài (từ EEPROM)
void imu_setOffset(float offset);

// Lấy Angle_Offset hiện tại
float imu_getOffset();
