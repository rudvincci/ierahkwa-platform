namespace Ierahkwa.JusticeService.Domain;

public class Inmate
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
    public string InmateNumber { get; set; } = string.Empty;
    public Guid SentenceId { get; set; }
    public string Facility { get; set; } = string.Empty;
    public DateTime IncarcerationDate { get; set; }
    public DateTime? ReleaseDate { get; set; }
}
