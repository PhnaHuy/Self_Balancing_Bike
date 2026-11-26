# Self-Balancing Bike — Firmware Documentation
## Tổ chức thư mục `firmware/`

```text
firmware/
│
├── platformio.ini              # Cấu hình PlatformIO đa môi trường (multi-env)
├── README.md                   # Tài liệu kỹ thuật đầy đủ (file này)
│
└── src/
    ├── shared/                 # Module dùng chung cho chương trình chính
    │   ├── config.h            # Hằng số chân GPIO, timing, giới hạn an toàn
    │   ├── imu.h / imu.cpp     # M4: MPU6050, Complementary Filter
    │   ├── pid.h / pid.cpp     # M5: PID Controller, Anti-Windup, Fail-Safe
    │   ├── actuators.h / .cpp  # M6: Nidec 24H, L298N, Servo
    │   ├── comm.h / comm.cpp   # M7: Parser lệnh \n, telemetry D:...
    │   └── eeprom_store.h/.cpp # M7: Đọc/ghi EEPROM (PID, góc offset, gyro bias)
    │
    ├── main/                   # [env:main] Chương trình tích hợp đầy đủ
    │   └── main.cpp
    │
    ├── test_hc05/              # [env:test_hc05] Test Bluetooth HC-05
    │   └── main.cpp
    │
    ├── test_mpu6050/           # [env:test_mpu6050] Test cảm biến MPU6050
    │   └── main.cpp
    │
    └── test_nidec/             # [env:test_nidec] Test motor bánh đà Nidec 24H
        └── main.cpp
```

---

## Sơ đồ chân GPIO (Arduino Uno R3)

| Chân | Tín hiệu | Thiết bị | Ghi chú |
|------|----------|----------|---------|
| `D0` (RX) | UART RX | HC-05 TX | Rút HC-05 khi nạp code! |
| `D1` (TX) | UART TX | HC-05 RX | — |
| `D2` | Nidec FG | Nidec 24H | Ngắt ngoài INT0, chưa dùng trong PID cơ bản |
| `D3` | L298N ENA (PWM) | L298N | Timer2, điều tốc bánh sau |
| `D4` | Nidec DIR | Nidec 24H | Chiều quay bánh đà |
| `D5` | Nidec BRAKE | Nidec 24H | HIGH=phanh, LOW=nhả |
| `D6` | Nidec PWM | Nidec 24H | Timer0 OC0A — logic nghịch! |
| `D7` | L298N IN1 | L298N | Chiều quay H-Bridge |
| `D8` | L298N IN2 | L298N | Chiều quay H-Bridge |
| `D9` | Servo PWM | Motor Servo | Timer1 OC1A, thư viện Servo.h |
| `A4` | SDA | MPU6050 | I2C Data (phần cứng cố định) |
| `A5` | SCL | MPU6050 | I2C Clock (phần cứng cố định) |

> **Lưu ý Timer:** D3 (Timer2B) và D6 (Timer0A) không xung đột. D9 dùng Timer1 cho Servo.
> `analogWrite()` trên D6 (Timer0) có thể ảnh hưởng `millis()`/`micros()` nếu prescaler bị thay đổi — KHÔNG thay đổi prescaler Timer0.

---

## Quy tắc nguồn điện an toàn (3S 18650)

> [!CAUTION]
> Nguồn 3S 18650: **11.1V–12.6V**. Kiểm tra cực (+/-) bằng đồng hồ trước khi cấp nguồn!

| Nguồn | Dùng cho | IC ổn áp | Tụ lọc | Lưu ý |
|-------|----------|----------|--------|-------|
| 12V từ 3S | Nidec 24H055M020 | Không (cấp trực tiếp) | — | Nidec chịu được 12V |
| 12V từ 3S | L298N (EXT 12V) | Không | — | Giới hạn PWM max 70% duty |
| 5V riêng | Motor Servo | **L7805** | 0.33µF (in) + 0.1µF (out) + tản nhiệt | Dòng Servo peak 500mA–1A |
| 5V USB | Arduino Uno | Regulator on-board | — | Chỉ dùng khi debug/nạp code |

> **Lý do tách nguồn Servo qua L7805:**
> Servo có dòng khởi động lên đến 1A. Nếu dùng chung 5V với Arduino, áp sụt gây reset MCU. L7805 cần tản nhiệt khi Servo chạy liên tục.
> **Nối chung GND là bắt buộc** giữa L7805-GND, Arduino-GND, L298N-GND và Nidec-GND.

---

## Giao thức Serial / Bluetooth HC-05

**Baudrate:** `38400` bps (cài đặt trong HC-05 phải khớp; mặc định AT+UART=38400,0,0)

| Chiều | Lệnh | Định dạng | Ví dụ |
|-------|------|-----------|-------|
| App→Arduino | Di chuyển & lái | `M:<speed>,<angle>\n` | `M:150,90\n` |
| App→Arduino | Cập nhật PID | `P:<Kp>,<Ki>,<Kd>\n` | `P:35.00,0.60,1.25\n` |
| App→Arduino | Hiệu chuẩn | `CAL\n` | `CAL\n` |
| App→Arduino | Lưu EEPROM | `SAVE\n` | `SAVE\n` |
| Arduino→App | Telemetry | `D:<angle>,<gyro>,<pwm>\n` | `D:-0.45,1.20,85\n` |
| Arduino→App | Phản hồi | `OK:<msg>\n` / `WARN:<msg>\n` | `OK:READY\n` |

---

## Hướng dẫn Build, Nạp và Test

### Yêu cầu
- **PlatformIO Core** (đã cài qua pip): `python -m platformio --version`
- VS Code với extension **PlatformIO IDE** (tùy chọn, dùng được cả CLI)

### Lệnh Build và Nạp từ Terminal VS Code

```powershell
# Di chuyển vào thư mục firmware
cd firmware

# Build chương trình chính
python -m platformio run -e main

# Nạp chương trình chính lên Arduino (thay COM8 đúng cổng)
python -m platformio run -e main --target upload --upload-port COM8

# Build và nạp từng test độc lập
python -m platformio run -e test_hc05   --target upload --upload-port COM8
python -m platformio run -e test_mpu6050 --target upload --upload-port COM8
python -m platformio run -e test_nidec  --target upload --upload-port COM8

# Mở Serial Monitor (giám sát output)
python -m platformio device monitor -e main          # 38400 baud
python -m platformio device monitor -e test_hc05     # 38400 baud
python -m platformio device monitor -e test_mpu6050  # 9600 baud
python -m platformio device monitor -e test_nidec    # 9600 baud
```

### Quy trình test đề xuất (theo thứ tự)

1. **test_mpu6050** → Xác nhận I2C hoạt động, kiểm tra hướng trục, giữ cảm biến đứng yên rồi chạy `CAL`. Bộ lọc khởi tạo theo góc gia tốc và hiệu chuẩn cả bias gyro; trong firmware chính, gửi thêm `SAVE` để lưu offset và bias vào EEPROM. Có thể kết nối Controller App ở `9600` baud để xem đồ thị và góc hiện tại; PWM hiển thị `0` vì firmware này chỉ test cảm biến.
2. **test_nidec** → Trên giá đỡ, kiểm tra DIR/BRAKE/PWM, chạy `RAMP`
3. **test_hc05** → Ghép nối HC-05, kiểm tra echo hai chiều, dùng controller_app gửi `PING`
4. **main** → Nạp chương trình chính, giữ xe đứng thẳng, gửi `CAL` từ app, quan sát đồ thị

---

## Lưu ý đấu nối trước khi cấp nguồn

> [!WARNING]
> **Rút cáp HC-05 khỏi D0/D1 mỗi khi nạp code!** HC-05 dùng chung chân UART phần cứng với bộ nạp USB-Serial. Nếu để HC-05 khi nạp sẽ gây xung đột và lỗi nạp.

> [!IMPORTANT]
> **Kiểm tra checklist trước khi cấp nguồn 3S:**
> - [ ] Xe trên giá đỡ, bánh đà không chạm đất
> - [ ] GND chung giữa L7805, Arduino, L298N, Nidec
> - [ ] L7805 có tụ lọc (0.33µF input, 0.1µF output) và tản nhiệt
> - [ ] Chân DIR/BRAKE/PWM Nidec đúng D4/D5/D6
> - [ ] Chân IN1/IN2/ENA L298N đúng D7/D8/D3
> - [ ] Servo gắn vào D9, cấp nguồn từ L7805 (không phải 5V Arduino)
> - [ ] HC-05 TX→D0(RX), RX→D1(TX) qua voltage divider (HC-05 RX chịu 3.3V; D1(TX) Arduino là 5V!)
