using System.Globalization;
using ControllerApp.Models;
using ControllerApp.Modules.M1_Bluetooth;

namespace ControllerApp.Modules.M2_Control;

/// <summary>
/// Dịch vụ quản lý điều khiển di chuyển, góc lái, tham số PID và hiệu chuẩn (Module M2).
/// Đảm bảo chuẩn hóa khung bản tin (\n, CultureInfo.InvariantCulture) và tiết chế tần suất gửi lệnh.
/// </summary>
public class BikeControlService : IDisposable
{
    private readonly ISerialCommunicationService _commService;
    private readonly System.Windows.Forms.Timer _throttleTimer;
    private readonly object _stateLock = new();

    private const int DefaultThrottleIntervalMs = 90; // 80ms - 100ms theo đặc tả
    private const int BaseCenterSteerAngle = 90;
    private const int MinAllowedAngle = 30;
    private const int MaxAllowedAngle = 150;
    private const int DefaultTurnDelta = 30;

    private int _maxSpeed = 160;
    private int _steeringTrim = 0;
    private int _turnDelta = DefaultTurnDelta;

    private int _currentTargetSpeed = 0;
    private int _currentTargetAngle = BaseCenterSteerAngle;
    private bool _isMovementActive = false;
    private bool _isDisposed = false;

    // Phục vụ theo dõi phím giữ
    private bool _keyForward;
    private bool _keyBackward;
    private bool _keyLeft;
    private bool _keyRight;

    public int MaxSpeed
    {
        get => _maxSpeed;
        set => _maxSpeed = Math.Clamp(value, 0, 255);
    }

    public int SteeringTrim
    {
        get => _steeringTrim;
        set
        {
            _steeringTrim = Math.Clamp(value, -25, 25);
            UpdateSteeringAngle();
        }
    }

    public int TurnDelta
    {
        get => _turnDelta;
        set => _turnDelta = Math.Clamp(value, 10, 45);
    }

    public int CenterSteerAngle => Math.Clamp(BaseCenterSteerAngle + _steeringTrim, MinAllowedAngle, MaxAllowedAngle);
    public int CurrentTargetSpeed => _currentTargetSpeed;
    public int CurrentTargetAngle => _currentTargetAngle;
    public bool IsMovementActive => _isMovementActive;

    public event Action<int, int>? MovementCommandDispatched;
    public event Action<string>? CommandStatusNotified;

    public BikeControlService(ISerialCommunicationService commService)
    {
        _commService = commService ?? throw new ArgumentNullException(nameof(commService));

        _throttleTimer = new System.Windows.Forms.Timer
        {
            Interval = DefaultThrottleIntervalMs
        };
        _throttleTimer.Tick += ThrottleTimer_Tick;

        UpdateSteeringAngle();
    }

    /// <summary>
    /// Cập nhật trạng thái tổ hợp phím điều khiển (W, S, A, D hoặc Mũi tên).
    /// Tự động tính toán tốc độ, góc lái và khởi động timer tiết chế.
    /// </summary>
    public void UpdateKeyInput(bool forward, bool backward, bool left, bool right)
    {
        lock (_stateLock)
        {
            _keyForward = forward;
            _keyBackward = backward;
            _keyLeft = left;
            _keyRight = right;

            CalculateTargetValues();
        }
    }

    /// <summary>
    /// Đặt trực tiếp hướng di chuyển thông qua các nút bấm ảo trên giao diện.
    /// </summary>
    public void SetDirectionalInput(int speedFactor, int steerDirection)
    {
        lock (_stateLock)
        {
            _keyForward = speedFactor > 0;
            _keyBackward = speedFactor < 0;
            _keyLeft = steerDirection < 0;
            _keyRight = steerDirection > 0;

            CalculateTargetValues();
        }
    }

    private void CalculateTargetValues()
    {
        // 1. Tính tốc độ bánh sau (L298N)
        if (_keyForward && !_keyBackward)
        {
            _currentTargetSpeed = _maxSpeed;
        }
        else if (_keyBackward && !_keyForward)
        {
            _currentTargetSpeed = -_maxSpeed;
        }
        else
        {
            _currentTargetSpeed = 0;
        }

        // 2. Tính góc lái bánh trước (Servo)
        int center = CenterSteerAngle;
        if (_keyLeft && !_keyRight)
        {
            _currentTargetAngle = Math.Clamp(center - _turnDelta, MinAllowedAngle, MaxAllowedAngle);
        }
        else if (_keyRight && !_keyLeft)
        {
            _currentTargetAngle = Math.Clamp(center + _turnDelta, MinAllowedAngle, MaxAllowedAngle);
        }
        else
        {
            _currentTargetAngle = center;
        }

        bool wasActive = _isMovementActive;
        _isMovementActive = (_currentTargetSpeed != 0) || (_currentTargetAngle != center);

        if (_isMovementActive)
        {
            if (!wasActive)
            {
                // Gửi ngay lệnh đầu tiên để phản hồi tức thì
                DispatchCurrentMovementCommand();
                _throttleTimer.Start();
            }
        }
        else
        {
            // Nếu người dùng vừa nhả hết phím hoặc dừng
            _throttleTimer.Stop();
            if (wasActive)
            {
                // Gửi lệnh dừng và trả lái về trung tâm ngay lập tức
                DispatchCurrentMovementCommand();
            }
        }
    }

    private void UpdateSteeringAngle()
    {
        if (!_keyLeft && !_keyRight)
        {
            _currentTargetAngle = CenterSteerAngle;
        }
    }

    private void ThrottleTimer_Tick(object? sender, EventArgs e)
    {
        if (_isMovementActive && _commService.IsOpen)
        {
            DispatchCurrentMovementCommand();
        }
        else
        {
            _throttleTimer.Stop();
        }
    }

    /// <summary>
    /// Gửi khung lệnh di chuyển M:<speed>,<angle>\n xuống xe.
    /// </summary>
    public bool DispatchCurrentMovementCommand()
    {
        int speed;
        int angle;

        lock (_stateLock)
        {
            speed = _currentTargetSpeed;
            angle = _currentTargetAngle;
        }

        string cmd = string.Format(CultureInfo.InvariantCulture, "M:{0},{1}\n", speed, angle);
        bool success = _commService.SendLine(cmd);

        if (success)
        {
            MovementCommandDispatched?.Invoke(speed, angle);
        }

        return success;
    }

    /// <summary>
    /// DỪNG KHẨN CẤP (Emergency Stop):
    /// Ngay lập tức ngắt tốc độ bánh sau về 0, trả góc lái về tâm và gửi lệnh M:0,<center>\n.
    /// </summary>
    public void EmergencyStop()
    {
        lock (_stateLock)
        {
            _keyForward = false;
            _keyBackward = false;
            _keyLeft = false;
            _keyRight = false;

            _currentTargetSpeed = 0;
            _currentTargetAngle = CenterSteerAngle;
            _isMovementActive = false;
            _throttleTimer.Stop();
        }

        string cmd = string.Format(CultureInfo.InvariantCulture, "M:0,{0}\n", CenterSteerAngle);
        _commService.SendLine(cmd);
        MovementCommandDispatched?.Invoke(0, CenterSteerAngle);
        CommandStatusNotified?.Invoke("ĐÃ KÍCH HOẠT DỪNG KHẨN CẤP!");
    }

    /// <summary>
    /// Gửi bộ tham số PID thời gian thực xuống vi điều khiển: P:<Kp>,<Ki>,<Kd>\n
    /// </summary>
    public bool SendPidParameters(PidParameters pid)
    {
        if (pid == null)
            throw new ArgumentNullException(nameof(pid));

        string cmd = pid.ToProtocolCommand();
        bool ok = _commService.SendLine(cmd);

        if (ok)
        {
            CommandStatusNotified?.Invoke($"Đã gửi tham số PID: {pid}");
        }
        else
        {
            CommandStatusNotified?.Invoke("Lỗi gửi tham số PID: Cổng chưa mở.");
        }

        return ok;
    }

    /// <summary>
    /// Gửi lệnh hiệu chuẩn điểm cân bằng: CAL\n
    /// </summary>
    public bool SendCalibrationCommand()
    {
        bool ok = _commService.SendLine("CAL\n");
        if (ok)
        {
            CommandStatusNotified?.Invoke("Đã gửi lệnh hiệu chuẩn điểm cân bằng (CAL). Giữ xe đứng thẳng!");
        }
        return ok;
    }

    /// <summary>
    /// Gửi lệnh lưu cấu hình vào bộ nhớ EEPROM của Arduino: SAVE\n
    /// </summary>
    public bool SendSaveEepromCommand()
    {
        bool ok = _commService.SendLine("SAVE\n");
        if (ok)
        {
            CommandStatusNotified?.Invoke("Đã gửi lệnh ghi cấu hình vào EEPROM (SAVE).");
        }
        return ok;
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _throttleTimer.Stop();
            _throttleTimer.Dispose();
            _isDisposed = true;
        }
    }
}
