using Microsoft.AspNetCore.Identity;

namespace Mamey.Identity.EntityFramework.Entities;

public class ApplicationRoleClaim : IdentityRoleClaim<string>
{
    public virtual ApplicationRole Role { get; set; } = null!;
}

