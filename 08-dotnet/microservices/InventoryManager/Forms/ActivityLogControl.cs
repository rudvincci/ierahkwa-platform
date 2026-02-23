using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using InventoryManager.Data;
using InventoryManager.Models;
using InventoryManager.Services;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Activity log control for audit trail viewing
    /// </summary>
    public class ActivityLogControl : XtraUserControl, IRefreshable, IExportable
    {
        private readonly User _currentUser;
        private readonly ActivityLogRepository _activityLogRepository = new();

        private GridControl _gridLog = null!;
        private GridView _gridView = null!;
        private DateEdit _dtStart = null!, _dtEnd = null!;
        private ComboBoxEdit _cboTable = null!;

        public ActivityLogControl(User currentUser)
        {
            _currentUser = currentUser;
            InitializeComponents();
            LoadData();
        }

        private void InitializeComponents()
        {
            this.SuspendLayout();

            // Toolbar
            var toolbarPanel = new PanelControl
            {
                Dock = DockStyle.Top,
                Height = 70,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            this.Controls.Add(toolbarPanel);

            var titleLabel = new LabelControl
            {
                Text = "Activity Log - Audit Trail",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(10, 10)
            };
            toolbarPanel.Controls.Add(titleLabel);

            // Filters
            toolbarPanel.Controls.Add(new LabelControl { Text = "From:", Location = new Point(10, 42) });
            _dtStart = new DateEdit
            {
                Location = new Point(50, 38),
                Size = new Size(120, 25),
                DateTime = DateTime.Today.AddDays(-7)
            };
            toolbarPanel.Controls.Add(_dtStart);

            toolbarPanel.Controls.Add(new LabelControl { Text = "To:", Location = new Point(180, 42) });
            _dtEnd = new DateEdit
            {
                Location = new Point(205, 38),
                Size = new Size(120, 25),
                DateTime = DateTime.Today
            };
            toolbarPanel.Controls.Add(_dtEnd);

            toolbarPanel.Controls.Add(new LabelControl { Text = "Table:", Location = new Point(340, 42) });
            _cboTable = new ComboBoxEdit
            {
                Location = new Point(385, 38),
                Size = new Size(120, 25)
            };
            _cboTable.Properties.Items.Add("All Tables");
            _cboTable.Properties.Items.Add("Products");
            _cboTable.Properties.Items.Add("Categories");
            _cboTable.Properties.Items.Add("Suppliers");
            _cboTable.Properties.Items.Add("StockMovements");
            _cboTable.Properties.Items.Add("Users");
            _cboTable.SelectedIndex = 0;
            toolbarPanel.Controls.Add(_cboTable);

            var btnFilter = new SimpleButton
            {
                Text = "Filter",
                Location = new Point(520, 36),
                Size = new Size(80, 28)
            };
            btnFilter.Click += (s, e) => LoadData();
            toolbarPanel.Controls.Add(btnFilter);

            var btnClearOld = new SimpleButton
            {
                Text = "Clear Old Logs",
                Location = new Point(610, 36),
                Size = new Size(120, 28)
            };
            btnClearOld.Click += BtnClearOld_Click;
            toolbarPanel.Controls.Add(btnClearOld);

            // Grid
            _gridLog = new GridControl { Dock = DockStyle.Fill };
            _gridView = new GridView(_gridLog);
            _gridLog.MainView = _gridView;

            _gridView.OptionsView.ShowGroupPanel = true;
            _gridView.OptionsBehavior.Editable = false;

            _gridView.Columns.AddVisible("CreatedAt", "Date/Time");
            _gridView.Columns.AddVisible("UserName", "User");
            _gridView.Columns.AddVisible("Action", "Action");
            _gridView.Columns.AddVisible("TableName", "Table");
            _gridView.Columns.AddVisible("RecordId", "Record ID");
            _gridView.Columns.AddVisible("ComputerName", "Computer");
            _gridView.Columns.AddVisible("IpAddress", "IP Address");
            _gridView.Columns.AddVisible("OldValues", "Old Values");
            _gridView.Columns.AddVisible("NewValues", "New Values");

            _gridView.Columns["CreatedAt"].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            _gridView.Columns["OldValues"].Width = 200;
            _gridView.Columns["NewValues"].Width = 200;

            this.Controls.Add(_gridLog);
            _gridLog.BringToFront();

            this.ResumeLayout(false);
        }

        private void LoadData()
        {
            try
            {
                var tableName = _cboTable.SelectedIndex > 0 ? _cboTable.Text : null;
                var logs = _activityLogRepository.GetAll(_dtStart.DateTime, _dtEnd.DateTime, tableName: tableName);
                _gridLog.DataSource = logs;
                _gridView.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error loading logs: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearOld_Click(object? sender, EventArgs e)
        {
            if (XtraMessageBox.Show("Clear logs older than 90 days?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var count = _activityLogRepository.ClearOldLogs(90);
                XtraMessageBox.Show($"Cleared {count} old log entries.", "Done",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }

        public new void Refresh() => LoadData();
        public void ExportToExcel() => ExportService.ExportGridToExcel(_gridView, "ActivityLog");
        public void ExportToPdf() => ExportService.ExportGridToPdf(_gridView, "ActivityLog");
        public void Print() => ExportService.PrintGrid(_gridView, "Activity Log Report");
    }
}
