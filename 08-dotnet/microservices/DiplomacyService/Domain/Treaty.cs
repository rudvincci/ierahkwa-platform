namespace Ierahkwa.DiplomacyService.Domain;

public class Treaty
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
    public string TreatyType { get; set; } = string.Empty;
    public string SignatoryCountries { get; set; } = string.Empty;
    public DateTime SignedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsRatified { get; set; }
}
