using System.IO.Ports;
using System.Text;
using ControllerApp.Models;

namespace ControllerApp.Modules.M1_Bluetooth;

/// <summary>
/// Cài đặt dịch vụ giao tiếp Serial / Bluetooth HC-05 (Module M1).
/// Đảm bảo quản lý vòng đời cổng, bắt ngoại lệ an toàn và đệm chuỗi nhận theo ký tự '\n'.
/// </summary>
public class SerialCommunicationService : ISerialCommunicationService
{
    private SerialPort? _serialPort;
    private readonly StringBuilder _receiveBuffer = new();
    private readonly object _syncLock = new();
    private bool _isDisposed;

    public bool IsOpen => _serialPort != null && _serialPort.IsOpen;
    public ConnectionStatus Status { get; private set; } = ConnectionStatus.Disconnected;
    public string CurrentPortName { get; private set; } = string.Empty;
    public int CurrentBaudRate { get; private set; } = 9600;

    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;
    public event EventHandler<string>? LineReceived;
    public event EventHandler<string>? CommandSent;
    public event EventHandler<Exception>? CommunicationErrorOccurred;

    /// <summary>
    /// Liệt kê các cổng COM hiện có trên hệ thống, sắp xếp tăng dần theo tên.
    /// </summary>
    public string[] GetAvailablePorts()
    {
        try
        {
            var ports = SerialPort.GetPortNames();
            Array.Sort(ports, StringComparer.OrdinalIgnoreCase);
            return ports;
        }
        catch (Exception ex)
        {
            CommunicationErrorOccurred?.Invoke(this, ex);
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Mở cổng Serial tới HC-05 với các thông số truyền thông tiêu chuẩn.
    /// </summary>
    public bool Connect(string portName, int baudRate = 9600)
    {
        if (string.IsNullOrWhiteSpace(portName))
        {
            throw new ArgumentException("Tên cổng COM không được để trống.", nameof(portName));
        }

        lock (_syncLock)
        {
            try
            {
                // Nếu đang mở cổng khác hoặc cổng cũ, đóng trước
                if (IsOpen)
                {
                    DisconnectInternal();
                }

                Status = ConnectionStatus.Connecting;
                CurrentPortName = portName;
                CurrentBaudRate = baudRate;
                OnConnectionStateChanged(ConnectionStatus.Connecting, "Đang kết nối tới cổng...");

                _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
                {
                    NewLine = "\n",
                    ReadTimeout = 1500,
                    WriteTimeout = 1500,
                    DtrEnable = true,
                    RtsEnable = true
                };

                _serialPort.DataReceived += SerialPort_DataReceived;
                _serialPort.ErrorReceived += SerialPort_ErrorReceived;

                _serialPort.Open();

                // Xóa buffer rác cũ còn sót
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();

                lock (_receiveBuffer)
                {
                    _receiveBuffer.Clear();
                }

                Status = ConnectionStatus.Connected;
                OnConnectionStateChanged(ConnectionStatus.Connected, "Kết nối thành công.");
                return true;
            }
            catch (Exception ex)
            {
                DisconnectInternal();
                Status = ConnectionStatus.Disconnected;
                OnConnectionStateChanged(ConnectionStatus.Disconnected, $"Lỗi mở cổng: {ex.Message}");
                CommunicationErrorOccurred?.Invoke(this, ex);
                return false;
            }
        }
    }

    /// <summary>
    /// Ngắt kết nối an toàn, bảo vệ không phát sinh ngoại lệ khi tắt ứng dụng.
    /// </summary>
    public void Disconnect()
    {
        lock (_syncLock)
        {
            DisconnectInternal();
            Status = ConnectionStatus.Disconnected;
            OnConnectionStateChanged(ConnectionStatus.Disconnected, "Đã ngắt kết nối cổng COM.");
        }
    }

    private void DisconnectInternal()
    {
        try
        {
            if (_serialPort != null)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                _serialPort.ErrorReceived -= SerialPort_ErrorReceived;

                if (_serialPort.IsOpen)
                {
                    _serialPort.DiscardInBuffer();
                    _serialPort.DiscardOutBuffer();
                    _serialPort.Close();
                }

                _serialPort.Dispose();
                _serialPort = null;
            }
        }
        catch (Exception ex)
        {
            CommunicationErrorOccurred?.Invoke(this, ex);
        }
        finally
        {
            lock (_receiveBuffer)
            {
                _receiveBuffer.Clear();
            }
        }
    }

    /// <summary>
    /// Gửi một dòng lệnh xuống xe với đuôi '\n'.
    /// Đảm bảo thread-safe và kiểm tra trạng thái cổng trước khi ghi.
    /// </summary>
    public bool SendLine(string command)
    {
        if (string.IsNullOrEmpty(command))
            return false;

        // Chuẩn hóa ký tự kết thúc bản tin: luôn đảm bảo kết thúc bằng đúng 1 '\n'
        string normalized = command.TrimEnd('\r', '\n') + "\n";

        lock (_syncLock)
        {
            if (!IsOpen || _serialPort == null)
            {
                return false;
            }

            try
            {
                _serialPort.Write(normalized);
                CommandSent?.Invoke(this, normalized);
                return true;
            }
            catch (Exception ex)
            {
                CommunicationErrorOccurred?.Invoke(this, ex);

                // Nếu thiết bị bị rút đột ngột
                if (!_serialPort.IsOpen)
                {
                    DisconnectInternal();
                    Status = ConnectionStatus.Disconnected;
                    OnConnectionStateChanged(ConnectionStatus.Disconnected, "Mất kết nối với thiết bị.");
                }
                return false;
            }
        }
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            if (_serialPort == null || !_serialPort.IsOpen)
                return;

            string incomingChunk = _serialPort.ReadExisting();
            if (string.IsNullOrEmpty(incomingChunk))
                return;

            List<string> completedLines = new();

            lock (_receiveBuffer)
            {
                _receiveBuffer.Append(incomingChunk);

                string content = _receiveBuffer.ToString();
                int newlineIndex;

                while ((newlineIndex = content.IndexOf('\n')) >= 0)
                {
                    string line = content.Substring(0, newlineIndex).TrimEnd('\r');
                    completedLines.Add(line);

                    content = content.Substring(newlineIndex + 1);
                }

                _receiveBuffer.Clear();
                _receiveBuffer.Append(content);
            }

            // Phát từng dòng hoàn chỉnh tới các subscriber (Module M3)
            foreach (var line in completedLines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    LineReceived?.Invoke(this, line);
                }
            }
        }
        catch (Exception ex)
        {
            CommunicationErrorOccurred?.Invoke(this, ex);
        }
    }

    private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
    {
        var ex = new IOException($"Lỗi phần cứng trên cổng Serial: {e.EventType}");
        CommunicationErrorOccurred?.Invoke(this, ex);
    }

    private void OnConnectionStateChanged(ConnectionStatus status, string message)
    {
        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(status, CurrentPortName, CurrentBaudRate, message));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                DisconnectInternal();
            }
            _isDisposed = true;
        }
    }
}
