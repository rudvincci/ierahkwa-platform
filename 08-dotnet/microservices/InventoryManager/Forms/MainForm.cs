using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Navigation;
using InventoryManager.Models;
using InventoryManager.Data;
using InventoryManager.Services;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Main application form with ribbon interface
    /// </summary>
    public partial class MainForm : RibbonForm
    {
        private readonly User _currentUser;
        private readonly UserRepository _userRepository = new();
        private readonly ProductRepository _productRepository = new();
        private readonly ActivityLogRepository _activityLog = new();
        private readonly SettingsRepository _settings = new();

        // Ribbon items
        private RibbonControl _ribbon = null!;
        private RibbonPage _pageHome = null!;
        private RibbonPage _pageInventory = null!;
        private RibbonPage _pageReports = null!;
        private RibbonPage _pageTools = null!;
        private RibbonPage _pageHelp = null!;
        private RibbonStatusBar _statusBar = null!;
        
        // Navigation
        private NavigationPane _navPane = null!;
        private XtraUserControl _contentPanel = null!;
        
        // Status bar items
        private BarStaticItem _statusUser = null!;
        private BarStaticItem _statusDatabase = null!;
        private BarStaticItem _statusStock = null!;
        private BarStaticItem _statusTime = null!;

        private System.Windows.Forms.Timer _statusTimer = null!;

        public MainForm(User currentUser)
        {
            _currentUser = currentUser;
            InitializeComponent();
            SetupRibbon();
            SetupNavigation();
            SetupStatusBar();
            LoadDashboard();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form settings
            this.Text = $"Inventory Manager Pro - {_currentUser.FullName}";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1200, 700);

            // Content panel
            _contentPanel = new XtraUserControl
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(_contentPanel);

            this.FormClosing += MainForm_FormClosing;

            this.ResumeLayout(false);
        }

        private void SetupRibbon()
        {
            _ribbon = new RibbonControl();
            this.Ribbon = _ribbon;

            // === HOME PAGE ===
            _pageHome = new RibbonPage("Home");
            _ribbon.Pages.Add(_pageHome);

            // Dashboard group
            var grpDashboard = new RibbonPageGroup("Dashboard");
            _pageHome.Groups.Add(grpDashboard);

            var btnDashboard = new BarButtonItem { Caption = "Dashboard", LargeGlyph = CreateIcon("dashboard", 32) };
            btnDashboard.ItemClick += (s, e) => LoadDashboard();
            grpDashboard.ItemLinks.Add(btnDashboard);

            var btnRefresh = new BarButtonItem { Caption = "Refresh", LargeGlyph = CreateIcon("refresh", 32) };
            btnRefresh.ItemClick += (s, e) => RefreshCurrentView();
            grpDashboard.ItemLinks.Add(btnRefresh);

            // Quick Actions group
            var grpQuickActions = new RibbonPageGroup("Quick Actions");
            _pageHome.Groups.Add(grpQuickActions);

            var btnQuickSearch = new BarButtonItem { Caption = "Quick Search\n(F3)", LargeGlyph = CreateIcon("search", 32) };
            btnQuickSearch.ItemClick += (s, e) => ShowQuickSearch();
            grpQuickActions.ItemLinks.Add(btnQuickSearch);

            var btnQuickEntry = new BarButtonItem { Caption = "Quick Stock\nEntry", LargeGlyph = CreateIcon("add", 32) };
            btnQuickEntry.ItemClick += (s, e) => ShowQuickStockEntry();
            grpQuickActions.ItemLinks.Add(btnQuickEntry);

            // === INVENTORY PAGE ===
            _pageInventory = new RibbonPage("Inventory");
            _ribbon.Pages.Add(_pageInventory);

            // Products group
            var grpProducts = new RibbonPageGroup("Products");
            _pageInventory.Groups.Add(grpProducts);

            var btnProducts = new BarButtonItem { Caption = "Products", LargeGlyph = CreateIcon("products", 32) };
            btnProducts.ItemClick += (s, e) => ShowProducts();
            grpProducts.ItemLinks.Add(btnProducts);

            var btnNewProduct = new BarButtonItem { Caption = "New Product", LargeGlyph = CreateIcon("add_product", 32) };
            btnNewProduct.ItemClick += (s, e) => ShowNewProduct();
            grpProducts.ItemLinks.Add(btnNewProduct);

            var btnCategories = new BarButtonItem { Caption = "Categories", LargeGlyph = CreateIcon("categories", 32) };
            btnCategories.ItemClick += (s, e) => ShowCategories();
            grpProducts.ItemLinks.Add(btnCategories);

            // Stock group
            var grpStock = new RibbonPageGroup("Stock");
            _pageInventory.Groups.Add(grpStock);

            var btnStockIn = new BarButtonItem { Caption = "Stock In\n(Purchase)", LargeGlyph = CreateIcon("stock_in", 32) };
            btnStockIn.ItemClick += (s, e) => ShowStockMovement(MovementType.Purchase);
            grpStock.ItemLinks.Add(btnStockIn);

            var btnStockOut = new BarButtonItem { Caption = "Stock Out\n(Sale)", LargeGlyph = CreateIcon("stock_out", 32) };
            btnStockOut.ItemClick += (s, e) => ShowStockMovement(MovementType.Sale);
            grpStock.ItemLinks.Add(btnStockOut);

            var btnAdjustment = new BarButtonItem { Caption = "Stock\nAdjustment", LargeGlyph = CreateIcon("adjustment", 32) };
            btnAdjustment.ItemClick += (s, e) => ShowStockMovement(MovementType.Adjustment);
            grpStock.ItemLinks.Add(btnAdjustment);

            var btnMovements = new BarButtonItem { Caption = "Movement\nHistory", LargeGlyph = CreateIcon("history", 32) };
            btnMovements.ItemClick += (s, e) => ShowMovementHistory();
            grpStock.ItemLinks.Add(btnMovements);

            // Suppliers group
            var grpSuppliers = new RibbonPageGroup("Suppliers");
            _pageInventory.Groups.Add(grpSuppliers);

            var btnSuppliers = new BarButtonItem { Caption = "Suppliers", LargeGlyph = CreateIcon("suppliers", 32) };
            btnSuppliers.ItemClick += (s, e) => ShowSuppliers();
            grpSuppliers.ItemLinks.Add(btnSuppliers);

            // === REPORTS PAGE ===
            _pageReports = new RibbonPage("Reports");
            _ribbon.Pages.Add(_pageReports);

            // Reports group
            var grpReports = new RibbonPageGroup("Reports");
            _pageReports.Groups.Add(grpReports);

            var btnStockReport = new BarButtonItem { Caption = "Stock\nReport", LargeGlyph = CreateIcon("report_stock", 32) };
            btnStockReport.ItemClick += (s, e) => ShowStockReport();
            grpReports.ItemLinks.Add(btnStockReport);

            var btnLowStock = new BarButtonItem { Caption = "Low Stock\nAlert", LargeGlyph = CreateIcon("alert", 32) };
            btnLowStock.ItemClick += (s, e) => ShowLowStockReport();
            grpReports.ItemLinks.Add(btnLowStock);

            var btnMovementReport = new BarButtonItem { Caption = "Movement\nReport", LargeGlyph = CreateIcon("report_movement", 32) };
            btnMovementReport.ItemClick += (s, e) => ShowMovementReport();
            grpReports.ItemLinks.Add(btnMovementReport);

            var btnStatistics = new BarButtonItem { Caption = "Statistics", LargeGlyph = CreateIcon("statistics", 32) };
            btnStatistics.ItemClick += (s, e) => ShowStatistics();
            grpReports.ItemLinks.Add(btnStatistics);

            // Export group
            var grpExport = new RibbonPageGroup("Export");
            _pageReports.Groups.Add(grpExport);

            var btnExportExcel = new BarButtonItem { Caption = "Export to\nExcel", LargeGlyph = CreateIcon("excel", 32) };
            btnExportExcel.ItemClick += (s, e) => ExportToExcel();
            grpExport.ItemLinks.Add(btnExportExcel);

            var btnExportPdf = new BarButtonItem { Caption = "Export to\nPDF", LargeGlyph = CreateIcon("pdf", 32) };
            btnExportPdf.ItemClick += (s, e) => ExportToPdf();
            grpExport.ItemLinks.Add(btnExportPdf);

            var btnPrint = new BarButtonItem { Caption = "Print", LargeGlyph = CreateIcon("print", 32) };
            btnPrint.ItemClick += (s, e) => PrintCurrentView();
            grpExport.ItemLinks.Add(btnPrint);

            // === TOOLS PAGE ===
            _pageTools = new RibbonPage("Tools");
            _ribbon.Pages.Add(_pageTools);

            // Database group
            var grpDatabase = new RibbonPageGroup("Database");
            _pageTools.Groups.Add(grpDatabase);

            var btnBackup = new BarButtonItem { Caption = "Backup\nDatabase", LargeGlyph = CreateIcon("backup", 32) };
            btnBackup.ItemClick += (s, e) => BackupDatabase();
            grpDatabase.ItemLinks.Add(btnBackup);

            var btnRestore = new BarButtonItem { Caption = "Restore\nDatabase", LargeGlyph = CreateIcon("restore", 32) };
            btnRestore.ItemClick += (s, e) => RestoreDatabase();
            grpDatabase.ItemLinks.Add(btnRestore);

            // Admin group (only for admins)
            if (_currentUser.Role == UserRole.Admin)
            {
                var grpAdmin = new RibbonPageGroup("Administration");
                _pageTools.Groups.Add(grpAdmin);

                var btnUsers = new BarButtonItem { Caption = "User\nManagement", LargeGlyph = CreateIcon("users", 32) };
                btnUsers.ItemClick += (s, e) => ShowUsers();
                grpAdmin.ItemLinks.Add(btnUsers);

                var btnSettings = new BarButtonItem { Caption = "Settings", LargeGlyph = CreateIcon("settings", 32) };
                btnSettings.ItemClick += (s, e) => ShowSettings();
                grpAdmin.ItemLinks.Add(btnSettings);

                var btnActivityLog = new BarButtonItem { Caption = "Activity\nLog", LargeGlyph = CreateIcon("log", 32) };
                btnActivityLog.ItemClick += (s, e) => ShowActivityLog();
                grpAdmin.ItemLinks.Add(btnActivityLog);
            }

            // === HELP PAGE ===
            _pageHelp = new RibbonPage("Help");
            _ribbon.Pages.Add(_pageHelp);

            var grpHelp = new RibbonPageGroup("Help");
            _pageHelp.Groups.Add(grpHelp);

            var btnHelp = new BarButtonItem { Caption = "Help\nDocumentation", LargeGlyph = CreateIcon("help", 32) };
            btnHelp.ItemClick += (s, e) => ShowHelp();
            grpHelp.ItemLinks.Add(btnHelp);

            var btnAbout = new BarButtonItem { Caption = "About", LargeGlyph = CreateIcon("about", 32) };
            btnAbout.ItemClick += (s, e) => ShowAbout();
            grpHelp.ItemLinks.Add(btnAbout);

            // Application menu
            _ribbon.ApplicationButtonText = "File";
            var appMenu = new ApplicationMenu();
            
            var btnChangePassword = new BarButtonItem { Caption = "Change Password" };
            btnChangePassword.ItemClick += (s, e) => ChangePassword();
            appMenu.ItemLinks.Add(btnChangePassword);

            var btnLogout = new BarButtonItem { Caption = "Logout" };
            btnLogout.ItemClick += (s, e) => Logout();
            appMenu.ItemLinks.Add(btnLogout);

            appMenu.ItemLinks.Add(new BarItemLink { BeginGroup = true });

            var btnExit = new BarButtonItem { Caption = "Exit" };
            btnExit.ItemClick += (s, e) => this.Close();
            appMenu.ItemLinks.Add(btnExit);

            _ribbon.ApplicationButtonDropDownControl = appMenu;

            // Keyboard shortcuts
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
        }

        private void SetupNavigation()
        {
            _navPane = new NavigationPane
            {
                Dock = DockStyle.Left,
                Width = 200,
                RegularSize = new Size(200, 0),
                ItemOrientation = Orientation.Vertical
            };

            // Dashboard
            var navDashboard = new NavigationPage { Caption = "Dashboard", Name = "navDashboard" };
            navDashboard.ImageOptions.Image = CreateIcon("dashboard", 24);
            _navPane.Pages.Add(navDashboard);

            // Products
            var navProducts = new NavigationPage { Caption = "Products", Name = "navProducts" };
            navProducts.ImageOptions.Image = CreateIcon("products", 24);
            _navPane.Pages.Add(navProducts);

            // Categories
            var navCategories = new NavigationPage { Caption = "Categories", Name = "navCategories" };
            navCategories.ImageOptions.Image = CreateIcon("categories", 24);
            _navPane.Pages.Add(navCategories);

            // Suppliers
            var navSuppliers = new NavigationPage { Caption = "Suppliers", Name = "navSuppliers" };
            navSuppliers.ImageOptions.Image = CreateIcon("suppliers", 24);
            _navPane.Pages.Add(navSuppliers);

            // Stock Movements
            var navMovements = new NavigationPage { Caption = "Stock Movements", Name = "navMovements" };
            navMovements.ImageOptions.Image = CreateIcon("history", 24);
            _navPane.Pages.Add(navMovements);

            // Reports
            var navReports = new NavigationPage { Caption = "Reports", Name = "navReports" };
            navReports.ImageOptions.Image = CreateIcon("report_stock", 24);
            _navPane.Pages.Add(navReports);

            _navPane.SelectedPageChanged += NavPane_SelectedPageChanged;

            this.Controls.Add(_navPane);
            _navPane.BringToFront();
        }

        private void SetupStatusBar()
        {
            _statusBar = new RibbonStatusBar();
            _ribbon.StatusBar = _statusBar;

            _statusUser = new BarStaticItem { Caption = $"User: {_currentUser.FullName}" };
            _statusBar.ItemLinks.Add(_statusUser);

            _statusDatabase = new BarStaticItem { Caption = "Database: Connected" };
            _statusBar.ItemLinks.Add(_statusDatabase);

            _statusStock = new BarStaticItem { Caption = "Loading..." };
            _statusBar.ItemLinks.Add(_statusStock);

            _statusTime = new BarStaticItem { Caption = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
            _statusBar.ItemLinks.Add(_statusTime, true); // Right-aligned

            // Timer for status updates
            _statusTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _statusTimer.Tick += StatusTimer_Tick;
            _statusTimer.Start();

            UpdateStockStatus();
        }

        private void StatusTimer_Tick(object? sender, EventArgs e)
        {
            _statusTime.Caption = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void UpdateStockStatus()
        {
            try
            {
                var (totalValue, totalItems, lowStockCount) = _productRepository.GetStockSummary();
                _statusStock.Caption = $"Stock Value: ${totalValue:N2} | Items: {totalItems:N0} | Low Stock: {lowStockCount}";
                
                if (lowStockCount > 0)
                    _statusStock.Appearance.ForeColor = Color.Red;
                else
                    _statusStock.Appearance.ForeColor = Color.Green;
            }
            catch { }
        }

        private void NavPane_SelectedPageChanged(object sender, NavigationPageChangedEventArgs e)
        {
            switch (e.Page?.Name)
            {
                case "navDashboard": LoadDashboard(); break;
                case "navProducts": ShowProducts(); break;
                case "navCategories": ShowCategories(); break;
                case "navSuppliers": ShowSuppliers(); break;
                case "navMovements": ShowMovementHistory(); break;
                case "navReports": ShowStatistics(); break;
            }
        }

        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                ShowQuickSearch();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F5)
            {
                RefreshCurrentView();
                e.Handled = true;
            }
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (XtraMessageBox.Show("Are you sure you want to exit?", "Confirm Exit",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            _statusTimer?.Stop();
            _userRepository.Logout(_currentUser.Id);
            _activityLog.Log(_currentUser.Id, "LOGOUT", "Users", _currentUser.Id);
        }

        // Helper method to create simple colored icons
        private Image CreateIcon(string name, int size)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var color = name switch
                {
                    "dashboard" => Color.FromArgb(0, 122, 204),
                    "products" => Color.FromArgb(76, 175, 80),
                    "categories" => Color.FromArgb(255, 152, 0),
                    "suppliers" => Color.FromArgb(156, 39, 176),
                    "search" => Color.FromArgb(33, 150, 243),
                    "add" or "add_product" => Color.FromArgb(76, 175, 80),
                    "stock_in" => Color.FromArgb(76, 175, 80),
                    "stock_out" => Color.FromArgb(244, 67, 54),
                    "adjustment" => Color.FromArgb(255, 193, 7),
                    "history" => Color.FromArgb(158, 158, 158),
                    "alert" => Color.FromArgb(244, 67, 54),
                    "report_stock" or "report_movement" => Color.FromArgb(63, 81, 181),
                    "statistics" => Color.FromArgb(0, 188, 212),
                    "excel" => Color.FromArgb(76, 175, 80),
                    "pdf" => Color.FromArgb(244, 67, 54),
                    "print" => Color.FromArgb(96, 125, 139),
                    "backup" => Color.FromArgb(33, 150, 243),
                    "restore" => Color.FromArgb(255, 152, 0),
                    "users" => Color.FromArgb(103, 58, 183),
                    "settings" => Color.FromArgb(96, 125, 139),
                    "log" => Color.FromArgb(121, 85, 72),
                    "help" => Color.FromArgb(0, 150, 136),
                    "about" => Color.FromArgb(63, 81, 181),
                    "refresh" => Color.FromArgb(76, 175, 80),
                    _ => Color.FromArgb(96, 125, 139)
                };
                
                using (var brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, 2, 2, size - 4, size - 4);
                }
                
                // Draw first letter of icon name
                using (var font = new Font("Segoe UI", size / 3, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.White))
                {
                    var letter = char.ToUpper(name[0]).ToString();
                    var textSize = g.MeasureString(letter, font);
                    g.DrawString(letter, font, brush, 
                        (size - textSize.Width) / 2, 
                        (size - textSize.Height) / 2);
                }
            }
            return bmp;
        }

        // Navigation methods
        private void LoadDashboard() => LoadControl(new DashboardControl(_currentUser));
        private void ShowProducts() => LoadControl(new ProductsControl(_currentUser));
        private void ShowNewProduct() => LoadControl(new ProductEditControl(_currentUser, null));
        private void ShowCategories() => LoadControl(new CategoriesControl(_currentUser));
        private void ShowSuppliers() => LoadControl(new SuppliersControl(_currentUser));
        private void ShowStockMovement(MovementType type) => LoadControl(new StockMovementControl(_currentUser, type));
        private void ShowMovementHistory() => LoadControl(new MovementHistoryControl(_currentUser));
        private void ShowQuickSearch() => new QuickSearchForm(_currentUser).ShowDialog(this);
        private void ShowQuickStockEntry() => new QuickStockEntryForm(_currentUser).ShowDialog(this);
        private void ShowStockReport() => LoadControl(new ReportsControl(_currentUser, "Stock"));
        private void ShowLowStockReport() => LoadControl(new ReportsControl(_currentUser, "LowStock"));
        private void ShowMovementReport() => LoadControl(new ReportsControl(_currentUser, "Movement"));
        private void ShowStatistics() => LoadControl(new StatisticsControl(_currentUser));
        private void ShowUsers() => LoadControl(new UsersControl(_currentUser));
        private void ShowSettings() => new SettingsForm().ShowDialog(this);
        private void ShowActivityLog() => LoadControl(new ActivityLogControl(_currentUser));
        private void ShowHelp() => new HelpForm().ShowDialog(this);
        private void ShowAbout() => new AboutForm().ShowDialog(this);

        private void LoadControl(UserControl control)
        {
            _contentPanel.Controls.Clear();
            control.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(control);
            UpdateStockStatus();
        }

        private void RefreshCurrentView()
        {
            if (_contentPanel.Controls.Count > 0 && _contentPanel.Controls[0] is IRefreshable refreshable)
            {
                refreshable.Refresh();
            }
            UpdateStockStatus();
        }

        private void ExportToExcel()
        {
            if (_contentPanel.Controls.Count > 0 && _contentPanel.Controls[0] is IExportable exportable)
            {
                exportable.ExportToExcel();
            }
        }

        private void ExportToPdf()
        {
            if (_contentPanel.Controls.Count > 0 && _contentPanel.Controls[0] is IExportable exportable)
            {
                exportable.ExportToPdf();
            }
        }

        private void PrintCurrentView()
        {
            if (_contentPanel.Controls.Count > 0 && _contentPanel.Controls[0] is IExportable exportable)
            {
                exportable.Print();
            }
        }

        private void BackupDatabase()
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "SQLite Database|*.db|All Files|*.*",
                FileName = $"inventory_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db",
                InitialDirectory = _settings.GetValue("BackupPath", 
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (DatabaseManager.BackupDatabase(dialog.FileName))
                {
                    _activityLog.Log(_currentUser.Id, "BACKUP", "Database");
                    XtraMessageBox.Show("Database backup completed successfully!", "Backup",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    XtraMessageBox.Show("Database backup failed!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RestoreDatabase()
        {
            if (_currentUser.Role != UserRole.Admin)
            {
                XtraMessageBox.Show("Only administrators can restore the database.", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dialog = new OpenFileDialog
            {
                Filter = "SQLite Database|*.db|All Files|*.*",
                InitialDirectory = _settings.GetValue("BackupPath", 
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (XtraMessageBox.Show(
                    "WARNING: This will replace all current data with the backup data.\n\nAre you sure you want to continue?",
                    "Confirm Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (DatabaseManager.RestoreDatabase(dialog.FileName))
                    {
                        XtraMessageBox.Show("Database restored successfully!\n\nThe application will now restart.",
                            "Restore", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Application.Restart();
                    }
                    else
                    {
                        XtraMessageBox.Show("Database restore failed!", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ChangePassword()
        {
            using var form = new ChangePasswordForm(_currentUser.Id);
            form.ShowDialog(this);
        }

        private void Logout()
        {
            if (XtraMessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _statusTimer?.Stop();
                _userRepository.Logout(_currentUser.Id);
                _activityLog.Log(_currentUser.Id, "LOGOUT", "Users", _currentUser.Id);
                
                this.Hide();
                
                using var loginForm = new LoginForm();
                if (loginForm.ShowDialog() == DialogResult.OK && loginForm.CurrentUser != null)
                {
                    Application.Restart();
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _statusTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // Interfaces for control capabilities
    public interface IRefreshable
    {
        new void Refresh();
    }

    public interface IExportable
    {
        void ExportToExcel();
        void ExportToPdf();
        void Print();
    }
}
