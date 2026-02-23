namespace Common.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Logo { get; set; }
    public string? Website { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal TaxRate { get; set; } = 0;
    public decimal OpeningCash { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public string? ConnectionString { get; set; }
    
    // Settings
    public string DefaultLanguage { get; set; } = "en";
    public string TimeZone { get; set; } = "UTC";
    public string DateFormat { get; set; } = "yyyy-MM-dd";
}
