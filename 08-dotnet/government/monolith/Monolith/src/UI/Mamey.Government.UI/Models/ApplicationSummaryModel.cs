namespace Mamey.Government.UI.Models;

/// <summary>
/// Application summary for list views.
/// </summary>
public class ApplicationSummaryModel
{
    public Guid Id { get; set; }
    public string ApplicationNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ApplicantName => $"{FirstName} {LastName}";
    public string? Email { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Step { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}