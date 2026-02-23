using DevExpress.XtraEditors;
using InventoryManager.Data;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Change password form for current user
    /// </summary>
    public class ChangePasswordForm : XtraForm
    {
        private readonly int _userId;
        private readonly UserRepository _userRepository = new();

        private TextEdit _txtCurrentPassword = null!;
        private TextEdit _txtNewPassword = null!;
        private TextEdit _txtConfirmPassword = null!;

        public ChangePasswordForm(int userId)
        {
            _userId = userId;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Change Password";
            this.Size = new Size(400, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int y = 30;

            this.Controls.Add(new LabelControl { Text = "Current Password:", Location = new Point(20, y + 3) });
            _txtCurrentPassword = new TextEdit { Location = new Point(150, y), Size = new Size(200, 25) };
            _txtCurrentPassword.Properties.PasswordChar = '*';
            this.Controls.Add(_txtCurrentPassword);

            y += 40;
            this.Controls.Add(new LabelControl { Text = "New Password:", Location = new Point(20, y + 3) });
            _txtNewPassword = new TextEdit { Location = new Point(150, y), Size = new Size(200, 25) };
            _txtNewPassword.Properties.PasswordChar = '*';
            this.Controls.Add(_txtNewPassword);

            y += 40;
            this.Controls.Add(new LabelControl { Text = "Confirm Password:", Location = new Point(20, y + 3) });
            _txtConfirmPassword = new TextEdit { Location = new Point(150, y), Size = new Size(200, 25) };
            _txtConfirmPassword.Properties.PasswordChar = '*';
            this.Controls.Add(_txtConfirmPassword);

            y += 50;
            var btnChange = new SimpleButton
            {
                Text = "Change Password",
                Location = new Point(150, y),
                Size = new Size(120, 35)
            };
            btnChange.Click += BtnChange_Click;
            this.Controls.Add(btnChange);

            var btnCancel = new SimpleButton
            {
                Text = "Cancel",
                Location = new Point(280, y),
                Size = new Size(80, 35)
            };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void BtnChange_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_txtCurrentPassword.Text))
            {
                XtraMessageBox.Show("Please enter your current password.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_txtNewPassword.Text) || _txtNewPassword.Text.Length < 4)
            {
                XtraMessageBox.Show("New password must be at least 4 characters.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_txtNewPassword.Text != _txtConfirmPassword.Text)
            {
                XtraMessageBox.Show("Passwords do not match.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Verify current password
                var user = _userRepository.GetById(_userId);
                if (user == null || !BCrypt.Net.BCrypt.Verify(_txtCurrentPassword.Text, user.PasswordHash))
                {
                    XtraMessageBox.Show("Current password is incorrect.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Change password
                _userRepository.ChangePassword(_userId, _txtNewPassword.Text);

                XtraMessageBox.Show("Password changed successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
