using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;
using Mamey.Portal.Shared.Auth;
using Mamey.Portal.Shared.Tenancy;

namespace Mamey.Portal.Web.Middleware;

/// <summary>
/// Middleware that redirects users to application status page if they try to access
/// citizen portal but don't have an approved citizenship status (Probationary, Resident, or Citizen).
/// </summary>
public sealed class CitizenApprovalMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly string[] ValidCitizenshipStatuses = { "Probationary", "Resident", "Citizen" };

    public CitizenApprovalMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentUserContext user,
        ITenantContext tenant,
        CitizenshipDbContext db)
    {
        // Only check for /citizen/* routes
        if (!context.Request.Path.StartsWithSegments("/citizen", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Skip if user is not authenticated
        if (!user.IsAuthenticated)
        {
            await _next(context);
            return;
        }

        // Skip if user is an Admin or Agent (they can access citizen portal for testing)
        if (user.IsInRole("Admin") || user.IsInRole("GovernmentAgent") || user.IsInRole("Agent"))
        {
            await _next(context);
            return;
        }

        // Check if user has a valid citizenship status (Probationary, Resident, or Citizen)
        var email = user.UserName?.Trim();
        if (string.IsNullOrWhiteSpace(email))
        {
            // No email - redirect to application status
            context.Response.Redirect("/application-status?error=no_email");
            return;
        }

        var tenantId = tenant.TenantId;
        var hasCitizenshipStatus = await db.CitizenshipStatuses
            .AsNoTracking()
            .Where(cs => cs.TenantId == tenantId && cs.Email == email)
            .AnyAsync(cs => ValidCitizenshipStatuses.Contains(cs.Status), context.RequestAborted);

        if (!hasCitizenshipStatus)
        {
            // No citizenship status - redirect to application status page
            context.Response.Redirect($"/application-status?redirected=true&email={Uri.EscapeDataString(email)}");
            return;
        }

        // User has valid citizenship status - allow access
        await _next(context);
    }
}


