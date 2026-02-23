using DevExpress.XtraEditors;
using DevExpress.XtraCharts;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using InventoryManager.Data;
using InventoryManager.Models;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Dashboard control showing overview statistics
    /// </summary>
    public class DashboardControl : XtraUserControl, IRefreshable
    {
        private readonly User _currentUser;
        private readonly ProductRepository _productRepository = new();
        private readonly StockMovementRepository _movementRepository = new();
        private readonly CategoryRepository _categoryRepository = new();
        private readonly SupplierRepository _supplierRepository = new();

        private LabelControl _lblTotalProducts = null!;
        private LabelControl _lblTotalValue = null!;
        private LabelControl _lblLowStock = null!;
        private LabelControl _lblTodayMovements = null!;
        private GridControl _gridLowStock = null!;
        private GridControl _gridRecentMovements = null!;
        private ChartControl _chartStock = null!;

        public DashboardControl(User currentUser)
        {
            _currentUser = currentUser;
            InitializeComponents();
            LoadData();
        }

        private void InitializeComponents()
        {
            this.SuspendLayout();

            // Title
            var titleLabel = new LabelControl
            {
                Text = "Dashboard Overview",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSizeMode = LabelAutoSizeMode.Default
            };
            this.Controls.Add(titleLabel);

            // Stats cards row
            var cardsPanel = new PanelControl
            {
                Location = new Point(20, 70),
                Size = new Size(1200, 120),
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            this.Controls.Add(cardsPanel);

            // Card 1 - Total Products
            var card1 = CreateStatCard("Total Products", Color.FromArgb(0, 122, 204), 0);
            _lblTotalProducts = (LabelControl)card1.Controls[1];
            cardsPanel.Controls.Add(card1);

            // Card 2 - Total Stock Value
            var card2 = CreateStatCard("Total Stock Value", Color.FromArgb(76, 175, 80), 290);
            _lblTotalValue = (LabelControl)card2.Controls[1];
            cardsPanel.Controls.Add(card2);

            // Card 3 - Low Stock Items
            var card3 = CreateStatCard("Low Stock Alert", Color.FromArgb(244, 67, 54), 580);
            _lblLowStock = (LabelControl)card3.Controls[1];
            cardsPanel.Controls.Add(card3);

            // Card 4 - Today's Movements
            var card4 = CreateStatCard("Today's Movements", Color.FromArgb(156, 39, 176), 870);
            _lblTodayMovements = (LabelControl)card4.Controls[1];
            cardsPanel.Controls.Add(card4);

            // Low Stock Grid
            var lblLowStockGrid = new LabelControl
            {
                Text = "Low Stock Items",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 210)
            };
            this.Controls.Add(lblLowStockGrid);

            _gridLowStock = new GridControl
            {
                Location = new Point(20, 240),
                Size = new Size(580, 250)
            };
            var viewLowStock = new GridView(_gridLowStock);
            _gridLowStock.MainView = viewLowStock;
            viewLowStock.OptionsView.ShowGroupPanel = false;
            viewLowStock.OptionsBehavior.Editable = false;
            viewLowStock.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.Controls.Add(_gridLowStock);

            // Recent Movements Grid
            var lblRecentMovements = new LabelControl
            {
                Text = "Recent Movements",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(620, 210)
            };
            this.Controls.Add(lblRecentMovements);

            _gridRecentMovements = new GridControl
            {
                Location = new Point(620, 240),
                Size = new Size(580, 250)
            };
            var viewMovements = new GridView(_gridRecentMovements);
            _gridRecentMovements.MainView = viewMovements;
            viewMovements.OptionsView.ShowGroupPanel = false;
            viewMovements.OptionsBehavior.Editable = false;
            viewMovements.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.Controls.Add(_gridRecentMovements);

            // Stock by Category Chart
            var lblChart = new LabelControl
            {
                Text = "Stock Distribution by Category",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 510)
            };
            this.Controls.Add(lblChart);

            _chartStock = new ChartControl
            {
                Location = new Point(20, 540),
                Size = new Size(1180, 250)
            };
            this.Controls.Add(_chartStock);

            this.ResumeLayout(false);
        }

        private PanelControl CreateStatCard(string title, Color color, int x)
        {
            var card = new PanelControl
            {
                Location = new Point(x, 0),
                Size = new Size(270, 110),
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple,
                Appearance = { BackColor = Color.White }
            };

            // Color bar on left
            var colorBar = new PanelControl
            {
                Location = new Point(0, 0),
                Size = new Size(8, 110),
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder,
                Appearance = { BackColor = color }
            };
            card.Controls.Add(colorBar);

            // Title
            var lblTitle = new LabelControl
            {
                Text = title,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                Location = new Point(20, 15)
            };
            card.Controls.Add(lblTitle);

            // Value
            var lblValue = new LabelControl
            {
                Text = "Loading...",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(20, 45),
                AutoSizeMode = LabelAutoSizeMode.Default
            };
            card.Controls.Add(lblValue);

            return card;
        }

        private void LoadData()
        {
            try
            {
                // Load statistics
                var products = _productRepository.GetAll();
                var (totalValue, totalItems, lowStockCount) = _productRepository.GetStockSummary();
                var lowStockProducts = _productRepository.GetLowStock();
                var recentMovements = _movementRepository.GetAll(DateTime.Today.AddDays(-7), DateTime.Now);
                var todayMovements = recentMovements.Count(m => m.MovementDate.Date == DateTime.Today);

                // Update cards
                _lblTotalProducts.Text = products.Count.ToString("N0");
                _lblTotalValue.Text = $"${totalValue:N2}";
                _lblLowStock.Text = lowStockCount.ToString();
                _lblTodayMovements.Text = todayMovements.ToString();

                // Low stock grid
                _gridLowStock.DataSource = lowStockProducts.Take(10).Select(p => new
                {
                    p.Code,
                    p.Name,
                    p.CurrentStock,
                    p.MinimumStock,
                    Deficit = p.MinimumStock - p.CurrentStock
                }).ToList();

                // Recent movements grid
                _gridRecentMovements.DataSource = recentMovements.Take(10).Select(m => new
                {
                    m.DocumentNumber,
                    m.ProductName,
                    Type = m.Type.ToString(),
                    m.Quantity,
                    Date = m.MovementDate.ToString("yyyy-MM-dd HH:mm")
                }).ToList();

                // Chart - Stock by category
                LoadChart(products);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error loading dashboard: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadChart(List<Product> products)
        {
            _chartStock.Series.Clear();

            var series = new Series("Stock Value", ViewType.Pie);

            var categoryData = products
                .GroupBy(p => p.CategoryName ?? "Uncategorized")
                .Select(g => new
                {
                    Category = g.Key,
                    Value = g.Sum(p => p.StockValue)
                })
                .OrderByDescending(x => x.Value)
                .Take(10);

            foreach (var item in categoryData)
            {
                series.Points.Add(new SeriesPoint(item.Category, item.Value));
            }

            ((PieSeriesView)series.View).ExplodeMode = PieExplodeMode.UsePoints;
            ((PieSeriesLabel)series.Label).TextPattern = "{A}: {VP:P1}";

            _chartStock.Series.Add(series);

            var legend = _chartStock.Legend;
            legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
            legend.AlignmentHorizontal = LegendAlignmentHorizontal.Right;
        }

        public new void Refresh()
        {
            LoadData();
        }
    }
}
