namespace Ierahkwa.DiplomacyService.Domain;

public class CustomsDeclaration
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
    public string DeclarationNumber { get; set; } = string.Empty;
    public string GoodsDescription { get; set; } = string.Empty;
    public decimal DeclaredValue { get; set; }
    public string OriginCountry { get; set; } = string.Empty;
    public decimal DutyAmount { get; set; }
}
