using Microsoft.AspNetCore.Components;

namespace Mamey.ApplicationName.BlazorWasm.Components.Pages.Secure;

public partial class Index: ComponentBase
{
    private List<DashboardCard> DashboardCards;

    protected override void OnInitialized()
    {
        // Add mock data here
        DashboardCards = new List<DashboardCard>
        {
            new DashboardCard { Title = "Sales", Description = "Monthly sales data", BackgroundColor = "#5c636a" },
            new DashboardCard { Title = "Orders", Description = "New orders this week", BackgroundColor = "#5c736a" },
            new DashboardCard { Title = "Revenue", Description = "Revenue for the quarter", BackgroundColor = "#5a636a" },
            new DashboardCard { Title = "Customers", Description = "Total active customers", BackgroundColor = "#5d636a" },
            new DashboardCard { Title = "Products", Description = "Total products in stock", BackgroundColor = "#5c636a" },
            new DashboardCard { Title = "Expenses", Description = "Monthly expenses", BackgroundColor = "#5c636b" },
            new DashboardCard { Title = "Growth", Description = "Annual growth rate", BackgroundColor = "#5c63ca" },
            new DashboardCard { Title = "Profits", Description = "Net profits", BackgroundColor = "#5c6a6a" },
            new DashboardCard { Title = "Feedback", Description = "Customer feedback", BackgroundColor = "#5c63ba" }
        };
    }
}
public class DashboardCard
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string BackgroundColor { get; set; }
}