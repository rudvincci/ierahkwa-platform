using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors.Repository;
using InventoryManager.Data;
using InventoryManager.Models;
using InventoryManager.Services;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Products management control with DevExpress grid
    /// </summary>
    public class ProductsControl : XtraUserControl, IRefreshable, IExportable
    {
        private readonly User _currentUser;
        private readonly ProductRepository _productRepository = new();
        private readonly CategoryRepository _categoryRepository = new();
        private readonly ActivityLogRepository _activityLog = new();

        private GridControl _gridProducts = null!;
        private GridView _gridView = null!;
        private SearchControl _searchControl = null!;
        private CheckEdit _chkShowInactive = null!;

        public ProductsControl(User currentUser)
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
                Text = "Product Management",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(10, 15)
            };
            toolbarPanel.Controls.Add(titleLabel);

            // Search control
            _searchControl = new SearchControl
            {
                Location = new Point(250, 15),
                Size = new Size(300, 30),
                NullValuePrompt = "Search products (code, barcode, name)..."
            };
            _searchControl.Client = _gridProducts;
            _searchControl.QueryIsSearchColumn += SearchControl_QueryIsSearchColumn;
            toolbarPanel.Controls.Add(_searchControl);

            // Show inactive checkbox
            _chkShowInactive = new CheckEdit
            {
                Text = "Show Inactive",
                Location = new Point(570, 18),
                Size = new Size(120, 25)
            };
            _chkShowInactive.CheckedChanged += (s, e) => LoadData();
            toolbarPanel.Controls.Add(_chkShowInactive);

            // Buttons
            var btnNew = new SimpleButton
            {
                Text = "New Product",
                Location = new Point(710, 15),
                Size = new Size(100, 30)
            };
            btnNew.Click += BtnNew_Click;
            toolbarPanel.Controls.Add(btnNew);

            var btnEdit = new SimpleButton
            {
                Text = "Edit",
                Location = new Point(820, 15),
                Size = new Size(80, 30)
            };
            btnEdit.Click += BtnEdit_Click;
            toolbarPanel.Controls.Add(btnEdit);

            var btnDelete = new SimpleButton
            {
                Text = "Delete",
                Location = new Point(910, 15),
                Size = new Size(80, 30)
            };
            btnDelete.Click += BtnDelete_Click;
            toolbarPanel.Controls.Add(btnDelete);

            var btnRefresh = new SimpleButton
            {
                Text = "Refresh",
                Location = new Point(1000, 15),
                Size = new Size(80, 30)
            };
            btnRefresh.Click += (s, e) => LoadData();
            toolbarPanel.Controls.Add(btnRefresh);

            // Grid
            _gridProducts = new GridControl
            {
                Dock = DockStyle.Fill
            };

            _gridView = new GridView(_gridProducts);
            _gridProducts.MainView = _gridView;

            // Configure grid view
            _gridView.OptionsView.ShowGroupPanel = true;
            _gridView.OptionsView.ShowFooter = true;
            _gridView.OptionsView.ColumnAutoWidth = false;
            _gridView.OptionsBehavior.Editable = false;
            _gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            _gridView.OptionsFind.AlwaysVisible = false;
            _gridView.DoubleClick += GridView_DoubleClick;
            _gridView.RowStyle += GridView_RowStyle;

            // Add columns
            AddColumn("Code", "Code", 80);
            AddColumn("Barcode", "Barcode", 100);
            AddColumn("Name", "Name", 200);
            AddColumn("CategoryName", "Category", 120);
            AddColumn("SupplierName", "Supplier", 120);
            AddColumn("CurrentStock", "Stock", 70, formatType: FormatType.Numeric, formatString: "N0");
            AddColumn("MinimumStock", "Min Stock", 70, formatType: FormatType.Numeric, formatString: "N0");
            AddColumn("PurchasePrice", "Purchase $", 90, formatType: FormatType.Numeric, formatString: "N2");
            AddColumn("SalePrice", "Sale $", 80, formatType: FormatType.Numeric, formatString: "N2");
            AddColumn("StockValue", "Value", 100, formatType: FormatType.Numeric, formatString: "N2");
            AddColumn("Unit", "Unit", 50);
            AddColumn("Location", "Location", 100);
            AddColumn("StockStatus", "Status", 70);
            AddColumn("IsActive", "Active", 60);

            // Summary items
            _gridView.Columns["CurrentStock"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            _gridView.Columns["CurrentStock"].SummaryItem.DisplayFormat = "Total: {0:N0}";
            _gridView.Columns["StockValue"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            _gridView.Columns["StockValue"].SummaryItem.DisplayFormat = "Total: ${0:N2}";

            this.Controls.Add(_gridProducts);
            _gridProducts.BringToFront();

            _searchControl.Client = _gridProducts;

            this.ResumeLayout(false);
        }

        private void AddColumn(string fieldName, string caption, int width, 
            FormatType formatType = FormatType.None, string? formatString = null)
        {
            var column = new GridColumn
            {
                FieldName = fieldName,
                Caption = caption,
                Width = width,
                Visible = true
            };

            if (formatType != FormatType.None && formatString != null)
            {
                column.DisplayFormat.FormatType = formatType;
                column.DisplayFormat.FormatString = formatString;
            }

            _gridView.Columns.Add(column);
        }

        private void LoadData()
        {
            try
            {
                var products = _productRepository.GetAll(_chkShowInactive.Checked);
                _gridProducts.DataSource = products;
                _gridView.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error loading products: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchControl_QueryIsSearchColumn(object sender, 
            DevExpress.XtraEditors.SearchControlQueryIsSearchColumnEventArgs e)
        {
            // Enable search on specific columns
            e.IsSearchColumn = e.FieldName is "Code" or "Barcode" or "Name" or "Description";
        }

        private void GridView_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            var view = sender as GridView;
            if (view == null) return;

            var status = view.GetRowCellValue(e.RowHandle, "StockStatus")?.ToString();
            var isActive = (bool?)view.GetRowCellValue(e.RowHandle, "IsActive");

            if (isActive == false)
            {
                e.Appearance.BackColor = Color.LightGray;
                e.Appearance.ForeColor = Color.DarkGray;
            }
            else if (status == "Low")
            {
                e.Appearance.BackColor = Color.MistyRose;
            }
            else if (status == "Over")
            {
                e.Appearance.BackColor = Color.LightYellow;
            }
        }

        private void GridView_DoubleClick(object? sender, EventArgs e)
        {
            BtnEdit_Click(sender, e);
        }

        private void BtnNew_Click(object? sender, EventArgs e)
        {
            var parentForm = this.FindForm();
            if (parentForm is MainForm mainForm)
            {
                // Navigate to product edit
                var editControl = new ProductEditControl(_currentUser, null);
                editControl.ProductSaved += (s, args) => LoadData();
                
                // Replace current control
                var parent = this.Parent;
                parent?.Controls.Clear();
                editControl.Dock = DockStyle.Fill;
                parent?.Controls.Add(editControl);
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            var product = GetSelectedProduct();
            if (product == null)
            {
                XtraMessageBox.Show("Please select a product to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var parentForm = this.FindForm();
            if (parentForm is MainForm)
            {
                var editControl = new ProductEditControl(_currentUser, product);
                editControl.ProductSaved += (s, args) => LoadData();
                
                var parent = this.Parent;
                parent?.Controls.Clear();
                editControl.Dock = DockStyle.Fill;
                parent?.Controls.Add(editControl);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var product = GetSelectedProduct();
            if (product == null)
            {
                XtraMessageBox.Show("Please select a product to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (XtraMessageBox.Show($"Are you sure you want to delete product '{product.Name}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _productRepository.Delete(product.Id);
                    _activityLog.Log(_currentUser.Id, "DELETE", "Products", product.Id, product);
                    LoadData();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Error deleting product: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Product? GetSelectedProduct()
        {
            var rowHandle = _gridView.FocusedRowHandle;
            if (rowHandle < 0) return null;
            return _gridView.GetRow(rowHandle) as Product;
        }

        public new void Refresh() => LoadData();

        public void ExportToExcel()
        {
            ExportService.ExportGridToExcel(_gridView, "Products");
        }

        public void ExportToPdf()
        {
            ExportService.ExportGridToPdf(_gridView, "Products");
        }

        public void Print()
        {
            ExportService.PrintGrid(_gridView, "Products Report");
        }
    }
}
