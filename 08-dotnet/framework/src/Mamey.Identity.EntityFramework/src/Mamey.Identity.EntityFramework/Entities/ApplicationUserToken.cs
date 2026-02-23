using Microsoft.AspNetCore.Identity;

namespace Mamey.Identity.EntityFramework.Entities;

public class ApplicationUserToken : IdentityUserToken<string>
{
    public virtual ApplicationUser User { get; set; } = null!;
}

