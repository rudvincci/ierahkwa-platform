using DevExpress.XtraEditors;

namespace InventoryManager.Forms
{
    /// <summary>
    /// About dialog
    /// </summary>
    public class AboutForm : XtraForm
    {
        public AboutForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "About Inventory Manager Pro";
            this.Size = new Size(450, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Logo/Title
            var titleLabel = new LabelControl
            {
                Text = "Inventory Manager Pro",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204),
                Location = new Point(20, 30),
                AutoSizeMode = LabelAutoSizeMode.Default
            };
            this.Controls.Add(titleLabel);

            // Version
            var versionLabel = new LabelControl
            {
                Text = "Version 1.0.0",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray,
                Location = new Point(20, 70)
            };
            this.Controls.Add(versionLabel);

            // Description
            var descriptionLabel = new LabelControl
            {
                Text = "Professional Inventory Management System\nwith DevExpress & SQLite",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 100),
                AutoSizeMode = LabelAutoSizeMode.Default
            };
            this.Controls.Add(descriptionLabel);

            // Features
            var featuresLabel = new LabelControl
            {
                Text = "Features:\n" +
                       "• Multi-user support\n" +
                       "• Real-time stock tracking\n" +
                       "• Barcode scanner support\n" +
                       "• Advanced reporting & exports\n" +
                       "• Database backup/restore\n" +
                       "• Activity logging",
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 150),
                AutoSizeMode = LabelAutoSizeMode.Default
            };
            this.Controls.Add(featuresLabel);

            // Copyright
            var copyrightLabel = new LabelControl
            {
                Text = "© 2026 Sovereign Akwesasne Government\nAll Rights Reserved",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Location = new Point(20, 260),
                AutoSizeMode = LabelAutoSizeMode.Default
            };
            this.Controls.Add(copyrightLabel);

            // Close button
            var btnClose = new SimpleButton
            {
                Text = "Close",
                Location = new Point(340, 270),
                Size = new Size(80, 30)
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }
    }
}
