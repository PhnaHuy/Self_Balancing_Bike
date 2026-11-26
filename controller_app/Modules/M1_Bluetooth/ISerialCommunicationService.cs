using ControllerApp.Models;

namespace ControllerApp.Modules.M1_Bluetooth;

/// <summary>
/// Giao diện dịch vụ giao tiếp Serial / Bluetooth HC-05 (Module M1).
/// </summary>
public interface ISerialCommunicationService : IDisposable
{
    /// <summary>
    /// Kiểm tra trạng thái cổng hiện tại có đang mở hay không.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Trạng thái kết nối hiện tại.
    /// </summary>
    ConnectionStatus Status { get; }

    /// <summary>
    /// Tên cổng COM đang mở hoặc vừa thao tác.
    /// </summary>
    string CurrentPortName { get; }

    /// <summary>
    /// Tốc độ Baud hiện tại.
    /// </summary>
    int CurrentBaudRate { get; }

    /// <summary>
    /// Quét và lấy danh sách các cổng COM khả dụng trên hệ điều hành.
    /// </summary>
    string[] GetAvailablePorts();

    /// <summary>
    /// Mở kết nối tới cổng Serial / Bluetooth HC-05 với cấu hình chuẩn.
    /// </summary>
    bool Connect(string portName, int baudRate = 9600);

    /// <summary>
    /// Đóng kết nối an toàn và giải phóng tài nguyên cổng.
    /// </summary>
    void Disconnect();

    /// <summary>
    /// Gửi một dòng lệnh xuống xe với đuôi '\n' bắt buộc.
    /// </summary>
    bool SendLine(string command);

    /// <summary>
    /// Sự kiện khi trạng thái kết nối thay đổi.
    /// </summary>
    event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>
    /// Sự kiện phát ra khi nhận được một dòng dữ liệu hoàn chỉnh (kết thúc bằng '\n').
    /// Chú ý: Sự kiện này chạy trên luồng nền của SerialPort, UI phải dùng Invoke/BeginInvoke.
    /// </summary>
    event EventHandler<string>? LineReceived;

    /// <summary>
    /// Sự kiện khi một dòng lệnh được gửi thành công.
    /// </summary>
    event EventHandler<string>? CommandSent;

    /// <summary>
    /// Sự kiện khi xảy ra lỗi truyền thông hoặc ngắt kết nối đột ngột.
    /// </summary>
    event EventHandler<Exception>? CommunicationErrorOccurred;
}
