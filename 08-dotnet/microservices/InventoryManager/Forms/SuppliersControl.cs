using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using InventoryManager.Data;
using InventoryManager.Models;
using InventoryManager.Services;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Suppliers management control
    /// </summary>
    public class SuppliersControl : XtraUserControl, IRefreshable, IExportable
    {
        private readonly User _currentUser;
        private readonly SupplierRepository _supplierRepository = new();
        private readonly ActivityLogRepository _activityLog = new();

        private GridControl _gridSuppliers = null!;
        private GridView _gridView = null!;
        private SearchControl _searchControl = null!;

        public SuppliersControl(User currentUser)
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
                Text = "Supplier Management",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(10, 15)
            };
            toolbarPanel.Controls.Add(titleLabel);

            // Search
            _searchControl = new SearchControl
            {
                Location = new Point(250, 15),
                Size = new Size(250, 30),
                NullValuePrompt = "Search suppliers..."
            };
            toolbarPanel.Controls.Add(_searchControl);

            // Buttons
            var btnNew = new SimpleButton
            {
                Text = "New Supplier",
                Location = new Point(520, 15),
                Size = new Size(110, 30)
            };
            btnNew.Click += BtnNew_Click;
            toolbarPanel.Controls.Add(btnNew);

            var btnEdit = new SimpleButton
            {
                Text = "Edit",
                Location = new Point(640, 15),
                Size = new Size(80, 30)
            };
            btnEdit.Click += BtnEdit_Click;
            toolbarPanel.Controls.Add(btnEdit);

            var btnDelete = new SimpleButton
            {
                Text = "Delete",
                Location = new Point(730, 15),
                Size = new Size(80, 30)
            };
            btnDelete.Click += BtnDelete_Click;
            toolbarPanel.Controls.Add(btnDelete);

            // Grid
            _gridSuppliers = new GridControl { Dock = DockStyle.Fill };
            _gridView = new GridView(_gridSuppliers);
            _gridSuppliers.MainView = _gridView;

            _gridView.OptionsView.ShowGroupPanel = false;
            _gridView.OptionsBehavior.Editable = false;
            _gridView.DoubleClick += GridView_DoubleClick;

            _gridView.Columns.AddVisible("Code", "Code");
            _gridView.Columns.AddVisible("Name", "Name");
            _gridView.Columns.AddVisible("ContactPerson", "Contact");
            _gridView.Columns.AddVisible("Phone", "Phone");
            _gridView.Columns.AddVisible("Email", "Email");
            _gridView.Columns.AddVisible("City", "City");
            _gridView.Columns.AddVisible("ProductCount", "Products");
            _gridView.Columns.AddVisible("IsActive", "Active");

            this.Controls.Add(_gridSuppliers);
            _gridSuppliers.BringToFront();

            _searchControl.Client = _gridSuppliers;

            this.ResumeLayout(false);
        }

        private void LoadData()
        {
            try
            {
                _gridSuppliers.DataSource = _supplierRepository.GetAll(true);
                _gridView.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error loading suppliers: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnNew_Click(object? sender, EventArgs e) => ShowEditForm(null);

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            var supplier = GetSelectedSupplier();
            if (supplier == null)
            {
                XtraMessageBox.Show("Please select a supplier.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ShowEditForm(supplier);
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var supplier = GetSelectedSupplier();
            if (supplier == null) return;

            if (supplier.ProductCount > 0)
            {
                XtraMessageBox.Show("Cannot delete supplier with associated products.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (XtraMessageBox.Show($"Delete supplier '{supplier.Name}'?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _supplierRepository.Delete(supplier.Id);
                _activityLog.Log(_currentUser.Id, "DELETE", "Suppliers", supplier.Id, supplier);
                LoadData();
            }
        }

        private void GridView_DoubleClick(object? sender, EventArgs e) => BtnEdit_Click(sender, e);

        private void ShowEditForm(Supplier? supplier)
        {
            using var form = new SupplierEditForm(supplier, _supplierRepository);
            if (form.ShowDialog() == DialogResult.OK)
            {
                var action = supplier == null ? "CREATE" : "UPDATE";
                _activityLog.Log(_currentUser.Id, action, "Suppliers", form.SavedSupplier?.Id);
                LoadData();
            }
        }

        private Supplier? GetSelectedSupplier()
        {
            var rowHandle = _gridView.FocusedRowHandle;
            return rowHandle >= 0 ? _gridView.GetRow(rowHandle) as Supplier : null;
        }

        public new void Refresh() => LoadData();
        public void ExportToExcel() => ExportService.ExportGridToExcel(_gridView, "Suppliers");
        public void ExportToPdf() => ExportService.ExportGridToPdf(_gridView, "Suppliers");
        public void Print() => ExportService.PrintGrid(_gridView, "Suppliers Report");
    }

    /// <summary>
    /// Supplier edit dialog
    /// </summary>
    public class SupplierEditForm : XtraForm
    {
        private readonly Supplier? _supplier;
        private readonly SupplierRepository _repository;
        public Supplier? SavedSupplier { get; private set; }

        private TextEdit _txtCode = null!, _txtName = null!, _txtContact = null!;
        private TextEdit _txtPhone = null!, _txtEmail = null!, _txtAddress = null!;
        private TextEdit _txtCity = null!, _txtCountry = null!, _txtTaxId = null!;
        private TextEdit _txtWebsite = null!, _txtPaymentTerms = null!;
        private MemoEdit _txtNotes = null!;
        private CheckEdit _chkActive = null!;

        public SupplierEditForm(Supplier? supplier, SupplierRepository repository)
        {
            _supplier = supplier;
            _repository = repository;
            InitializeComponents();
            LoadData();
        }

        private void InitializeComponents()
        {
            this.Text = _supplier == null ? "New Supplier" : "Edit Supplier";
            this.Size = new Size(500, 550);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int y = 20;
            int labelX = 20, controlX = 140, width = 320;

            AddRow(ref y, "Code:", out _txtCode);
            if (_supplier == null) _txtCode.Text = _repository.GenerateNextCode();
            AddRow(ref y, "Name:", out _txtName);
            AddRow(ref y, "Contact Person:", out _txtContact);
            AddRow(ref y, "Phone:", out _txtPhone);
            AddRow(ref y, "Email:", out _txtEmail);
            AddRow(ref y, "Address:", out _txtAddress);
            AddRow(ref y, "City:", out _txtCity);
            AddRow(ref y, "Country:", out _txtCountry);
            AddRow(ref y, "Tax ID:", out _txtTaxId);
            AddRow(ref y, "Website:", out _txtWebsite);
            AddRow(ref y, "Payment Terms:", out _txtPaymentTerms);

            this.Controls.Add(new LabelControl { Text = "Notes:", Location = new Point(labelX, y + 3) });
            _txtNotes = new MemoEdit { Location = new Point(controlX, y), Size = new Size(width, 60) };
            this.Controls.Add(_txtNotes);
            y += 75;

            _chkActive = new CheckEdit { Text = "Active", Location = new Point(controlX, y), Checked = true };
            this.Controls.Add(_chkActive);
            y += 40;

            var btnSave = new SimpleButton { Text = "Save", Location = new Point(controlX, y), Size = new Size(100, 30) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            var btnCancel = new SimpleButton { Text = "Cancel", Location = new Point(controlX + 110, y), Size = new Size(100, 30) };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancel);
        }

        private void AddRow(ref int y, string label, out TextEdit textEdit)
        {
            this.Controls.Add(new LabelControl { Text = label, Location = new Point(20, y + 3) });
            textEdit = new TextEdit { Location = new Point(140, y), Size = new Size(320, 25) };
            this.Controls.Add(textEdit);
            y += 35;
        }

        private void LoadData()
        {
            if (_supplier == null) return;
            _txtCode.Text = _supplier.Code;
            _txtName.Text = _supplier.Name;
            _txtContact.Text = _supplier.ContactPerson;
            _txtPhone.Text = _supplier.Phone;
            _txtEmail.Text = _supplier.Email;
            _txtAddress.Text = _supplier.Address;
            _txtCity.Text = _supplier.City;
            _txtCountry.Text = _supplier.Country;
            _txtTaxId.Text = _supplier.TaxId;
            _txtWebsite.Text = _supplier.Website;
            _txtPaymentTerms.Text = _supplier.PaymentTerms;
            _txtNotes.Text = _supplier.Notes;
            _chkActive.Checked = _supplier.IsActive;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtCode.Text) || string.IsNullOrWhiteSpace(_txtName.Text))
            {
                XtraMessageBox.Show("Code and Name are required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var supplier = _supplier ?? new Supplier();
                supplier.Code = _txtCode.Text.Trim();
                supplier.Name = _txtName.Text.Trim();
                supplier.ContactPerson = _txtContact.Text?.Trim() ?? "";
                supplier.Phone = _txtPhone.Text?.Trim() ?? "";
                supplier.Email = _txtEmail.Text?.Trim() ?? "";
                supplier.Address = _txtAddress.Text?.Trim() ?? "";
                supplier.City = _txtCity.Text?.Trim() ?? "";
                supplier.Country = _txtCountry.Text?.Trim() ?? "";
                supplier.TaxId = _txtTaxId.Text?.Trim() ?? "";
                supplier.Website = _txtWebsite.Text?.Trim() ?? "";
                supplier.PaymentTerms = _txtPaymentTerms.Text?.Trim() ?? "";
                supplier.Notes = _txtNotes.Text?.Trim();
                supplier.IsActive = _chkActive.Checked;

                if (_supplier == null)
                    supplier.Id = _repository.Create(supplier);
                else
                    _repository.Update(supplier);

                SavedSupplier = supplier;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error saving: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
