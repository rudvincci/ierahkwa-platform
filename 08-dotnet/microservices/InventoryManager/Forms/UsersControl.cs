using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using InventoryManager.Data;
using InventoryManager.Models;

namespace InventoryManager.Forms
{
    /// <summary>
    /// User management control (Admin only)
    /// </summary>
    public class UsersControl : XtraUserControl, IRefreshable
    {
        private readonly User _currentUser;
        private readonly UserRepository _userRepository = new();
        private readonly ActivityLogRepository _activityLog = new();

        private GridControl _gridUsers = null!;
        private GridView _gridView = null!;

        public UsersControl(User currentUser)
        {
            _currentUser = currentUser;
            InitializeComponents();
            LoadData();
        }

        private void InitializeComponents()
        {
            this.SuspendLayout();

            // Toolbar panel
            var toolbarPanel = new PanelControl
            {
                Dock = DockStyle.Top,
                Height = 60,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            this.Controls.Add(toolbarPanel);

            // Title
            var titleLabel = new LabelControl
            {
                Text = "User Management",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(10, 15)
            };
            toolbarPanel.Controls.Add(titleLabel);

            // Buttons
            var btnNew = new SimpleButton
            {
                Text = "New User",
                Location = new Point(400, 15),
                Size = new Size(100, 30)
            };
            btnNew.Click += BtnNew_Click;
            toolbarPanel.Controls.Add(btnNew);

            var btnEdit = new SimpleButton
            {
                Text = "Edit",
                Location = new Point(510, 15),
                Size = new Size(80, 30)
            };
            btnEdit.Click += BtnEdit_Click;
            toolbarPanel.Controls.Add(btnEdit);

            var btnResetPassword = new SimpleButton
            {
                Text = "Reset Password",
                Location = new Point(600, 15),
                Size = new Size(120, 30)
            };
            btnResetPassword.Click += BtnResetPassword_Click;
            toolbarPanel.Controls.Add(btnResetPassword);

            var btnDelete = new SimpleButton
            {
                Text = "Deactivate",
                Location = new Point(730, 15),
                Size = new Size(100, 30)
            };
            btnDelete.Click += BtnDelete_Click;
            toolbarPanel.Controls.Add(btnDelete);

            // Grid
            _gridUsers = new GridControl { Dock = DockStyle.Fill };
            _gridView = new GridView(_gridUsers);
            _gridUsers.MainView = _gridView;

            _gridView.OptionsView.ShowGroupPanel = false;
            _gridView.OptionsBehavior.Editable = false;
            _gridView.DoubleClick += GridView_DoubleClick;
            _gridView.RowStyle += GridView_RowStyle;

            _gridView.Columns.AddVisible("Username", "Username");
            _gridView.Columns.AddVisible("FullName", "Full Name");
            _gridView.Columns.AddVisible("Email", "Email");
            _gridView.Columns.AddVisible("Role", "Role");
            _gridView.Columns.AddVisible("IsActive", "Active");
            _gridView.Columns.AddVisible("LastLoginAt", "Last Login");
            _gridView.Columns.AddVisible("ComputerName", "Last Computer");

            _gridView.Columns["LastLoginAt"].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";

            this.Controls.Add(_gridUsers);
            _gridUsers.BringToFront();

            this.ResumeLayout(false);
        }

        private void LoadData()
        {
            try
            {
                _gridUsers.DataSource = _userRepository.GetAll();
                _gridView.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error loading users: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GridView_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            var view = sender as GridView;
            if (view == null) return;

            var isActive = (bool?)view.GetRowCellValue(e.RowHandle, "IsActive");
            if (isActive == false)
            {
                e.Appearance.ForeColor = Color.Gray;
                e.Appearance.BackColor = Color.LightGray;
            }
        }

        private void GridView_DoubleClick(object? sender, EventArgs e) => BtnEdit_Click(sender, e);

        private void BtnNew_Click(object? sender, EventArgs e) => ShowEditForm(null);

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            var user = GetSelectedUser();
            if (user == null)
            {
                XtraMessageBox.Show("Please select a user.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ShowEditForm(user);
        }

        private void BtnResetPassword_Click(object? sender, EventArgs e)
        {
            var user = GetSelectedUser();
            if (user == null) return;

            using var form = new ResetPasswordForm(user.Id, user.Username, _userRepository);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _activityLog.Log(_currentUser.Id, "RESET_PASSWORD", "Users", user.Id);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var user = GetSelectedUser();
            if (user == null) return;

            if (user.Id == _currentUser.Id)
            {
                XtraMessageBox.Show("You cannot deactivate your own account.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (XtraMessageBox.Show($"Deactivate user '{user.Username}'?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _userRepository.Delete(user.Id);
                _activityLog.Log(_currentUser.Id, "DEACTIVATE", "Users", user.Id);
                LoadData();
            }
        }

        private void ShowEditForm(User? user)
        {
            using var form = new UserEditForm(user, _userRepository);
            if (form.ShowDialog() == DialogResult.OK)
            {
                var action = user == null ? "CREATE" : "UPDATE";
                _activityLog.Log(_currentUser.Id, action, "Users", form.SavedUser?.Id);
                LoadData();
            }
        }

        private User? GetSelectedUser()
        {
            var rowHandle = _gridView.FocusedRowHandle;
            return rowHandle >= 0 ? _gridView.GetRow(rowHandle) as User : null;
        }

        public new void Refresh() => LoadData();
    }

    /// <summary>
    /// User edit dialog
    /// </summary>
    public class UserEditForm : XtraForm
    {
        private readonly User? _user;
        private readonly UserRepository _repository;
        public User? SavedUser { get; private set; }

        private TextEdit _txtUsername = null!, _txtFullName = null!, _txtEmail = null!;
        private TextEdit _txtPassword = null!, _txtConfirmPassword = null!;
        private ComboBoxEdit _cboRole = null!;
        private CheckEdit _chkActive = null!;

        public UserEditForm(User? user, UserRepository repository)
        {
            _user = user;
            _repository = repository;
            InitializeComponents();
            LoadData();
        }

        private void InitializeComponents()
        {
            this.Text = _user == null ? "New User" : "Edit User";
            this.Size = new Size(400, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int y = 20;

            this.Controls.Add(new LabelControl { Text = "Username:", Location = new Point(20, y + 3) });
            _txtUsername = new TextEdit { Location = new Point(140, y), Size = new Size(220, 25) };
            this.Controls.Add(_txtUsername);

            y += 35;
            this.Controls.Add(new LabelControl { Text = "Full Name:", Location = new Point(20, y + 3) });
            _txtFullName = new TextEdit { Location = new Point(140, y), Size = new Size(220, 25) };
            this.Controls.Add(_txtFullName);

            y += 35;
            this.Controls.Add(new LabelControl { Text = "Email:", Location = new Point(20, y + 3) });
            _txtEmail = new TextEdit { Location = new Point(140, y), Size = new Size(220, 25) };
            this.Controls.Add(_txtEmail);

            y += 35;
            this.Controls.Add(new LabelControl { Text = "Role:", Location = new Point(20, y + 3) });
            _cboRole = new ComboBoxEdit { Location = new Point(140, y), Size = new Size(220, 25) };
            _cboRole.Properties.Items.Add("Admin");
            _cboRole.Properties.Items.Add("Manager");
            _cboRole.Properties.Items.Add("User");
            _cboRole.Properties.Items.Add("ReadOnly");
            _cboRole.SelectedIndex = 2;
            this.Controls.Add(_cboRole);

            if (_user == null)
            {
                y += 35;
                this.Controls.Add(new LabelControl { Text = "Password:", Location = new Point(20, y + 3) });
                _txtPassword = new TextEdit { Location = new Point(140, y), Size = new Size(220, 25) };
                _txtPassword.Properties.PasswordChar = '*';
                this.Controls.Add(_txtPassword);

                y += 35;
                this.Controls.Add(new LabelControl { Text = "Confirm:", Location = new Point(20, y + 3) });
                _txtConfirmPassword = new TextEdit { Location = new Point(140, y), Size = new Size(220, 25) };
                _txtConfirmPassword.Properties.PasswordChar = '*';
                this.Controls.Add(_txtConfirmPassword);
            }

            y += 35;
            _chkActive = new CheckEdit { Text = "Active", Location = new Point(140, y), Checked = true };
            this.Controls.Add(_chkActive);

            y += 45;
            var btnSave = new SimpleButton { Text = "Save", Location = new Point(140, y), Size = new Size(100, 30) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            var btnCancel = new SimpleButton { Text = "Cancel", Location = new Point(250, y), Size = new Size(100, 30) };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancel);
        }

        private void LoadData()
        {
            if (_user == null) return;
            _txtUsername.Text = _user.Username;
            _txtFullName.Text = _user.FullName;
            _txtEmail.Text = _user.Email;
            _cboRole.SelectedIndex = (int)_user.Role - 1;
            _chkActive.Checked = _user.IsActive;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtUsername.Text) || string.IsNullOrWhiteSpace(_txtFullName.Text))
            {
                XtraMessageBox.Show("Username and Full Name are required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_user == null && (string.IsNullOrEmpty(_txtPassword?.Text) || 
                _txtPassword.Text != _txtConfirmPassword?.Text))
            {
                XtraMessageBox.Show("Passwords do not match or are empty.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var user = _user ?? new User();
                user.Username = _txtUsername.Text.Trim();
                user.FullName = _txtFullName.Text.Trim();
                user.Email = _txtEmail.Text?.Trim() ?? "";
                user.Role = (UserRole)(_cboRole.SelectedIndex + 1);
                user.IsActive = _chkActive.Checked;

                if (_user == null)
                    user.Id = _repository.Create(user, _txtPassword!.Text);
                else
                    _repository.Update(user);

                SavedUser = user;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error saving: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    /// Reset password dialog
    /// </summary>
    public class ResetPasswordForm : XtraForm
    {
        private readonly int _userId;
        private readonly UserRepository _repository;
        private TextEdit _txtPassword = null!, _txtConfirm = null!;

        public ResetPasswordForm(int userId, string username, UserRepository repository)
        {
            _userId = userId;
            _repository = repository;

            this.Text = $"Reset Password - {username}";
            this.Size = new Size(350, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Controls.Add(new LabelControl { Text = "New Password:", Location = new Point(20, 30) });
            _txtPassword = new TextEdit { Location = new Point(130, 27), Size = new Size(180, 25) };
            _txtPassword.Properties.PasswordChar = '*';
            this.Controls.Add(_txtPassword);

            this.Controls.Add(new LabelControl { Text = "Confirm:", Location = new Point(20, 65) });
            _txtConfirm = new TextEdit { Location = new Point(130, 62), Size = new Size(180, 25) };
            _txtConfirm.Properties.PasswordChar = '*';
            this.Controls.Add(_txtConfirm);

            var btnSave = new SimpleButton { Text = "Reset", Location = new Point(130, 110), Size = new Size(80, 30) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            var btnCancel = new SimpleButton { Text = "Cancel", Location = new Point(220, 110), Size = new Size(80, 30) };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancel);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_txtPassword.Text) || _txtPassword.Text.Length < 4)
            {
                XtraMessageBox.Show("Password must be at least 4 characters.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_txtPassword.Text != _txtConfirm.Text)
            {
                XtraMessageBox.Show("Passwords do not match.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _repository.ChangePassword(_userId, _txtPassword.Text);
            XtraMessageBox.Show("Password reset successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
