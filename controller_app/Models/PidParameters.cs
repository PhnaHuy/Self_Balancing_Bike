using System.Globalization;

namespace ControllerApp.Models;

/// <summary>
/// Đại diện cho bộ thông số điều khiển PID cân bằng xe (M2).
/// Định dạng gửi xuống xe: P:<Kp>,<Ki>,<Kd>\n
/// </summary>
public class PidParameters
{
    public float Kp { get; set; }
    public float Ki { get; set; }
    public float Kd { get; set; }

    public PidParameters(float kp = 25.0f, float ki = 0.5f, float kd = 1.2f)
    {
        Kp = kp;
        Ki = ki;
        Kd = kd;
    }

    /// <summary>
    /// Chuyển đổi bộ thông số sang khung bản tin chuẩn hóa với CultureInfo.InvariantCulture.
    /// Ví dụ: "P:45.50,0.80,1.25\n"
    /// </summary>
    public string ToProtocolCommand()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "P:{0:0.00},{1:0.00},{2:0.00}\n",
            Kp, Ki, Kd
        );
    }

    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture, "Kp={0:0.00}, Ki={1:0.00}, Kd={2:0.00}", Kp, Ki, Kd);
    }
}
