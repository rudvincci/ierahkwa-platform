using Microsoft.AspNetCore.Identity;

namespace Mamey.Identity.EntityFramework.Entities;

public class ApplicationUserClaim : IdentityUserClaim<string>
{
    public virtual ApplicationUser User { get; set; } = null!;
}

