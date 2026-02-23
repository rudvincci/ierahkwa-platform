using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Mamey.Auth.Identity;
using Mamey.MicroMonolith.Abstractions.Contexts;

namespace Mamey.MicroMonolith.Infrastructure.Contexts;

public class IdentityContext : IIdentityContext
{
    public bool IsAuthenticated { get; private set;}
    public Guid Id { get; private set; }
    public string? UserName { get; private set; }
    public string? Role { get; private set;}

    public Dictionary<string, IEnumerable<string>> Claims { get; private set; } 
        = new Dictionary<string, IEnumerable<string>>();

    private IdentityContext()
    {
    }

    public IdentityContext(Guid? id)
    {
        Id = id ?? Guid.Empty;
        IsAuthenticated = id.HasValue;
    }

    public IdentityContext(ClaimsPrincipal principal)
    {
        if (principal?.Identity is null || !principal.Identity.IsAuthenticated)
        {
            return;
        }
            
        IsAuthenticated = principal.Identity?.IsAuthenticated is true;
        UserName = IsAuthenticated ? principal.Identity?.Name : string.Empty;
        
        Role = principal.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        var claims = principal.Claims.GroupBy(x => x.Type)
            .ToDictionary(x => x.Key, x => x.Select(c => c.Value.ToString()));
        Claims = claims;
        claims.TryGetValue(ClaimTypes.NameIdentifier, out var sub);
        Guid.TryParse(sub?.FirstOrDefault(), out var userId);
        Id = userId;
    }
        
    public bool IsUser() => Role is ClaimValues.Role.User;
        
    public bool IsAdmin() => Role is ClaimValues.Role.Admin;
        
    public static IIdentityContext Empty => new IdentityContext();
}