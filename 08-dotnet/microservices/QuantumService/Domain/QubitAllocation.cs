namespace Ierahkwa.QuantumService.Domain;

public class QubitAllocation
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
    public Guid CircuitId { get; set; }
    public int PhysicalQubits { get; set; }
    public int LogicalQubits { get; set; }
    public double ErrorRate { get; set; }
}
