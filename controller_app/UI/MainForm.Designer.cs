using ControllerApp.Modules.M3_Telemetry;

namespace ControllerApp.UI;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    // Header & Connection
    private Panel pnlHeader;
    private Label lblAppTitle;
    private Label lblPort;
    private ComboBox cboPorts;
    private Button btnRefreshPorts;
    private Label lblBaud;
    private ComboBox cboBaudRate;
    private Button btnConnect;
    private Label lblStatusBadge;

    // Main Layout
    private TableLayoutPanel tlpMain;
    private Panel pnlLeftColumn;
    private Panel pnlRightColumn;

    // Telemetry Gauges (M3)
    private Panel pnlTelemetryCard;
    private Label lblTelemetryHeader;
    private TableLayoutPanel tlpGauges;
    private Panel pnlGaugeAngle;
    private Label lblGaugeAngleTitle;
    private Label lblGaugeAngleValue;
    private Panel pnlGaugeGyro;
    private Label lblGaugeGyroTitle;
    private Label lblGaugeGyroValue;
    private Panel pnlGaugePwm;
    private Label lblGaugePwmTitle;
    private Label lblGaugePwmValue;
    private ProgressBar pbPwmLevel;
    private Panel pnlGaugeSafety;
    private Label lblGaugeSafetyTitle;
    private Label lblGaugeSafetyValue;

    // Realtime Graph (M3)
    private Panel pnlGraphCard;
    private Label lblGraphHeader;
    private Button btnClearGraph;
    private RealtimeGraphControl realtimeGraph;

    // Motion Control (M2)
    private Panel pnlControlCard;
    private Label lblControlHeader;
    private Button btnForward;
    private Button btnBackward;
    private Button btnLeft;
    private Button btnRight;
    private Button btnCenterSteer;
    private Button btnEmergencyStop;
    private Label lblMaxSpeed;
    private NumericUpDown numMaxSpeed;
    private TrackBar trackMaxSpeed;
    private Label lblTrim;
    private NumericUpDown numTrim;
    private TrackBar trackTrim;
    private Label lblCurrentCommand;
    private Label lblKeyboardHint;

    // PID & EEPROM (M2)
    private Panel pnlPidCard;
    private Label lblPidHeader;
    private Label lblKp;
    private NumericUpDown numKp;
    private Label lblKi;
    private NumericUpDown numKi;
    private Label lblKd;
    private NumericUpDown numKd;
    private Button btnSendPid;
    private Button btnCalibrate;
    private Button btnSaveEeprom;

    // Serial Terminal / Log
    private Panel pnlLogCard;
    private Label lblLogHeader;
    private RichTextBox rtbConsole;
    private CheckBox chkLogTelemetry;
    private Button btnClearLog;

    // Status Strip
    private StatusStrip statusStrip;
    private ToolStripStatusLabel tslConnectionInfo;
    private ToolStripStatusLabel tslRxCount;
    private ToolStripStatusLabel tslTxCount;
    private ToolStripStatusLabel tslErrorCount;
    private ToolStripStatusLabel tslLastUpdate;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblAppTitle = new Label();
        lblPort = new Label();
        cboPorts = new ComboBox();
        btnRefreshPorts = new Button();
        lblBaud = new Label();
        cboBaudRate = new ComboBox();
        btnConnect = new Button();
        lblStatusBadge = new Label();

        tlpMain = new TableLayoutPanel();
        pnlLeftColumn = new Panel();
        pnlRightColumn = new Panel();

        // Gauges
        pnlTelemetryCard = new Panel();
        lblTelemetryHeader = new Label();
        tlpGauges = new TableLayoutPanel();
        pnlGaugeAngle = new Panel();
        lblGaugeAngleTitle = new Label();
        lblGaugeAngleValue = new Label();
        pnlGaugeGyro = new Panel();
        lblGaugeGyroTitle = new Label();
        lblGaugeGyroValue = new Label();
        pnlGaugePwm = new Panel();
        lblGaugePwmTitle = new Label();
        lblGaugePwmValue = new Label();
        pbPwmLevel = new ProgressBar();
        pnlGaugeSafety = new Panel();
        lblGaugeSafetyTitle = new Label();
        lblGaugeSafetyValue = new Label();

        // Graph
        pnlGraphCard = new Panel();
        lblGraphHeader = new Label();
        btnClearGraph = new Button();
        realtimeGraph = new RealtimeGraphControl();

        // Control
        pnlControlCard = new Panel();
        lblControlHeader = new Label();
        btnForward = new Button();
        btnBackward = new Button();
        btnLeft = new Button();
        btnRight = new Button();
        btnCenterSteer = new Button();
        btnEmergencyStop = new Button();
        lblMaxSpeed = new Label();
        numMaxSpeed = new NumericUpDown();
        trackMaxSpeed = new TrackBar();
        lblTrim = new Label();
        numTrim = new NumericUpDown();
        trackTrim = new TrackBar();
        lblCurrentCommand = new Label();
        lblKeyboardHint = new Label();

        // PID
        pnlPidCard = new Panel();
        lblPidHeader = new Label();
        lblKp = new Label();
        numKp = new NumericUpDown();
        lblKi = new Label();
        numKi = new NumericUpDown();
        lblKd = new Label();
        numKd = new NumericUpDown();
        btnSendPid = new Button();
        btnCalibrate = new Button();
        btnSaveEeprom = new Button();

        // Log
        pnlLogCard = new Panel();
        lblLogHeader = new Label();
        rtbConsole = new RichTextBox();
        chkLogTelemetry = new CheckBox();
        btnClearLog = new Button();

        // Status Strip
        statusStrip = new StatusStrip();
        tslConnectionInfo = new ToolStripStatusLabel();
        tslRxCount = new ToolStripStatusLabel();
        tslTxCount = new ToolStripStatusLabel();
        tslErrorCount = new ToolStripStatusLabel();
        tslLastUpdate = new ToolStripStatusLabel();

        pnlHeader.SuspendLayout();
        tlpMain.SuspendLayout();
        pnlLeftColumn.SuspendLayout();
        pnlRightColumn.SuspendLayout();
        pnlTelemetryCard.SuspendLayout();
        tlpGauges.SuspendLayout();
        pnlGaugeAngle.SuspendLayout();
        pnlGaugeGyro.SuspendLayout();
        pnlGaugePwm.SuspendLayout();
        pnlGaugeSafety.SuspendLayout();
        pnlGraphCard.SuspendLayout();
        pnlControlCard.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numMaxSpeed).BeginInit();
        ((System.ComponentModel.ISupportInitialize)trackMaxSpeed).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numTrim).BeginInit();
        ((System.ComponentModel.ISupportInitialize)trackTrim).BeginInit();
        pnlPidCard.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numKp).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numKi).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numKd).BeginInit();
        pnlLogCard.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();

        // ================= HEADER & CONNECTION (M1) =================
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Height = 56;
        pnlHeader.BackColor = ModernTheme.CardHeader;
        pnlHeader.Padding = new Padding(12, 10, 12, 10);

        lblAppTitle.Text = "🚴 SELF-BALANCING BIKE CONTROLLER";
        lblAppTitle.Font = ModernTheme.HeaderFont;
        lblAppTitle.ForeColor = ModernTheme.TextPrimary;
        lblAppTitle.AutoSize = true;
        lblAppTitle.Location = new Point(14, 16);

        lblPort.Text = "Cổng COM:";
        lblPort.Font = ModernTheme.BodyFont;
        lblPort.ForeColor = ModernTheme.TextSecondary;
        lblPort.AutoSize = true;
        lblPort.Location = new Point(400, 18);

        cboPorts.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPorts.BackColor = ModernTheme.InputBackground;
        cboPorts.ForeColor = ModernTheme.TextPrimary;
        cboPorts.FlatStyle = FlatStyle.Flat;
        cboPorts.Font = ModernTheme.BodyFont;
        cboPorts.Size = new Size(110, 26);
        cboPorts.Location = new Point(475, 14);

        btnRefreshPorts.Text = "🔄";
        btnRefreshPorts.Size = new Size(32, 27);
        btnRefreshPorts.Location = new Point(590, 13);
        btnRefreshPorts.BackColor = ModernTheme.CardBackground;
        btnRefreshPorts.ForeColor = ModernTheme.TextPrimary;
        btnRefreshPorts.FlatStyle = FlatStyle.Flat;
        btnRefreshPorts.FlatAppearance.BorderColor = ModernTheme.BorderColor;

        lblBaud.Text = "Baud:";
        lblBaud.Font = ModernTheme.BodyFont;
        lblBaud.ForeColor = ModernTheme.TextSecondary;
        lblBaud.AutoSize = true;
        lblBaud.Location = new Point(635, 18);

        cboBaudRate.DropDownStyle = ComboBoxStyle.DropDownList;
        cboBaudRate.BackColor = ModernTheme.InputBackground;
        cboBaudRate.ForeColor = ModernTheme.TextPrimary;
        cboBaudRate.FlatStyle = FlatStyle.Flat;
        cboBaudRate.Font = ModernTheme.BodyFont;
        cboBaudRate.Size = new Size(88, 26);
        cboBaudRate.Location = new Point(678, 14);
        cboBaudRate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
        cboBaudRate.SelectedIndex = 0; // 9600

        btnConnect.Text = "⚡ KẾT NỐI";
        btnConnect.Size = new Size(115, 30);
        btnConnect.Location = new Point(780, 12);
        btnConnect.BackColor = ModernTheme.SuccessGreen;
        btnConnect.ForeColor = Color.White;
        btnConnect.Font = ModernTheme.SubHeaderFont;
        btnConnect.FlatStyle = FlatStyle.Flat;
        btnConnect.FlatAppearance.BorderSize = 0;
        btnConnect.Cursor = Cursors.Hand;

        lblStatusBadge.Text = "● ĐÃ NGẮT KẾT NỐI";
        lblStatusBadge.Font = ModernTheme.SubHeaderFont;
        lblStatusBadge.ForeColor = ModernTheme.DangerRed;
        lblStatusBadge.AutoSize = true;
        lblStatusBadge.Location = new Point(915, 18);

        pnlHeader.Controls.AddRange(new Control[] {
            lblAppTitle, lblPort, cboPorts, btnRefreshPorts, lblBaud, cboBaudRate, btnConnect, lblStatusBadge
        });

        // ================= MAIN LAYOUT =================
        tlpMain.Dock = DockStyle.Fill;
        tlpMain.ColumnCount = 2;
        tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
        tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
        tlpMain.RowCount = 1;
        tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tlpMain.Controls.Add(pnlLeftColumn, 0, 0);
        tlpMain.Controls.Add(pnlRightColumn, 1, 0);

        pnlLeftColumn.Dock = DockStyle.Fill;
        pnlLeftColumn.AutoScroll = true;
        pnlLeftColumn.Padding = new Padding(10);

        pnlRightColumn.Dock = DockStyle.Fill;
        pnlRightColumn.AutoScroll = true;
        pnlRightColumn.Padding = new Padding(10);

        // ================= LEFT: TELEMETRY GAUGES (M3) =================
        pnlTelemetryCard.Dock = DockStyle.Top;
        pnlTelemetryCard.Height = 135;
        pnlTelemetryCard.BackColor = ModernTheme.CardBackground;
        pnlTelemetryCard.Padding = new Padding(10);

        lblTelemetryHeader.Text = "📊 DỮ LIỆU TELEMETRY XE CÂN BẰNG (M3)";
        lblTelemetryHeader.Font = ModernTheme.SubHeaderFont;
        lblTelemetryHeader.ForeColor = ModernTheme.PrimaryAccent;
        lblTelemetryHeader.Dock = DockStyle.Top;
        lblTelemetryHeader.Height = 25;

        tlpGauges.Dock = DockStyle.Fill;
        tlpGauges.ColumnCount = 4;
        tlpGauges.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tlpGauges.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tlpGauges.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tlpGauges.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tlpGauges.RowCount = 1;
        tlpGauges.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // Gauge: Angle
        pnlGaugeAngle.Dock = DockStyle.Fill;
        pnlGaugeAngle.BackColor = ModernTheme.InputBackground;
        pnlGaugeAngle.Margin = new Padding(4);
        lblGaugeAngleTitle.Text = "GÓC NGHIÊNG";
        lblGaugeAngleTitle.Font = ModernTheme.BodyFont;
        lblGaugeAngleTitle.ForeColor = ModernTheme.TextSecondary;
        lblGaugeAngleTitle.Dock = DockStyle.Top;
        lblGaugeAngleTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblGaugeAngleTitle.Height = 22;
        lblGaugeAngleValue.Text = "0.00°";
        lblGaugeAngleValue.Font = ModernTheme.LargeNumberFont;
        lblGaugeAngleValue.ForeColor = ModernTheme.SuccessGreen;
        lblGaugeAngleValue.Dock = DockStyle.Fill;
        lblGaugeAngleValue.TextAlign = ContentAlignment.MiddleCenter;
        pnlGaugeAngle.Controls.AddRange(new Control[] { lblGaugeAngleValue, lblGaugeAngleTitle });

        // Gauge: Gyro Rate
        pnlGaugeGyro.Dock = DockStyle.Fill;
        pnlGaugeGyro.BackColor = ModernTheme.InputBackground;
        pnlGaugeGyro.Margin = new Padding(4);
        lblGaugeGyroTitle.Text = "VẬN TỐC GÓC";
        lblGaugeGyroTitle.Font = ModernTheme.BodyFont;
        lblGaugeGyroTitle.ForeColor = ModernTheme.TextSecondary;
        lblGaugeGyroTitle.Dock = DockStyle.Top;
        lblGaugeGyroTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblGaugeGyroTitle.Height = 22;
        lblGaugeGyroValue.Text = "0.00°/s";
        lblGaugeGyroValue.Font = ModernTheme.LargeNumberFont;
        lblGaugeGyroValue.ForeColor = ModernTheme.PrimaryAccent;
        lblGaugeGyroValue.Dock = DockStyle.Fill;
        lblGaugeGyroValue.TextAlign = ContentAlignment.MiddleCenter;
        pnlGaugeGyro.Controls.AddRange(new Control[] { lblGaugeGyroValue, lblGaugeGyroTitle });

        // Gauge: Reaction Wheel PWM
        pnlGaugePwm.Dock = DockStyle.Fill;
        pnlGaugePwm.BackColor = ModernTheme.InputBackground;
        pnlGaugePwm.Margin = new Padding(4);
        lblGaugePwmTitle.Text = "PWM BÁNH ĐÀ";
        lblGaugePwmTitle.Font = ModernTheme.BodyFont;
        lblGaugePwmTitle.ForeColor = ModernTheme.TextSecondary;
        lblGaugePwmTitle.Dock = DockStyle.Top;
        lblGaugePwmTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblGaugePwmTitle.Height = 22;
        lblGaugePwmValue.Text = "0";
        lblGaugePwmValue.Font = ModernTheme.LargeNumberFont;
        lblGaugePwmValue.ForeColor = ModernTheme.WarningYellow;
        lblGaugePwmValue.Dock = DockStyle.Top;
        lblGaugePwmValue.TextAlign = ContentAlignment.MiddleCenter;
        lblGaugePwmValue.Height = 35;
        pbPwmLevel.Dock = DockStyle.Bottom;
        pbPwmLevel.Height = 8;
        pbPwmLevel.Maximum = 255;
        pbPwmLevel.Value = 0;
        pnlGaugePwm.Controls.AddRange(new Control[] { lblGaugePwmValue, pbPwmLevel, lblGaugePwmTitle });

        // Gauge: Safety Status
        pnlGaugeSafety.Dock = DockStyle.Fill;
        pnlGaugeSafety.BackColor = ModernTheme.InputBackground;
        pnlGaugeSafety.Margin = new Padding(4);
        lblGaugeSafetyTitle.Text = "TRẠNG THÁI";
        lblGaugeSafetyTitle.Font = ModernTheme.BodyFont;
        lblGaugeSafetyTitle.ForeColor = ModernTheme.TextSecondary;
        lblGaugeSafetyTitle.Dock = DockStyle.Top;
        lblGaugeSafetyTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblGaugeSafetyTitle.Height = 22;
        lblGaugeSafetyValue.Text = "SẴN SÀNG";
        lblGaugeSafetyValue.Font = ModernTheme.SubHeaderFont;
        lblGaugeSafetyValue.ForeColor = ModernTheme.SuccessGreen;
        lblGaugeSafetyValue.Dock = DockStyle.Fill;
        lblGaugeSafetyValue.TextAlign = ContentAlignment.MiddleCenter;
        pnlGaugeSafety.Controls.AddRange(new Control[] { lblGaugeSafetyValue, lblGaugeSafetyTitle });

        tlpGauges.Controls.Add(pnlGaugeAngle, 0, 0);
        tlpGauges.Controls.Add(pnlGaugeGyro, 1, 0);
        tlpGauges.Controls.Add(pnlGaugePwm, 2, 0);
        tlpGauges.Controls.Add(pnlGaugeSafety, 3, 0);

        pnlTelemetryCard.Controls.Add(tlpGauges);
        pnlTelemetryCard.Controls.Add(lblTelemetryHeader);

        // ================= LEFT: REALTIME GRAPH (M3) =================
        pnlGraphCard.Dock = DockStyle.Top;
        pnlGraphCard.Height = 290;
        pnlGraphCard.BackColor = ModernTheme.CardBackground;
        pnlGraphCard.Padding = new Padding(10);
        pnlGraphCard.Margin = new Padding(0, 10, 0, 0);

        lblGraphHeader.Text = "📈 BIỂU ĐỒ GÓC NGHIÊNG THỜI GIAN THỰC (SO VỚI 0°)";
        lblGraphHeader.Font = ModernTheme.SubHeaderFont;
        lblGraphHeader.ForeColor = ModernTheme.PrimaryAccent;
        lblGraphHeader.Location = new Point(10, 10);
        lblGraphHeader.AutoSize = true;

        btnClearGraph.Text = "Xóa đồ thị";
        btnClearGraph.Size = new Size(80, 24);
        btnClearGraph.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnClearGraph.Location = new Point(pnlGraphCard.Width - 95, 8);
        btnClearGraph.BackColor = ModernTheme.InputBackground;
        btnClearGraph.ForeColor = ModernTheme.TextSecondary;
        btnClearGraph.FlatStyle = FlatStyle.Flat;
        btnClearGraph.FlatAppearance.BorderColor = ModernTheme.BorderColor;

        realtimeGraph.Dock = DockStyle.Bottom;
        realtimeGraph.Height = 245;

        pnlGraphCard.Controls.AddRange(new Control[] { lblGraphHeader, btnClearGraph, realtimeGraph });

        // ================= LEFT: CONSOLE LOG =================
        pnlLogCard.Dock = DockStyle.Fill;
        pnlLogCard.BackColor = ModernTheme.CardBackground;
        pnlLogCard.Padding = new Padding(10);
        pnlLogCard.Margin = new Padding(0, 10, 0, 0);

        lblLogHeader.Text = "📋 NHẬT KÝ TRUYỀN THÔNG SERIAL / BLUETOOTH";
        lblLogHeader.Font = ModernTheme.SubHeaderFont;
        lblLogHeader.ForeColor = ModernTheme.PrimaryAccent;
        lblLogHeader.Location = new Point(10, 10);
        lblLogHeader.AutoSize = true;

        chkLogTelemetry.Text = "Hiện Telemetry D:...";
        chkLogTelemetry.Font = ModernTheme.BodyFont;
        chkLogTelemetry.ForeColor = ModernTheme.TextSecondary;
        chkLogTelemetry.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        chkLogTelemetry.Location = new Point(340, 8);
        chkLogTelemetry.AutoSize = true;
        chkLogTelemetry.Checked = false; // Mặc định tắt lọc bớt spam để dễ nhìn ack

        btnClearLog.Text = "Xóa Log";
        btnClearLog.Size = new Size(70, 24);
        btnClearLog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnClearLog.Location = new Point(485, 8);
        btnClearLog.BackColor = ModernTheme.InputBackground;
        btnClearLog.ForeColor = ModernTheme.TextSecondary;
        btnClearLog.FlatStyle = FlatStyle.Flat;
        btnClearLog.FlatAppearance.BorderColor = ModernTheme.BorderColor;

        rtbConsole.Dock = DockStyle.Bottom;
        rtbConsole.Height = 160;
        rtbConsole.BackColor = Color.FromArgb(12, 15, 22);
        rtbConsole.ForeColor = Color.FromArgb(200, 220, 240);
        rtbConsole.Font = ModernTheme.MonospaceFont;
        rtbConsole.ReadOnly = true;
        rtbConsole.BorderStyle = BorderStyle.None;

        pnlLogCard.Controls.AddRange(new Control[] { lblLogHeader, chkLogTelemetry, btnClearLog, rtbConsole });

        // Add to Left Column
        pnlLeftColumn.Controls.Add(pnlLogCard);
        pnlLeftColumn.Controls.Add(pnlGraphCard);
        pnlLeftColumn.Controls.Add(pnlTelemetryCard);

        // ================= RIGHT: MOTION CONTROL (M2) =================
        pnlControlCard.Dock = DockStyle.Top;
        pnlControlCard.Height = 365;
        pnlControlCard.BackColor = ModernTheme.CardBackground;
        pnlControlCard.Padding = new Padding(12);

        lblControlHeader.Text = "🎮 ĐIỀU KHIỂN DI CHUYỂN & LÁI (M2)";
        lblControlHeader.Font = ModernTheme.SubHeaderFont;
        lblControlHeader.ForeColor = ModernTheme.PrimaryAccent;
        lblControlHeader.Dock = DockStyle.Top;
        lblControlHeader.Height = 24;

        // D-pad Buttons
        btnForward.Text = "▲ TIẾN (W)";
        btnForward.Size = new Size(110, 42);
        btnForward.Location = new Point(145, 38);
        btnForward.BackColor = ModernTheme.InputBackground;
        btnForward.ForeColor = ModernTheme.TextPrimary;
        btnForward.Font = ModernTheme.SubHeaderFont;
        btnForward.FlatStyle = FlatStyle.Flat;
        btnForward.FlatAppearance.BorderColor = ModernTheme.BorderColor;

        btnLeft.Text = "◄ TRÁI (A)";
        btnLeft.Size = new Size(100, 42);
        btnLeft.Location = new Point(40, 84);
        btnLeft.BackColor = ModernTheme.InputBackground;
        btnLeft.ForeColor = ModernTheme.TextPrimary;
        btnLeft.Font = ModernTheme.SubHeaderFont;
        btnLeft.FlatStyle = FlatStyle.Flat;
        btnLeft.FlatAppearance.BorderColor = ModernTheme.BorderColor;

        btnCenterSteer.Text = "◉ THẲNG";
        btnCenterSteer.Size = new Size(95, 42);
        btnCenterSteer.Location = new Point(152, 84);
        btnCenterSteer.BackColor = ModernTheme.CardHeader;
        btnCenterSteer.ForeColor = ModernTheme.PrimaryAccent;
        btnCenterSteer.Font = ModernTheme.SubHeaderFont;
        btnCenterSteer.FlatStyle = FlatStyle.Flat;
        btnCenterSteer.FlatAppearance.BorderColor = ModernTheme.BorderColor;

        btnRight.Text = "PHẢI (D) ►";
        btnRight.Size = new Size(100, 42);
        btnRight.Location = new Point(260, 84);
        btnRight.BackColor = ModernTheme.InputBackground;
        btnRight.ForeColor = ModernTheme.TextPrimary;
        btnRight.Font = ModernTheme.SubHeaderFont;
        btnRight.FlatStyle = FlatStyle.Flat;
        btnRight.FlatAppearance.BorderColor = ModernTheme.BorderColor;

        btnBackward.Text = "▼ LÙI (S)";
        btnBackward.Size = new Size(110, 42);
        btnBackward.Location = new Point(145, 130);
        btnBackward.BackColor = ModernTheme.InputBackground;
        btnBackward.ForeColor = ModernTheme.TextPrimary;
        btnBackward.Font = ModernTheme.SubHeaderFont;
        btnBackward.FlatStyle = FlatStyle.Flat;
        btnBackward.FlatAppearance.BorderColor = ModernTheme.BorderColor;

        // EMERGENCY STOP BUTTON
        btnEmergencyStop.Text = "🛑 DỪNG KHẨN CẤP (SPACEBAR)";
        btnEmergencyStop.Size = new Size(320, 44);
        btnEmergencyStop.Location = new Point(40, 180);
        btnEmergencyStop.BackColor = ModernTheme.DangerRed;
        btnEmergencyStop.ForeColor = Color.White;
        btnEmergencyStop.Font = ModernTheme.SubHeaderFont;
        btnEmergencyStop.FlatStyle = FlatStyle.Flat;
        btnEmergencyStop.FlatAppearance.BorderSize = 0;
        btnEmergencyStop.Cursor = Cursors.Hand;

        // Speed limit
        lblMaxSpeed.Text = "Tốc độ tối đa (Max Speed):";
        lblMaxSpeed.Font = ModernTheme.BodyFont;
        lblMaxSpeed.ForeColor = ModernTheme.TextSecondary;
        lblMaxSpeed.Location = new Point(20, 235);
        lblMaxSpeed.AutoSize = true;

        numMaxSpeed.Minimum = 0;
        numMaxSpeed.Maximum = 255;
        numMaxSpeed.Value = 160;
        numMaxSpeed.Size = new Size(60, 24);
        numMaxSpeed.Location = new Point(310, 233);
        numMaxSpeed.BackColor = ModernTheme.InputBackground;
        numMaxSpeed.ForeColor = ModernTheme.TextPrimary;

        trackMaxSpeed.Minimum = 0;
        trackMaxSpeed.Maximum = 255;
        trackMaxSpeed.Value = 160;
        trackMaxSpeed.TickFrequency = 25;
        trackMaxSpeed.Size = new Size(280, 28);
        trackMaxSpeed.Location = new Point(20, 258);

        // Steering Trim
        lblTrim.Text = "Hiệu chỉnh lệch tâm lái (Trim °):";
        lblTrim.Font = ModernTheme.BodyFont;
        lblTrim.ForeColor = ModernTheme.TextSecondary;
        lblTrim.Location = new Point(20, 292);
        lblTrim.AutoSize = true;

        numTrim.Minimum = -25;
        numTrim.Maximum = 25;
        numTrim.Value = 0;
        numTrim.Size = new Size(60, 24);
        numTrim.Location = new Point(310, 290);
        numTrim.BackColor = ModernTheme.InputBackground;
        numTrim.ForeColor = ModernTheme.TextPrimary;

        trackTrim.Minimum = -25;
        trackTrim.Maximum = 25;
        trackTrim.Value = 0;
        trackTrim.TickFrequency = 5;
        trackTrim.Size = new Size(280, 28);
        trackTrim.Location = new Point(20, 315);

        lblCurrentCommand.Text = "Lệnh hiện hành: M:0,90\\n (Tiết chế ~90ms)";
        lblCurrentCommand.Font = ModernTheme.MonospaceFont;
        lblCurrentCommand.ForeColor = ModernTheme.TextMuted;
        lblCurrentCommand.Location = new Point(20, 344);
        lblCurrentCommand.AutoSize = true;

        pnlControlCard.Controls.AddRange(new Control[] {
            lblControlHeader, btnForward, btnLeft, btnCenterSteer, btnRight, btnBackward,
            btnEmergencyStop, lblMaxSpeed, numMaxSpeed, trackMaxSpeed, lblTrim, numTrim, trackTrim,
            lblCurrentCommand
        });

        // ================= RIGHT: PID TUNING & EEPROM (M2) =================
        pnlPidCard.Dock = DockStyle.Fill;
        pnlPidCard.BackColor = ModernTheme.CardBackground;
        pnlPidCard.Padding = new Padding(12);
        pnlPidCard.Margin = new Padding(0, 10, 0, 0);

        lblPidHeader.Text = "⚙️ THÔNG SỐ PID CÂN BẰNG & EEPROM (M2)";
        lblPidHeader.Font = ModernTheme.SubHeaderFont;
        lblPidHeader.ForeColor = ModernTheme.PrimaryAccent;
        lblPidHeader.Dock = DockStyle.Top;
        lblPidHeader.Height = 25;

        // Kp
        lblKp.Text = "Kp (Hệ số tỉ lệ):";
        lblKp.Font = ModernTheme.BodyFont;
        lblKp.ForeColor = ModernTheme.TextSecondary;
        lblKp.Location = new Point(20, 38);
        lblKp.AutoSize = true;

        numKp.DecimalPlaces = 2;
        numKp.Increment = 0.5M;
        numKp.Minimum = 0M;
        numKp.Maximum = 200M;
        numKp.Value = 35.00M;
        numKp.Size = new Size(80, 24);
        numKp.Location = new Point(180, 35);
        numKp.BackColor = ModernTheme.InputBackground;
        numKp.ForeColor = ModernTheme.TextPrimary;

        // Ki
        lblKi.Text = "Ki (Hệ số tích phân):";
        lblKi.Font = ModernTheme.BodyFont;
        lblKi.ForeColor = ModernTheme.TextSecondary;
        lblKi.Location = new Point(20, 70);
        lblKi.AutoSize = true;

        numKi.DecimalPlaces = 2;
        numKi.Increment = 0.1M;
        numKi.Minimum = 0M;
        numKi.Maximum = 50M;
        numKi.Value = 0.60M;
        numKi.Size = new Size(80, 24);
        numKi.Location = new Point(180, 67);
        numKi.BackColor = ModernTheme.InputBackground;
        numKi.ForeColor = ModernTheme.TextPrimary;

        // Kd
        lblKd.Text = "Kd (Hệ số vi phân):";
        lblKd.Font = ModernTheme.BodyFont;
        lblKd.ForeColor = ModernTheme.TextSecondary;
        lblKd.Location = new Point(20, 102);
        lblKd.AutoSize = true;

        numKd.DecimalPlaces = 2;
        numKd.Increment = 0.1M;
        numKd.Minimum = 0M;
        numKd.Maximum = 50M;
        numKd.Value = 1.25M;
        numKd.Size = new Size(80, 24);
        numKd.Location = new Point(180, 99);
        numKd.BackColor = ModernTheme.InputBackground;
        numKd.ForeColor = ModernTheme.TextPrimary;

        // Send PID button
        btnSendPid.Text = "📤 GỬI THAM SỐ PID (P:...)";
        btnSendPid.Size = new Size(150, 85);
        btnSendPid.Location = new Point(275, 36);
        btnSendPid.BackColor = ModernTheme.PrimaryAccent;
        btnSendPid.ForeColor = Color.White;
        btnSendPid.Font = ModernTheme.SubHeaderFont;
        btnSendPid.FlatStyle = FlatStyle.Flat;
        btnSendPid.FlatAppearance.BorderSize = 0;
        btnSendPid.Cursor = Cursors.Hand;

        // Calibrate & Save Buttons
        btnCalibrate.Text = "⚖️ HIỆU CHUẨN GÓC (CAL)";
        btnCalibrate.Size = new Size(190, 36);
        btnCalibrate.Location = new Point(20, 138);
        btnCalibrate.BackColor = ModernTheme.InputBackground;
        btnCalibrate.ForeColor = ModernTheme.WarningYellow;
        btnCalibrate.Font = ModernTheme.SubHeaderFont;
        btnCalibrate.FlatStyle = FlatStyle.Flat;
        btnCalibrate.FlatAppearance.BorderColor = ModernTheme.BorderColor;
        btnCalibrate.Cursor = Cursors.Hand;

        btnSaveEeprom.Text = "💾 LƯU EEPROM (SAVE)";
        btnSaveEeprom.Size = new Size(195, 36);
        btnSaveEeprom.Location = new Point(230, 138);
        btnSaveEeprom.BackColor = ModernTheme.InputBackground;
        btnSaveEeprom.ForeColor = ModernTheme.SuccessGreen;
        btnSaveEeprom.Font = ModernTheme.SubHeaderFont;
        btnSaveEeprom.FlatStyle = FlatStyle.Flat;
        btnSaveEeprom.FlatAppearance.BorderColor = ModernTheme.BorderColor;
        btnSaveEeprom.Cursor = Cursors.Hand;

        lblKeyboardHint.Text = "⌨️ Phím W/A/S/D hoặc Mũi tên: Điều khiển di chuyển | Space: Dừng khẩn cấp";
        lblKeyboardHint.Font = ModernTheme.BodyFont;
        lblKeyboardHint.ForeColor = ModernTheme.TextMuted;
        lblKeyboardHint.Location = new Point(20, 185);
        lblKeyboardHint.AutoSize = true;

        pnlPidCard.Controls.AddRange(new Control[] {
            lblPidHeader, lblKp, numKp, lblKi, numKi, lblKd, numKd, btnSendPid,
            btnCalibrate, btnSaveEeprom, lblKeyboardHint
        });

        // Add to Right Column
        pnlRightColumn.Controls.Add(pnlPidCard);
        pnlRightColumn.Controls.Add(pnlControlCard);

        // ================= STATUS STRIP =================
        statusStrip.BackColor = ModernTheme.CardHeader;
        statusStrip.ForeColor = ModernTheme.TextSecondary;
        statusStrip.Font = ModernTheme.BodyFont;
        statusStrip.SizingGrip = true;

        tslConnectionInfo.Text = "Cổng: Chưa kết nối";
        tslConnectionInfo.BorderSides = ToolStripStatusLabelBorderSides.Right;
        tslConnectionInfo.BorderStyle = Border3DStyle.Etched;

        tslRxCount.Text = "RX: 0 gói";
        tslRxCount.BorderSides = ToolStripStatusLabelBorderSides.Right;
        tslRxCount.BorderStyle = Border3DStyle.Etched;

        tslTxCount.Text = "TX: 0 lệnh";
        tslTxCount.BorderSides = ToolStripStatusLabelBorderSides.Right;
        tslTxCount.BorderStyle = Border3DStyle.Etched;

        tslErrorCount.Text = "Lỗi: 0";
        tslErrorCount.BorderSides = ToolStripStatusLabelBorderSides.Right;
        tslErrorCount.BorderStyle = Border3DStyle.Etched;

        tslLastUpdate.Text = "Thời gian: --:--:--";
        tslLastUpdate.Spring = true;
        tslLastUpdate.TextAlign = ContentAlignment.MiddleRight;

        statusStrip.Items.AddRange(new ToolStripItem[] {
            tslConnectionInfo, tslRxCount, tslTxCount, tslErrorCount, tslLastUpdate
        });

        // ================= MAIN FORM CONFIG =================
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1180, 760);
        MinimumSize = new Size(1000, 700);
        BackColor = ModernTheme.BackgroundDark;
        ForeColor = ModernTheme.TextPrimary;
        Font = ModernTheme.BodyFont;
        Text = "Self-Balancing Bike Controller & Telemetry Monitor (G1: M1, M2, M3)";
        KeyPreview = true; // Bắt buộc để nhận diện phím điều khiển W/A/S/D trên toàn form

        Controls.Add(tlpMain);
        Controls.Add(pnlHeader);
        Controls.Add(statusStrip);

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        tlpMain.ResumeLayout(false);
        pnlLeftColumn.ResumeLayout(false);
        pnlRightColumn.ResumeLayout(false);
        pnlTelemetryCard.ResumeLayout(false);
        tlpGauges.ResumeLayout(false);
        pnlGaugeAngle.ResumeLayout(false);
        pnlGaugeGyro.ResumeLayout(false);
        pnlGaugePwm.ResumeLayout(false);
        pnlGaugeSafety.ResumeLayout(false);
        pnlGraphCard.ResumeLayout(false);
        pnlGraphCard.PerformLayout();
        pnlControlCard.ResumeLayout(false);
        pnlControlCard.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numMaxSpeed).EndInit();
        ((System.ComponentModel.ISupportInitialize)trackMaxSpeed).EndInit();
        ((System.ComponentModel.ISupportInitialize)numTrim).EndInit();
        ((System.ComponentModel.ISupportInitialize)trackTrim).EndInit();
        pnlPidCard.ResumeLayout(false);
        pnlPidCard.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numKp).EndInit();
        ((System.ComponentModel.ISupportInitialize)numKi).EndInit();
        ((System.ComponentModel.ISupportInitialize)numKd).EndInit();
        pnlLogCard.ResumeLayout(false);
        pnlLogCard.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
