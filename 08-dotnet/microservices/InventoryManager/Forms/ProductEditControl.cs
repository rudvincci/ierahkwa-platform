using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using InventoryManager.Data;
using InventoryManager.Models;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Product edit/create control
    /// </summary>
    public class ProductEditControl : XtraUserControl
    {
        private readonly User _currentUser;
        private readonly Product? _product;
        private readonly bool _isNew;
        private readonly ProductRepository _productRepository = new();
        private readonly CategoryRepository _categoryRepository = new();
        private readonly SupplierRepository _supplierRepository = new();
        private readonly ActivityLogRepository _activityLog = new();

        public event EventHandler? ProductSaved;

        // Form controls
        private TextEdit _txtCode = null!;
        private TextEdit _txtBarcode = null!;
        private TextEdit _txtName = null!;
        private MemoEdit _txtDescription = null!;
        private LookUpEdit _cboCategory = null!;
        private LookUpEdit _cboSupplier = null!;
        private SpinEdit _txtPurchasePrice = null!;
        private SpinEdit _txtSalePrice = null!;
        private SpinEdit _txtCurrentStock = null!;
        private SpinEdit _txtMinStock = null!;
        private SpinEdit _txtMaxStock = null!;
        private TextEdit _txtUnit = null!;
        private TextEdit _txtLocation = null!;
        private CheckEdit _chkActive = null!;
        private MemoEdit _txtNotes = null!;
        private PictureEdit _picImage = null!;

        public ProductEditControl(User currentUser, Product? product)
        {
            _currentUser = currentUser;
            _product = product;
            _isNew = product == null;
            InitializeComponents();
            LoadLookups();
            LoadProduct();
        }

        private void InitializeComponents()
        {
            this.SuspendLayout();

            // Scroll panel
            var scrollPanel = new XtraScrollableControl
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };
            this.Controls.Add(scrollPanel);

            // Title
            var titleLabel = new LabelControl
            {
                Text = _isNew ? "New Product" : "Edit Product",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 20)
            };
            scrollPanel.Controls.Add(titleLabel);

            // Back button
            var btnBack = new SimpleButton
            {
                Text = "< Back to Products",
                Location = new Point(20, 60),
                Size = new Size(150, 30)
            };
            btnBack.Click += BtnBack_Click;
            scrollPanel.Controls.Add(btnBack);

            int y = 110;
            int labelWidth = 120;
            int controlWidth = 250;
            int col2X = 420;

            // === Column 1 ===

            // Code
            scrollPanel.Controls.Add(CreateLabel("Code: *", 20, y));
            _txtCode = new TextEdit { Location = new Point(140, y), Size = new Size(controlWidth, 25) };
            if (_isNew)
            {
                _txtCode.Text = _productRepository.GenerateNextCode();
            }
            scrollPanel.Controls.Add(_txtCode);

            // Barcode
            y += 35;
            scrollPanel.Controls.Add(CreateLabel("Barcode:", 20, y));
            _txtBarcode = new TextEdit { Location = new Point(140, y), Size = new Size(controlWidth, 25) };
            scrollPanel.Controls.Add(_txtBarcode);

            // Name
            y += 35;
            scrollPanel.Controls.Add(CreateLabel("Name: *", 20, y));
            _txtName = new TextEdit { Location = new Point(140, y), Size = new Size(controlWidth, 25) };
            scrollPanel.Controls.Add(_txtName);

            // Category
            y += 35;
            scrollPanel.Controls.Add(CreateLabel("Category:", 20, y));
            _cboCategory = new LookUpEdit { Location = new Point(140, y), Size = new Size(controlWidth, 25) };
            _cboCategory.Properties.NullText = "(Select category)";
            _cboCategory.Properties.Columns.Add(new LookUpColumnInfo("Name", "Category"));
            _cboCategory.Properties.DisplayMember = "Name";
            _cboCategory.Properties.ValueMember = "Id";
            scrollPanel.Controls.Add(_cboCategory);

            // Supplier
            y += 35;
            scrollPanel.Controls.Add(CreateLabel("Supplier:", 20, y));
            _cboSupplier = new LookUpEdit { Location = new Point(140, y), Size = new Size(controlWidth, 25) };
            _cboSupplier.Properties.NullText = "(Select supplier)";
            _cboSupplier.Properties.Columns.Add(new LookUpColumnInfo("Name", "Supplier"));
            _cboSupplier.Properties.DisplayMember = "Name";
            _cboSupplier.Properties.ValueMember = "Id";
            scrollPanel.Controls.Add(_cboSupplier);

            // Purchase Price
            y += 35;
            scrollPanel.Controls.Add(CreateLabel("Purchase Price: *", 20, y));
            _txtPurchasePrice = new SpinEdit { Location = new Point(140, y), Size = new Size(controlWidth, 25) };
            _txtPurchasePrice.Properties.IsFloatValue = true;
            _txtPurchasePrice.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            _txtPurchasePrice.Properties.Mask.EditMask = "n2";
            _txtPurchasePrice.Properties.MinValue = 0;
            scrollPanel.Controls.Add(_txtPurchasePrice);

            // Sale Price
            y += 35;
            scrollPanel.Controls.Add(CreateLabel("Sale Price: *", 20, y));
            _txtSalePrice = new SpinEdit { Location = new Point(140, y), Size = new Size(controlWidth, 25) };
            _txtSalePrice.Properties.IsFloatValue = true;
            _txtSalePrice.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            _txtSalePrice.Properties.Mask.EditMask = "n2";
            _txtSalePrice.Properties.MinValue = 0;
            scrollPanel.Controls.Add(_txtSalePrice);

            // === Column 2 ===

            int y2 = 110;

            // Current Stock
            scrollPanel.Controls.Add(CreateLabel("Current Stock:", col2X, y2));
            _txtCurrentStock = new SpinEdit { Location = new Point(col2X + labelWidth, y2), Size = new Size(controlWidth, 25) };
            _txtCurrentStock.Properties.MinValue = 0;
            _txtCurrentStock.Properties.IsFloatValue = false;
            if (!_isNew) _txtCurrentStock.Enabled = false; // Stock changed via movements
            scrollPanel.Controls.Add(_txtCurrentStock);

            // Min Stock
            y2 += 35;
            scrollPanel.Controls.Add(CreateLabel("Minimum Stock:", col2X, y2));
            _txtMinStock = new SpinEdit { Location = new Point(col2X + labelWidth, y2), Size = new Size(controlWidth, 25) };
            _txtMinStock.Properties.MinValue = 0;
            _txtMinStock.Properties.IsFloatValue = false;
            scrollPanel.Controls.Add(_txtMinStock);

            // Max Stock
            y2 += 35;
            scrollPanel.Controls.Add(CreateLabel("Maximum Stock:", col2X, y2));
            _txtMaxStock = new SpinEdit { Location = new Point(col2X + labelWidth, y2), Size = new Size(controlWidth, 25) };
            _txtMaxStock.Properties.MinValue = 0;
            _txtMaxStock.Properties.IsFloatValue = false;
            scrollPanel.Controls.Add(_txtMaxStock);

            // Unit
            y2 += 35;
            scrollPanel.Controls.Add(CreateLabel("Unit:", col2X, y2));
            _txtUnit = new TextEdit { Location = new Point(col2X + labelWidth, y2), Size = new Size(100, 25) };
            _txtUnit.Text = "PCS";
            scrollPanel.Controls.Add(_txtUnit);

            // Location
            y2 += 35;
            scrollPanel.Controls.Add(CreateLabel("Location:", col2X, y2));
            _txtLocation = new TextEdit { Location = new Point(col2X + labelWidth, y2), Size = new Size(controlWidth, 25) };
            scrollPanel.Controls.Add(_txtLocation);

            // Active
            y2 += 35;
            _chkActive = new CheckEdit
            {
                Text = "Active",
                Location = new Point(col2X + labelWidth, y2),
                Size = new Size(100, 25),
                Checked = true
            };
            scrollPanel.Controls.Add(_chkActive);

            // Image
            y2 += 35;
            scrollPanel.Controls.Add(CreateLabel("Image:", col2X, y2));
            _picImage = new PictureEdit
            {
                Location = new Point(col2X + labelWidth, y2),
                Size = new Size(150, 150),
                BorderStyle = BorderStyles.Simple
            };
            _picImage.Properties.SizeMode = PictureSizeMode.Zoom;
            _picImage.Properties.ShowCameraMenuItem = CameraMenuItemVisibility.Auto;
            scrollPanel.Controls.Add(_picImage);

            // Description
            y += 50;
            scrollPanel.Controls.Add(CreateLabel("Description:", 20, y));
            _txtDescription = new MemoEdit
            {
                Location = new Point(140, y),
                Size = new Size(250, 80)
            };
            scrollPanel.Controls.Add(_txtDescription);

            // Notes
            y += 95;
            scrollPanel.Controls.Add(CreateLabel("Notes:", 20, y));
            _txtNotes = new MemoEdit
            {
                Location = new Point(140, y),
                Size = new Size(250, 80)
            };
            scrollPanel.Controls.Add(_txtNotes);

            // Buttons
            y += 100;
            var btnSave = new SimpleButton
            {
                Text = "Save",
                Location = new Point(140, y),
                Size = new Size(100, 35),
                Appearance = { BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White }
            };
            btnSave.Click += BtnSave_Click;
            scrollPanel.Controls.Add(btnSave);

            var btnCancel = new SimpleButton
            {
                Text = "Cancel",
                Location = new Point(250, y),
                Size = new Size(100, 35)
            };
            btnCancel.Click += BtnBack_Click;
            scrollPanel.Controls.Add(btnCancel);

            this.ResumeLayout(false);
        }

        private LabelControl CreateLabel(string text, int x, int y)
        {
            return new LabelControl
            {
                Text = text,
                Location = new Point(x, y + 3),
                AutoSizeMode = LabelAutoSizeMode.Default
            };
        }

        private void LoadLookups()
        {
            try
            {
                _cboCategory.Properties.DataSource = _categoryRepository.GetAll();
                _cboSupplier.Properties.DataSource = _supplierRepository.GetAll();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error loading lookups: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProduct()
        {
            if (_product == null) return;

            _txtCode.Text = _product.Code;
            _txtBarcode.Text = _product.Barcode;
            _txtName.Text = _product.Name;
            _txtDescription.Text = _product.Description;
            _cboCategory.EditValue = _product.CategoryId > 0 ? _product.CategoryId : null;
            _cboSupplier.EditValue = _product.SupplierId > 0 ? _product.SupplierId : null;
            _txtPurchasePrice.Value = _product.PurchasePrice;
            _txtSalePrice.Value = _product.SalePrice;
            _txtCurrentStock.Value = _product.CurrentStock;
            _txtMinStock.Value = _product.MinimumStock;
            _txtMaxStock.Value = _product.MaximumStock;
            _txtUnit.Text = _product.Unit;
            _txtLocation.Text = _product.Location;
            _chkActive.Checked = _product.IsActive;
            _txtNotes.Text = _product.Notes;

            if (_product.Image != null && _product.Image.Length > 0)
            {
                using var ms = new MemoryStream(_product.Image);
                _picImage.Image = Image.FromStream(ms);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(_txtCode.Text))
            {
                XtraMessageBox.Show("Code is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtCode.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(_txtName.Text))
            {
                XtraMessageBox.Show("Name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtName.Focus();
                return;
            }

            if (_productRepository.CodeExists(_txtCode.Text, _product?.Id))
            {
                XtraMessageBox.Show("Product code already exists.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtCode.Focus();
                return;
            }

            try
            {
                var product = _product ?? new Product();
                
                product.Code = _txtCode.Text.Trim();
                product.Barcode = _txtBarcode.Text?.Trim() ?? "";
                product.Name = _txtName.Text.Trim();
                product.Description = _txtDescription.Text?.Trim() ?? "";
                product.CategoryId = _cboCategory.EditValue as int? ?? 0;
                product.SupplierId = _cboSupplier.EditValue as int? ?? 0;
                product.PurchasePrice = (decimal)_txtPurchasePrice.Value;
                product.SalePrice = (decimal)_txtSalePrice.Value;
                product.MinimumStock = (int)_txtMinStock.Value;
                product.MaximumStock = (int)_txtMaxStock.Value;
                product.Unit = _txtUnit.Text?.Trim() ?? "PCS";
                product.Location = _txtLocation.Text?.Trim() ?? "";
                product.IsActive = _chkActive.Checked;
                product.Notes = _txtNotes.Text?.Trim();
                product.UpdatedBy = _currentUser.Id;

                if (_picImage.Image != null)
                {
                    using var ms = new MemoryStream();
                    _picImage.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    product.Image = ms.ToArray();
                }

                if (_isNew)
                {
                    product.CurrentStock = (int)_txtCurrentStock.Value;
                    product.CreatedBy = _currentUser.Id;
                    product.Id = _productRepository.Create(product);
                    _activityLog.Log(_currentUser.Id, "CREATE", "Products", product.Id, null, product);

                    // Create initial stock movement if there's stock
                    if (product.CurrentStock > 0)
                    {
                        var movementRepo = new StockMovementRepository();
                        movementRepo.CreateMovement(new StockMovement
                        {
                            ProductId = product.Id,
                            Type = MovementType.Initial,
                            Quantity = product.CurrentStock,
                            UnitPrice = product.PurchasePrice,
                            UserId = _currentUser.Id,
                            Reference = "Initial stock entry"
                        });
                    }
                }
                else
                {
                    _productRepository.Update(product);
                    _activityLog.Log(_currentUser.Id, "UPDATE", "Products", product.Id, _product, product);
                }

                ProductSaved?.Invoke(this, EventArgs.Empty);
                
                XtraMessageBox.Show("Product saved successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                BtnBack_Click(sender, e);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error saving product: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBack_Click(object? sender, EventArgs e)
        {
            var parent = this.Parent;
            parent?.Controls.Clear();
            var productsControl = new ProductsControl(_currentUser) { Dock = DockStyle.Fill };
            parent?.Controls.Add(productsControl);
        }
    }
}
