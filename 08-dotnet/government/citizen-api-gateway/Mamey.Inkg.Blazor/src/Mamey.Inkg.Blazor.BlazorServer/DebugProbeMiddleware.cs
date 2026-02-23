using System.Diagnostics;
using System.Security.Claims;

namespace Mamey.Inkg.Blazor.BlazorServer;

public sealed class DebugProbeMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<DebugProbeMiddleware>>();
        var sw = Stopwatch.StartNew();

        // Capture request basics
        var method = context.Request.Method;
        var path   = context.Request.Path.Value ?? "";
        var user   = context.User;

        // Let the pipeline run
        await next(context);

        sw.Stop();

        // Endpoint info (after next so routing has matched)
        var ep = context.GetEndpoint();
        var epName = ep?.DisplayName ?? "(no endpoint)";
        var routeValues = context.Request.RouteValues.Count > 0
            ? string.Join(", ", context.Request.RouteValues.Select(kvp => $"{kvp.Key}={kvp.Value}"))
            : "(none)";

        // Auth snapshot
        var isAuth = user?.Identity?.IsAuthenticated == true;
        var authTypes = user?.Identities is null ? "(none)" :
            string.Join(",", user.Identities.Select(i => i.AuthenticationType ?? "(null)"));

        var name = user?.Identity?.Name ?? user?.FindFirst("name")?.Value
            ?? user?.FindFirst(ClaimTypes.Name)?.Value ?? "(anon)";

        var roles = user?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .Distinct()
            .Take(5); // avoid huge dumps

        // Status code last
        var status = context.Response?.StatusCode;

        logger.LogInformation("[DBG] {Method} {Path} => {Status} in {Elapsed} ms | EP: {Endpoint} | Routes: {Routes} | Auth: {Auth} ({AuthTypes}) Name: {Name} Roles: {Roles}",
            method, path, status, sw.ElapsedMilliseconds, epName, routeValues,
            isAuth, authTypes, name, roles is null ? "(none)" : string.Join(",", roles));

        // if (DumpAllClaims && isAuth)
        // {
        //     var claims = user!.Claims.Select(c => new { c.Type, c.Value }).ToArray();
        //     logger.LogDebug("[DBG:CLAIMS] {Claims}", System.Text.Json.JsonSerializer.Serialize(claims));
        // }
    }
}

