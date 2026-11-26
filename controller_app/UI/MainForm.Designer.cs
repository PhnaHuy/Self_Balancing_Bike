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
        pnlLogCard = new Panel();
        lblLogHeader = new Label();
        chkLogTelemetry = new CheckBox();
        btnClearLog = new Button();
        rtbConsole = new RichTextBox();
        pnlGraphCard = new Panel();
        lblGraphHeader = new Label();
        btnClearGraph = new Button();
        realtimeGraph = new RealtimeGraphControl();
        pnlTelemetryCard = new Panel();
        tlpGauges = new TableLayoutPanel();
        pnlGaugeAngle = new Panel();
        lblGaugeAngleValue = new Label();
        lblGaugeAngleTitle = new Label();
        pnlGaugeGyro = new Panel();
        lblGaugeGyroValue = new Label();
        lblGaugeGyroTitle = new Label();
        pnlGaugePwm = new Panel();
        lblGaugePwmValue = new Label();
        pbPwmLevel = new ProgressBar();
        lblGaugePwmTitle = new Label();
        pnlGaugeSafety = new Panel();
        lblGaugeSafetyValue = new Label();
        lblGaugeSafetyTitle = new Label();
        lblTelemetryHeader = new Label();
        pnlRightColumn = new Panel();
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
        lblKeyboardHint = new Label();
        pnlControlCard = new Panel();
        lblControlHeader = new Label();
        btnForward = new Button();
        btnLeft = new Button();
        btnCenterSteer = new Button();
        btnRight = new Button();
        btnBackward = new Button();
        btnEmergencyStop = new Button();
        lblMaxSpeed = new Label();
        numMaxSpeed = new NumericUpDown();
        trackMaxSpeed = new TrackBar();
        lblTrim = new Label();
        numTrim = new NumericUpDown();
        trackTrim = new TrackBar();
        lblCurrentCommand = new Label();
        statusStrip = new StatusStrip();
        tslConnectionInfo = new ToolStripStatusLabel();
        tslRxCount = new ToolStripStatusLabel();
        tslTxCount = new ToolStripStatusLabel();
        tslErrorCount = new ToolStripStatusLabel();
        tslLastUpdate = new ToolStripStatusLabel();
        pnlHeader.SuspendLayout();
        tlpMain.SuspendLayout();
        pnlLeftColumn.SuspendLayout();
        pnlLogCard.SuspendLayout();
        pnlGraphCard.SuspendLayout();
        pnlTelemetryCard.SuspendLayout();
        tlpGauges.SuspendLayout();
        pnlGaugeAngle.SuspendLayout();
        pnlGaugeGyro.SuspendLayout();
        pnlGaugePwm.SuspendLayout();
        pnlGaugeSafety.SuspendLayout();
        pnlRightColumn.SuspendLayout();
        pnlPidCard.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numKp).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numKi).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numKd).BeginInit();
        pnlControlCard.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numMaxSpeed).BeginInit();
        ((System.ComponentModel.ISupportInitialize)trackMaxSpeed).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numTrim).BeginInit();
        ((System.ComponentModel.ISupportInitialize)trackTrim).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = Color.FromArgb(32, 40, 56);
        pnlHeader.Controls.Add(lblAppTitle);
        pnlHeader.Controls.Add(lblPort);
        pnlHeader.Controls.Add(cboPorts);
        pnlHeader.Controls.Add(btnRefreshPorts);
        pnlHeader.Controls.Add(lblBaud);
        pnlHeader.Controls.Add(cboBaudRate);
        pnlHeader.Controls.Add(btnConnect);
        pnlHeader.Controls.Add(lblStatusBadge);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Margin = new Padding(3, 4, 3, 4);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Padding = new Padding(14, 13, 14, 13);
        pnlHeader.Size = new Size(1349, 75);
        pnlHeader.TabIndex = 1;
        // 
        // lblAppTitle
        // 
        lblAppTitle.AutoSize = true;
        lblAppTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblAppTitle.ForeColor = Color.FromArgb(240, 244, 250);
        lblAppTitle.Location = new Point(16, 21);
        lblAppTitle.Name = "lblAppTitle";
        lblAppTitle.Size = new Size(368, 25);
        lblAppTitle.TabIndex = 0;
        lblAppTitle.Text = "🚴 SELF-BALANCING BIKE CONTROLLER";
        // 
        // lblPort
        // 
        lblPort.AutoSize = true;
        lblPort.Font = new Font("Segoe UI", 9F);
        lblPort.ForeColor = Color.FromArgb(155, 170, 195);
        lblPort.Location = new Point(457, 24);
        lblPort.Name = "lblPort";
        lblPort.Size = new Size(84, 20);
        lblPort.TabIndex = 1;
        lblPort.Text = "Cổng COM:";
        // 
        // cboPorts
        // 
        cboPorts.BackColor = Color.FromArgb(35, 44, 62);
        cboPorts.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPorts.FlatStyle = FlatStyle.Flat;
        cboPorts.Font = new Font("Segoe UI", 9F);
        cboPorts.ForeColor = Color.FromArgb(240, 244, 250);
        cboPorts.Location = new Point(543, 19);
        cboPorts.Margin = new Padding(3, 4, 3, 4);
        cboPorts.Name = "cboPorts";
        cboPorts.Size = new Size(125, 28);
        cboPorts.TabIndex = 2;
        // 
        // btnRefreshPorts
        // 
        btnRefreshPorts.BackColor = Color.FromArgb(24, 30, 42);
        btnRefreshPorts.FlatAppearance.BorderColor = Color.FromArgb(50, 62, 85);
        btnRefreshPorts.FlatStyle = FlatStyle.Flat;
        btnRefreshPorts.ForeColor = Color.FromArgb(240, 244, 250);
        btnRefreshPorts.Location = new Point(674, 17);
        btnRefreshPorts.Margin = new Padding(3, 4, 3, 4);
        btnRefreshPorts.Name = "btnRefreshPorts";
        btnRefreshPorts.Size = new Size(37, 36);
        btnRefreshPorts.TabIndex = 3;
        btnRefreshPorts.Text = "🔄";
        btnRefreshPorts.UseVisualStyleBackColor = false;
        // 
        // lblBaud
        // 
        lblBaud.AutoSize = true;
        lblBaud.Font = new Font("Segoe UI", 9F);
        lblBaud.ForeColor = Color.FromArgb(155, 170, 195);
        lblBaud.Location = new Point(726, 24);
        lblBaud.Name = "lblBaud";
        lblBaud.Size = new Size(46, 20);
        lblBaud.TabIndex = 4;
        lblBaud.Text = "Baud:";
        // 
        // cboBaudRate
        // 
        cboBaudRate.BackColor = Color.FromArgb(35, 44, 62);
        cboBaudRate.DropDownStyle = ComboBoxStyle.DropDownList;
        cboBaudRate.FlatStyle = FlatStyle.Flat;
        cboBaudRate.Font = new Font("Segoe UI", 9F);
        cboBaudRate.ForeColor = Color.FromArgb(240, 244, 250);
        cboBaudRate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
        cboBaudRate.Location = new Point(775, 19);
        cboBaudRate.Margin = new Padding(3, 4, 3, 4);
        cboBaudRate.Name = "cboBaudRate";
        cboBaudRate.Size = new Size(100, 28);
        cboBaudRate.TabIndex = 5;
        // 
        // btnConnect
        // 
        btnConnect.BackColor = Color.FromArgb(46, 204, 113);
        btnConnect.Cursor = Cursors.Hand;
        btnConnect.FlatAppearance.BorderSize = 0;
        btnConnect.FlatStyle = FlatStyle.Flat;
        btnConnect.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnConnect.ForeColor = Color.White;
        btnConnect.Location = new Point(891, 16);
        btnConnect.Margin = new Padding(3, 4, 3, 4);
        btnConnect.Name = "btnConnect";
        btnConnect.Size = new Size(131, 40);
        btnConnect.TabIndex = 6;
        btnConnect.Text = "⚡ KẾT NỐI";
        btnConnect.UseVisualStyleBackColor = false;
        // 
        // lblStatusBadge
        // 
        lblStatusBadge.AutoSize = true;
        lblStatusBadge.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblStatusBadge.ForeColor = Color.FromArgb(231, 76, 60);
        lblStatusBadge.Location = new Point(1046, 24);
        lblStatusBadge.Name = "lblStatusBadge";
        lblStatusBadge.Size = new Size(161, 21);
        lblStatusBadge.TabIndex = 7;
        lblStatusBadge.Text = "● ĐÃ NGẮT KẾT NỐI";
        // 
        // tlpMain
        // 
        tlpMain.ColumnCount = 2;
        tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
        tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
        tlpMain.Controls.Add(pnlLeftColumn, 0, 0);
        tlpMain.Controls.Add(pnlRightColumn, 1, 0);
        tlpMain.Dock = DockStyle.Fill;
        tlpMain.Location = new Point(0, 75);
        tlpMain.Margin = new Padding(3, 4, 3, 4);
        tlpMain.Name = "tlpMain";
        tlpMain.RowCount = 1;
        tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tlpMain.Size = new Size(1349, 908);
        tlpMain.TabIndex = 0;
        // 
        // pnlLeftColumn
        // 
        pnlLeftColumn.AutoScroll = true;
        pnlLeftColumn.Controls.Add(pnlLogCard);
        pnlLeftColumn.Controls.Add(pnlGraphCard);
        pnlLeftColumn.Controls.Add(pnlTelemetryCard);
        pnlLeftColumn.Dock = DockStyle.Fill;
        pnlLeftColumn.Location = new Point(3, 4);
        pnlLeftColumn.Margin = new Padding(3, 4, 3, 4);
        pnlLeftColumn.Name = "pnlLeftColumn";
        pnlLeftColumn.Padding = new Padding(11, 13, 11, 13);
        pnlLeftColumn.Size = new Size(776, 900);
        pnlLeftColumn.TabIndex = 0;
        // 
        // pnlLogCard
        // 
        pnlLogCard.BackColor = Color.FromArgb(24, 30, 42);
        pnlLogCard.Controls.Add(lblLogHeader);
        pnlLogCard.Controls.Add(chkLogTelemetry);
        pnlLogCard.Controls.Add(btnClearLog);
        pnlLogCard.Controls.Add(rtbConsole);
        pnlLogCard.Dock = DockStyle.Fill;
        pnlLogCard.Location = new Point(11, 580);
        pnlLogCard.Margin = new Padding(0, 13, 0, 0);
        pnlLogCard.Name = "pnlLogCard";
        pnlLogCard.Padding = new Padding(11, 13, 11, 13);
        pnlLogCard.Size = new Size(754, 307);
        pnlLogCard.TabIndex = 0;
        // 
        // lblLogHeader
        // 
        lblLogHeader.AutoSize = true;
        lblLogHeader.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblLogHeader.ForeColor = Color.FromArgb(0, 168, 255);
        lblLogHeader.Location = new Point(11, 13);
        lblLogHeader.Name = "lblLogHeader";
        lblLogHeader.Size = new Size(396, 21);
        lblLogHeader.TabIndex = 0;
        lblLogHeader.Text = "📋 NHẬT KÝ TRUYỀN THÔNG SERIAL / BLUETOOTH";
        // 
        // chkLogTelemetry
        // 
        chkLogTelemetry.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        chkLogTelemetry.AutoSize = true;
        chkLogTelemetry.Font = new Font("Segoe UI", 9F);
        chkLogTelemetry.ForeColor = Color.FromArgb(155, 170, 195);
        chkLogTelemetry.Location = new Point(875, 11);
        chkLogTelemetry.Margin = new Padding(3, 4, 3, 4);
        chkLogTelemetry.Name = "chkLogTelemetry";
        chkLogTelemetry.Size = new Size(158, 24);
        chkLogTelemetry.TabIndex = 1;
        chkLogTelemetry.Text = "Hiện Telemetry D:...";
        // 
        // btnClearLog
        // 
        btnClearLog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnClearLog.BackColor = Color.FromArgb(35, 44, 62);
        btnClearLog.FlatAppearance.BorderColor = Color.FromArgb(50, 62, 85);
        btnClearLog.FlatStyle = FlatStyle.Flat;
        btnClearLog.ForeColor = Color.FromArgb(155, 170, 195);
        btnClearLog.Location = new Point(1080, 11);
        btnClearLog.Margin = new Padding(3, 4, 3, 4);
        btnClearLog.Name = "btnClearLog";
        btnClearLog.Size = new Size(80, 32);
        btnClearLog.TabIndex = 2;
        btnClearLog.Text = "Xóa Log";
        btnClearLog.UseVisualStyleBackColor = false;
        // 
        // rtbConsole
        // 
        rtbConsole.BackColor = Color.FromArgb(12, 15, 22);
        rtbConsole.BorderStyle = BorderStyle.None;
        rtbConsole.Dock = DockStyle.Bottom;
        rtbConsole.Font = new Font("Cascadia Code", 8.5F);
        rtbConsole.ForeColor = Color.FromArgb(200, 220, 240);
        rtbConsole.Location = new Point(11, 81);
        rtbConsole.Margin = new Padding(3, 4, 3, 4);
        rtbConsole.Name = "rtbConsole";
        rtbConsole.ReadOnly = true;
        rtbConsole.Size = new Size(732, 213);
        rtbConsole.TabIndex = 3;
        rtbConsole.Text = "";
        // 
        // pnlGraphCard
        // 
        pnlGraphCard.BackColor = Color.FromArgb(24, 30, 42);
        pnlGraphCard.Controls.Add(lblGraphHeader);
        pnlGraphCard.Controls.Add(btnClearGraph);
        pnlGraphCard.Controls.Add(realtimeGraph);
        pnlGraphCard.Dock = DockStyle.Top;
        pnlGraphCard.Location = new Point(11, 193);
        pnlGraphCard.Margin = new Padding(0, 13, 0, 0);
        pnlGraphCard.Name = "pnlGraphCard";
        pnlGraphCard.Padding = new Padding(11, 13, 11, 13);
        pnlGraphCard.Size = new Size(754, 387);
        pnlGraphCard.TabIndex = 1;
        // 
        // lblGraphHeader
        // 
        lblGraphHeader.AutoSize = true;
        lblGraphHeader.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblGraphHeader.ForeColor = Color.FromArgb(0, 168, 255);
        lblGraphHeader.Location = new Point(11, 13);
        lblGraphHeader.Name = "lblGraphHeader";
        lblGraphHeader.Size = new Size(438, 21);
        lblGraphHeader.TabIndex = 0;
        lblGraphHeader.Text = "📈 BIỂU ĐỒ GÓC NGHIÊNG THỜI GIAN THỰC (SO VỚI 0°)";
        // 
        // btnClearGraph
        // 
        btnClearGraph.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnClearGraph.BackColor = Color.FromArgb(35, 44, 62);
        btnClearGraph.FlatAppearance.BorderColor = Color.FromArgb(50, 62, 85);
        btnClearGraph.FlatStyle = FlatStyle.Flat;
        btnClearGraph.ForeColor = Color.FromArgb(155, 170, 195);
        btnClearGraph.Location = new Point(754, 11);
        btnClearGraph.Margin = new Padding(3, 4, 3, 4);
        btnClearGraph.Name = "btnClearGraph";
        btnClearGraph.Size = new Size(91, 32);
        btnClearGraph.TabIndex = 1;
        btnClearGraph.Text = "Xóa đồ thị";
        btnClearGraph.UseVisualStyleBackColor = false;
        // 
        // realtimeGraph
        // 
        realtimeGraph.BackColor = Color.FromArgb(20, 24, 33);
        realtimeGraph.Dock = DockStyle.Bottom;
        realtimeGraph.Font = new Font("Segoe UI", 9F);
        realtimeGraph.ForeColor = Color.FromArgb(170, 185, 205);
        realtimeGraph.Location = new Point(11, 47);
        realtimeGraph.Margin = new Padding(3, 4, 3, 4);
        realtimeGraph.Name = "realtimeGraph";
        realtimeGraph.Size = new Size(732, 327);
        realtimeGraph.TabIndex = 2;
        // 
        // pnlTelemetryCard
        // 
        pnlTelemetryCard.BackColor = Color.FromArgb(24, 30, 42);
        pnlTelemetryCard.Controls.Add(tlpGauges);
        pnlTelemetryCard.Controls.Add(lblTelemetryHeader);
        pnlTelemetryCard.Dock = DockStyle.Top;
        pnlTelemetryCard.Location = new Point(11, 13);
        pnlTelemetryCard.Margin = new Padding(3, 4, 3, 4);
        pnlTelemetryCard.Name = "pnlTelemetryCard";
        pnlTelemetryCard.Padding = new Padding(11, 13, 11, 13);
        pnlTelemetryCard.Size = new Size(754, 180);
        pnlTelemetryCard.TabIndex = 2;
        // 
        // tlpGauges
        // 
        tlpGauges.ColumnCount = 4;
        tlpGauges.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tlpGauges.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tlpGauges.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tlpGauges.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tlpGauges.Controls.Add(pnlGaugeAngle, 0, 0);
        tlpGauges.Controls.Add(pnlGaugeGyro, 1, 0);
        tlpGauges.Controls.Add(pnlGaugePwm, 2, 0);
        tlpGauges.Controls.Add(pnlGaugeSafety, 3, 0);
        tlpGauges.Dock = DockStyle.Fill;
        tlpGauges.Location = new Point(11, 46);
        tlpGauges.Margin = new Padding(3, 4, 3, 4);
        tlpGauges.Name = "tlpGauges";
        tlpGauges.RowCount = 1;
        tlpGauges.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tlpGauges.Size = new Size(732, 121);
        tlpGauges.TabIndex = 0;
        // 
        // pnlGaugeAngle
        // 
        pnlGaugeAngle.BackColor = Color.FromArgb(35, 44, 62);
        pnlGaugeAngle.Controls.Add(lblGaugeAngleValue);
        pnlGaugeAngle.Controls.Add(lblGaugeAngleTitle);
        pnlGaugeAngle.Dock = DockStyle.Fill;
        pnlGaugeAngle.Location = new Point(5, 5);
        pnlGaugeAngle.Margin = new Padding(5);
        pnlGaugeAngle.Name = "pnlGaugeAngle";
        pnlGaugeAngle.Size = new Size(173, 111);
        pnlGaugeAngle.TabIndex = 0;
        // 
        // lblGaugeAngleValue
        // 
        lblGaugeAngleValue.Dock = DockStyle.Fill;
        lblGaugeAngleValue.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblGaugeAngleValue.ForeColor = Color.FromArgb(46, 204, 113);
        lblGaugeAngleValue.Location = new Point(0, 29);
        lblGaugeAngleValue.Name = "lblGaugeAngleValue";
        lblGaugeAngleValue.Size = new Size(173, 82);
        lblGaugeAngleValue.TabIndex = 0;
        lblGaugeAngleValue.Text = "0.00°";
        lblGaugeAngleValue.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblGaugeAngleTitle
        // 
        lblGaugeAngleTitle.Dock = DockStyle.Top;
        lblGaugeAngleTitle.Font = new Font("Segoe UI", 9F);
        lblGaugeAngleTitle.ForeColor = Color.FromArgb(155, 170, 195);
        lblGaugeAngleTitle.Location = new Point(0, 0);
        lblGaugeAngleTitle.Name = "lblGaugeAngleTitle";
        lblGaugeAngleTitle.Size = new Size(173, 29);
        lblGaugeAngleTitle.TabIndex = 1;
        lblGaugeAngleTitle.Text = "GÓC NGHIÊNG";
        lblGaugeAngleTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pnlGaugeGyro
        // 
        pnlGaugeGyro.BackColor = Color.FromArgb(35, 44, 62);
        pnlGaugeGyro.Controls.Add(lblGaugeGyroValue);
        pnlGaugeGyro.Controls.Add(lblGaugeGyroTitle);
        pnlGaugeGyro.Dock = DockStyle.Fill;
        pnlGaugeGyro.Location = new Point(188, 5);
        pnlGaugeGyro.Margin = new Padding(5);
        pnlGaugeGyro.Name = "pnlGaugeGyro";
        pnlGaugeGyro.Size = new Size(173, 111);
        pnlGaugeGyro.TabIndex = 1;
        // 
        // lblGaugeGyroValue
        // 
        lblGaugeGyroValue.Dock = DockStyle.Fill;
        lblGaugeGyroValue.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblGaugeGyroValue.ForeColor = Color.FromArgb(0, 168, 255);
        lblGaugeGyroValue.Location = new Point(0, 29);
        lblGaugeGyroValue.Name = "lblGaugeGyroValue";
        lblGaugeGyroValue.Size = new Size(173, 82);
        lblGaugeGyroValue.TabIndex = 0;
        lblGaugeGyroValue.Text = "0.00°/s";
        lblGaugeGyroValue.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblGaugeGyroTitle
        // 
        lblGaugeGyroTitle.Dock = DockStyle.Top;
        lblGaugeGyroTitle.Font = new Font("Segoe UI", 9F);
        lblGaugeGyroTitle.ForeColor = Color.FromArgb(155, 170, 195);
        lblGaugeGyroTitle.Location = new Point(0, 0);
        lblGaugeGyroTitle.Name = "lblGaugeGyroTitle";
        lblGaugeGyroTitle.Size = new Size(173, 29);
        lblGaugeGyroTitle.TabIndex = 1;
        lblGaugeGyroTitle.Text = "VẬN TỐC GÓC";
        lblGaugeGyroTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pnlGaugePwm
        // 
        pnlGaugePwm.BackColor = Color.FromArgb(35, 44, 62);
        pnlGaugePwm.Controls.Add(lblGaugePwmValue);
        pnlGaugePwm.Controls.Add(pbPwmLevel);
        pnlGaugePwm.Controls.Add(lblGaugePwmTitle);
        pnlGaugePwm.Dock = DockStyle.Fill;
        pnlGaugePwm.Location = new Point(371, 5);
        pnlGaugePwm.Margin = new Padding(5);
        pnlGaugePwm.Name = "pnlGaugePwm";
        pnlGaugePwm.Size = new Size(173, 111);
        pnlGaugePwm.TabIndex = 2;
        // 
        // lblGaugePwmValue
        // 
        lblGaugePwmValue.Dock = DockStyle.Top;
        lblGaugePwmValue.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblGaugePwmValue.ForeColor = Color.FromArgb(241, 196, 15);
        lblGaugePwmValue.Location = new Point(0, 29);
        lblGaugePwmValue.Name = "lblGaugePwmValue";
        lblGaugePwmValue.Size = new Size(173, 47);
        lblGaugePwmValue.TabIndex = 0;
        lblGaugePwmValue.Text = "0";
        lblGaugePwmValue.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pbPwmLevel
        // 
        pbPwmLevel.Dock = DockStyle.Bottom;
        pbPwmLevel.Location = new Point(0, 100);
        pbPwmLevel.Margin = new Padding(3, 4, 3, 4);
        pbPwmLevel.Maximum = 10000;
        pbPwmLevel.Name = "pbPwmLevel";
        pbPwmLevel.Size = new Size(173, 11);
        pbPwmLevel.TabIndex = 1;
        // 
        // lblGaugePwmTitle
        // 
        lblGaugePwmTitle.Dock = DockStyle.Top;
        lblGaugePwmTitle.Font = new Font("Segoe UI", 9F);
        lblGaugePwmTitle.ForeColor = Color.FromArgb(155, 170, 195);
        lblGaugePwmTitle.Location = new Point(0, 0);
        lblGaugePwmTitle.Name = "lblGaugePwmTitle";
        lblGaugePwmTitle.Size = new Size(173, 29);
        lblGaugePwmTitle.TabIndex = 2;
        lblGaugePwmTitle.Text = "TẦN SỐ NIDEC";
        lblGaugePwmTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pnlGaugeSafety
        // 
        pnlGaugeSafety.BackColor = Color.FromArgb(35, 44, 62);
        pnlGaugeSafety.Controls.Add(lblGaugeSafetyValue);
        pnlGaugeSafety.Controls.Add(lblGaugeSafetyTitle);
        pnlGaugeSafety.Dock = DockStyle.Fill;
        pnlGaugeSafety.Location = new Point(554, 5);
        pnlGaugeSafety.Margin = new Padding(5);
        pnlGaugeSafety.Name = "pnlGaugeSafety";
        pnlGaugeSafety.Size = new Size(173, 111);
        pnlGaugeSafety.TabIndex = 3;
        // 
        // lblGaugeSafetyValue
        // 
        lblGaugeSafetyValue.Dock = DockStyle.Fill;
        lblGaugeSafetyValue.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblGaugeSafetyValue.ForeColor = Color.FromArgb(46, 204, 113);
        lblGaugeSafetyValue.Location = new Point(0, 29);
        lblGaugeSafetyValue.Name = "lblGaugeSafetyValue";
        lblGaugeSafetyValue.Size = new Size(173, 82);
        lblGaugeSafetyValue.TabIndex = 0;
        lblGaugeSafetyValue.Text = "SẴN SÀNG";
        lblGaugeSafetyValue.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblGaugeSafetyTitle
        // 
        lblGaugeSafetyTitle.Dock = DockStyle.Top;
        lblGaugeSafetyTitle.Font = new Font("Segoe UI", 9F);
        lblGaugeSafetyTitle.ForeColor = Color.FromArgb(155, 170, 195);
        lblGaugeSafetyTitle.Location = new Point(0, 0);
        lblGaugeSafetyTitle.Name = "lblGaugeSafetyTitle";
        lblGaugeSafetyTitle.Size = new Size(173, 29);
        lblGaugeSafetyTitle.TabIndex = 1;
        lblGaugeSafetyTitle.Text = "TRẠNG THÁI";
        lblGaugeSafetyTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblTelemetryHeader
        // 
        lblTelemetryHeader.Dock = DockStyle.Top;
        lblTelemetryHeader.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblTelemetryHeader.ForeColor = Color.FromArgb(0, 168, 255);
        lblTelemetryHeader.Location = new Point(11, 13);
        lblTelemetryHeader.Name = "lblTelemetryHeader";
        lblTelemetryHeader.Size = new Size(732, 33);
        lblTelemetryHeader.TabIndex = 1;
        lblTelemetryHeader.Text = "📊 DỮ LIỆU TELEMETRY XE CÂN BẰNG (M3)";
        // 
        // pnlRightColumn
        // 
        pnlRightColumn.AutoScroll = true;
        pnlRightColumn.Controls.Add(pnlPidCard);
        pnlRightColumn.Controls.Add(pnlControlCard);
        pnlRightColumn.Dock = DockStyle.Fill;
        pnlRightColumn.Location = new Point(785, 4);
        pnlRightColumn.Margin = new Padding(3, 4, 3, 4);
        pnlRightColumn.Name = "pnlRightColumn";
        pnlRightColumn.Padding = new Padding(11, 13, 11, 13);
        pnlRightColumn.Size = new Size(561, 900);
        pnlRightColumn.TabIndex = 1;
        // 
        // pnlPidCard
        // 
        pnlPidCard.BackColor = Color.FromArgb(24, 30, 42);
        pnlPidCard.Controls.Add(lblPidHeader);
        pnlPidCard.Controls.Add(lblKp);
        pnlPidCard.Controls.Add(numKp);
        pnlPidCard.Controls.Add(lblKi);
        pnlPidCard.Controls.Add(numKi);
        pnlPidCard.Controls.Add(lblKd);
        pnlPidCard.Controls.Add(numKd);
        pnlPidCard.Controls.Add(btnSendPid);
        pnlPidCard.Controls.Add(btnCalibrate);
        pnlPidCard.Controls.Add(btnSaveEeprom);
        pnlPidCard.Controls.Add(lblKeyboardHint);
        pnlPidCard.Dock = DockStyle.Fill;
        pnlPidCard.Location = new Point(11, 540);
        pnlPidCard.Margin = new Padding(0, 13, 0, 0);
        pnlPidCard.Name = "pnlPidCard";
        pnlPidCard.Padding = new Padding(14, 16, 14, 16);
        pnlPidCard.Size = new Size(539, 347);
        pnlPidCard.TabIndex = 0;
        // 
        // lblPidHeader
        // 
        lblPidHeader.Dock = DockStyle.Top;
        lblPidHeader.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblPidHeader.ForeColor = Color.FromArgb(0, 168, 255);
        lblPidHeader.Location = new Point(14, 16);
        lblPidHeader.Name = "lblPidHeader";
        lblPidHeader.Size = new Size(511, 33);
        lblPidHeader.TabIndex = 0;
        lblPidHeader.Text = "⚙️ THÔNG SỐ PID CÂN BẰNG & EEPROM (M2)";
        // 
        // lblKp
        // 
        lblKp.AutoSize = true;
        lblKp.Font = new Font("Segoe UI", 9F);
        lblKp.ForeColor = Color.FromArgb(155, 170, 195);
        lblKp.Location = new Point(23, 51);
        lblKp.Name = "lblKp";
        lblKp.Size = new Size(111, 20);
        lblKp.TabIndex = 1;
        lblKp.Text = "Kp (Hệ số tỉ lệ):";
        // 
        // numKp
        // 
        numKp.BackColor = Color.FromArgb(35, 44, 62);
        numKp.DecimalPlaces = 2;
        numKp.ForeColor = Color.FromArgb(240, 244, 250);
        numKp.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
        numKp.Location = new Point(206, 47);
        numKp.Margin = new Padding(3, 4, 3, 4);
        numKp.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
        numKp.Name = "numKp";
        numKp.Size = new Size(91, 27);
        numKp.TabIndex = 2;
        numKp.Value = new decimal(new int[] { 3500, 0, 0, 131072 });
        // 
        // lblKi
        // 
        lblKi.AutoSize = true;
        lblKi.Font = new Font("Segoe UI", 9F);
        lblKi.ForeColor = Color.FromArgb(155, 170, 195);
        lblKi.Location = new Point(23, 93);
        lblKi.Name = "lblKi";
        lblKi.Size = new Size(142, 20);
        lblKi.TabIndex = 3;
        lblKi.Text = "Ki (Hệ số tích phân):";
        // 
        // numKi
        // 
        numKi.BackColor = Color.FromArgb(35, 44, 62);
        numKi.DecimalPlaces = 2;
        numKi.ForeColor = Color.FromArgb(240, 244, 250);
        numKi.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        numKi.Location = new Point(206, 89);
        numKi.Margin = new Padding(3, 4, 3, 4);
        numKi.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
        numKi.Name = "numKi";
        numKi.Size = new Size(91, 27);
        numKi.TabIndex = 4;
        numKi.Value = new decimal(new int[] { 60, 0, 0, 131072 });
        // 
        // lblKd
        // 
        lblKd.AutoSize = true;
        lblKd.Font = new Font("Segoe UI", 9F);
        lblKd.ForeColor = Color.FromArgb(155, 170, 195);
        lblKd.Location = new Point(23, 136);
        lblKd.Name = "lblKd";
        lblKd.Size = new Size(134, 20);
        lblKd.TabIndex = 5;
        lblKd.Text = "Kd (Hệ số vi phân):";
        // 
        // numKd
        // 
        numKd.BackColor = Color.FromArgb(35, 44, 62);
        numKd.DecimalPlaces = 2;
        numKd.ForeColor = Color.FromArgb(240, 244, 250);
        numKd.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        numKd.Location = new Point(206, 132);
        numKd.Margin = new Padding(3, 4, 3, 4);
        numKd.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
        numKd.Name = "numKd";
        numKd.Size = new Size(91, 27);
        numKd.TabIndex = 6;
        numKd.Value = new decimal(new int[] { 125, 0, 0, 131072 });
        // 
        // btnSendPid
        // 
        btnSendPid.BackColor = Color.FromArgb(0, 168, 255);
        btnSendPid.Cursor = Cursors.Hand;
        btnSendPid.FlatAppearance.BorderSize = 0;
        btnSendPid.FlatStyle = FlatStyle.Flat;
        btnSendPid.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSendPid.ForeColor = Color.White;
        btnSendPid.Location = new Point(314, 48);
        btnSendPid.Margin = new Padding(3, 4, 3, 4);
        btnSendPid.Name = "btnSendPid";
        btnSendPid.Size = new Size(171, 113);
        btnSendPid.TabIndex = 7;
        btnSendPid.Text = "📤 GỬI THAM SỐ PID (P:...)";
        btnSendPid.UseVisualStyleBackColor = false;
        // 
        // btnCalibrate
        // 
        btnCalibrate.BackColor = Color.FromArgb(35, 44, 62);
        btnCalibrate.Cursor = Cursors.Hand;
        btnCalibrate.FlatAppearance.BorderColor = Color.FromArgb(50, 62, 85);
        btnCalibrate.FlatStyle = FlatStyle.Flat;
        btnCalibrate.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnCalibrate.ForeColor = Color.FromArgb(241, 196, 15);
        btnCalibrate.Location = new Point(23, 184);
        btnCalibrate.Margin = new Padding(3, 4, 3, 4);
        btnCalibrate.Name = "btnCalibrate";
        btnCalibrate.Size = new Size(217, 48);
        btnCalibrate.TabIndex = 8;
        btnCalibrate.Text = "⚖️ HIỆU CHUẨN GÓC (CAL)";
        btnCalibrate.UseVisualStyleBackColor = false;
        // 
        // btnSaveEeprom
        // 
        btnSaveEeprom.BackColor = Color.FromArgb(35, 44, 62);
        btnSaveEeprom.Cursor = Cursors.Hand;
        btnSaveEeprom.FlatAppearance.BorderColor = Color.FromArgb(50, 62, 85);
        btnSaveEeprom.FlatStyle = FlatStyle.Flat;
        btnSaveEeprom.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSaveEeprom.ForeColor = Color.FromArgb(46, 204, 113);
        btnSaveEeprom.Location = new Point(263, 184);
        btnSaveEeprom.Margin = new Padding(3, 4, 3, 4);
        btnSaveEeprom.Name = "btnSaveEeprom";
        btnSaveEeprom.Size = new Size(223, 48);
        btnSaveEeprom.TabIndex = 9;
        btnSaveEeprom.Text = "💾 LƯU EEPROM (SAVE)";
        btnSaveEeprom.UseVisualStyleBackColor = false;
        // 
        // lblKeyboardHint
        // 
        lblKeyboardHint.AutoSize = true;
        lblKeyboardHint.Font = new Font("Segoe UI", 9F);
        lblKeyboardHint.ForeColor = Color.FromArgb(105, 120, 145);
        lblKeyboardHint.Location = new Point(23, 247);
        lblKeyboardHint.Name = "lblKeyboardHint";
        lblKeyboardHint.Size = new Size(525, 20);
        lblKeyboardHint.TabIndex = 10;
        lblKeyboardHint.Text = "⌨️ Phím W/A/S/D hoặc Mũi tên: Điều khiển di chuyển | Space: Dừng khẩn cấp";
        // 
        // pnlControlCard
        // 
        pnlControlCard.BackColor = Color.FromArgb(24, 30, 42);
        pnlControlCard.Controls.Add(lblControlHeader);
        pnlControlCard.Controls.Add(btnForward);
        pnlControlCard.Controls.Add(btnLeft);
        pnlControlCard.Controls.Add(btnCenterSteer);
        pnlControlCard.Controls.Add(btnRight);
        pnlControlCard.Controls.Add(btnBackward);
        pnlControlCard.Controls.Add(btnEmergencyStop);
        pnlControlCard.Controls.Add(lblMaxSpeed);
        pnlControlCard.Controls.Add(numMaxSpeed);
        pnlControlCard.Controls.Add(trackMaxSpeed);
        pnlControlCard.Controls.Add(lblTrim);
        pnlControlCard.Controls.Add(numTrim);
        pnlControlCard.Controls.Add(trackTrim);
        pnlControlCard.Controls.Add(lblCurrentCommand);
        pnlControlCard.Dock = DockStyle.Top;
        pnlControlCard.Location = new Point(11, 13);
        pnlControlCard.Margin = new Padding(3, 4, 3, 4);
        pnlControlCard.Name = "pnlControlCard";
        pnlControlCard.Padding = new Padding(14, 16, 14, 16);
        pnlControlCard.Size = new Size(539, 527);
        pnlControlCard.TabIndex = 1;
        // 
        // lblControlHeader
        // 
        lblControlHeader.Dock = DockStyle.Top;
        lblControlHeader.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblControlHeader.ForeColor = Color.FromArgb(0, 168, 255);
        lblControlHeader.Location = new Point(14, 16);
        lblControlHeader.Name = "lblControlHeader";
        lblControlHeader.Size = new Size(511, 32);
        lblControlHeader.TabIndex = 0;
        lblControlHeader.Text = "🎮 ĐIỀU KHIỂN DI CHUYỂN & LÁI (M2)";
        // 
        // btnForward
        // 
        btnForward.BackColor = Color.FromArgb(35, 44, 62);
        btnForward.FlatAppearance.BorderColor = Color.FromArgb(50, 62, 85);
        btnForward.FlatStyle = FlatStyle.Flat;
        btnForward.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnForward.ForeColor = Color.FromArgb(240, 244, 250);
        btnForward.Location = new Point(166, 51);
        btnForward.Margin = new Padding(3, 4, 3, 4);
        btnForward.Name = "btnForward";
        btnForward.Size = new Size(126, 56);
        btnForward.TabIndex = 1;
        btnForward.Text = "▲ TIẾN (W)";
        btnForward.UseVisualStyleBackColor = false;
        // 
        // btnLeft
        // 
        btnLeft.BackColor = Color.FromArgb(35, 44, 62);
        btnLeft.FlatAppearance.BorderColor = Color.FromArgb(50, 62, 85);
        btnLeft.FlatStyle = FlatStyle.Flat;
        btnLeft.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnLeft.ForeColor = Color.FromArgb(240, 244, 250);
        btnLeft.Location = new Point(46, 112);
        btnLeft.Margin = new Padding(3, 4, 3, 4);
        btnLeft.Name = "btnLeft";
        btnLeft.Size = new Size(114, 56);
        btnLeft.TabIndex = 2;
        btnLeft.Text = "◄ TRÁI (A)";
        btnLeft.UseVisualStyleBackColor = false;
        // 
        // btnCenterSteer
        // 
        btnCenterSteer.BackColor = Color.FromArgb(32, 40, 56);
        btnCenterSteer.FlatAppearance.BorderColor = Color.FromArgb(50, 62, 85);
        btnCenterSteer.FlatStyle = FlatStyle.Flat;
        btnCenterSteer.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnCenterSteer.ForeColor = Color.FromArgb(0, 168, 255);
        btnCenterSteer.Location = new Point(174, 112);
        btnCenterSteer.Margin = new Padding(3, 4, 3, 4);
        btnCenterSteer.Name = "btnCenterSteer";
        btnCenterSteer.Size = new Size(109, 56);
        btnCenterSteer.TabIndex = 3;
        btnCenterSteer.Text = "◉ THẲNG";
        btnCenterSteer.UseVisualStyleBackColor = false;
        // 
        // btnRight
        // 
        btnRight.BackColor = Color.FromArgb(35, 44, 62);
        btnRight.FlatAppearance.BorderColor = Color.FromArgb(50, 62, 85);
        btnRight.FlatStyle = FlatStyle.Flat;
        btnRight.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnRight.ForeColor = Color.FromArgb(240, 244, 250);
        btnRight.Location = new Point(297, 112);
        btnRight.Margin = new Padding(3, 4, 3, 4);
        btnRight.Name = "btnRight";
        btnRight.Size = new Size(114, 56);
        btnRight.TabIndex = 4;
        btnRight.Text = "PHẢI (D) ►";
        btnRight.UseVisualStyleBackColor = false;
        // 
        // btnBackward
        // 
        btnBackward.BackColor = Color.FromArgb(35, 44, 62);
        btnBackward.FlatAppearance.BorderColor = Color.FromArgb(50, 62, 85);
        btnBackward.FlatStyle = FlatStyle.Flat;
        btnBackward.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnBackward.ForeColor = Color.FromArgb(240, 244, 250);
        btnBackward.Location = new Point(166, 173);
        btnBackward.Margin = new Padding(3, 4, 3, 4);
        btnBackward.Name = "btnBackward";
        btnBackward.Size = new Size(126, 56);
        btnBackward.TabIndex = 5;
        btnBackward.Text = "▼ LÙI (S)";
        btnBackward.UseVisualStyleBackColor = false;
        // 
        // btnEmergencyStop
        // 
        btnEmergencyStop.BackColor = Color.FromArgb(231, 76, 60);
        btnEmergencyStop.Cursor = Cursors.Hand;
        btnEmergencyStop.FlatAppearance.BorderSize = 0;
        btnEmergencyStop.FlatStyle = FlatStyle.Flat;
        btnEmergencyStop.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnEmergencyStop.ForeColor = Color.White;
        btnEmergencyStop.Location = new Point(46, 240);
        btnEmergencyStop.Margin = new Padding(3, 4, 3, 4);
        btnEmergencyStop.Name = "btnEmergencyStop";
        btnEmergencyStop.Size = new Size(366, 59);
        btnEmergencyStop.TabIndex = 6;
        btnEmergencyStop.Text = "\U0001f6d1 DỪNG KHẨN CẤP (SPACEBAR)";
        btnEmergencyStop.UseVisualStyleBackColor = false;
        // 
        // lblMaxSpeed
        // 
        lblMaxSpeed.AutoSize = true;
        lblMaxSpeed.Font = new Font("Segoe UI", 9F);
        lblMaxSpeed.ForeColor = Color.FromArgb(155, 170, 195);
        lblMaxSpeed.Location = new Point(23, 313);
        lblMaxSpeed.Name = "lblMaxSpeed";
        lblMaxSpeed.Size = new Size(189, 20);
        lblMaxSpeed.TabIndex = 7;
        lblMaxSpeed.Text = "Tốc độ tối đa (Max Speed):";
        // 
        // numMaxSpeed
        // 
        numMaxSpeed.BackColor = Color.FromArgb(35, 44, 62);
        numMaxSpeed.ForeColor = Color.FromArgb(240, 244, 250);
        numMaxSpeed.Location = new Point(354, 311);
        numMaxSpeed.Margin = new Padding(3, 4, 3, 4);
        numMaxSpeed.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
        numMaxSpeed.Name = "numMaxSpeed";
        numMaxSpeed.Size = new Size(69, 27);
        numMaxSpeed.TabIndex = 8;
        numMaxSpeed.Value = new decimal(new int[] { 160, 0, 0, 0 });
        // 
        // trackMaxSpeed
        // 
        trackMaxSpeed.Location = new Point(23, 344);
        trackMaxSpeed.Margin = new Padding(3, 4, 3, 4);
        trackMaxSpeed.Maximum = 255;
        trackMaxSpeed.Name = "trackMaxSpeed";
        trackMaxSpeed.Size = new Size(320, 56);
        trackMaxSpeed.TabIndex = 9;
        trackMaxSpeed.TickFrequency = 25;
        trackMaxSpeed.Value = 160;
        // 
        // lblTrim
        // 
        lblTrim.AutoSize = true;
        lblTrim.Font = new Font("Segoe UI", 9F);
        lblTrim.ForeColor = Color.FromArgb(155, 170, 195);
        lblTrim.Location = new Point(23, 404);
        lblTrim.Name = "lblTrim";
        lblTrim.Size = new Size(216, 20);
        lblTrim.TabIndex = 10;
        lblTrim.Text = "Hiệu chỉnh lệch tâm lái (Trim °):";
        // 
        // numTrim
        // 
        numTrim.BackColor = Color.FromArgb(35, 44, 62);
        numTrim.ForeColor = Color.FromArgb(240, 244, 250);
        numTrim.Location = new Point(354, 402);
        numTrim.Margin = new Padding(3, 4, 3, 4);
        numTrim.Maximum = new decimal(new int[] { 25, 0, 0, 0 });
        numTrim.Minimum = new decimal(new int[] { 25, 0, 0, int.MinValue });
        numTrim.Name = "numTrim";
        numTrim.Size = new Size(69, 27);
        numTrim.TabIndex = 11;
        // 
        // trackTrim
        // 
        trackTrim.Location = new Point(23, 431);
        trackTrim.Margin = new Padding(3, 4, 3, 4);
        trackTrim.Maximum = 25;
        trackTrim.Minimum = -25;
        trackTrim.Name = "trackTrim";
        trackTrim.Size = new Size(320, 56);
        trackTrim.TabIndex = 12;
        trackTrim.TickFrequency = 5;
        // 
        // lblCurrentCommand
        // 
        lblCurrentCommand.AutoSize = true;
        lblCurrentCommand.Font = new Font("Cascadia Code", 8.5F);
        lblCurrentCommand.ForeColor = Color.FromArgb(105, 120, 145);
        lblCurrentCommand.Location = new Point(23, 491);
        lblCurrentCommand.Name = "lblCurrentCommand";
        lblCurrentCommand.Size = new Size(378, 20);
        lblCurrentCommand.TabIndex = 13;
        lblCurrentCommand.Text = "Lệnh hiện hành: M:0,90\\n (Tiết chế ~90ms)";
        // 
        // statusStrip
        // 
        statusStrip.BackColor = Color.FromArgb(32, 40, 56);
        statusStrip.Font = new Font("Segoe UI", 9F);
        statusStrip.ForeColor = Color.FromArgb(155, 170, 195);
        statusStrip.ImageScalingSize = new Size(20, 20);
        statusStrip.Items.AddRange(new ToolStripItem[] { tslConnectionInfo, tslRxCount, tslTxCount, tslErrorCount, tslLastUpdate });
        statusStrip.Location = new Point(0, 983);
        statusStrip.Name = "statusStrip";
        statusStrip.Padding = new Padding(1, 0, 16, 0);
        statusStrip.Size = new Size(1349, 30);
        statusStrip.TabIndex = 2;
        // 
        // tslConnectionInfo
        // 
        tslConnectionInfo.BorderSides = ToolStripStatusLabelBorderSides.Right;
        tslConnectionInfo.BorderStyle = Border3DStyle.Etched;
        tslConnectionInfo.Name = "tslConnectionInfo";
        tslConnectionInfo.Size = new Size(138, 24);
        tslConnectionInfo.Text = "Cổng: Chưa kết nối";
        // 
        // tslRxCount
        // 
        tslRxCount.BorderSides = ToolStripStatusLabelBorderSides.Right;
        tslRxCount.BorderStyle = Border3DStyle.Etched;
        tslRxCount.Name = "tslRxCount";
        tslRxCount.Size = new Size(72, 24);
        tslRxCount.Text = "RX: 0 gói";
        // 
        // tslTxCount
        // 
        tslTxCount.BorderSides = ToolStripStatusLabelBorderSides.Right;
        tslTxCount.BorderStyle = Border3DStyle.Etched;
        tslTxCount.Name = "tslTxCount";
        tslTxCount.Size = new Size(77, 24);
        tslTxCount.Text = "TX: 0 lệnh";
        // 
        // tslErrorCount
        // 
        tslErrorCount.BorderSides = ToolStripStatusLabelBorderSides.Right;
        tslErrorCount.BorderStyle = Border3DStyle.Etched;
        tslErrorCount.Name = "tslErrorCount";
        tslErrorCount.Size = new Size(48, 24);
        tslErrorCount.Text = "Lỗi: 0";
        // 
        // tslLastUpdate
        // 
        tslLastUpdate.Name = "tslLastUpdate";
        tslLastUpdate.Size = new Size(997, 24);
        tslLastUpdate.Spring = true;
        tslLastUpdate.Text = "Thời gian: --:--:--";
        tslLastUpdate.TextAlign = ContentAlignment.MiddleRight;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(16, 20, 28);
        ClientSize = new Size(1349, 1013);
        Controls.Add(tlpMain);
        Controls.Add(pnlHeader);
        Controls.Add(statusStrip);
        Font = new Font("Segoe UI", 9F);
        ForeColor = Color.FromArgb(240, 244, 250);
        KeyPreview = true;
        Margin = new Padding(3, 4, 3, 4);
        MinimumSize = new Size(1140, 918);
        Name = "MainForm";
        Text = "Self-Balancing Bike Controller & Telemetry Monitor (G1: M1, M2, M3)";
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        tlpMain.ResumeLayout(false);
        pnlLeftColumn.ResumeLayout(false);
        pnlLogCard.ResumeLayout(false);
        pnlLogCard.PerformLayout();
        pnlGraphCard.ResumeLayout(false);
        pnlGraphCard.PerformLayout();
        pnlTelemetryCard.ResumeLayout(false);
        tlpGauges.ResumeLayout(false);
        pnlGaugeAngle.ResumeLayout(false);
        pnlGaugeGyro.ResumeLayout(false);
        pnlGaugePwm.ResumeLayout(false);
        pnlGaugeSafety.ResumeLayout(false);
        pnlRightColumn.ResumeLayout(false);
        pnlPidCard.ResumeLayout(false);
        pnlPidCard.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numKp).EndInit();
        ((System.ComponentModel.ISupportInitialize)numKi).EndInit();
        ((System.ComponentModel.ISupportInitialize)numKd).EndInit();
        pnlControlCard.ResumeLayout(false);
        pnlControlCard.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numMaxSpeed).EndInit();
        ((System.ComponentModel.ISupportInitialize)trackMaxSpeed).EndInit();
        ((System.ComponentModel.ISupportInitialize)numTrim).EndInit();
        ((System.ComponentModel.ISupportInitialize)trackTrim).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
