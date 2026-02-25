namespace Ierahkwa.MilitaryService.Domain;

public class LogisticsOrder
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
    public string OrderNumber { get; set; } = string.Empty;
    public string SupplyType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string DestinationBase { get; set; } = string.Empty;
    public string Priority { get; set; } = "Normal";
}
