namespace AppBuilder.Core.Models;

/// <summary>User - IERAHKWA Appy. SaaS customer. GDPR: export, delete.</summary>
public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? Name { get; set; }
    public string? AvatarUrl { get; set; }

    /// <summary>Social login: google, facebook, github</summary>
    public string? SocialProvider { get; set; }
    public string? SocialId { get; set; }

    public PlanTier PlanTier { get; set; } = PlanTier.Free;
    public int BuildCredits { get; set; } = 5; // Free: 5/mo; Pro/Enterprise per plan

    public bool EmailVerified { get; set; }
    public bool AcceptsMarketing { get; set; }
    public bool CookieConsent { get; set; }
    public string? PreferredLocale { get; set; } = "en";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
}

public enum PlanTier
{
    Free,
    Pro,
    Enterprise
}
