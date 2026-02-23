using System.Security.Claims;
using Common.Application.Interfaces;

namespace SmartSchool.Web.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public string? UserName => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;

    public string? Email => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

    public int? TenantId
    {
        get
        {
            var tenantIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("TenantId")?.Value;
            return int.TryParse(tenantIdClaim, out var tenantId) ? tenantId : null;
        }
    }

    public IEnumerable<string> Roles => 
        _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? Enumerable.Empty<string>();

    public bool IsAuthenticated => 
        _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role) => 
        _httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
}
