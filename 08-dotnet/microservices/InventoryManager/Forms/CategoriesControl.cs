using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using InventoryManager.Data;
using InventoryManager.Models;
using InventoryManager.Services;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Categories management control
    /// </summary>
    public class CategoriesControl : XtraUserControl, IRefreshable, IExportable
    {
        private readonly User _currentUser;
        private readonly CategoryRepository _categoryRepository = new();
        private readonly ActivityLogRepository _activityLog = new();

        private GridControl _gridCategories = null!;
        private GridView _gridView = null!;

        public CategoriesControl(User currentUser)
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
                Text = "Category Management",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(10, 15)
            };
            toolbarPanel.Controls.Add(titleLabel);

            // Buttons
            var btnNew = new SimpleButton
            {
                Text = "New Category",
                Location = new Point(400, 15),
                Size = new Size(120, 30)
            };
            btnNew.Click += BtnNew_Click;
            toolbarPanel.Controls.Add(btnNew);

            var btnEdit = new SimpleButton
            {
                Text = "Edit",
                Location = new Point(530, 15),
                Size = new Size(80, 30)
            };
            btnEdit.Click += BtnEdit_Click;
            toolbarPanel.Controls.Add(btnEdit);

            var btnDelete = new SimpleButton
            {
                Text = "Delete",
                Location = new Point(620, 15),
                Size = new Size(80, 30)
            };
            btnDelete.Click += BtnDelete_Click;
            toolbarPanel.Controls.Add(btnDelete);

            // Grid
            _gridCategories = new GridControl
            {
                Dock = DockStyle.Fill
            };

            _gridView = new GridView(_gridCategories);
            _gridCategories.MainView = _gridView;

            _gridView.OptionsView.ShowGroupPanel = false;
            _gridView.OptionsBehavior.Editable = false;
            _gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            _gridView.DoubleClick += GridView_DoubleClick;

            _gridView.Columns.AddVisible("Code", "Code");
            _gridView.Columns.AddVisible("Name", "Name");
            _gridView.Columns.AddVisible("Description", "Description");
            _gridView.Columns.AddVisible("ParentName", "Parent Category");
            _gridView.Columns.AddVisible("ProductCount", "Products");
            _gridView.Columns.AddVisible("IsActive", "Active");

            this.Controls.Add(_gridCategories);
            _gridCategories.BringToFront();

            this.ResumeLayout(false);
        }

        private void LoadData()
        {
            try
            {
                _gridCategories.DataSource = _categoryRepository.GetAll(true);
                _gridView.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error loading categories: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnNew_Click(object? sender, EventArgs e)
        {
            ShowEditForm(null);
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            var category = GetSelectedCategory();
            if (category == null)
            {
                XtraMessageBox.Show("Please select a category.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ShowEditForm(category);
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var category = GetSelectedCategory();
            if (category == null)
            {
                XtraMessageBox.Show("Please select a category.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (category.ProductCount > 0)
            {
                XtraMessageBox.Show("Cannot delete category with associated products.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (XtraMessageBox.Show($"Delete category '{category.Name}'?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _categoryRepository.Delete(category.Id);
                _activityLog.Log(_currentUser.Id, "DELETE", "Categories", category.Id, category);
                LoadData();
            }
        }

        private void GridView_DoubleClick(object? sender, EventArgs e)
        {
            BtnEdit_Click(sender, e);
        }

        private void ShowEditForm(Category? category)
        {
            using var form = new CategoryEditForm(category, _categoryRepository);
            if (form.ShowDialog() == DialogResult.OK)
            {
                var action = category == null ? "CREATE" : "UPDATE";
                _activityLog.Log(_currentUser.Id, action, "Categories", form.SavedCategory?.Id);
                LoadData();
            }
        }

        private Category? GetSelectedCategory()
        {
            var rowHandle = _gridView.FocusedRowHandle;
            if (rowHandle < 0) return null;
            return _gridView.GetRow(rowHandle) as Category;
        }

        public new void Refresh() => LoadData();

        public void ExportToExcel() => ExportService.ExportGridToExcel(_gridView, "Categories");
        public void ExportToPdf() => ExportService.ExportGridToPdf(_gridView, "Categories");
        public void Print() => ExportService.PrintGrid(_gridView, "Categories Report");
    }

    /// <summary>
    /// Category edit dialog
    /// </summary>
    public class CategoryEditForm : XtraForm
    {
        private readonly Category? _category;
        private readonly CategoryRepository _repository;
        public Category? SavedCategory { get; private set; }

        private TextEdit _txtCode = null!;
        private TextEdit _txtName = null!;
        private MemoEdit _txtDescription = null!;
        private LookUpEdit _cboParent = null!;
        private CheckEdit _chkActive = null!;

        public CategoryEditForm(Category? category, CategoryRepository repository)
        {
            _category = category;
            _repository = repository;
            InitializeComponents();
            LoadData();
        }

        private void InitializeComponents()
        {
            this.Text = _category == null ? "New Category" : "Edit Category";
            this.Size = new Size(400, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int y = 20;

            this.Controls.Add(new LabelControl { Text = "Code:", Location = new Point(20, y + 3) });
            _txtCode = new TextEdit { Location = new Point(120, y), Size = new Size(240, 25) };
            if (_category == null) _txtCode.Text = _repository.GenerateNextCode();
            this.Controls.Add(_txtCode);

            y += 35;
            this.Controls.Add(new LabelControl { Text = "Name:", Location = new Point(20, y + 3) });
            _txtName = new TextEdit { Location = new Point(120, y), Size = new Size(240, 25) };
            this.Controls.Add(_txtName);

            y += 35;
            this.Controls.Add(new LabelControl { Text = "Description:", Location = new Point(20, y + 3) });
            _txtDescription = new MemoEdit { Location = new Point(120, y), Size = new Size(240, 60) };
            this.Controls.Add(_txtDescription);

            y += 75;
            this.Controls.Add(new LabelControl { Text = "Parent:", Location = new Point(20, y + 3) });
            _cboParent = new LookUpEdit { Location = new Point(120, y), Size = new Size(240, 25) };
            _cboParent.Properties.NullText = "(None)";
            _cboParent.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name"));
            _cboParent.Properties.DisplayMember = "Name";
            _cboParent.Properties.ValueMember = "Id";
            _cboParent.Properties.DataSource = _repository.GetAll()
                .Where(c => c.Id != _category?.Id).ToList();
            this.Controls.Add(_cboParent);

            y += 35;
            _chkActive = new CheckEdit { Text = "Active", Location = new Point(120, y), Checked = true };
            this.Controls.Add(_chkActive);

            y += 45;
            var btnSave = new SimpleButton { Text = "Save", Location = new Point(120, y), Size = new Size(100, 30) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            var btnCancel = new SimpleButton { Text = "Cancel", Location = new Point(230, y), Size = new Size(100, 30) };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancel);
        }

        private void LoadData()
        {
            if (_category == null) return;
            _txtCode.Text = _category.Code;
            _txtName.Text = _category.Name;
            _txtDescription.Text = _category.Description;
            _cboParent.EditValue = _category.ParentId;
            _chkActive.Checked = _category.IsActive;
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
                var category = _category ?? new Category();
                category.Code = _txtCode.Text.Trim();
                category.Name = _txtName.Text.Trim();
                category.Description = _txtDescription.Text?.Trim() ?? "";
                category.ParentId = _cboParent.EditValue as int?;
                category.IsActive = _chkActive.Checked;

                if (_category == null)
                    category.Id = _repository.Create(category);
                else
                    _repository.Update(category);

                SavedCategory = category;
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
