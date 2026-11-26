using System.Globalization;
using ControllerApp.Models;

namespace ControllerApp.Modules.M3_Telemetry;

/// <summary>
/// Bộ phân tích dữ liệu telemetry phản hồi từ vi điều khiển Arduino qua Bluetooth (Module M3).
/// Tuân thủ quy tắc InvariantCulture, xử lý ngoại lệ và lọc bỏ các bản tin rác/thiếu dữ liệu.
/// </summary>
public static class TelemetryParser
{
    private const string TelemetryPrefix = "D:";

    /// <summary>
    /// Thử phân tích một dòng dữ liệu từ Serial thành đối tượng TelemetryData.
    /// Khung bản tin hợp lệ: D:<angle>,<gyro_rate>,<pwm_wheel>
    /// </summary>
    /// <param name="rawLine">Dòng chuỗi nhận từ Serial (đã loại bỏ \r, \n)</param>
    /// <param name="telemetry">Dữ liệu phân tích thành công (nếu có)</param>
    /// <returns>True nếu phân tích thành công, False nếu chuỗi không phải telemetry hoặc lỗi định dạng</returns>
    public static bool TryParse(string? rawLine, out TelemetryData? telemetry)
    {
        telemetry = null;

        if (string.IsNullOrWhiteSpace(rawLine))
            return false;

        string trimmed = rawLine.Trim();

        // Kiểm tra tiền tố bắt buộc của bản tin dữ liệu
        if (!trimmed.StartsWith(TelemetryPrefix, StringComparison.OrdinalIgnoreCase))
            return false;

        // Tách phần dữ liệu phía sau "D:"
        string payload = trimmed.Substring(TelemetryPrefix.Length);
        string[] parts = payload.Split(',');

        if (parts.Length < 3)
            return false;

        // Phân tích an toàn với CultureInfo.InvariantCulture (dấu chấm thập phân)
        bool angleOk = float.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float angle);
        bool gyroOk = float.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float gyroRate);
        bool pwmOk = int.TryParse(parts[2].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int pwmWheel);

        // Nếu trường PWM dạng float (ví dụ 85.0), fallback parse float rồi ép kiểu
        if (!pwmOk && float.TryParse(parts[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float pwmFloat))
        {
            pwmWheel = (int)Math.Round(pwmFloat);
            pwmOk = true;
        }

        if (angleOk && gyroOk && pwmOk)
        {
            telemetry = new TelemetryData(angle, gyroRate, pwmWheel, trimmed);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Kiểm tra xem dòng nhận được có phải là thông điệp phản hồi hệ thống (CAL, SAVE, OK, ERROR...) hay không.
    /// </summary>
    public static bool IsSystemMessage(string? rawLine)
    {
        if (string.IsNullOrWhiteSpace(rawLine))
            return false;

        string trimmed = rawLine.Trim();
        return !trimmed.StartsWith(TelemetryPrefix, StringComparison.OrdinalIgnoreCase);
    }
}
