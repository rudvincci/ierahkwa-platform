using Microsoft.AspNetCore.Identity;

namespace Mamey.Identity.EntityFramework.Entities;

public class ApplicationUserLogin : IdentityUserLogin<string>
{
    public virtual ApplicationUser User { get; set; } = null!;
}

