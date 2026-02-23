namespace Mamey.FWID.Notifications.Application.Templates.Models;

/// <summary>
/// DTO for approval request email template.
/// </summary>
public class ApprovalRequestDto : IEmailTemplate
{
    public string ApprovalId { get; set; } = null!;
    public string ApprovalType { get; set; } = null!;
    public string RequesterName { get; set; } = null!;
    public string RequestDescription { get; set; } = null!;
    public DateTime RequestedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string ApprovalUrl { get; set; } = null!;
}

/// <summary>
/// DTO for credential expiring email template.
/// </summary>
public class CredentialExpiringDto : IEmailTemplate
{
    public string CredentialType { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public int DaysRemaining { get; set; }
    public string RenewalUrl { get; set; } = null!;
}

/// <summary>
/// DTO for security alert email template.
/// </summary>
public class SecurityAlertDto : IEmailTemplate
{
    public string AlertType { get; set; } = null!;
    public string AlertLevel { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime OccurredAt { get; set; }
    public string? IpAddress { get; set; }
    public string? Location { get; set; }
    public string? ActionRequired { get; set; }
}

/// <summary>
/// DTO for registration completed email template.
/// </summary>
public class RegistrationCompletedDto : IEmailTemplate
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string DID { get; set; } = null!;
    public string Zone { get; set; } = null!;
    public DateTime CompletedAt { get; set; }
    public string PortalUrl { get; set; } = null!;
}
