using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Splash screen form shown during application startup
    /// </summary>
    public partial class SplashScreenForm : XtraForm
    {
        private System.Windows.Forms.Timer _timer;
        private ProgressBarControl _progressBar;
        private LabelControl _statusLabel;
        private LabelControl _titleLabel;
        private LabelControl _versionLabel;
        private int _progress = 0;

        public SplashScreenForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(500, 300);
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ShowInTaskbar = false;

            // Title label
            _titleLabel = new LabelControl
            {
                Text = "INVENTORY MANAGER PRO",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new Size(460, 50),
                Location = new Point(20, 60),
                Appearance = { TextOptions = { HAlignment = DevExpress.Utils.HorzAlignment.Center } }
            };
            this.Controls.Add(_titleLabel);

            // Subtitle
            var subtitleLabel = new LabelControl
            {
                Text = "Sovereign Akwesasne Government",
                Font = new Font("Segoe UI", 12, FontStyle.Italic),
                ForeColor = Color.FromArgb(200, 200, 200),
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new Size(460, 25),
                Location = new Point(20, 115),
                Appearance = { TextOptions = { HAlignment = DevExpress.Utils.HorzAlignment.Center } }
            };
            this.Controls.Add(subtitleLabel);

            // Progress bar
            _progressBar = new ProgressBarControl
            {
                Location = new Point(50, 200),
                Size = new Size(400, 20),
                Properties = { Step = 1, PercentView = false }
            };
            this.Controls.Add(_progressBar);

            // Status label
            _statusLabel = new LabelControl
            {
                Text = "Initializing...",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new Size(400, 20),
                Location = new Point(50, 225),
                Appearance = { TextOptions = { HAlignment = DevExpress.Utils.HorzAlignment.Center } }
            };
            this.Controls.Add(_statusLabel);

            // Version label
            _versionLabel = new LabelControl
            {
                Text = "Version 1.0.0",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new Size(460, 20),
                Location = new Point(20, 270),
                Appearance = { TextOptions = { HAlignment = DevExpress.Utils.HorzAlignment.Center } }
            };
            this.Controls.Add(_versionLabel);

            // Timer for progress
            _timer = new System.Windows.Forms.Timer
            {
                Interval = 30
            };
            _timer.Tick += Timer_Tick;

            this.Load += SplashScreenForm_Load;
            
            this.ResumeLayout(false);
        }

        private void SplashScreenForm_Load(object? sender, EventArgs e)
        {
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _progress += 2;
            _progressBar.Position = _progress;

            // Update status messages
            if (_progress < 20)
                _statusLabel.Text = "Initializing application...";
            else if (_progress < 40)
                _statusLabel.Text = "Loading database...";
            else if (_progress < 60)
                _statusLabel.Text = "Checking configuration...";
            else if (_progress < 80)
                _statusLabel.Text = "Loading components...";
            else
                _statusLabel.Text = "Ready!";

            if (_progress >= 100)
            {
                _timer.Stop();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
