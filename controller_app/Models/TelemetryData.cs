namespace ControllerApp.Models;

/// <summary>
/// Đại diện cho bản tin telemetry phản hồi từ xe qua Bluetooth (M3).
/// Định dạng chuẩn: D:<angle>,<gyro_rate>,<pwm_wheel>\n
/// </summary>
public class TelemetryData
{
    /// <summary>
    /// Thời điểm nhận bản tin trên PC.
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Góc nghiêng hiện tại của xe so với phương thẳng đứng (độ, °).
    /// Điểm cân bằng lý tưởng là 0.00°.
    /// </summary>
    public float Angle { get; }

    /// <summary>
    /// Vận tốc góc xoay của cảm biến IMU (độ/giây, °/s).
    /// </summary>
    public float GyroRate { get; }

    /// <summary>
    /// Tín hiệu xung PWM điều khiển động cơ bánh đà (Reaction Wheel).
    /// </summary>
    public int PwmWheel { get; }

    /// <summary>
    /// Chuỗi thô gốc nhận được từ Serial.
    /// </summary>
    public string RawMessage { get; }

    /// <summary>
    /// Trạng thái cảnh báo nghiêng quá ngưỡng nguy hiểm (|Angle| >= 35.0°).
    /// </summary>
    public bool IsTiltAlarm => Math.Abs(Angle) >= 35.0f;

    /// <summary>
    /// Trạng thái xe đang ổn định trong dải cân bằng hẹp (|Angle| <= 3.0°).
    /// </summary>
    public bool IsBalanced => Math.Abs(Angle) <= 3.0f;

    public TelemetryData(float angle, float gyroRate, int pwmWheel, string rawMessage = "")
    {
        Timestamp = DateTime.Now;
        Angle = angle;
        GyroRate = gyroRate;
        PwmWheel = pwmWheel;
        RawMessage = rawMessage;
    }

    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss.fff}] Góc: {Angle:F2}°, V_Góc: {GyroRate:F2}°/s, PWM: {PwmWheel}";
    }
}
