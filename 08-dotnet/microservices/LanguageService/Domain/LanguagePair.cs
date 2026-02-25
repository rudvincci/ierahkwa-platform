namespace Ierahkwa.LanguageService.Domain;

public class LanguagePair
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
    public string LanguageA { get; set; } = string.Empty;
    public string LanguageB { get; set; } = string.Empty;
    public int DictionarySize { get; set; }
    public double AccuracyScore { get; set; }
}
