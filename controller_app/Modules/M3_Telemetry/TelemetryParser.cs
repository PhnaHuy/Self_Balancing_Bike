using System.Globalization;
using System.Text.RegularExpressions;
using ControllerApp.Models;

namespace ControllerApp.Modules.M3_Telemetry;

/// <summary>
/// Bộ phân tích dữ liệu telemetry phản hồi từ vi điều khiển Arduino qua Bluetooth (Module M3).
/// Tuân thủ quy tắc InvariantCulture, xử lý ngoại lệ và lọc bỏ các bản tin rác/thiếu dữ liệu.
/// </summary>
public static class TelemetryParser
{
    private const string TelemetryPrefix = "D:";
    private static readonly Regex Mpu6050TestLineRegex = new(
        @"^(?:RAW:[^|]*\|\s*)?ANGLE:\s*(?<angle>[+-]?(?:\d+(?:\.\d*)?|\.\d+)(?:[eE][+-]?\d+)?)\s*(?:deg)?\s*(?:\|\s*|\s+)GYRO:\s*(?<gyro>[+-]?(?:\d+(?:\.\d*)?|\.\d+)(?:[eE][+-]?\d+)?)\s*(?:dps)?$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>
    /// Thử phân tích một dòng dữ liệu từ Serial thành đối tượng TelemetryData.
    /// Hỗ trợ telemetry chuẩn D:<angle>,<gyro_rate>,<pwm_wheel> và output test_mpu6050.
    /// Với output test_mpu6050, PWM được đặt bằng 0 vì firmware test không điều khiển motor.
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

        if (trimmed.StartsWith(TelemetryPrefix, StringComparison.OrdinalIgnoreCase))
        {
            // Tách phần dữ liệu phía sau "D:"
            string payload = trimmed.Substring(TelemetryPrefix.Length);
            string[] parts = payload.Split(',');

            if (parts.Length >= 3)
            {
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
            }
        }

        Match testLine = Mpu6050TestLineRegex.Match(trimmed);
        if (testLine.Success
            && float.TryParse(testLine.Groups["angle"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float testAngle)
            && float.TryParse(testLine.Groups["gyro"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float testGyroRate))
        {
            telemetry = new TelemetryData(testAngle, testGyroRate, 0, trimmed);
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
        return !trimmed.StartsWith(TelemetryPrefix, StringComparison.OrdinalIgnoreCase)
            && !TryParse(trimmed, out _);
    }
}
