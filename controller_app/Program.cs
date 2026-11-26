using System.Globalization;
using ControllerApp.Models;
using ControllerApp.Modules.M1_Bluetooth;
using ControllerApp.Modules.M2_Control;
using ControllerApp.Modules.M3_Telemetry;
using ControllerApp.UI;

namespace ControllerApp;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        // Chế độ tự kiểm tra đơn vị (Self-Test / CLI Verification)
        if (args.Length > 0 && args[0].Equals("--test", StringComparison.OrdinalIgnoreCase))
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            RunModuleSelfTests();
            return;
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }

    private static void RunModuleSelfTests()
    {
        Console.WriteLine("=================================================");
        Console.WriteLine("🧪 BẮT ĐẦU CHẠY SELF-TEST CÁC MODULE (M1, M2, M3)");
        Console.WriteLine("=================================================\n");

        int passed = 0;
        int failed = 0;

        void Assert(bool condition, string testName)
        {
            if (condition)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[PASS] {testName}");
                passed++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FAIL] {testName}");
                failed++;
            }
            Console.ResetColor();
        }

        // --- TEST MODULE M3: TelemetryParser ---
        Console.WriteLine("--- Kiểm thử Module M3: TelemetryParser ---");
        
        // Test 1: Parse chuẩn bản tin D:<angle>,<gyro_rate>,<pwm_wheel>
        bool t1Ok = TelemetryParser.TryParse("D:-0.45,1.20,85", out var t1);
        Assert(t1Ok && t1 != null && Math.Abs(t1.Angle - (-0.45f)) < 0.001f && Math.Abs(t1.GyroRate - 1.20f) < 0.001f && t1.PwmWheel == 85,
            "M3.1: Parse bản tin telemetry chuẩn 'D:-0.45,1.20,85'");

        // Test 2: Parse kèm khoảng trắng và \r
        bool t2Ok = TelemetryParser.TryParse("  D:15.35,-4.20,-128\r  ", out var t2);
        Assert(t2Ok && t2 != null && Math.Abs(t2.Angle - 15.35f) < 0.001f && t2.PwmWheel == -128,
            "M3.2: Parse bản tin telemetry có khoảng trắng và ký tự \\r");

        // Test 3: Phát hiện ngưỡng cảnh báo ngã (|Angle| >= 35.0°)
        bool t3Ok = TelemetryParser.TryParse("D:-36.20,5.10,255", out var t3);
        Assert(t3Ok && t3 != null && t3.IsTiltAlarm,
            "M3.3: Nhận diện chính xác cờ cảnh báo nghiêng nguy hiểm (|Angle| >= 35°)");

        // Test 4: Kháng lỗi dữ liệu rác / sai định dạng
        Assert(!TelemetryParser.TryParse("D:invalid,1.2,50", out _), "M3.4a: Từ chối gói tin có ký tự chữ 'invalid'");
        Assert(!TelemetryParser.TryParse("D:1.2,50", out _), "M3.4b: Từ chối gói tin thiếu trường dữ liệu");
        Assert(!TelemetryParser.TryParse("M:100,90", out _), "M3.4c: Từ chối gói tin không có tiền tố D:");
        Assert(!TelemetryParser.TryParse("", out _), "M3.4d: Từ chối chuỗi rỗng an toàn");
        Assert(!TelemetryParser.TryParse(null, out _), "M3.4e: Từ chối chuỗi null an toàn");

        // Test 5: Nhận diện thông điệp hệ thống
        Assert(TelemetryParser.IsSystemMessage("OK"), "M3.5a: Nhận diện thông điệp hệ thống 'OK'");
        Assert(TelemetryParser.IsSystemMessage("CAL_DONE"), "M3.5b: Nhận diện thông điệp 'CAL_DONE'");
        Assert(!TelemetryParser.IsSystemMessage("D:0,0,0"), "M3.5c: Phân biệt chính xác giữa telemetry và tin nhắn hệ thống");

        // --- TEST MODULE M2: PID & CultureInfo.InvariantCulture ---
        Console.WriteLine("\n--- Kiểm thử Module M2: PidParameters & InvariantCulture ---");
        
        // Đặt thử văn hóa máy sang tiếng Pháp / Việt Nam (dùng dấu phẩy cho số thập phân)
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            var pid = new PidParameters(45.50f, 0.80f, 1.25f);
            string pidCmd = pid.ToProtocolCommand();
            Assert(pidCmd == "P:45.50,0.80,1.25\n",
                "M2.1: Bộ thông số PID luôn xuất dấu chấm '.' độc lập với Culture của máy (fr-FR)");
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }

        // --- TEST MODULE M2: BikeControlService ---
        Console.WriteLine("\n--- Kiểm thử Module M2: BikeControlService & Emergency Stop ---");
        
        using (var dummyComm = new DummySerialService())
        using (var controlService = new BikeControlService(dummyComm))
        {
            // Kiểm tra Trim lái
            controlService.SteeringTrim = 5;
            Assert(controlService.CenterSteerAngle == 95, "M2.2: Góc lái trung tâm tính đúng bù Trim: 90 + 5 = 95°");

            // Kiểm tra Kẹp MaxSpeed
            controlService.MaxSpeed = 999;
            Assert(controlService.MaxSpeed == 255, "M2.3: Giới hạn an toàn MaxSpeed tự động kẹp về 255");

            // Kiểm tra Dừng khẩn cấp (Emergency Stop)
            controlService.EmergencyStop();
            Assert(dummyComm.LastSentCommand == "M:0,95\n",
                "M2.4: Lệnh Dừng khẩn cấp lập tức gửi M:0,95\\n (tốc độ = 0, góc về tâm đã bù Trim)");
        }

        Console.WriteLine("\n=================================================");
        Console.WriteLine($"KẾT QUẢ: ĐÃ VƯỢT QUA {passed} TEST, THẤT BẠI {failed} TEST.");
        Console.WriteLine("=================================================");

        if (failed > 0)
        {
            Environment.Exit(1);
        }
    }
}

#pragma warning disable CS0067
/// <summary>
/// Mock Serial Service phục vụ kiểm thử đơn vị nội bộ.
/// </summary>
internal class DummySerialService : ISerialCommunicationService
{
    public bool IsOpen => true;
    public ConnectionStatus Status => ConnectionStatus.Connected;
    public string CurrentPortName => "COM_MOCK";
    public int CurrentBaudRate => 9600;
    public string LastSentCommand { get; private set; } = string.Empty;

    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;
    public event EventHandler<string>? LineReceived;
    public event EventHandler<string>? CommandSent;
    public event EventHandler<Exception>? CommunicationErrorOccurred;

    public string[] GetAvailablePorts() => new[] { "COM_MOCK" };
    public bool Connect(string portName, int baudRate = 9600) => true;
    public void Disconnect() { }
    public bool SendLine(string command)
    {
        LastSentCommand = command.TrimEnd('\r', '\n') + "\n";
        CommandSent?.Invoke(this, LastSentCommand);
        return true;
    }
    public void Dispose() { }
}
#pragma warning restore CS0067
