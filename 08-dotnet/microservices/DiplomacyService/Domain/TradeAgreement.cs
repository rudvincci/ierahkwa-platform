namespace Ierahkwa.DiplomacyService.Domain;

public class TradeAgreement
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
    public string AgreementType { get; set; } = string.Empty;
    public string PartnerCountry { get; set; } = string.Empty;
    public decimal TradeVolume { get; set; }
    public decimal TariffRate { get; set; }
    public DateTime EffectiveDate { get; set; }
}
