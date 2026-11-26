namespace ControllerApp.UI;

/// <summary>
/// Bảng màu và kiểu dáng dùng chung cho ứng dụng WinForms điều khiển xe cân bằng.
/// </summary>
public static class ModernTheme
{
    // Màu nền chính
    public static readonly Color BackgroundDark = Color.FromArgb(16, 20, 28);
    public static readonly Color CardBackground = Color.FromArgb(24, 30, 42);
    public static readonly Color CardHeader = Color.FromArgb(32, 40, 56);
    public static readonly Color InputBackground = Color.FromArgb(35, 44, 62);

    // Màu nhấn (Accents)
    public static readonly Color PrimaryAccent = Color.FromArgb(0, 168, 255);
    public static readonly Color PrimaryHover = Color.FromArgb(30, 185, 255);
    public static readonly Color SuccessGreen = Color.FromArgb(46, 204, 113);
    public static readonly Color WarningYellow = Color.FromArgb(241, 196, 15);
    public static readonly Color DangerRed = Color.FromArgb(231, 76, 60);
    public static readonly Color DangerHover = Color.FromArgb(205, 50, 40);

    // Màu chữ
    public static readonly Color TextPrimary = Color.FromArgb(240, 244, 250);
    public static readonly Color TextSecondary = Color.FromArgb(155, 170, 195);
    public static readonly Color TextMuted = Color.FromArgb(105, 120, 145);

    // Viền
    public static readonly Color BorderColor = Color.FromArgb(50, 62, 85);

    // Font chuẩn
    public static readonly Font HeaderFont = new Font("Segoe UI", 11F, FontStyle.Bold);
    public static readonly Font SubHeaderFont = new Font("Segoe UI", 9.5F, FontStyle.Bold);
    public static readonly Font BodyFont = new Font("Segoe UI", 9F, FontStyle.Regular);
    public static readonly Font LargeNumberFont = new Font("Segoe UI", 18F, FontStyle.Bold);
    public static readonly Font MonospaceFont = new Font("Cascadia Code", 8.5F, FontStyle.Regular);
}
