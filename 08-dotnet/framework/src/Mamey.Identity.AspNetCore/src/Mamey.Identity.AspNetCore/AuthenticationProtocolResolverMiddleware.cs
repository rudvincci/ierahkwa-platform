using Microsoft.AspNetCore.Http;

namespace Mamey.Identity.AspNetCore;

public class AuthenticationProtocolResolverMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationProtocolResolverMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add any authentication protocol resolution logic here
        await _next(context);
    }
}
