using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using InventoryManager.Data;
using InventoryManager.Models;
using InventoryManager.Services;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Reports control with various report types
    /// </summary>
    public class ReportsControl : XtraUserControl, IRefreshable, IExportable
    {
        private readonly User _currentUser;
        private readonly string _reportType;
        private readonly ProductRepository _productRepository = new();
        private readonly StockMovementRepository _movementRepository = new();

        private GridControl _gridReport = null!;
        private GridView _gridView = null!;
        private LabelControl _lblTitle = null!;
        private LabelControl _lblSummary = null!;

        public ReportsControl(User currentUser, string reportType)
        {
            _currentUser = currentUser;
            _reportType = reportType;
            InitializeComponents();
            LoadReport();
        }

        private void InitializeComponents()
        {
            this.SuspendLayout();

            // Title
            _lblTitle = new LabelControl
            {
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 20)
            };
            this.Controls.Add(_lblTitle);

            // Summary
            _lblSummary = new LabelControl
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 55),
                ForeColor = Color.Gray
            };
            this.Controls.Add(_lblSummary);

            // Export buttons panel
            var buttonPanel = new PanelControl
            {
                Location = new Point(20, 85),
                Size = new Size(800, 40),
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            this.Controls.Add(buttonPanel);

            var btnExcel = new SimpleButton
            {
                Text = "Export to Excel",
                Location = new Point(0, 5),
                Size = new Size(120, 30)
            };
            btnExcel.Click += (s, e) => ExportToExcel();
            buttonPanel.Controls.Add(btnExcel);

            var btnPdf = new SimpleButton
            {
                Text = "Export to PDF",
                Location = new Point(130, 5),
                Size = new Size(120, 30)
            };
            btnPdf.Click += (s, e) => ExportToPdf();
            buttonPanel.Controls.Add(btnPdf);

            var btnHtml = new SimpleButton
            {
                Text = "Export to HTML",
                Location = new Point(260, 5),
                Size = new Size(120, 30)
            };
            btnHtml.Click += (s, e) => ExportToHtml();
            buttonPanel.Controls.Add(btnHtml);

            var btnTxt = new SimpleButton
            {
                Text = "Export to TXT",
                Location = new Point(390, 5),
                Size = new Size(120, 30)
            };
            btnTxt.Click += (s, e) => ExportToTxt();
            buttonPanel.Controls.Add(btnTxt);

            var btnPrint = new SimpleButton
            {
                Text = "Print",
                Location = new Point(520, 5),
                Size = new Size(100, 30)
            };
            btnPrint.Click += (s, e) => Print();
            buttonPanel.Controls.Add(btnPrint);

            var btnRefresh = new SimpleButton
            {
                Text = "Refresh",
                Location = new Point(630, 5),
                Size = new Size(100, 30)
            };
            btnRefresh.Click += (s, e) => LoadReport();
            buttonPanel.Controls.Add(btnRefresh);

            // Grid
            _gridReport = new GridControl
            {
                Location = new Point(20, 130),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            _gridView = new GridView(_gridReport);
            _gridReport.MainView = _gridView;

            _gridView.OptionsView.ShowGroupPanel = true;
            _gridView.OptionsView.ShowFooter = true;
            _gridView.OptionsBehavior.Editable = false;

            this.Controls.Add(_gridReport);

            this.Resize += ReportsControl_Resize;

            this.ResumeLayout(false);
        }

        private void ReportsControl_Resize(object? sender, EventArgs e)
        {
            _gridReport.Size = new Size(this.Width - 40, this.Height - 150);
        }

        private void LoadReport()
        {
            _gridView.Columns.Clear();

            switch (_reportType)
            {
                case "Stock":
                    LoadStockReport();
                    break;
                case "LowStock":
                    LoadLowStockReport();
                    break;
                case "Movement":
                    LoadMovementReport();
                    break;
                default:
                    LoadStockReport();
                    break;
            }

            _gridView.BestFitColumns();
        }

        private void LoadStockReport()
        {
            _lblTitle.Text = "Stock Report";
            
            var products = _productRepository.GetAll();
            var (totalValue, totalItems, lowStockCount) = _productRepository.GetStockSummary();
            
            _lblSummary.Text = $"Total Products: {products.Count} | Total Items: {totalItems:N0} | " +
                              $"Total Value: ${totalValue:N2} | Low Stock Items: {lowStockCount}";

            _gridView.Columns.AddVisible("Code", "Code");
            _gridView.Columns.AddVisible("Name", "Product");
            _gridView.Columns.AddVisible("CategoryName", "Category");
            _gridView.Columns.AddVisible("CurrentStock", "Stock");
            _gridView.Columns.AddVisible("MinimumStock", "Min");
            _gridView.Columns.AddVisible("MaximumStock", "Max");
            _gridView.Columns.AddVisible("PurchasePrice", "Purchase $");
            _gridView.Columns.AddVisible("SalePrice", "Sale $");
            _gridView.Columns.AddVisible("StockValue", "Value");
            _gridView.Columns.AddVisible("Location", "Location");
            _gridView.Columns.AddVisible("StockStatus", "Status");

            _gridView.Columns["PurchasePrice"].DisplayFormat.FormatString = "N2";
            _gridView.Columns["SalePrice"].DisplayFormat.FormatString = "N2";
            _gridView.Columns["StockValue"].DisplayFormat.FormatString = "N2";

            _gridView.Columns["StockValue"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            _gridView.Columns["StockValue"].SummaryItem.DisplayFormat = "Total: ${0:N2}";
            _gridView.Columns["CurrentStock"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            _gridView.Columns["CurrentStock"].SummaryItem.DisplayFormat = "Total: {0:N0}";

            _gridReport.DataSource = products;
        }

        private void LoadLowStockReport()
        {
            _lblTitle.Text = "Low Stock Alert Report";
            _lblTitle.ForeColor = Color.Red;

            var products = _productRepository.GetLowStock();
            
            _lblSummary.Text = $"Items Below Minimum Stock: {products.Count}";

            _gridView.Columns.AddVisible("Code", "Code");
            _gridView.Columns.AddVisible("Name", "Product");
            _gridView.Columns.AddVisible("CategoryName", "Category");
            _gridView.Columns.AddVisible("SupplierName", "Supplier");
            _gridView.Columns.AddVisible("CurrentStock", "Current");
            _gridView.Columns.AddVisible("MinimumStock", "Minimum");
            var deficitCol = new GridColumn { FieldName = "Deficit", Caption = "Deficit", Visible = true };
            _gridView.Columns.Add(deficitCol);
            _gridView.Columns.AddVisible("PurchasePrice", "Price");
            _gridView.Columns.AddVisible("Location", "Location");

            _gridView.Columns["PurchasePrice"].DisplayFormat.FormatString = "N2";

            // Add calculated deficit column
            var data = products.Select(p => new
            {
                p.Code,
                p.Name,
                p.CategoryName,
                p.SupplierName,
                p.CurrentStock,
                p.MinimumStock,
                Deficit = p.MinimumStock - p.CurrentStock,
                p.PurchasePrice,
                p.Location
            }).OrderByDescending(p => p.Deficit).ToList();

            _gridReport.DataSource = data;
        }

        private void LoadMovementReport()
        {
            _lblTitle.Text = "Movement Summary Report";

            var startDate = DateTime.Today.AddMonths(-1);
            var endDate = DateTime.Today;
            var (totalPurchases, totalSales, totalMovements) = _movementRepository.GetStatistics(startDate, endDate);

            _lblSummary.Text = $"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd} | " +
                              $"Purchases: ${totalPurchases:N2} | Sales: ${totalSales:N2} | " +
                              $"Total Movements: {totalMovements}";

            var movements = _movementRepository.GetAll(startDate, endDate);

            _gridView.Columns.AddVisible("DocumentNumber", "Document");
            _gridView.Columns.AddVisible("MovementDate", "Date");
            _gridView.Columns.AddVisible("ProductCode", "Code");
            _gridView.Columns.AddVisible("ProductName", "Product");
            _gridView.Columns.AddVisible("Type", "Type");
            _gridView.Columns.AddVisible("Quantity", "Qty");
            _gridView.Columns.AddVisible("UnitPrice", "Price");
            _gridView.Columns.AddVisible("TotalValue", "Total");
            _gridView.Columns.AddVisible("UserName", "User");

            _gridView.Columns["MovementDate"].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
            _gridView.Columns["UnitPrice"].DisplayFormat.FormatString = "N2";
            _gridView.Columns["TotalValue"].DisplayFormat.FormatString = "N2";

            _gridView.Columns["TotalValue"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            _gridView.Columns["TotalValue"].SummaryItem.DisplayFormat = "Total: ${0:N2}";

            _gridReport.DataSource = movements;
        }

        public new void Refresh() => LoadReport();

        public void ExportToExcel() => ExportService.ExportGridToExcel(_gridView, $"{_reportType}Report");
        public void ExportToPdf() => ExportService.ExportGridToPdf(_gridView, $"{_reportType}Report");
        public void Print() => ExportService.PrintGrid(_gridView, $"{_reportType} Report");

        public void ExportToHtml()
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "HTML Files|*.html",
                FileName = $"{_reportType}Report_{DateTime.Now:yyyyMMdd}.html"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var options = new HtmlExportOptions { ExportMode = HtmlExportMode.SingleFile };
                    _gridView.ExportToHtml(dialog.FileName, options);
                    XtraMessageBox.Show("Exported successfully!", "Export",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Export error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void ExportToTxt()
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "Text Files|*.txt",
                FileName = $"{_reportType}Report_{DateTime.Now:yyyyMMdd}.txt"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _gridView.ExportToText(dialog.FileName);
                    XtraMessageBox.Show("Exported successfully!", "Export",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Export error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
