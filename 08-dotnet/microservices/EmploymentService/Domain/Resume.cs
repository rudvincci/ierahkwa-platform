namespace Ierahkwa.EmploymentService.Domain;

public class Resume
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string CreatedBy { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public Guid ApplicantId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
}
