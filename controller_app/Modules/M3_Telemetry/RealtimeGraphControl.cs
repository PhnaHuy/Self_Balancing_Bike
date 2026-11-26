using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace ControllerApp.Modules.M3_Telemetry;

/// <summary>
/// Control tùy biến vẽ biểu đồ góc nghiêng thời gian thực sử dụng GDI+ tối ưu hai lớp đệm (Double Buffering).
/// Trực quan hóa góc nghiêng so với mốc cân bằng 0°, các ngưỡng cảnh báo và dao động điều khiển.
/// </summary>
public class RealtimeGraphControl : Control
{
    private readonly List<float> _history = new();
    private readonly object _lock = new();

    private int _maxPoints = 250;
    private float _minRange = -40.0f;
    private float _maxRange = 40.0f;
    private float _alarmThreshold = 35.0f;
    private float _latestAngle = 0.0f;

    // Màu sắc giao diện công nghiệp hiện đại
    private readonly Color _bgColor = Color.FromArgb(20, 24, 33);
    private readonly Color _gridColor = Color.FromArgb(40, 48, 65);
    private readonly Color _centerLineColor = Color.FromArgb(0, 220, 180);
    private readonly Color _alarmLineColor = Color.FromArgb(240, 70, 70);
    private readonly Color _curveColor = Color.FromArgb(255, 200, 50);
    private readonly Color _curveFillColor = Color.FromArgb(40, 255, 200, 50);
    private readonly Color _textColor = Color.FromArgb(170, 185, 205);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MaxPoints
    {
        get => _maxPoints;
        set { _maxPoints = Math.Max(50, value); Invalidate(); }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float AlarmThreshold
    {
        get => _alarmThreshold;
        set { _alarmThreshold = Math.Abs(value); Invalidate(); }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float LatestAngle => _latestAngle;

    public RealtimeGraphControl()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint |
                 ControlStyles.ResizeRedraw, true);

        UpdateStyles();
        BackColor = _bgColor;
        ForeColor = _textColor;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);
    }

    /// <summary>
    /// Thêm một mẫu góc nghiêng mới vào biểu đồ và kích hoạt vẽ lại.
    /// An toàn khi gọi từ luồng khác bằng BeginInvoke.
    /// </summary>
    public void AddSample(float angle)
    {
        if (InvokeRequired)
        {
            if (IsHandleCreated && !IsDisposed)
            {
                try
                {
                    BeginInvoke(new Action<float>(AddSample), angle);
                }
                catch (ObjectDisposedException) { }
            }
            return;
        }

        lock (_lock)
        {
            _latestAngle = angle;
            _history.Add(angle);

            if (_history.Count > _maxPoints)
            {
                _history.RemoveAt(0);
            }
        }

        Invalidate();
    }

    /// <summary>
    /// Xóa toàn bộ lịch sử đồ thị.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _history.Clear();
            _latestAngle = 0.0f;
        }
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        int width = ClientSize.Width;
        int height = ClientSize.Height;

        if (width < 60 || height < 40)
            return;

        // Vùng lề hiển thị nhãn trục
        const int leftMargin = 48;
        const int rightMargin = 16;
        const int topMargin = 24;
        const int bottomMargin = 24;

        int plotWidth = width - leftMargin - rightMargin;
        int plotHeight = height - topMargin - bottomMargin;

        if (plotWidth <= 0 || plotHeight <= 0)
            return;

        Rectangle plotRect = new Rectangle(leftMargin, topMargin, plotWidth, plotHeight);

        // Vẽ nền vùng đồ thị
        using (var plotBgBrush = new SolidBrush(Color.FromArgb(14, 17, 24)))
        {
            g.FillRectangle(plotBgBrush, plotRect);
        }

        // 1. Vẽ các đường lưới độ nghiêng
        float[] degreeMarks = new float[] { -35f, -20f, 0f, 20f, 35f };
        using (var gridPen = new Pen(_gridColor, 1f) { DashStyle = DashStyle.Dash })
        using (var textBrush = new SolidBrush(_textColor))
        using (var stringFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center })
        {
            foreach (float deg in degreeMarks)
            {
                float y = DegreeToY(deg, plotHeight, topMargin);
                g.DrawLine(gridPen, leftMargin, y, leftMargin + plotWidth, y);

                // Nhãn góc
                g.DrawString($"{deg:+0;-0;0}°", Font, textBrush, leftMargin - 4, y, stringFormat);
            }
        }

        // 2. Đường tham chiếu cân bằng lý tưởng 0° (Equilibrium Baseline)
        float zeroY = DegreeToY(0.0f, plotHeight, topMargin);
        using (var centerPen = new Pen(_centerLineColor, 1.8f))
        {
            g.DrawLine(centerPen, leftMargin, zeroY, leftMargin + plotWidth, zeroY);
        }

        // 3. Đường cảnh báo nguy hiểm ±35° (Alarm Thresholds)
        using (var alarmPen = new Pen(_alarmLineColor, 1.2f) { DashStyle = DashStyle.DashDot })
        {
            float alarmY1 = DegreeToY(_alarmThreshold, plotHeight, topMargin);
            float alarmY2 = DegreeToY(-_alarmThreshold, plotHeight, topMargin);

            g.DrawLine(alarmPen, leftMargin, alarmY1, leftMargin + plotWidth, alarmY1);
            g.DrawLine(alarmPen, leftMargin, alarmY2, leftMargin + plotWidth, alarmY2);
        }

        // 4. Vẽ đường biểu diễn góc nghiêng thời gian thực
        List<PointF> points = new();
        float minVal = float.MaxValue;
        float maxVal = float.MinValue;

        lock (_lock)
        {
            if (_history.Count > 1)
            {
                float stepX = (float)plotWidth / (_maxPoints - 1);
                int startIndex = _maxPoints - _history.Count;

                for (int i = 0; i < _history.Count; i++)
                {
                    float angle = _history[i];
                    float x = leftMargin + (startIndex + i) * stepX;
                    float y = DegreeToY(angle, plotHeight, topMargin);

                    points.Add(new PointF(x, y));

                    if (angle < minVal) minVal = angle;
                    if (angle > maxVal) maxVal = angle;
                }
            }
        }

        if (points.Count > 1)
        {
            // Tô vùng diện tích dưới đường cong với gradient
            using (var fillPath = new GraphicsPath())
            {
                fillPath.AddLines(points.ToArray());
                fillPath.AddLine(points[^1], new PointF(points[^1].X, zeroY));
                fillPath.AddLine(new PointF(points[^1].X, zeroY), new PointF(points[0].X, zeroY));
                fillPath.AddLine(new PointF(points[0].X, zeroY), points[0]);
                fillPath.CloseFigure();

                using (var fillBrush = new SolidBrush(_curveFillColor))
                {
                    g.FillPath(fillBrush, fillPath);
                }
            }

            // Vẽ đường cong chính
            using (var curvePen = new Pen(_curveColor, 2.0f) { LineJoin = LineJoin.Round })
            {
                g.DrawLines(curvePen, points.ToArray());
            }

            // Điểm mới nhất với hiệu ứng phát sáng
            PointF lastPt = points[^1];
            using (var ptBrush = new SolidBrush(Color.White))
            using (var ringPen = new Pen(_curveColor, 2.0f))
            {
                g.FillEllipse(ptBrush, lastPt.X - 3, lastPt.Y - 3, 6, 6);
                g.DrawEllipse(ringPen, lastPt.X - 5, lastPt.Y - 5, 10, 10);
            }
        }

        // Khung viền vùng đồ thị
        using (var borderPen = new Pen(Color.FromArgb(60, 72, 95), 1.2f))
        {
            g.DrawRectangle(borderPen, plotRect);
        }

        // 5. Header thông tin góc hiện tại & Min/Max
        DrawHeaderInfo(g, width, topMargin, minVal, maxVal);
    }

    private void DrawHeaderInfo(Graphics g, int width, int topMargin, float minVal, float maxVal)
    {
        string title = "GÓC NGHIÊNG THỜI GIAN THỰC (°)";
        using (var titleBrush = new SolidBrush(Color.White))
        using (var titleFont = new Font("Segoe UI", 9F, FontStyle.Bold))
        {
            g.DrawString(title, titleFont, titleBrush, 48, 5);
        }

        string stats;
        Color valColor;

        if (Math.Abs(_latestAngle) >= _alarmThreshold)
            valColor = Color.FromArgb(255, 75, 75); // Đỏ cảnh báo ngã
        else if (Math.Abs(_latestAngle) <= 3.0f)
            valColor = Color.FromArgb(50, 225, 130); // Xanh thăng bằng tốt
        else
            valColor = Color.FromArgb(255, 195, 60); // Vàng đang bù nghiêng

        if (minVal <= maxVal && minVal != float.MaxValue)
        {
            stats = $"Hiện tại: {_latestAngle:+0.00;-0.00;0.00}° | Min: {minVal:+0.00;-0.00;0.00}° | Max: {maxVal:+0.00;-0.00;0.00}°";
        }
        else
        {
            stats = $"Hiện tại: {_latestAngle:+0.00;-0.00;0.00}°";
        }

        using (var statBrush = new SolidBrush(valColor))
        using (var statFont = new Font("Segoe UI", 9F, FontStyle.Bold))
        using (var sf = new StringFormat { Alignment = StringAlignment.Far })
        {
            g.DrawString(stats, statFont, statBrush, width - 16, 5, sf);
        }
    }

    private float DegreeToY(float degree, int plotHeight, int topMargin)
    {
        float clamped = Math.Clamp(degree, _minRange, _maxRange);
        float normalized = (clamped - _minRange) / (_maxRange - _minRange);
        // Trục Y ngược: góc dương ở trên, góc âm ở dưới
        return topMargin + plotHeight * (1.0f - normalized);
    }
}
