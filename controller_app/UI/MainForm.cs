using System.Globalization;
using ControllerApp.Models;
using ControllerApp.Modules.M1_Bluetooth;
using ControllerApp.Modules.M2_Control;
using ControllerApp.Modules.M3_Telemetry;

namespace ControllerApp.UI;

/// <summary>
/// Giao diện điều khiển và giám sát trung tâm cho xe tự cân bằng (Group G1: M1, M2, M3).
/// Đảm bảo an toàn luồng tuyệt đối (Invoke/BeginInvoke), xử lý ngoại lệ và bắt phím điều khiển.
/// </summary>
public partial class MainForm : Form
{
    private readonly ISerialCommunicationService _commService;
    private readonly BikeControlService _controlService;

    // Trạng thái bàn phím
    private bool _isKeyForward;
    private bool _isKeyBackward;
    private bool _isKeyLeft;
    private bool _isKeyRight;

    // Thống kê phiên làm việc
    private long _rxCount = 0;
    private long _txCount = 0;
    private long _errCount = 0;

    public MainForm()
    {
        InitializeComponent();

        // Khởi tạo các module dịch vụ (M1 & M2)
        _commService = new SerialCommunicationService();
        _controlService = new BikeControlService(_commService);

        // Đăng ký sự kiện dịch vụ truyền thông (M1)
        _commService.ConnectionStateChanged += CommService_ConnectionStateChanged;
        _commService.LineReceived += CommService_LineReceived;
        _commService.CommandSent += CommService_CommandSent;
        _commService.CommunicationErrorOccurred += CommService_ErrorOccurred;

        // Đăng ký sự kiện dịch vụ điều khiển (M2)
        _controlService.MovementCommandDispatched += ControlService_MovementCommandDispatched;
        _controlService.CommandStatusNotified += ControlService_CommandStatusNotified;

        // Cấu hình sự kiện UI
        WireUpUiEvents();
    }

    private void WireUpUiEvents()
    {
        Load += MainForm_Load;
        FormClosing += MainForm_FormClosing;

        // M1: Cổng COM & Kết nối
        btnRefreshPorts.Click += (s, e) => RefreshComPorts();
        btnConnect.Click += BtnConnect_Click;

        // M2: Phím ảo và chuột điều khiển di chuyển
        btnForward.MouseDown += (s, e) => { _controlService.SetDirectionalInput(1, 0); };
        btnForward.MouseUp += (s, e) => { _controlService.SetDirectionalInput(0, 0); };

        btnBackward.MouseDown += (s, e) => { _controlService.SetDirectionalInput(-1, 0); };
        btnBackward.MouseUp += (s, e) => { _controlService.SetDirectionalInput(0, 0); };

        btnLeft.MouseDown += (s, e) => { _controlService.SetDirectionalInput(0, -1); };
        btnLeft.MouseUp += (s, e) => { _controlService.SetDirectionalInput(0, 0); };

        btnRight.MouseDown += (s, e) => { _controlService.SetDirectionalInput(0, 1); };
        btnRight.MouseUp += (s, e) => { _controlService.SetDirectionalInput(0, 0); };

        btnCenterSteer.Click += (s, e) => { _controlService.SetDirectionalInput(0, 0); };

        // Dừng khẩn cấp
        btnEmergencyStop.Click += (s, e) =>
        {
            _controlService.EmergencyStop();
            LogMessage("🛑 ĐÃ NHẤN DỪNG KHẨN CẤP! Bánh sau dừng, lái về tâm.", ModernTheme.DangerRed);
        };

        // Đồng bộ Max Speed Slider & NumericUpDown
        trackMaxSpeed.Scroll += (s, e) =>
        {
            numMaxSpeed.Value = trackMaxSpeed.Value;
            _controlService.MaxSpeed = trackMaxSpeed.Value;
        };
        numMaxSpeed.ValueChanged += (s, e) =>
        {
            trackMaxSpeed.Value = (int)numMaxSpeed.Value;
            _controlService.MaxSpeed = (int)numMaxSpeed.Value;
        };

        // Đồng bộ Trim Slider & NumericUpDown
        trackTrim.Scroll += (s, e) =>
        {
            numTrim.Value = trackTrim.Value;
            _controlService.SteeringTrim = trackTrim.Value;
        };
        numTrim.ValueChanged += (s, e) =>
        {
            trackTrim.Value = (int)numTrim.Value;
            _controlService.SteeringTrim = (int)numTrim.Value;
        };

        // M2: Gửi PID, CAL, SAVE
        btnSendPid.Click += BtnSendPid_Click;
        btnCalibrate.Click += BtnCalibrate_Click;
        btnSaveEeprom.Click += BtnSaveEeprom_Click;

        // M3: Xóa đồ thị & Xóa log
        btnClearGraph.Click += (s, e) => realtimeGraph.Clear();
        btnClearLog.Click += (s, e) => rtbConsole.Clear();
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        RefreshComPorts();
        UpdateConnectionUi(ConnectionStatus.Disconnected);
        SetControlsEnabled(false);

        // Khởi tạo thông số mặc định cho service
        _controlService.MaxSpeed = (int)numMaxSpeed.Value;
        _controlService.SteeringTrim = (int)numTrim.Value;

        LogMessage("🚀 Ứng dụng điều khiển & giám sát xe tự cân bằng sẵn sàng.", ModernTheme.SuccessGreen);
        LogMessage("👉 Kết nối cổng Bluetooth HC-05 để bắt đầu nhận dữ liệu và điều khiển.", ModernTheme.TextSecondary);
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // Giải phóng an toàn các tài nguyên M1, M2 trước khi thoát ứng dụng
        try
        {
            _controlService.EmergencyStop();
            _controlService.Dispose();
            _commService.Disconnect();
            _commService.Dispose();
        }
        catch { }
    }

    #region M1: Quản lý kết nối Serial / Bluetooth

    private void RefreshComPorts()
    {
        cboPorts.Items.Clear();
        string[] ports = _commService.GetAvailablePorts();

        if (ports.Length > 0)
        {
            cboPorts.Items.AddRange(ports);
            cboPorts.SelectedIndex = 0;
            LogMessage($"🔍 Tìm thấy {ports.Length} cổng COM: {string.Join(", ", ports)}", ModernTheme.TextSecondary);
        }
        else
        {
            LogMessage("⚠️ Không tìm thấy cổng COM nào. Hãy cắm hoặc ghép nối Bluetooth HC-05.", ModernTheme.WarningYellow);
        }
    }

    private void BtnConnect_Click(object? sender, EventArgs e)
    {
        if (_commService.IsOpen)
        {
            _commService.Disconnect();
        }
        else
        {
            if (cboPorts.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn cổng COM trước khi kết nối.", "Chưa chọn cổng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string portName = cboPorts.SelectedItem.ToString()!;
            if (!int.TryParse(cboBaudRate.SelectedItem?.ToString(), out int baudRate))
            {
                baudRate = 9600;
            }

            LogMessage($"⏳ Đang kết nối tới {portName} @ {baudRate} bps...", ModernTheme.TextSecondary);
            _commService.Connect(portName, baudRate);
        }
    }

    private void CommService_ConnectionStateChanged(object? sender, ConnectionStateChangedEventArgs e)
    {
        SafeUiInvoke(() =>
        {
            UpdateConnectionUi(e.Status);

            if (e.Status == ConnectionStatus.Connected)
            {
                SetControlsEnabled(true);
                tslConnectionInfo.Text = $"Cổng: {e.PortName} ({e.BaudRate} bps)";
                LogMessage($"✅ Đã kết nối thành công tới {e.PortName}.", ModernTheme.SuccessGreen);
            }
            else if (e.Status == ConnectionStatus.Disconnected)
            {
                SetControlsEnabled(false);
                tslConnectionInfo.Text = "Cổng: Chưa kết nối";
                LogMessage($"❌ Ngắt kết nối: {e.Message}", ModernTheme.TextMuted);
            }
        });
    }

    private void UpdateConnectionUi(ConnectionStatus status)
    {
        switch (status)
        {
            case ConnectionStatus.Connected:
                btnConnect.Text = "🔌 NGẮT KẾT NỐI";
                btnConnect.BackColor = ModernTheme.DangerRed;
                lblStatusBadge.Text = "● ĐÃ KẾT NỐI (ONLINE)";
                lblStatusBadge.ForeColor = ModernTheme.SuccessGreen;
                cboPorts.Enabled = false;
                btnRefreshPorts.Enabled = false;
                cboBaudRate.Enabled = false;
                break;

            case ConnectionStatus.Connecting:
                btnConnect.Text = "⏳ ĐANG KẾT NỐI";
                btnConnect.BackColor = ModernTheme.WarningYellow;
                lblStatusBadge.Text = "● ĐANG KẾT NỐI...";
                lblStatusBadge.ForeColor = ModernTheme.WarningYellow;
                break;

            case ConnectionStatus.Disconnected:
            default:
                btnConnect.Text = "⚡ KẾT NỐI";
                btnConnect.BackColor = ModernTheme.SuccessGreen;
                lblStatusBadge.Text = "● ĐÃ NGẮT KẾT NỐI";
                lblStatusBadge.ForeColor = ModernTheme.DangerRed;
                cboPorts.Enabled = true;
                btnRefreshPorts.Enabled = true;
                cboBaudRate.Enabled = true;
                break;
        }
    }

    private void SetControlsEnabled(bool enabled)
    {
        btnForward.Enabled = enabled;
        btnBackward.Enabled = enabled;
        btnLeft.Enabled = enabled;
        btnRight.Enabled = enabled;
        btnCenterSteer.Enabled = enabled;
        btnEmergencyStop.Enabled = enabled;

        btnSendPid.Enabled = enabled;
        btnCalibrate.Enabled = enabled;
        btnSaveEeprom.Enabled = enabled;
    }

    private void CommService_CommandSent(object? sender, string command)
    {
        Interlocked.Increment(ref _txCount);
        SafeUiInvoke(() =>
        {
            tslTxCount.Text = $"TX: {_txCount} lệnh";
        });
    }

    private void CommService_ErrorOccurred(object? sender, Exception ex)
    {
        Interlocked.Increment(ref _errCount);
        SafeUiInvoke(() =>
        {
            tslErrorCount.Text = $"Lỗi: {_errCount}";
            LogMessage($"⚠️ Lỗi Serial: {ex.Message}", ModernTheme.DangerRed);
        });
    }

    #endregion

    #region M3: Xử lý Telemetry & Vẽ đồ thị thời gian thực

    private void CommService_LineReceived(object? sender, string line)
    {
        Interlocked.Increment(ref _rxCount);

        // Phân tích dữ liệu telemetry bằng bộ phân tích chuẩn InvariantCulture (M3)
        if (TelemetryParser.TryParse(line, out TelemetryData? telemetry) && telemetry != null)
        {
            SafeUiInvoke(() =>
            {
                UpdateTelemetryDisplay(telemetry);
            });
        }
        else
        {
            // Nếu là dòng tin nhắn phản hồi hệ thống (CAL, SAVE, OK...)
            SafeUiInvoke(() =>
            {
                tslRxCount.Text = $"RX: {_rxCount} gói";
                if (TelemetryParser.IsSystemMessage(line))
                {
                    LogMessage($"📥 Arduino: {line}", ModernTheme.PrimaryAccent);
                }
            });
        }
    }

    private void UpdateTelemetryDisplay(TelemetryData t)
    {
        tslRxCount.Text = $"RX: {_rxCount} gói";
        tslLastUpdate.Text = $"Cập nhật: {t.Timestamp:HH:mm:ss.fff}";

        // 1. Góc nghiêng & Màu sắc cảnh báo
        lblGaugeAngleValue.Text = string.Format(CultureInfo.InvariantCulture, "{0:+0.00;-0.00;0.00}°", t.Angle);

        if (t.IsTiltAlarm)
        {
            lblGaugeAngleValue.ForeColor = ModernTheme.DangerRed;
            lblGaugeSafetyValue.Text = "CẢNH BÁO NGÃ!";
            lblGaugeSafetyValue.ForeColor = ModernTheme.DangerRed;
        }
        else if (t.IsBalanced)
        {
            lblGaugeAngleValue.ForeColor = ModernTheme.SuccessGreen;
            lblGaugeSafetyValue.Text = "THĂNG BẰNG TỐT";
            lblGaugeSafetyValue.ForeColor = ModernTheme.SuccessGreen;
        }
        else
        {
            lblGaugeAngleValue.ForeColor = ModernTheme.WarningYellow;
            lblGaugeSafetyValue.Text = "ĐANG BÙ NGHIÊNG";
            lblGaugeSafetyValue.ForeColor = ModernTheme.WarningYellow;
        }

        // 2. Vận tốc góc
        lblGaugeGyroValue.Text = string.Format(CultureInfo.InvariantCulture, "{0:+0.00;-0.00;0.00}°/s", t.GyroRate);

        // 3. Tần số PFM bánh đà
        lblGaugePwmValue.Text = string.Format(CultureInfo.InvariantCulture, "{0} Hz", t.NidecFrequencyHz);
        pbPwmLevel.Value = Math.Clamp(Math.Abs(t.NidecFrequencyHz), 0, pbPwmLevel.Maximum);

        // 4. Đẩy mẫu vào biểu đồ thời gian thực (M3)
        realtimeGraph.AddSample(t.Angle);

        // 5. Nếu người dùng bật tùy chọn hiển thị chi tiết telemetry trong console
        if (chkLogTelemetry.Checked)
        {
            LogMessage(t.ToString(), ModernTheme.TextMuted);
        }
    }

    #endregion

    #region M2: Điều khiển Di chuyển, PID, CAL, SAVE

    private void ControlService_MovementCommandDispatched(int speed, int angle)
    {
        SafeUiInvoke(() =>
        {
            lblCurrentCommand.Text = string.Format(CultureInfo.InvariantCulture, "Lệnh gửi: M:{0},{1} (Tiết chế ~90ms)", speed, angle);
        });
    }

    private void ControlService_CommandStatusNotified(string status)
    {
        SafeUiInvoke(() =>
        {
            LogMessage(status, ModernTheme.PrimaryAccent);
        });
    }

    private void BtnSendPid_Click(object? sender, EventArgs e)
    {
        var pid = new PidParameters(
            (float)numKp.Value,
            (float)numKi.Value,
            (float)numKd.Value
        );

        _controlService.SendPidParameters(pid);
        LogMessage($"📤 Gửi thông số PID: {pid.ToProtocolCommand().TrimEnd()}", ModernTheme.PrimaryAccent);
    }

    private void BtnCalibrate_Click(object? sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "Bạn có chắc chắn muốn hiệu chuẩn điểm cân bằng?\n\nHãy giữ xe đứng thẳng hoàn hảo trên giá đỡ trước khi nhấn OK!",
            "Xác nhận hiệu chuẩn (CAL)",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Question);

        if (result == DialogResult.OK)
        {
            _controlService.SendCalibrationCommand();
            LogMessage("⚖️ ĐÃ GỬI LỆNH HIỆU CHUẨN (CAL). Vi điều khiển đang lấy mẫu góc 0°...", ModernTheme.WarningYellow);
        }
    }

    private void BtnSaveEeprom_Click(object? sender, EventArgs e)
    {
        _controlService.SendSaveEepromCommand();
        LogMessage("💾 ĐÃ GỬI LỆNH LƯU CẤU HÌNH EEPROM (SAVE).", ModernTheme.SuccessGreen);
    }

    #endregion

    #region M2: Bắt phím bàn phím điều khiển (KeyDown / KeyUp)

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // Phím Spacebar luôn kích hoạt Dừng khẩn cấp bất kể control nào đang focus
        if (keyData == Keys.Space)
        {
            _controlService.EmergencyStop();
            LogMessage("🛑 PHÍM SPACEBAR: KÍCH HOẠT DỪNG KHẨN CẤP!", ModernTheme.DangerRed);
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        // Bỏ qua nếu đang gõ số trong NumericUpDown
        if (ActiveControl is NumericUpDown || ActiveControl is TextBox)
            return;

        bool stateChanged = false;

        switch (e.KeyCode)
        {
            case Keys.W:
            case Keys.Up:
                if (!_isKeyForward) { _isKeyForward = true; stateChanged = true; }
                break;

            case Keys.S:
            case Keys.Down:
                if (!_isKeyBackward) { _isKeyBackward = true; stateChanged = true; }
                break;

            case Keys.A:
            case Keys.Left:
                if (!_isKeyLeft) { _isKeyLeft = true; stateChanged = true; }
                break;

            case Keys.D:
            case Keys.Right:
                if (!_isKeyRight) { _isKeyRight = true; stateChanged = true; }
                break;
        }

        if (stateChanged)
        {
            _controlService.UpdateKeyInput(_isKeyForward, _isKeyBackward, _isKeyLeft, _isKeyRight);
            e.Handled = true;
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);

        if (ActiveControl is NumericUpDown || ActiveControl is TextBox)
            return;

        bool stateChanged = false;

        switch (e.KeyCode)
        {
            case Keys.W:
            case Keys.Up:
                if (_isKeyForward) { _isKeyForward = false; stateChanged = true; }
                break;

            case Keys.S:
            case Keys.Down:
                if (_isKeyBackward) { _isKeyBackward = false; stateChanged = true; }
                break;

            case Keys.A:
            case Keys.Left:
                if (_isKeyLeft) { _isKeyLeft = false; stateChanged = true; }
                break;

            case Keys.D:
            case Keys.Right:
                if (_isKeyRight) { _isKeyRight = false; stateChanged = true; }
                break;
        }

        if (stateChanged)
        {
            _controlService.UpdateKeyInput(_isKeyForward, _isKeyBackward, _isKeyLeft, _isKeyRight);
            e.Handled = true;
        }
    }

    #endregion

    #region Trợ giúp an toàn luồng UI & Ghi Log

    private void SafeUiInvoke(Action action)
    {
        if (IsDisposed || !IsHandleCreated)
            return;

        try
        {
            if (InvokeRequired)
            {
                BeginInvoke(action);
            }
            else
            {
                action();
            }
        }
        catch (ObjectDisposedException) { }
        catch (InvalidOperationException) { }
    }

    private void LogMessage(string text, Color color)
    {
        SafeUiInvoke(() =>
        {
            if (rtbConsole.IsDisposed) return;

            // Giới hạn số dòng console để không làm nặng bộ nhớ
            if (rtbConsole.Lines.Length > 300)
            {
                rtbConsole.Select(0, rtbConsole.GetFirstCharIndexFromLine(50));
                rtbConsole.SelectedText = "";
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string entry = $"[{timestamp}] {text}\n";

            rtbConsole.SelectionStart = rtbConsole.TextLength;
            rtbConsole.SelectionLength = 0;
            rtbConsole.SelectionColor = color;
            rtbConsole.AppendText(entry);
            rtbConsole.ScrollToCaret();
        });
    }

    #endregion
}
