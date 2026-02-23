using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Mamey.Identity.EntityFramework.Entities;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? FullName { get; set; }

    public Guid? TenantId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? ProfilePictureUrl { get; set; }

    [MaxLength(50)]
    public string? TimeZone { get; set; }

    [MaxLength(10)]
    public string? Locale { get; set; }

    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

    public virtual ICollection<ApplicationUserClaim> UserClaims { get; set; } = new List<ApplicationUserClaim>();

    public virtual ICollection<ApplicationUserLogin> UserLogins { get; set; } = new List<ApplicationUserLogin>();

    public virtual ICollection<ApplicationUserToken> UserTokens { get; set; } = new List<ApplicationUserToken>();
}

