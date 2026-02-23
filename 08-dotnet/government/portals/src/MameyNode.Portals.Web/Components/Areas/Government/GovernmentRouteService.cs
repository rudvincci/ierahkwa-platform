using Mamey.BlazorWasm.Routing;
using Route = Mamey.BlazorWasm.Routing.Route;

namespace MameyNode.Portals.Web.Components.Areas.Government;

public class GovernmentRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();
    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/admin",
                Title = "Government Services",
                Icon = "fas fa-building",
                AuthenticationRequired = false,
                RequiredRoles = new List<string>(),
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/admin/citizenship", Title = "Citizenship Applications", Icon = "fas fa-id-card" },
                    new Route { Page = "/admin/citizens", Title = "Citizen Management", Icon = "fas fa-users" },
                    new Route { Page = "/admin/passports", Title = "Passports", Icon = "fas fa-passport" },
                    new Route { Page = "/admin/travel-identity", Title = "Travel Identity", Icon = "fas fa-plane" },
                    new Route { Page = "/admin/diplomats", Title = "Diplomats", Icon = "fas fa-user-tie" },
                    new Route { Page = "/admin/payment-plans", Title = "Payment Plans", Icon = "fas fa-calendar-check" },
                    new Route { Page = "/admin/nodes", Title = "Node Management", Icon = "fas fa-server" },
                    new Route { Page = "/admin/compliance", Title = "Compliance", Icon = "fas fa-gavel" },
                    new Route { Page = "/admin/identity", Title = "Identity Management", Icon = "fas fa-id-badge" },
                    new Route { Page = "/citizen", Title = "Citizen Portal", Icon = "fas fa-user-circle" },
                    // Compliance - Part of Government Services
                    new Route { Page = "/compliance", Title = "Compliance", Icon = "fas fa-shield-alt" },
                    new Route { Page = "/compliance/kyc", Title = "KYC", Icon = "fas fa-user-check" },
                    new Route { Page = "/compliance/aml", Title = "AML/CFT", Icon = "fas fa-exclamation-triangle" },
                    new Route { Page = "/compliance/fraud", Title = "Fraud Detection", Icon = "fas fa-user-secret" },
                    new Route { Page = "/compliance/sanctions", Title = "Sanctions Screening", Icon = "fas fa-ban" },
                    new Route { Page = "/compliance/monitoring", Title = "Transaction Monitoring", Icon = "fas fa-eye" },
                    new Route { Page = "/compliance/reporting", Title = "Regulatory Reporting", Icon = "fas fa-file-alt" },
                    new Route { Page = "/compliance/privacy", Title = "Data Privacy", Icon = "fas fa-lock" },
                    new Route { Page = "/compliance/surveillance", Title = "Market Surveillance", Icon = "fas fa-binoculars" },
                    new Route { Page = "/compliance/whitelist", Title = "Whitelist/Blacklist", Icon = "fas fa-list" },
                    new Route { Page = "/compliance/enforcement", Title = "Enforcement", Icon = "fas fa-gavel" },
                    new Route { Page = "/compliance/limits", Title = "Limits", Icon = "fas fa-sliders-h" },
                    new Route { Page = "/compliance/audit", Title = "Enhanced Audit", Icon = "fas fa-clipboard-check" },
                    new Route { Page = "/compliance/zkp", Title = "ZKP Compliance", Icon = "fas fa-key" },
                    new Route { Page = "/compliance/cdd", Title = "Customer Due Diligence", Icon = "fas fa-user-shield" },
                    // RBAC - Part of Government Services
                    new Route { Page = "/rbac", Title = "Role-Based Access Control", Icon = "fas fa-user-lock" },
                    new Route { Page = "/rbac/roles", Title = "Role Management", Icon = "fas fa-users-cog" },
                    new Route { Page = "/rbac/permissions", Title = "Permission Management", Icon = "fas fa-key" },
                    new Route { Page = "/rbac/hierarchy", Title = "Role Hierarchy", Icon = "fas fa-sitemap" },
                    new Route { Page = "/rbac/guard", Title = "Access Guard", Icon = "fas fa-shield-alt" }
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}

