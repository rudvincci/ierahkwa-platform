using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using InventoryManager.Data;
using InventoryManager.Models;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Stock movement entry control for purchases, sales, adjustments
    /// </summary>
    public class StockMovementControl : XtraUserControl
    {
        private readonly User _currentUser;
        private readonly MovementType _movementType;
        private readonly ProductRepository _productRepository = new();
        private readonly StockMovementRepository _movementRepository = new();
        private readonly ActivityLogRepository _activityLog = new();

        private SearchLookUpEdit _cboProduct = null!;
        private SpinEdit _txtQuantity = null!;
        private SpinEdit _txtUnitPrice = null!;
        private TextEdit _txtReference = null!;
        private MemoEdit _txtNotes = null!;
        private LabelControl _lblCurrentStock = null!;
        private LabelControl _lblNewStock = null!;
        private LabelControl _lblTotalValue = null!;
        private GridControl _gridItems = null!;
        private GridView _gridView = null!;
        private List<MovementItem> _items = new();

        public StockMovementControl(User currentUser, MovementType movementType)
        {
            _currentUser = currentUser;
            _movementType = movementType;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.SuspendLayout();

            var title = _movementType switch
            {
                MovementType.Purchase => "Stock In - Purchase",
                MovementType.Sale => "Stock Out - Sale",
                MovementType.Adjustment => "Stock Adjustment",
                MovementType.Return => "Stock Return",
                MovementType.Damage => "Stock Out - Damage",
                _ => "Stock Movement"
            };

            // Title
            var titleLabel = new LabelControl
            {
                Text = title,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 20),
                ForeColor = _movementType == MovementType.Purchase ? Color.Green : 
                            _movementType == MovementType.Sale ? Color.Red : Color.Orange
            };
            this.Controls.Add(titleLabel);

            // Left panel - Entry form
            var entryPanel = new GroupControl
            {
                Text = "Add Item",
                Location = new Point(20, 60),
                Size = new Size(400, 350)
            };
            this.Controls.Add(entryPanel);

            int y = 30;

            // Product lookup with search
            entryPanel.Controls.Add(new LabelControl { Text = "Product:", Location = new Point(15, y + 3) });
            _cboProduct = new SearchLookUpEdit
            {
                Location = new Point(120, y),
                Size = new Size(260, 25)
            };
            
            // Configure search lookup
            _cboProduct.Properties.DataSource = _productRepository.GetAll();
            _cboProduct.Properties.DisplayMember = "Name";
            _cboProduct.Properties.ValueMember = "Id";
            _cboProduct.Properties.NullText = "Search product...";
            
            var view = _cboProduct.Properties.View;
            view.Columns.AddField("Code").Visible = true;
            view.Columns.AddField("Name").Visible = true;
            view.Columns.AddField("CurrentStock").Visible = true;
            view.Columns.AddField("SalePrice").Visible = true;
            view.Columns["Code"].Width = 80;
            view.Columns["Name"].Width = 150;
            view.Columns["CurrentStock"].Caption = "Stock";
            view.Columns["CurrentStock"].Width = 60;
            view.Columns["SalePrice"].Caption = "Price";
            view.Columns["SalePrice"].Width = 70;

            _cboProduct.EditValueChanged += CboProduct_EditValueChanged;
            entryPanel.Controls.Add(_cboProduct);

            y += 35;
            entryPanel.Controls.Add(new LabelControl { Text = "Current Stock:", Location = new Point(15, y + 3) });
            _lblCurrentStock = new LabelControl
            {
                Text = "0",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(120, y + 3)
            };
            entryPanel.Controls.Add(_lblCurrentStock);

            y += 30;
            entryPanel.Controls.Add(new LabelControl { Text = "Quantity:", Location = new Point(15, y + 3) });
            _txtQuantity = new SpinEdit
            {
                Location = new Point(120, y),
                Size = new Size(120, 25)
            };
            _txtQuantity.Properties.MinValue = 1;
            _txtQuantity.Properties.IsFloatValue = false;
            _txtQuantity.Value = 1;
            _txtQuantity.EditValueChanged += UpdateCalculations;
            entryPanel.Controls.Add(_txtQuantity);

            y += 35;
            entryPanel.Controls.Add(new LabelControl { Text = "Unit Price:", Location = new Point(15, y + 3) });
            _txtUnitPrice = new SpinEdit
            {
                Location = new Point(120, y),
                Size = new Size(120, 25)
            };
            _txtUnitPrice.Properties.IsFloatValue = true;
            _txtUnitPrice.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            _txtUnitPrice.Properties.Mask.EditMask = "n2";
            _txtUnitPrice.EditValueChanged += UpdateCalculations;
            entryPanel.Controls.Add(_txtUnitPrice);

            y += 35;
            entryPanel.Controls.Add(new LabelControl { Text = "New Stock:", Location = new Point(15, y + 3) });
            _lblNewStock = new LabelControl
            {
                Text = "0",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(120, y + 3),
                ForeColor = Color.Blue
            };
            entryPanel.Controls.Add(_lblNewStock);

            y += 30;
            entryPanel.Controls.Add(new LabelControl { Text = "Total Value:", Location = new Point(15, y + 3) });
            _lblTotalValue = new LabelControl
            {
                Text = "$0.00",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(120, y + 3),
                ForeColor = Color.Green
            };
            entryPanel.Controls.Add(_lblTotalValue);

            y += 35;
            entryPanel.Controls.Add(new LabelControl { Text = "Reference:", Location = new Point(15, y + 3) });
            _txtReference = new TextEdit
            {
                Location = new Point(120, y),
                Size = new Size(260, 25)
            };
            _txtReference.Text = _movementRepository.GenerateDocumentNumber(_movementType);
            entryPanel.Controls.Add(_txtReference);

            y += 35;
            entryPanel.Controls.Add(new LabelControl { Text = "Notes:", Location = new Point(15, y + 3) });
            _txtNotes = new MemoEdit
            {
                Location = new Point(120, y),
                Size = new Size(260, 50)
            };
            entryPanel.Controls.Add(_txtNotes);

            y += 60;
            var btnAdd = new SimpleButton
            {
                Text = "Add to List",
                Location = new Point(120, y),
                Size = new Size(120, 30),
                Appearance = { BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White }
            };
            btnAdd.Click += BtnAdd_Click;
            entryPanel.Controls.Add(btnAdd);

            // Right panel - Items list
            var listPanel = new GroupControl
            {
                Text = "Items List",
                Location = new Point(440, 60),
                Size = new Size(500, 350)
            };
            this.Controls.Add(listPanel);

            _gridItems = new GridControl
            {
                Location = new Point(10, 30),
                Size = new Size(480, 260)
            };
            _gridView = new GridView(_gridItems);
            _gridItems.MainView = _gridView;
            _gridView.OptionsView.ShowGroupPanel = false;
            _gridView.OptionsBehavior.Editable = false;
            _gridView.Columns.AddVisible("ProductCode", "Code");
            _gridView.Columns.AddVisible("ProductName", "Product");
            _gridView.Columns.AddVisible("Quantity", "Qty");
            _gridView.Columns.AddVisible("UnitPrice", "Price");
            _gridView.Columns.AddVisible("TotalValue", "Total");
            listPanel.Controls.Add(_gridItems);

            var btnRemove = new SimpleButton
            {
                Text = "Remove Selected",
                Location = new Point(10, 300),
                Size = new Size(120, 25)
            };
            btnRemove.Click += BtnRemove_Click;
            listPanel.Controls.Add(btnRemove);

            var btnClear = new SimpleButton
            {
                Text = "Clear All",
                Location = new Point(140, 300),
                Size = new Size(100, 25)
            };
            btnClear.Click += (s, e) => { _items.Clear(); RefreshGrid(); };
            listPanel.Controls.Add(btnClear);

            // Save button
            var btnSave = new SimpleButton
            {
                Text = "Save Movement",
                Location = new Point(440, 420),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Appearance = { BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White }
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            this.ResumeLayout(false);
        }

        private void CboProduct_EditValueChanged(object? sender, EventArgs e)
        {
            var productId = _cboProduct.EditValue as int?;
            if (productId == null) return;

            var product = _productRepository.GetById(productId.Value);
            if (product != null)
            {
                _lblCurrentStock.Text = product.CurrentStock.ToString();
                _txtUnitPrice.Value = _movementType == MovementType.Sale ? 
                    product.SalePrice : product.PurchasePrice;
                UpdateCalculations(sender, e);
            }
        }

        private void UpdateCalculations(object? sender, EventArgs e)
        {
            var productId = _cboProduct.EditValue as int?;
            if (productId == null) return;

            var product = _productRepository.GetById(productId.Value);
            if (product == null) return;

            var quantity = (int)_txtQuantity.Value;
            var unitPrice = (decimal)_txtUnitPrice.Value;

            int newStock = _movementType switch
            {
                MovementType.Purchase or MovementType.Return => product.CurrentStock + quantity,
                MovementType.Sale or MovementType.Damage => product.CurrentStock - quantity,
                MovementType.Adjustment => quantity,
                _ => product.CurrentStock
            };

            _lblNewStock.Text = newStock.ToString();
            _lblNewStock.ForeColor = newStock < product.MinimumStock ? Color.Red : Color.Blue;
            _lblTotalValue.Text = $"${quantity * unitPrice:N2}";
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var productId = _cboProduct.EditValue as int?;
            if (productId == null)
            {
                XtraMessageBox.Show("Please select a product.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var product = _productRepository.GetById(productId.Value);
            if (product == null) return;

            var quantity = (int)_txtQuantity.Value;

            // Check if sale quantity exceeds stock
            if (_movementType == MovementType.Sale && quantity > product.CurrentStock)
            {
                XtraMessageBox.Show("Quantity exceeds available stock.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if item already in list
            var existing = _items.FirstOrDefault(i => i.ProductId == productId);
            if (existing != null)
            {
                existing.Quantity += quantity;
                existing.TotalValue = existing.Quantity * existing.UnitPrice;
            }
            else
            {
                _items.Add(new MovementItem
                {
                    ProductId = productId.Value,
                    ProductCode = product.Code,
                    ProductName = product.Name,
                    Quantity = quantity,
                    UnitPrice = (decimal)_txtUnitPrice.Value,
                    TotalValue = quantity * (decimal)_txtUnitPrice.Value
                });
            }

            RefreshGrid();
            _cboProduct.EditValue = null;
            _txtQuantity.Value = 1;
            _lblCurrentStock.Text = "0";
            _lblNewStock.Text = "0";
            _lblTotalValue.Text = "$0.00";
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

        private void RefreshGrid()
        {
            _gridItems.DataSource = null;
            _gridItems.DataSource = _items;
            _gridView.BestFitColumns();
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (_items.Count == 0)
            {
                XtraMessageBox.Show("Please add items to the list.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (XtraMessageBox.Show($"Save {_items.Count} item(s)?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                var reference = _txtReference.Text?.Trim() ?? "";
                var notes = _txtNotes.Text?.Trim() ?? "";

                foreach (var item in _items)
                {
                    var movement = new StockMovement
                    {
                        ProductId = item.ProductId,
                        Type = _movementType,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        UserId = _currentUser.Id,
                        Reference = reference,
                        Notes = notes,
                        DocumentNumber = reference
                    };

                    _movementRepository.CreateMovement(movement);
                }

                _activityLog.Log(_currentUser.Id, "STOCK_MOVEMENT", "StockMovements", null, null,
                    new { Type = _movementType.ToString(), Items = _items.Count, Reference = reference });

                XtraMessageBox.Show($"Stock movement saved successfully!\n{_items.Count} item(s) processed.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear form
                _items.Clear();
                RefreshGrid();
                _txtReference.Text = _movementRepository.GenerateDocumentNumber(_movementType);
                _txtNotes.Text = "";
                _cboProduct.Properties.DataSource = _productRepository.GetAll(); // Refresh products
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error saving: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class MovementItem
        {
            public int ProductId { get; set; }
            public string ProductCode { get; set; } = "";
            public string ProductName { get; set; } = "";
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalValue { get; set; }
        }
    }
}
