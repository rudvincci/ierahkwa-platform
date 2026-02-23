using Microsoft.AspNetCore.Components.Routing;

namespace Mamey.BlazorWasm.Routing;

public class Route
{
    public string Page { get; set; }
    public string Icon { get; set; }
    public string Title { get; set; }
    public bool AuthenticationRequired { get; set; } = false;
    public NavLinkMatch Match { get; set; }
    public List<Route> ChildRoutes { get; set; } = new List<Route>();// Enumerable.Empty<Route>();
    public List<string> RequiredRoles { get; set; } = new List<string>();
    public List<string> RequiredScopes { get; set; } = new List<string>();
    
}
