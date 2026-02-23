using System.Security.Claims;

namespace Mamey.Auth.Identity.Middleware;

public interface IJwtTokenValidator
{
    Task<ClaimsPrincipal> ValidateAsync(string token);
}