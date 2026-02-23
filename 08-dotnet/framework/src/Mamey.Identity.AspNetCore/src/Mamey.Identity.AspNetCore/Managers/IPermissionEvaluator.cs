using System.Security.Claims;

namespace Mamey.Identity.AspNetCore.Managers;

/// <summary>
/// Determines if a principal has a given permission.
/// </summary>
public interface IPermissionEvaluator
{
    Task<bool> HasPermissionAsync(
        ClaimsPrincipal user,
        string permission,
        CancellationToken ct = default);
}

