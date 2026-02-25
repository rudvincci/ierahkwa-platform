namespace Ierahkwa.CitizenService.Domain;

public class BirthCertificate
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
    // Domain-specific
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string PlaceOfBirth { get; set; } = string.Empty;
    public string MotherName { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
}
