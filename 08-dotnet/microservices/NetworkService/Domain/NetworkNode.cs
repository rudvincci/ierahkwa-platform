namespace Ierahkwa.NetworkService.Domain;

public class NetworkNode
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
    public string NodeType { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public bool IsOnline { get; set; } = true;
}
