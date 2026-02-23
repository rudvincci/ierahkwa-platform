using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using InventoryManager.Data;
using InventoryManager.Models;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Quick stock entry form with barcode scanning support and auto-complete
    /// </summary>
    public class QuickStockEntryForm : XtraForm
    {
        private readonly User _currentUser;
        private readonly ProductRepository _productRepository = new();
        private readonly StockMovementRepository _movementRepository = new();
        private readonly ActivityLogRepository _activityLog = new();

        private TextEdit _txtBarcode = null!;
        private GridControl _gridItems = null!;
        private GridView _gridView = null!;
        private ComboBoxEdit _cboMovementType = null!;
        private LabelControl _lblTotal = null!;
        private List<QuickEntryItem> _items = new();

        public QuickStockEntryForm(User currentUser)
        {
            _currentUser = currentUser;
            InitializeComponents();
            _txtBarcode.Focus();
        }

        private void InitializeComponents()
        {
            this.Text = "Quick Stock Entry - Barcode Scanner";
            this.Size = new Size(900, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.KeyPreview = true;

            // Top panel
            var topPanel = new PanelControl
            {
                Dock = DockStyle.Top,
                Height = 80,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            this.Controls.Add(topPanel);

            // Movement type
            topPanel.Controls.Add(new LabelControl { Text = "Movement Type:", Location = new Point(20, 15) });
            _cboMovementType = new ComboBoxEdit
            {
                Location = new Point(130, 12),
                Size = new Size(150, 25)
            };
            _cboMovementType.Properties.Items.Add("Stock In (Purchase)");
            _cboMovementType.Properties.Items.Add("Stock Out (Sale)");
            _cboMovementType.SelectedIndex = 0;
            topPanel.Controls.Add(_cboMovementType);

            // Barcode input
            topPanel.Controls.Add(new LabelControl 
            { 
                Text = "Scan/Enter Barcode:", 
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 50) 
            });
            _txtBarcode = new TextEdit
            {
                Location = new Point(170, 45),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 14)
            };
            _txtBarcode.Properties.NullValuePrompt = "Scan barcode or enter code...";
            _txtBarcode.KeyDown += TxtBarcode_KeyDown;
            topPanel.Controls.Add(_txtBarcode);

            var lblTip = new LabelControl
            {
                Text = "Tip: Connect a barcode scanner. Press Enter after each scan.",
                Location = new Point(490, 50),
                ForeColor = Color.Gray
            };
            topPanel.Controls.Add(lblTip);

            // Grid
            _gridItems = new GridControl
            {
                Dock = DockStyle.Fill
            };
            _gridView = new GridView(_gridItems);
            _gridItems.MainView = _gridView;

            _gridView.OptionsView.ShowGroupPanel = false;
            _gridView.OptionsView.ShowFooter = true;
            _gridView.OptionsBehavior.Editable = true;

            _gridView.Columns.AddVisible("Code", "Code");
            _gridView.Columns.AddVisible("Barcode", "Barcode");
            _gridView.Columns.AddVisible("Name", "Product Name");
            _gridView.Columns.AddVisible("CurrentStock", "Current Stock");
            _gridView.Columns.AddVisible("Quantity", "Qty");
            _gridView.Columns.AddVisible("UnitPrice", "Unit Price");
            _gridView.Columns.AddVisible("Total", "Total");

            // Make Quantity and UnitPrice editable
            _gridView.Columns["Code"].OptionsColumn.AllowEdit = false;
            _gridView.Columns["Barcode"].OptionsColumn.AllowEdit = false;
            _gridView.Columns["Name"].OptionsColumn.AllowEdit = false;
            _gridView.Columns["CurrentStock"].OptionsColumn.AllowEdit = false;
            _gridView.Columns["Total"].OptionsColumn.AllowEdit = false;

            _gridView.Columns["Quantity"].Width = 60;
            _gridView.Columns["UnitPrice"].DisplayFormat.FormatString = "N2";
            _gridView.Columns["Total"].DisplayFormat.FormatString = "N2";
            _gridView.Columns["Name"].Width = 200;

            _gridView.CellValueChanged += GridView_CellValueChanged;

            this.Controls.Add(_gridItems);
            _gridItems.BringToFront();

            // Bottom panel
            var bottomPanel = new PanelControl
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            this.Controls.Add(bottomPanel);

            _lblTotal = new LabelControl
            {
                Text = "Total: $0.00 | Items: 0",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20)
            };
            bottomPanel.Controls.Add(_lblTotal);

            var btnRemove = new SimpleButton
            {
                Text = "Remove Selected",
                Location = new Point(400, 17),
                Size = new Size(120, 35)
            };
            btnRemove.Click += BtnRemove_Click;
            bottomPanel.Controls.Add(btnRemove);

            var btnClear = new SimpleButton
            {
                Text = "Clear All",
                Location = new Point(530, 17),
                Size = new Size(100, 35)
            };
            btnClear.Click += (s, e) => { _items.Clear(); RefreshGrid(); };
            bottomPanel.Controls.Add(btnClear);

            var btnSave = new SimpleButton
            {
                Text = "Save All",
                Location = new Point(650, 17),
                Size = new Size(100, 35),
                Appearance = { BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White }
            };
            btnSave.Click += BtnSave_Click;
            bottomPanel.Controls.Add(btnSave);

            var btnClose = new SimpleButton
            {
                Text = "Close",
                Location = new Point(760, 17),
                Size = new Size(80, 35)
            };
            btnClose.Click += (s, e) => this.Close();
            bottomPanel.Controls.Add(btnClose);
        }

        private void TxtBarcode_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcessBarcode();
                e.Handled = true;
            }
        }

        private void ProcessBarcode()
        {
            var input = _txtBarcode.Text?.Trim();
            if (string.IsNullOrEmpty(input))
                return;

            // Try to find product by barcode or code
            var product = _productRepository.GetByBarcode(input) ?? _productRepository.GetByCode(input);

            if (product == null)
            {
                // Try quick search
                var results = _productRepository.QuickSearch(input);
                if (results.Count == 1)
                    product = _productRepository.GetById(results[0].Id);
            }

            if (product == null)
            {
                XtraMessageBox.Show($"Product not found: {input}", "Not Found",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtBarcode.SelectAll();
                return;
            }

            // Check if already in list
            var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);
            if (existing != null)
            {
                existing.Quantity++;
                existing.Total = existing.Quantity * existing.UnitPrice;
            }
            else
            {
                var isStockIn = _cboMovementType.SelectedIndex == 0;
                _items.Add(new QuickEntryItem
                {
                    ProductId = product.Id,
                    Code = product.Code,
                    Barcode = product.Barcode,
                    Name = product.Name,
                    CurrentStock = product.CurrentStock,
                    Quantity = 1,
                    UnitPrice = isStockIn ? product.PurchasePrice : product.SalePrice,
                    Total = isStockIn ? product.PurchasePrice : product.SalePrice
                });
            }

            RefreshGrid();
            _txtBarcode.Text = "";
            _txtBarcode.Focus();

            // Play beep sound for successful scan
            System.Media.SystemSounds.Beep.Play();
        }

        private void GridView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName is "Quantity" or "UnitPrice")
            {
                var item = _items[e.RowHandle];
                item.Total = item.Quantity * item.UnitPrice;
                RefreshGrid();
            }
        }

        private void RefreshGrid()
        {
            _gridItems.DataSource = null;
            _gridItems.DataSource = _items;
            _gridView.BestFitColumns();

            var totalValue = _items.Sum(i => i.Total);
            var totalItems = _items.Sum(i => i.Quantity);
            _lblTotal.Text = $"Total: ${totalValue:N2} | Items: {totalItems}";
        }

        private void BtnRemove_Click(object? sender, EventArgs e)
        {
            var rowHandle = _gridView.FocusedRowHandle;
            if (rowHandle >= 0 && rowHandle < _items.Count)
            {
                _items.RemoveAt(rowHandle);
                RefreshGrid();
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (_items.Count == 0)
            {
                XtraMessageBox.Show("No items to save.", "Empty",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var isStockIn = _cboMovementType.SelectedIndex == 0;
            var movementType = isStockIn ? MovementType.Purchase : MovementType.Sale;

            // Validate stock for sales
            if (!isStockIn)
            {
                foreach (var item in _items)
                {
                    if (item.Quantity > item.CurrentStock)
                    {
                        XtraMessageBox.Show($"Insufficient stock for {item.Name}. Available: {item.CurrentStock}",
                            "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            if (XtraMessageBox.Show($"Save {_items.Count} items as {(isStockIn ? "Stock In" : "Stock Out")}?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                var documentNumber = _movementRepository.GenerateDocumentNumber(movementType);

                foreach (var item in _items)
                {
                    _movementRepository.CreateMovement(new StockMovement
                    {
                        ProductId = item.ProductId,
                        Type = movementType,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        UserId = _currentUser.Id,
                        Reference = "Quick Entry",
                        DocumentNumber = documentNumber
                    });
                }

                _activityLog.Log(_currentUser.Id, "QUICK_STOCK_ENTRY", "StockMovements", null, null,
                    new { Type = movementType.ToString(), Items = _items.Count });

                XtraMessageBox.Show($"Saved {_items.Count} items successfully!\nDocument: {documentNumber}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _items.Clear();
                RefreshGrid();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error saving: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class QuickEntryItem
        {
            public int ProductId { get; set; }
            public string Code { get; set; } = "";
            public string Barcode { get; set; } = "";
            public string Name { get; set; } = "";
            public int CurrentStock { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Total { get; set; }
        }
    }
}
