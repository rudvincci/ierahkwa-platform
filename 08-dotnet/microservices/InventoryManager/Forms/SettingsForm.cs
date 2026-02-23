using DevExpress.XtraEditors;
using InventoryManager.Data;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Application settings form
    /// </summary>
    public class SettingsForm : XtraForm
    {
        private readonly SettingsRepository _settingsRepository = new();

        private TextEdit _txtCompanyName = null!;
        private TextEdit _txtCompanyAddress = null!;
        private TextEdit _txtCompanyPhone = null!;
        private TextEdit _txtCompanyEmail = null!;
        private TextEdit _txtCurrency = null!;
        private TextEdit _txtBackupPath = null!;
        private CheckEdit _chkAutoBackup = null!;
        private SpinEdit _txtBackupInterval = null!;
        private CheckEdit _chkLowStockAlert = null!;
        private ComboBoxEdit _cboTheme = null!;

        public SettingsForm()
        {
            InitializeComponents();
            LoadSettings();
        }

        private void InitializeComponents()
        {
            this.Text = "Application Settings";
            this.Size = new Size(550, 550);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Company Info Group
            var grpCompany = new GroupControl
            {
                Text = "Company Information",
                Location = new Point(20, 20),
                Size = new Size(500, 160)
            };
            this.Controls.Add(grpCompany);

            int y = 30;
            grpCompany.Controls.Add(new LabelControl { Text = "Name:", Location = new Point(15, y + 3) });
            _txtCompanyName = new TextEdit { Location = new Point(120, y), Size = new Size(360, 25) };
            grpCompany.Controls.Add(_txtCompanyName);

            y += 30;
            grpCompany.Controls.Add(new LabelControl { Text = "Address:", Location = new Point(15, y + 3) });
            _txtCompanyAddress = new TextEdit { Location = new Point(120, y), Size = new Size(360, 25) };
            grpCompany.Controls.Add(_txtCompanyAddress);

            y += 30;
            grpCompany.Controls.Add(new LabelControl { Text = "Phone:", Location = new Point(15, y + 3) });
            _txtCompanyPhone = new TextEdit { Location = new Point(120, y), Size = new Size(200, 25) };
            grpCompany.Controls.Add(_txtCompanyPhone);

            y += 30;
            grpCompany.Controls.Add(new LabelControl { Text = "Email:", Location = new Point(15, y + 3) });
            _txtCompanyEmail = new TextEdit { Location = new Point(120, y), Size = new Size(200, 25) };
            grpCompany.Controls.Add(_txtCompanyEmail);

            // Application Group
            var grpApp = new GroupControl
            {
                Text = "Application Settings",
                Location = new Point(20, 190),
                Size = new Size(500, 120)
            };
            this.Controls.Add(grpApp);

            y = 30;
            grpApp.Controls.Add(new LabelControl { Text = "Currency:", Location = new Point(15, y + 3) });
            _txtCurrency = new TextEdit { Location = new Point(120, y), Size = new Size(100, 25) };
            grpApp.Controls.Add(_txtCurrency);

            grpApp.Controls.Add(new LabelControl { Text = "Theme:", Location = new Point(240, y + 3) });
            _cboTheme = new ComboBoxEdit { Location = new Point(300, y), Size = new Size(180, 25) };
            _cboTheme.Properties.Items.Add("Office2019Colorful");
            _cboTheme.Properties.Items.Add("Office2019Black");
            _cboTheme.Properties.Items.Add("Office2019White");
            _cboTheme.Properties.Items.Add("VS2017Blue");
            _cboTheme.Properties.Items.Add("VS2017Dark");
            grpApp.Controls.Add(_cboTheme);

            y += 35;
            _chkLowStockAlert = new CheckEdit
            {
                Text = "Enable Low Stock Alerts",
                Location = new Point(120, y),
                Size = new Size(200, 25)
            };
            grpApp.Controls.Add(_chkLowStockAlert);

            // Backup Group
            var grpBackup = new GroupControl
            {
                Text = "Backup Settings",
                Location = new Point(20, 320),
                Size = new Size(500, 130)
            };
            this.Controls.Add(grpBackup);

            y = 30;
            grpBackup.Controls.Add(new LabelControl { Text = "Backup Path:", Location = new Point(15, y + 3) });
            _txtBackupPath = new TextEdit { Location = new Point(120, y), Size = new Size(290, 25) };
            grpBackup.Controls.Add(_txtBackupPath);

            var btnBrowse = new SimpleButton { Text = "...", Location = new Point(420, y), Size = new Size(60, 25) };
            btnBrowse.Click += BtnBrowse_Click;
            grpBackup.Controls.Add(btnBrowse);

            y += 35;
            _chkAutoBackup = new CheckEdit
            {
                Text = "Enable Auto Backup",
                Location = new Point(120, y),
                Size = new Size(150, 25)
            };
            grpBackup.Controls.Add(_chkAutoBackup);

            y += 30;
            grpBackup.Controls.Add(new LabelControl { Text = "Interval (hours):", Location = new Point(15, y + 3) });
            _txtBackupInterval = new SpinEdit { Location = new Point(120, y), Size = new Size(80, 25) };
            _txtBackupInterval.Properties.MinValue = 1;
            _txtBackupInterval.Properties.MaxValue = 168;
            grpBackup.Controls.Add(_txtBackupInterval);

            // Buttons
            var btnSave = new SimpleButton
            {
                Text = "Save Settings",
                Location = new Point(300, 465),
                Size = new Size(110, 35)
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            var btnCancel = new SimpleButton
            {
                Text = "Cancel",
                Location = new Point(420, 465),
                Size = new Size(100, 35)
            };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void LoadSettings()
        {
            try
            {
                var settings = _settingsRepository.GetAll();

                _txtCompanyName.Text = settings.GetValueOrDefault("CompanyName", "");
                _txtCompanyAddress.Text = settings.GetValueOrDefault("CompanyAddress", "");
                _txtCompanyPhone.Text = settings.GetValueOrDefault("CompanyPhone", "");
                _txtCompanyEmail.Text = settings.GetValueOrDefault("CompanyEmail", "");
                _txtCurrency.Text = settings.GetValueOrDefault("Currency", "USD");
                _txtBackupPath.Text = settings.GetValueOrDefault("BackupPath", "");
                _chkAutoBackup.Checked = settings.GetValueOrDefault("AutoBackup", "false") == "true";
                _txtBackupInterval.Value = int.Parse(settings.GetValueOrDefault("AutoBackupInterval", "24"));
                _chkLowStockAlert.Checked = settings.GetValueOrDefault("LowStockAlert", "true") == "true";

                var theme = settings.GetValueOrDefault("Theme", "Office2019Colorful");
                _cboTheme.SelectedIndex = _cboTheme.Properties.Items.IndexOf(theme);
                if (_cboTheme.SelectedIndex < 0) _cboTheme.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error loading settings: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBrowse_Click(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = _txtBackupPath.Text;
            
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _txtBackupPath.Text = dialog.SelectedPath;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                _settingsRepository.SetValue("CompanyName", _txtCompanyName.Text?.Trim() ?? "");
                _settingsRepository.SetValue("CompanyAddress", _txtCompanyAddress.Text?.Trim() ?? "");
                _settingsRepository.SetValue("CompanyPhone", _txtCompanyPhone.Text?.Trim() ?? "");
                _settingsRepository.SetValue("CompanyEmail", _txtCompanyEmail.Text?.Trim() ?? "");
                _settingsRepository.SetValue("Currency", _txtCurrency.Text?.Trim() ?? "USD");
                _settingsRepository.SetValue("BackupPath", _txtBackupPath.Text?.Trim() ?? "");
                _settingsRepository.SetValue("AutoBackup", _chkAutoBackup.Checked ? "true" : "false");
                _settingsRepository.SetValue("AutoBackupInterval", ((int)_txtBackupInterval.Value).ToString());
                _settingsRepository.SetValue("LowStockAlert", _chkLowStockAlert.Checked ? "true" : "false");
                _settingsRepository.SetValue("Theme", _cboTheme.Text ?? "Office2019Colorful");

                XtraMessageBox.Show("Settings saved successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error saving settings: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
