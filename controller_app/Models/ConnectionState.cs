namespace ControllerApp.Models;

/// <summary>
/// Trạng thái kết nối của cổng giao tiếp Serial / Bluetooth HC-05.
/// </summary>
public enum ConnectionStatus
{
    Disconnected,
    Connecting,
    Connected
}

/// <summary>
/// Dữ liệu sự kiện khi trạng thái kết nối cổng Serial thay đổi.
/// </summary>
public class ConnectionStateChangedEventArgs : EventArgs
{
    public ConnectionStatus Status { get; }
    public string PortName { get; }
    public int BaudRate { get; }
    public string Message { get; }

    public ConnectionStateChangedEventArgs(ConnectionStatus status, string portName, int baudRate, string message = "")
    {
        Status = status;
        PortName = portName;
        BaudRate = baudRate;
        Message = message;
    }
}
