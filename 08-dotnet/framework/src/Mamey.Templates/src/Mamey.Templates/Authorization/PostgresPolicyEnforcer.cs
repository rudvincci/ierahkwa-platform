using System.Security.Claims;
using Mamey.Templates.EF;
using Mamey.Templates.Registries;
using Mamey.Templates.Services;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Templates.Authorization;

public class PostgresPolicyEnforcer : ITemplatePolicyEnforcer
{
    private readonly TemplatesDbContext _db;
    private readonly MameyTemplatesOptions _opts;

    public PostgresPolicyEnforcer(TemplatesDbContext db, MameyTemplatesOptions opts)
    {
        _db = db;
        _opts = opts;
    }
    
    public async Task EnsureCanAccessAsync(TemplateId id, int version, TemplateAccessContext ctx, CancellationToken ct = default)
    {
        var rules = await _db.TemplatePolicies.AsNoTracking()
            .Where(p => p.TemplateId == id.Value && (p.Version == null || p.Version == version))
            .ToListAsync(ct);

        if (rules.Count == 0)
        {
            if (_opts.DefaultAllowWhenNoPolicy) return;
            throw new UnauthorizedAccessException($"No policy allows access to {id}@{version}.");
        }

        // explicit deny precedes allow
        if (rules.Any(r => r.Effect == "deny" && Match(r, ctx)))
            throw new UnauthorizedAccessException($"Access denied for {ctx.Service} to {id}@{version}.");

        if (rules.Any(r => r.Effect == "allow" && Match(r, ctx)))
            return;

        if (_opts.DefaultAllowWhenNoPolicy) return;

        throw new UnauthorizedAccessException($"Access denied (no allow) for {ctx.Service} to {id}@{version}.");
    }

    private static bool Match(TemplatePolicy r, TemplateAccessContext ctx) =>
        r.PrincipalType switch
        {
            "service" => string.Equals(r.Principal, ctx.Service, StringComparison.OrdinalIgnoreCase),
            "tenant"  => !string.IsNullOrWhiteSpace(ctx.TenantId) &&
                         string.Equals(r.Principal, ctx.TenantId, StringComparison.OrdinalIgnoreCase),
            "role"    => ctx.Claims?.Any(c => c.Type == ClaimTypes.Role && c.Value == r.Principal) == true,
            "claim"   => ctx.Claims?.Any(c => c.Type == r.Principal && (r.ClaimValue == null || r.ClaimValue == c.Value)) == true,
            _ => false
        };
}