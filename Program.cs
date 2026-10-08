using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AutoShutdownApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class ModernButton : Button
    {
        private Color _normalColor;
        private Color _hoverColor;
        private Color _pressedColor;
        private int _borderRadius;

        public Color NormalColor
        {
            get { return _normalColor; }
            set { _normalColor = value; }
        }

        public Color HoverColor
        {
            get { return _hoverColor; }
            set { _hoverColor = value; }
        }

        public Color PressedColor
        {
            get { return _pressedColor; }
            set { _pressedColor = value; }
        }

        public int BorderRadius
        {
            get { return _borderRadius; }
            set { _borderRadius = value; }
        }

        private bool _isHovered;
        private bool _isPressed;

        public ModernButton()
        {
            _normalColor = Color.FromArgb(59, 130, 246);
            _hoverColor = Color.FromArgb(37, 99, 235);
            _pressedColor = Color.FromArgb(29, 78, 216);
            _borderRadius = 10;
            _isHovered = false;
            _isPressed = false;

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            Cursor = Cursors.Hand;
            DoubleBuffered = true;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            _isPressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            _isPressed = true;
            Invalidate();
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseUp(mevent);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color currentBg = _isPressed ? _pressedColor : (_isHovered ? _hoverColor : _normalColor);
            if (!Enabled)
            {
                currentBg = Color.FromArgb(50, 56, 70);
            }

            using (GraphicsPath path = GetRoundedRectangle(ClientRectangle, _borderRadius))
            using (SolidBrush brush = new SolidBrush(currentBg))
            {
                pevent.Graphics.Clear(Parent != null ? Parent.BackColor : Color.FromArgb(24, 27, 36));
                g.FillPath(brush, path);
            }

            Color textColor = Enabled ? ForeColor : Color.FromArgb(120, 125, 140);
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private static GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            Rectangle arc = new Rectangle(rect.X, rect.Y, d, d);
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - d - 1;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - d - 1;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public class ModernCard : Panel
    {
        private Color _borderLineColor;
        private int _borderRadius;

        public Color BorderLineColor
        {
            get { return _borderLineColor; }
            set { _borderLineColor = value; }
        }

        public int BorderRadius
        {
            get { return _borderRadius; }
            set { _borderRadius = value; }
        }

        public ModernCard()
        {
            _borderLineColor = Color.FromArgb(45, 52, 68);
            _borderRadius = 14;
            BackColor = Color.FromArgb(28, 33, 46);
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = GetRoundedRectangle(new Rectangle(0, 0, Width - 1, Height - 1), _borderRadius))
            using (Pen pen = new Pen(_borderLineColor, 1.2f))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private static GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public class MainForm : Form
    {
        private NumericUpDown numHours;
        private NumericUpDown numMinutes;
        private Label lblStatus;
        private Label lblTimeRemaining;
        private ModernButton btnStart;
        private ModernButton btnCancel;
        private Timer countdownTimer;
        private DateTime targetTime;
        private bool isScheduled;

        public MainForm()
        {
            isScheduled = false;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "AutoApagado";
            Width = 460;
            Height = 560;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(17, 20, 29);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9.5F);

            // Title label
            Label lblHeader = new Label();
            lblHeader.Text = "⚡ Temporizador de Apagado";
            lblHeader.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblHeader.ForeColor = Color.FromArgb(240, 243, 246);
            lblHeader.Location = new Point(24, 20);
            lblHeader.AutoSize = true;
            Controls.Add(lblHeader);

            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Programa la suspensión o apagado de tu equipo fácilmente";
            lblSubtitle.Font = new Font("Segoe UI", 9.5F);
            lblSubtitle.ForeColor = Color.FromArgb(148, 163, 184);
            lblSubtitle.Location = new Point(26, 52);
            lblSubtitle.AutoSize = true;
            Controls.Add(lblSubtitle);

            // Quick Presets Card
            ModernCard presetCard = new ModernCard();
            presetCard.Location = new Point(24, 85);
            presetCard.Size = new Size(396, 85);
            Controls.Add(presetCard);

            Label lblPresetsTitle = new Label();
            lblPresetsTitle.Text = "ACCESOS RÁPIDOS";
            lblPresetsTitle.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblPresetsTitle.ForeColor = Color.FromArgb(100, 116, 139);
            lblPresetsTitle.Location = new Point(14, 10);
            lblPresetsTitle.AutoSize = true;
            presetCard.Controls.Add(lblPresetsTitle);

            string[] presetLabels = new string[] { "15 min", "30 min", "1 hora", "2 horas" };
            int[] presetMinutes = new int[] { 15, 30, 60, 120 };

            for (int i = 0; i < presetLabels.Length; i++)
            {
                int mins = presetMinutes[i];
                ModernButton pBtn = new ModernButton();
                pBtn.Text = presetLabels[i];
                pBtn.NormalColor = Color.FromArgb(37, 44, 62);
                pBtn.HoverColor = Color.FromArgb(49, 58, 82);
                pBtn.PressedColor = Color.FromArgb(30, 36, 52);
                pBtn.BorderRadius = 8;
                pBtn.Size = new Size(82, 34);
                pBtn.Location = new Point(14 + (i * 92), 34);
                pBtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                pBtn.Click += delegate(object s, EventArgs e)
                {
                    numHours.Value = mins / 60;
                    numMinutes.Value = mins % 60;
                };
                presetCard.Controls.Add(pBtn);
            }

            // Custom Time Card
            ModernCard timeCard = new ModernCard();
            timeCard.Location = new Point(24, 182);
            timeCard.Size = new Size(396, 120);
            Controls.Add(timeCard);

            Label lblCustomTitle = new Label();
            lblCustomTitle.Text = "TIEMPO PERSONALIZADO";
            lblCustomTitle.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblCustomTitle.ForeColor = Color.FromArgb(100, 116, 139);
            lblCustomTitle.Location = new Point(14, 12);
            lblCustomTitle.AutoSize = true;
            timeCard.Controls.Add(lblCustomTitle);

            // Hours
            Label lblHours = new Label();
            lblHours.Text = "Horas";
            lblHours.Font = new Font("Segoe UI", 9F);
            lblHours.ForeColor = Color.FromArgb(203, 213, 225);
            lblHours.Location = new Point(40, 42);
            lblHours.AutoSize = true;
            timeCard.Controls.Add(lblHours);

            numHours = new NumericUpDown();
            numHours.Minimum = 0;
            numHours.Maximum = 72;
            numHours.Value = 0;
            numHours.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            numHours.ForeColor = Color.White;
            numHours.BackColor = Color.FromArgb(20, 24, 34);
            numHours.BorderStyle = BorderStyle.FixedSingle;
            numHours.TextAlign = HorizontalAlignment.Center;
            numHours.Location = new Point(40, 64);
            numHours.Width = 120;
            timeCard.Controls.Add(numHours);

            // Minutes
            Label lblMins = new Label();
            lblMins.Text = "Minutos";
            lblMins.Font = new Font("Segoe UI", 9F);
            lblMins.ForeColor = Color.FromArgb(203, 213, 225);
            lblMins.Location = new Point(230, 42);
            lblMins.AutoSize = true;
            timeCard.Controls.Add(lblMins);

            numMinutes = new NumericUpDown();
            numMinutes.Minimum = 0;
            numMinutes.Maximum = 59;
            numMinutes.Value = 30;
            numMinutes.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            numMinutes.ForeColor = Color.White;
            numMinutes.BackColor = Color.FromArgb(20, 24, 34);
            numMinutes.BorderStyle = BorderStyle.FixedSingle;
            numMinutes.TextAlign = HorizontalAlignment.Center;
            numMinutes.Location = new Point(230, 64);
            numMinutes.Width = 120;
            timeCard.Controls.Add(numMinutes);

            // Status Card
            ModernCard statusCard = new ModernCard();
            statusCard.Location = new Point(24, 314);
            statusCard.Size = new Size(396, 115);
            statusCard.BackColor = Color.FromArgb(22, 27, 39);
            Controls.Add(statusCard);

            lblStatus = new Label();
            lblStatus.Text = "Estado: En espera de programación";
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblStatus.ForeColor = Color.FromArgb(148, 163, 184);
            lblStatus.Location = new Point(14, 14);
            lblStatus.AutoSize = true;
            statusCard.Controls.Add(lblStatus);

            lblTimeRemaining = new Label();
            lblTimeRemaining.Text = "--:--:--";
            lblTimeRemaining.Font = new Font("Consolas", 28F, FontStyle.Bold);
            lblTimeRemaining.ForeColor = Color.FromArgb(100, 116, 139);
            lblTimeRemaining.Location = new Point(12, 42);
            lblTimeRemaining.Width = 370;
            lblTimeRemaining.TextAlign = ContentAlignment.MiddleCenter;
            statusCard.Controls.Add(lblTimeRemaining);

            // Action Buttons
            btnStart = new ModernButton();
            btnStart.Text = "Iniciar Apagado";
            btnStart.NormalColor = Color.FromArgb(37, 99, 235);
            btnStart.HoverColor = Color.FromArgb(59, 130, 246);
            btnStart.PressedColor = Color.FromArgb(29, 78, 216);
            btnStart.Location = new Point(24, 445);
            btnStart.Size = new Size(205, 48);
            btnStart.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
            btnStart.Click += BtnStart_Click;
            Controls.Add(btnStart);

            btnCancel = new ModernButton();
            btnCancel.Text = "Cancelar";
            btnCancel.NormalColor = Color.FromArgb(225, 29, 72);
            btnCancel.HoverColor = Color.FromArgb(244, 63, 94);
            btnCancel.PressedColor = Color.FromArgb(190, 18, 60);
            btnCancel.Location = new Point(245, 445);
            btnCancel.Size = new Size(175, 48);
            btnCancel.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
            btnCancel.Enabled = false;
            btnCancel.Click += BtnCancel_Click;
            Controls.Add(btnCancel);

            countdownTimer = new Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += CountdownTimer_Tick;
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            int totalSeconds = (int)((numHours.Value * 3600) + (numMinutes.Value * 60));

            if (totalSeconds <= 0)
            {
                MessageBox.Show("Por favor, introduce un tiempo mayor a 0 minutos.", "Tiempo requerido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("shutdown", string.Format("/s /t {0}", totalSeconds));
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                Process.Start(psi);

                targetTime = DateTime.Now.AddSeconds(totalSeconds);
                isScheduled = true;

                btnStart.Enabled = false;
                btnCancel.Enabled = true;
                numHours.Enabled = false;
                numMinutes.Enabled = false;

                lblStatus.Text = string.Format("Apagado fijado para las: {0}", targetTime.ToString("HH:mm:ss"));
                lblStatus.ForeColor = Color.FromArgb(74, 222, 128);
                lblTimeRemaining.ForeColor = Color.FromArgb(56, 189, 248);

                countdownTimer.Start();
                UpdateRemainingDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al programar el apagado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("shutdown", "/a");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                Process.Start(psi);

                countdownTimer.Stop();
                isScheduled = false;

                btnStart.Enabled = true;
                btnCancel.Enabled = false;
                numHours.Enabled = true;
                numMinutes.Enabled = true;

                lblStatus.Text = "Apagado cancelado correctamente";
                lblStatus.ForeColor = Color.FromArgb(248, 113, 113);
                lblTimeRemaining.Text = "--:--:--";
                lblTimeRemaining.ForeColor = Color.FromArgb(100, 116, 139);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cancelar el apagado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (!isScheduled) return;
            UpdateRemainingDisplay();
        }

        private void UpdateRemainingDisplay()
        {
            TimeSpan rem = targetTime - DateTime.Now;
            if (rem.TotalSeconds <= 0)
            {
                countdownTimer.Stop();
                lblTimeRemaining.Text = "00:00:00";
                lblStatus.Text = "Apagando el equipo...";
            }
            else
            {
                lblTimeRemaining.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)rem.TotalHours, rem.Minutes, rem.Seconds);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isScheduled)
            {
                DialogResult res = MessageBox.Show(
                    "Hay un apagado programado activo.\n\n¿Quieres que el apagado continúe en segundo plano?\n(Pulsa 'No' para cancelar el apagado antes de salir)",
                    "Apagado en curso",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (res == DialogResult.No)
                {
                    try
                    {
                        ProcessStartInfo psi = new ProcessStartInfo("shutdown", "/a");
                        psi.CreateNoWindow = true;
                        psi.UseShellExecute = false;
                        Process.Start(psi);
                    }
                    catch { }
                }
                else if (res == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            base.OnFormClosing(e);
        }
    }
}
