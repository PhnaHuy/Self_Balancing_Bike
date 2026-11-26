# Controller App — Self-Balancing Bike (Group G1: Modules M1, M2, M3)

Ứng dụng giám sát và điều khiển trên máy tính dành cho dự án **Self-Balancing Bike**, phát triển bằng C# / .NET Windows Forms, tương thích hoàn toàn trên cả **Visual Studio Code** và **Visual Studio**.

---

## 1. Cấu trúc Module Tách Tầng (Clean Architecture)

```text
controller_app/
│
├── ControllerApp.csproj          # File cấu hình project .NET WinForms
├── ControllerApp.sln             # File Solution tương thích VS Code & Visual Studio
├── Program.cs                    # Điểm khởi chạy app & Runner Self-Test
├── README.md                     # Hướng dẫn chi tiết
│
├── Models/                       # Tầng mô hình dữ liệu (Data Models)
│   ├── ConnectionState.cs        # Trạng thái kết nối Serial (Disconnected, Connecting, Connected)
│   ├── TelemetryData.cs          # Bản tin phản hồi D:<angle>,<gyro_rate>,<pwm_wheel>\n
│   └── PidParameters.cs          # Cấu trúc tham số Kp, Ki, Kd định dạng InvariantCulture
│
├── Modules/                      # Tầng nghiệp vụ theo đặc tả hệ thống
│   ├── M1_Bluetooth/             # Module M1: Quản lý Serial & Bluetooth HC-05
│   │   ├── ISerialCommunicationService.cs
│   │   └── SerialCommunicationService.cs   # Vòng đời cổng COM, buffer \n an toàn, IDisposable, try-catch
│   │
│   ├── M2_Control/               # Module M2: Điều khiển di chuyển, lái, PID & EEPROM
│   │   └── BikeControlService.cs           # Lệnh M:..., P:..., CAL, SAVE, Dừng khẩn cấp, Throttle 90ms
│   │
│   └── M3_Telemetry/             # Module M3: Phân tích dữ liệu & Trực quan hóa
│       ├── TelemetryParser.cs              # Parse float.TryParse với CultureInfo.InvariantCulture
│       └── RealtimeGraphControl.cs         # Biểu đồ GDI+ DoubleBuffered vẽ góc nghiêng so với 0°
│
└── UI/                           # Tầng giao diện người dùng (Presentation Layer)
    ├── MainForm.cs               # Logic hiển thị an toàn luồng (Invoke/BeginInvoke), bắt phím
    ├── MainForm.Designer.cs      # Layout giao diện hiện đại (Header, Gauges, Graph, Controls, PID, Log)
    └── ModernTheme.cs            # Bảng màu Dark Tech hiện đại & font chuẩn
```

---

## 2. Đặc Tả Giao Thức Truyền Thông (HC-05 / Serial)

Mọi bản tin đều tuân thủ nguyên tắc:
1. Kết thúc bằng đúng một ký tự xuống dòng `\n`.
2. Định dạng số thực luôn sử dụng `CultureInfo.InvariantCulture` (dấu chấm thập phân `.`).

| Chiều | Loại lệnh / Dữ liệu | Cú pháp | Ví dụ |
| :--- | :--- | :--- | :--- |
| **PC ➔ Xe** | Điều khiển di chuyển & lái | `M:<speed>,<angle>\n` | `M:160,90\n` (Tiến), `M:-160,90\n` (Lùi), `M:0,60\n` (Rẽ trái), `M:0,90\n` (Dừng) |
| **PC ➔ Xe** | Cập nhật tham số PID | `P:<Kp>,<Ki>,<Kd>\n` | `P:35.00,0.60,1.25\n` |
| **PC ➔ Xe** | Hiệu chuẩn góc cân bằng | `CAL\n` | `CAL\n` |
| **PC ➔ Xe** | Lưu cấu hình vào EEPROM | `SAVE\n` | `SAVE\n` |
| **Xe ➔ PC** | Bản tin phản hồi định kỳ | `D:<angle>,<gyro_rate>,<pwm_wheel>\n` | `D:-0.45,1.20,85\n` |

---

## 3. Các Tính Năng Nổi Bật

1. **An Toàn Luồng (Thread-Safety):** Toàn bộ dữ liệu từ `SerialPort.DataReceived` được đẩy về luồng UI thông qua `BeginInvoke` / `Invoke` có kiểm tra `IsDisposed` và `IsHandleCreated`.
2. **Tiết Chế Tần Suất Gửi Khi Giữ Phím (Command Throttling):** Bộ định thời nội bộ duy trì chu kỳ gửi lệnh di chuyển ở mức ~90ms (80ms - 100ms) để chống tràn buffer UART của ATmega328P và HC-05.
3. **Dừng Khẩn Cấp (Emergency Stop):** Phím `Space` hoặc nút bấm đỏ `DỪNG KHẨN CẤP` lập tức ngắt xung bánh sau về `0` và trả lái về góc trung tâm.
4. **Biểu Đồ Thời Gian Thực GDI+:** Tùy biến `RealtimeGraphControl` với công nghệ Double Buffering vẽ mốc cân bằng `0.00°`, đường cảnh báo ngã `±35°`, lưới chia độ và vệt sáng lịch sử góc nghiêng.

---

## 4. Hướng Dẫn Biên Dịch & Chạy Thử

### Yêu cầu môi trường
* .NET SDK (phiên bản 8.0, 9.0 hoặc 10.0 trên Windows).
* Visual Studio Code (hoặc Visual Studio 2022).

### 1. Biên dịch dự án (Build)
Tại thư mục gốc dự án hoặc trong `controller_app/`:
```powershell
dotnet build controller_app/ControllerApp.csproj
```

### 2. Chạy bộ tự kiểm tra đơn vị (Self-Test CLI)
Không cần cài thêm thư viện test bên ngoài, ứng dụng tích hợp sẵn chế độ kiểm thử CLI:
```powershell
dotnet run --project controller_app -- --test
```
Kết quả kiểm tra xác nhận tính đúng đắn của `TelemetryParser`, `PidParameters`, định dạng `InvariantCulture`, bù `Trim`, và lệnh `EmergencyStop`.

### 3. Chạy giao diện ứng dụng (Run GUI)
```powershell
dotnet run --project controller_app
```

---

## 5. Thao Tác Bàn Phím Điều Khiển

* **W** hoặc **Mũi tên Lên (↑):** Chạy tiến (Tốc độ = `Max Speed`)
* **S** hoặc **Mũi tên Xuống (↓):** Chạy lùi (Tốc độ = `-Max Speed`)
* **A** hoặc **Mũi tên Trái (←):** Bẻ lái sang trái
* **D** hoặc **Mũi tên Phải (→):** Bẻ lái sang phải
* **Phím Cách (Spacebar):** **DỪNG KHẨN CẤP LẬP TỨC**
