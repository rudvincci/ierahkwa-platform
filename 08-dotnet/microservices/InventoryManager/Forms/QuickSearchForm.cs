using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using InventoryManager.Data;
using InventoryManager.Models;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Quick search popup form with instant results
    /// </summary>
    public class QuickSearchForm : XtraForm
    {
        private readonly User _currentUser;
        private readonly ProductRepository _productRepository = new();

        private TextEdit _txtSearch = null!;
        private GridControl _gridResults = null!;
        private GridView _gridView = null!;
        private System.Windows.Forms.Timer _searchTimer = null!;

        public QuickSearchForm(User currentUser)
        {
            _currentUser = currentUser;
            InitializeComponents();
            _txtSearch.Focus();
        }

        private void InitializeComponents()
        {
            this.Text = "Quick Product Search (F3)";
            this.Size = new Size(800, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.KeyPreview = true;
            this.KeyDown += QuickSearchForm_KeyDown;

            // Search box
            var searchPanel = new PanelControl
            {
                Dock = DockStyle.Top,
                Height = 60,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            this.Controls.Add(searchPanel);

            var lblSearch = new LabelControl
            {
                Text = "Search:",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 20)
            };
            searchPanel.Controls.Add(lblSearch);

            _txtSearch = new TextEdit
            {
                Location = new Point(90, 15),
                Size = new Size(500, 30),
                Font = new Font("Segoe UI", 14)
            };
            _txtSearch.Properties.NullValuePrompt = "Enter product code, barcode, or name...";
            _txtSearch.Properties.NullValuePromptShowForEmptyValue = true;
            _txtSearch.TextChanged += TxtSearch_TextChanged;
            _txtSearch.KeyDown += TxtSearch_KeyDown;
            searchPanel.Controls.Add(_txtSearch);

            var btnClear = new SimpleButton
            {
                Text = "Clear",
                Location = new Point(600, 15),
                Size = new Size(70, 30)
            };
            btnClear.Click += (s, e) => { _txtSearch.Text = ""; _txtSearch.Focus(); };
            searchPanel.Controls.Add(btnClear);

            var btnClose = new SimpleButton
            {
                Text = "Close",
                Location = new Point(680, 15),
                Size = new Size(70, 30)
            };
            btnClose.Click += (s, e) => this.Close();
            searchPanel.Controls.Add(btnClose);

            // Results grid
            _gridResults = new GridControl
            {
                Dock = DockStyle.Fill
            };
            _gridView = new GridView(_gridResults);
            _gridResults.MainView = _gridView;

            _gridView.OptionsView.ShowGroupPanel = false;
            _gridView.OptionsBehavior.Editable = false;
            _gridView.OptionsSelection.EnableAppearanceFocusedRow = true;
            _gridView.RowStyle += GridView_RowStyle;

            _gridView.Columns.AddVisible("Code", "Code");
            _gridView.Columns.AddVisible("Barcode", "Barcode");
            _gridView.Columns.AddVisible("Name", "Product Name");
            _gridView.Columns.AddVisible("CategoryName", "Category");
            _gridView.Columns.AddVisible("CurrentStock", "Stock");
            _gridView.Columns.AddVisible("MinimumStock", "Min");
            _gridView.Columns.AddVisible("SalePrice", "Price");
            _gridView.Columns.AddVisible("Location", "Location");
            _gridView.Columns.AddVisible("StockStatus", "Status");

            _gridView.Columns["SalePrice"].DisplayFormat.FormatString = "C2";
            _gridView.Columns["Name"].Width = 200;

            this.Controls.Add(_gridResults);
            _gridResults.BringToFront();

            // Search timer for debouncing
            _searchTimer = new System.Windows.Forms.Timer { Interval = 300 };
            _searchTimer.Tick += SearchTimer_Tick;

            // Status bar
            var statusPanel = new PanelControl
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            this.Controls.Add(statusPanel);

            var lblStatus = new LabelControl
            {
                Text = "Type to search. Press Enter to select, Escape to close.",
                Location = new Point(20, 8),
                ForeColor = Color.Gray
            };
            statusPanel.Controls.Add(lblStatus);
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private void SearchTimer_Tick(object? sender, EventArgs e)
        {
            _searchTimer.Stop();
            PerformSearch();
        }

        private void PerformSearch()
        {
            var searchTerm = _txtSearch.Text?.Trim();
            
            if (string.IsNullOrEmpty(searchTerm))
            {
                _gridResults.DataSource = null;
                return;
            }

            try
            {
                var results = _productRepository.Search(searchTerm);
                _gridResults.DataSource = results;
                _gridView.BestFitColumns();

                if (results.Count > 0)
                    _gridView.FocusedRowHandle = 0;
            }
            catch { }
        }

        private void TxtSearch_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && _gridView.RowCount > 0)
            {
                _gridResults.Focus();
                _gridView.FocusedRowHandle = 0;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                ShowProductDetails();
                e.Handled = true;
            }
        }

        private void QuickSearchForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
                e.Handled = true;
            }
        }

        private void GridView_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            var view = sender as GridView;
            if (view == null) return;

            var status = view.GetRowCellValue(e.RowHandle, "StockStatus")?.ToString();

            if (status == "Low")
            {
                e.Appearance.BackColor = Color.MistyRose;
            }
            else if (status == "Over")
            {
                e.Appearance.BackColor = Color.LightYellow;
            }
        }

        private void ShowProductDetails()
        {
            var rowHandle = _gridView.FocusedRowHandle;
            if (rowHandle < 0) return;

            var product = _gridView.GetRow(rowHandle) as Product;
            if (product == null) return;

            var details = $"Code: {product.Code}\n" +
                         $"Barcode: {product.Barcode}\n" +
                         $"Name: {product.Name}\n" +
                         $"Category: {product.CategoryName}\n" +
                         $"Supplier: {product.SupplierName}\n" +
                         $"Current Stock: {product.CurrentStock} {product.Unit}\n" +
                         $"Min Stock: {product.MinimumStock}\n" +
                         $"Purchase Price: ${product.PurchasePrice:N2}\n" +
                         $"Sale Price: ${product.SalePrice:N2}\n" +
                         $"Stock Value: ${product.StockValue:N2}\n" +
                         $"Location: {product.Location}\n" +
                         $"Status: {product.StockStatus}";

            XtraMessageBox.Show(details, $"Product: {product.Name}",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _searchTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
