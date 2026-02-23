namespace Mamey.BlazorWasm.Routing;

public static class Extensions
{
    public static Task<List<Route>> FilterByRolesAndScopesAsync(this List<Route> routes, List<string> userRoles, List<string> userScopes)
        => Task.FromResult(routes.Where(route =>
            (route.RequiredRoles == null || route.RequiredRoles.Any(role => userRoles.Contains(role))) &&
            (route.RequiredScopes == null || route.RequiredScopes.Any(scope => userScopes.Contains(scope))))
        .ToList());
    public static Task<List<Route>> AnonymousRoutesAsync(this List<Route> routes)
        => Task.FromResult(routes.Where(route => route.AuthenticationRequired)
        .ToList());
}