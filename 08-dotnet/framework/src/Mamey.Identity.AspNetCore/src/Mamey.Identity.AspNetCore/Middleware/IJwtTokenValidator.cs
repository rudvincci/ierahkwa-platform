using System.Security.Claims;

namespace Mamey.Identity.AspNetCore.Middleware;

public interface IJwtTokenValidator
{
    Task<ClaimsPrincipal> ValidateAsync(string token);
}

