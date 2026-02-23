using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Mamey.Microservice.Abstractions.Services;

namespace Mamey.Microservice.Infrastructure.Services;

internal class UserDataService : IUserDataService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserDataService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
    {
        var userClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userClaim?.Value is null)
        {
            return Guid.Empty;
        }
        return new Guid(userClaim.Value);
    }
}

