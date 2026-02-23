using DevExpress.XtraEditors;
using DevExpress.XtraCharts;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using InventoryManager.Data;
using InventoryManager.Models;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Statistics and analytics control with charts
    /// </summary>
    public class StatisticsControl : XtraUserControl, IRefreshable
    {
        private readonly User _currentUser;
        private readonly ProductRepository _productRepository = new();
        private readonly StockMovementRepository _movementRepository = new();
        private readonly CategoryRepository _categoryRepository = new();

        private ChartControl _chartCategory = null!;
        private ChartControl _chartMovements = null!;
        private ChartControl _chartTopProducts = null!;
        private GridControl _gridStats = null!;

        public StatisticsControl(User currentUser)
        {
            _currentUser = currentUser;
            InitializeComponents();
            LoadStatistics();
        }

        private void InitializeComponents()
        {
            this.SuspendLayout();

            // Title
            var titleLabel = new LabelControl
            {
                Text = "Inventory Statistics & Analytics",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 20)
            };
            this.Controls.Add(titleLabel);

            // Category pie chart
            var lblCategory = new LabelControl
            {
                Text = "Stock Value by Category",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 60)
            };
            this.Controls.Add(lblCategory);

            _chartCategory = new ChartControl
            {
                Location = new Point(20, 85),
                Size = new Size(400, 300)
            };
            this.Controls.Add(_chartCategory);

            // Movements bar chart
            var lblMovements = new LabelControl
            {
                Text = "Monthly Movements",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(440, 60)
            };
            this.Controls.Add(lblMovements);

            _chartMovements = new ChartControl
            {
                Location = new Point(440, 85),
                Size = new Size(500, 300)
            };
            this.Controls.Add(_chartMovements);

            // Top products chart
            var lblTop = new LabelControl
            {
                Text = "Top 10 Products by Value",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 400)
            };
            this.Controls.Add(lblTop);

            _chartTopProducts = new ChartControl
            {
                Location = new Point(20, 425),
                Size = new Size(920, 250)
            };
            this.Controls.Add(_chartTopProducts);

            this.ResumeLayout(false);
        }

        private void LoadStatistics()
        {
            try
            {
                LoadCategoryChart();
                LoadMovementsChart();
                LoadTopProductsChart();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error loading statistics: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCategoryChart()
        {
            _chartCategory.Series.Clear();

            var products = _productRepository.GetAll();
            var categoryData = products
                .GroupBy(p => p.CategoryName ?? "Uncategorized")
                .Select(g => new
                {
                    Category = g.Key,
                    Value = g.Sum(p => p.StockValue),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Value)
                .Take(8)
                .ToList();

            var series = new Series("Value", ViewType.Pie3D);

            foreach (var item in categoryData)
            {
                series.Points.Add(new SeriesPoint(item.Category, (double)item.Value));
            }

            var pieView = series.View as Pie3DSeriesView;
            if (pieView != null)
            {
                pieView.ExplodeMode = PieExplodeMode.UsePoints;
            }

            var pieLabel = series.Label as PieSeriesLabel;
            if (pieLabel != null)
            {
                pieLabel.TextPattern = "{A}: {VP:P1}";
                pieLabel.Position = PieSeriesLabelPosition.TwoColumns;
            }

            _chartCategory.Series.Add(series);
            _chartCategory.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
        }

        private void LoadMovementsChart()
        {
            _chartMovements.Series.Clear();

            // Get last 6 months data
            var endDate = DateTime.Today;
            var startDate = endDate.AddMonths(-6);

            var movements = _movementRepository.GetAll(startDate, endDate);

            var monthlyData = movements
                .GroupBy(m => new { m.MovementDate.Year, m.MovementDate.Month })
                .Select(g => new
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Purchases = g.Where(m => m.Type == MovementType.Purchase).Sum(m => m.TotalValue),
                    Sales = g.Where(m => m.Type == MovementType.Sale).Sum(m => m.TotalValue)
                })
                .OrderBy(x => x.Month)
                .ToList();

            var purchaseSeries = new Series("Purchases", ViewType.Bar);
            var salesSeries = new Series("Sales", ViewType.Bar);

            foreach (var item in monthlyData)
            {
                purchaseSeries.Points.Add(new SeriesPoint(item.Month, (double)item.Purchases));
                salesSeries.Points.Add(new SeriesPoint(item.Month, (double)item.Sales));
            }

            purchaseSeries.View.Color = Color.FromArgb(76, 175, 80);
            salesSeries.View.Color = Color.FromArgb(244, 67, 54);

            _chartMovements.Series.Add(purchaseSeries);
            _chartMovements.Series.Add(salesSeries);

            _chartMovements.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;

            var diagram = _chartMovements.Diagram as XYDiagram;
            if (diagram != null)
            {
                diagram.AxisY.Title.Text = "Value ($)";
                diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            }
        }

        private void LoadTopProductsChart()
        {
            _chartTopProducts.Series.Clear();

            var products = _productRepository.GetAll()
                .OrderByDescending(p => p.StockValue)
                .Take(10)
                .ToList();

            var series = new Series("Stock Value", ViewType.Bar);

            foreach (var product in products)
            {
                series.Points.Add(new SeriesPoint(product.Name, (double)product.StockValue));
            }

            var barView = series.View as BarSeriesView;
            if (barView != null)
            {
                barView.ColorEach = true;
            }

            series.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            var seriesLabel = series.Label;
            seriesLabel.TextPattern = "${V:N0}";

            _chartTopProducts.Series.Add(series);
            _chartTopProducts.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;

            var diagram = _chartTopProducts.Diagram as XYDiagram;
            if (diagram != null)
            {
                diagram.Rotated = true;
                diagram.AxisY.Title.Text = "Stock Value ($)";
                diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            }
        }

        public new void Refresh() => LoadStatistics();
    }
}
