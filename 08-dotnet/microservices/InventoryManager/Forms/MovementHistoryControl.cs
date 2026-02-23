using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using InventoryManager.Data;
using InventoryManager.Models;
using InventoryManager.Services;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Stock movement history control
    /// </summary>
    public class MovementHistoryControl : XtraUserControl, IRefreshable, IExportable
    {
        private readonly User _currentUser;
        private readonly StockMovementRepository _movementRepository = new();

        private GridControl _gridMovements = null!;
        private GridView _gridView = null!;
        private DateEdit _dtStart = null!;
        private DateEdit _dtEnd = null!;
        private ComboBoxEdit _cboType = null!;

        public MovementHistoryControl(User currentUser)
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
                Height = 70,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            this.Controls.Add(toolbarPanel);

            // Title
            var titleLabel = new LabelControl
            {
                Text = "Stock Movement History",
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
                DateTime = DateTime.Today.AddMonths(-1)
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

            toolbarPanel.Controls.Add(new LabelControl { Text = "Type:", Location = new Point(340, 42) });
            _cboType = new ComboBoxEdit
            {
                Location = new Point(380, 38),
                Size = new Size(150, 25)
            };
            _cboType.Properties.Items.Add("All Types");
            _cboType.Properties.Items.Add("Purchase");
            _cboType.Properties.Items.Add("Sale");
            _cboType.Properties.Items.Add("Return");
            _cboType.Properties.Items.Add("Adjustment");
            _cboType.Properties.Items.Add("Transfer");
            _cboType.Properties.Items.Add("Damage");
            _cboType.Properties.Items.Add("Initial");
            _cboType.SelectedIndex = 0;
            toolbarPanel.Controls.Add(_cboType);

            var btnFilter = new SimpleButton
            {
                Text = "Apply Filter",
                Location = new Point(550, 36),
                Size = new Size(100, 28)
            };
            btnFilter.Click += (s, e) => LoadData();
            toolbarPanel.Controls.Add(btnFilter);

            // Grid
            _gridMovements = new GridControl { Dock = DockStyle.Fill };
            _gridView = new GridView(_gridMovements);
            _gridMovements.MainView = _gridView;

            _gridView.OptionsView.ShowGroupPanel = true;
            _gridView.OptionsView.ShowFooter = true;
            _gridView.OptionsBehavior.Editable = false;
            _gridView.RowStyle += GridView_RowStyle;

            _gridView.Columns.AddVisible("DocumentNumber", "Document #");
            _gridView.Columns.AddVisible("MovementDate", "Date");
            _gridView.Columns.AddVisible("ProductCode", "Code");
            _gridView.Columns.AddVisible("ProductName", "Product");
            _gridView.Columns.AddVisible("Type", "Type");
            _gridView.Columns.AddVisible("PreviousStock", "Prev Stock");
            _gridView.Columns.AddVisible("Quantity", "Qty");
            _gridView.Columns.AddVisible("NewStock", "New Stock");
            _gridView.Columns.AddVisible("UnitPrice", "Unit Price");
            _gridView.Columns.AddVisible("TotalValue", "Total");
            _gridView.Columns.AddVisible("Reference", "Reference");
            _gridView.Columns.AddVisible("UserName", "User");

            _gridView.Columns["MovementDate"].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
            _gridView.Columns["UnitPrice"].DisplayFormat.FormatString = "N2";
            _gridView.Columns["TotalValue"].DisplayFormat.FormatString = "N2";
            _gridView.Columns["TotalValue"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            _gridView.Columns["TotalValue"].SummaryItem.DisplayFormat = "Total: ${0:N2}";

            this.Controls.Add(_gridMovements);
            _gridMovements.BringToFront();

            this.ResumeLayout(false);
        }

        private void LoadData()
        {
            try
            {
                var movements = _movementRepository.GetAll(_dtStart.DateTime, _dtEnd.DateTime);

                // Filter by type
                var selectedType = _cboType.SelectedIndex;
                if (selectedType > 0)
                {
                    var type = (MovementType)selectedType;
                    movements = movements.Where(m => m.Type == type).ToList();
                }

                _gridMovements.DataSource = movements;
                _gridView.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error loading movements: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GridView_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            var view = sender as GridView;
            if (view == null) return;

            var type = view.GetRowCellValue(e.RowHandle, "Type")?.ToString();

            if (type == MovementType.Purchase.ToString() || type == MovementType.Return.ToString())
                e.Appearance.ForeColor = Color.Green;
            else if (type == MovementType.Sale.ToString() || type == MovementType.Damage.ToString())
                e.Appearance.ForeColor = Color.Red;
            else if (type == MovementType.Adjustment.ToString())
                e.Appearance.ForeColor = Color.Orange;
        }

        public new void Refresh() => LoadData();
        public void ExportToExcel() => ExportService.ExportGridToExcel(_gridView, "StockMovements");
        public void ExportToPdf() => ExportService.ExportGridToPdf(_gridView, "StockMovements");
        public void Print() => ExportService.PrintGrid(_gridView, "Stock Movement Report");
    }
}
