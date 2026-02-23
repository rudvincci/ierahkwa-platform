using DevExpress.XtraEditors;
using InventoryManager.Data;
using InventoryManager.Models;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Login form with user authentication
    /// </summary>
    public partial class LoginForm : XtraForm
    {
        private readonly UserRepository _userRepository = new();
        public User? CurrentUser { get; private set; }

        private TextEdit _txtUsername = null!;
        private TextEdit _txtPassword = null!;
        private SimpleButton _btnLogin = null!;
        private SimpleButton _btnCancel = null!;
        private CheckEdit _chkRemember = null!;
        private LabelControl _lblError = null!;

        public LoginForm()
        {
            InitializeComponent();
            LoadSavedCredentials();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form settings
            this.Text = "Login - Inventory Manager Pro";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(420, 350);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AcceptButton = null;
            this.CancelButton = null;

            // Panel for content
            var panel = new PanelControl
            {
                Dock = DockStyle.Fill,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            this.Controls.Add(panel);

            // Logo/Title area
            var titleLabel = new LabelControl
            {
                Text = "Inventory Manager Pro",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204),
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new Size(380, 40),
                Location = new Point(20, 20),
                Appearance = { TextOptions = { HAlignment = DevExpress.Utils.HorzAlignment.Center } }
            };
            panel.Controls.Add(titleLabel);

            var subtitleLabel = new LabelControl
            {
                Text = "Please enter your credentials",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new Size(380, 25),
                Location = new Point(20, 60),
                Appearance = { TextOptions = { HAlignment = DevExpress.Utils.HorzAlignment.Center } }
            };
            panel.Controls.Add(subtitleLabel);

            // Username label
            var lblUsername = new LabelControl
            {
                Text = "Username:",
                Location = new Point(50, 110),
                Font = new Font("Segoe UI", 10)
            };
            panel.Controls.Add(lblUsername);

            // Username textbox
            _txtUsername = new TextEdit
            {
                Location = new Point(50, 135),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 11)
            };
            _txtUsername.Properties.NullValuePrompt = "Enter your username";
            _txtUsername.Properties.NullValuePromptShowForEmptyValue = true;
            _txtUsername.KeyDown += TxtUsername_KeyDown;
            panel.Controls.Add(_txtUsername);

            // Password label
            var lblPassword = new LabelControl
            {
                Text = "Password:",
                Location = new Point(50, 175),
                Font = new Font("Segoe UI", 10)
            };
            panel.Controls.Add(lblPassword);

            // Password textbox
            _txtPassword = new TextEdit
            {
                Location = new Point(50, 200),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 11)
            };
            _txtPassword.Properties.PasswordChar = '*';
            _txtPassword.Properties.NullValuePrompt = "Enter your password";
            _txtPassword.Properties.NullValuePromptShowForEmptyValue = true;
            _txtPassword.KeyDown += TxtPassword_KeyDown;
            panel.Controls.Add(_txtPassword);

            // Remember me checkbox
            _chkRemember = new CheckEdit
            {
                Text = "Remember me",
                Location = new Point(50, 240),
                Size = new Size(150, 25)
            };
            panel.Controls.Add(_chkRemember);

            // Error label
            _lblError = new LabelControl
            {
                Text = "",
                ForeColor = Color.Red,
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new Size(300, 20),
                Location = new Point(50, 265),
                Visible = false
            };
            panel.Controls.Add(_lblError);

            // Login button
            _btnLogin = new SimpleButton
            {
                Text = "Login",
                Location = new Point(50, 290),
                Size = new Size(145, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Appearance = { BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White }
            };
            _btnLogin.Click += BtnLogin_Click;
            panel.Controls.Add(_btnLogin);

            // Cancel button
            _btnCancel = new SimpleButton
            {
                Text = "Exit",
                Location = new Point(205, 290),
                Size = new Size(145, 35),
                Font = new Font("Segoe UI", 10)
            };
            _btnCancel.Click += BtnCancel_Click;
            panel.Controls.Add(_btnCancel);

            this.ResumeLayout(false);
        }

        private void LoadSavedCredentials()
        {
            try
            {
                var settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "InventoryManager", "login.dat");

                if (File.Exists(settingsPath))
                {
                    var lines = File.ReadAllLines(settingsPath);
                    if (lines.Length >= 1)
                    {
                        _txtUsername.Text = Decrypt(lines[0]);
                        _chkRemember.Checked = true;
                    }
                }
            }
            catch { /* Ignore errors */ }
        }

        private void SaveCredentials()
        {
            try
            {
                var settingsDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "InventoryManager");
                
                if (!Directory.Exists(settingsDir))
                    Directory.CreateDirectory(settingsDir);

                var settingsPath = Path.Combine(settingsDir, "login.dat");

                if (_chkRemember.Checked)
                {
                    File.WriteAllText(settingsPath, Encrypt(_txtUsername.Text));
                }
                else if (File.Exists(settingsPath))
                {
                    File.Delete(settingsPath);
                }
            }
            catch { /* Ignore errors */ }
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            PerformLogin();
        }

        private void PerformLogin()
        {
            _lblError.Visible = false;

            var username = _txtUsername.Text?.Trim();
            var password = _txtPassword.Text;

            if (string.IsNullOrEmpty(username))
            {
                ShowError("Please enter your username");
                _txtUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Please enter your password");
                _txtPassword.Focus();
                return;
            }

            try
            {
                _btnLogin.Enabled = false;
                _btnLogin.Text = "Logging in...";
                Application.DoEvents();

                CurrentUser = _userRepository.Authenticate(username, password);

                if (CurrentUser != null)
                {
                    SaveCredentials();
                    
                    // Log the login
                    var activityLog = new ActivityLogRepository();
                    activityLog.Log(CurrentUser.Id, "LOGIN", "Users", CurrentUser.Id);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowError("Invalid username or password");
                    _txtPassword.Text = "";
                    _txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Login error: {ex.Message}");
            }
            finally
            {
                _btnLogin.Enabled = true;
                _btnLogin.Text = "Login";
            }
        }

        private void ShowError(string message)
        {
            _lblError.Text = message;
            _lblError.Visible = true;
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void TxtUsername_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _txtPassword.Focus();
                e.Handled = true;
            }
        }

        private void TxtPassword_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PerformLogin();
                e.Handled = true;
            }
        }

        // Simple encryption for stored username (not highly secure, just basic obfuscation)
        private string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        private string Decrypt(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            try
            {
                var bytes = Convert.FromBase64String(text);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch { return ""; }
        }
    }
}
