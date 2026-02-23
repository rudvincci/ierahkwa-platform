using Mamey.BlazorWasm.Routing;

namespace Mamey.FWID.Identities.BlazorWasm.Services;

/// <summary>
/// Route service for Identity pages.
/// Implements IRouteService for navigation discovery.
/// </summary>
public class IdentityRouteService : IRouteService
{
    private readonly List<Route> _routes = new()
    {
        // Citizen Routes
        new Route 
        { 
            Page = "/identity", 
            Title = "Dashboard", 
            Icon = "PersonOutline", 
            RequiredRoles = new List<string>(),
            AuthenticationRequired = false  // Disabled for UI development
        },
        new Route 
        { 
            Page = "/identity/profile", 
            Title = "My Profile", 
            Icon = "AccountCircle", 
            RequiredRoles = new List<string>(),
            AuthenticationRequired = false  // Disabled for UI development
        },
        new Route 
        { 
            Page = "/identity/biometrics", 
            Title = "Biometrics", 
            Icon = "Fingerprint", 
            RequiredRoles = new List<string>(),
            AuthenticationRequired = false  // Disabled for UI development
        },
        new Route 
        { 
            Page = "/identity/verification", 
            Title = "Verification", 
            Icon = "VerifiedUser", 
            RequiredRoles = new List<string>(),
            AuthenticationRequired = false  // Disabled for UI development
        },
        new Route 
        { 
            Page = "/identity/mfa", 
            Title = "Multi-Factor Auth", 
            Icon = "Security", 
            RequiredRoles = new List<string>(),
            AuthenticationRequired = false  // Disabled for UI development
        },

        // Admin Routes
        new Route 
        { 
            Page = "/identity/admin", 
            Title = "Identity Management", 
            Icon = "SupervisorAccount", 
            RequiredRoles = new List<string>(),
            AuthenticationRequired = false  // Disabled for UI development
        },
        new Route 
        { 
            Page = "/identity/admin/biometrics", 
            Title = "Biometric Review", 
            Icon = "Visibility", 
            RequiredRoles = new List<string>(),
            AuthenticationRequired = false  // Disabled for UI development
        },
        new Route 
        { 
            Page = "/identity/admin/verification", 
            Title = "Verification Queue", 
            Icon = "FactCheck", 
            RequiredRoles = new List<string>(),
            AuthenticationRequired = false  // Disabled for UI development
        }
    };

    public List<Route> Routes => _routes;

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        RoutesChanged?.Invoke(_routes);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets routes filtered by role.
    /// </summary>
    public IEnumerable<Route> GetRoutesByRole(string role) =>
        _routes.Where(r => !r.RequiredRoles.Any() || r.RequiredRoles.Contains(role));
}
