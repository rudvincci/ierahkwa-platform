using AdvocateOffice.Services;
using System.Text.Json;

namespace AdvocateOffice.Middleware;

public class TokenAuthMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly string[] Allowed = { "/api/auth/login", "/api/auth/check" };

    public TokenAuthMiddleware(RequestDelegate next) => _next = next;

    public async System.Threading.Tasks.Task InvokeAsync(HttpContext ctx)
    {
        var path = ctx.Request.Path.Value ?? "";
        if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) ||
            Allowed.Any(a => path.StartsWith(a, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(ctx);
            return;
        }

        var auth = ctx.Request.Headers.Authorization.FirstOrDefault();
        var token = auth?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();
        if (TokenStore.Validate(token, out var uid))
        {
            ctx.Items["UserId"] = uid;
            await _next(ctx);
            return;
        }

        ctx.Response.StatusCode = 401;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Unauthorized", authenticated = false }));
    }
}
