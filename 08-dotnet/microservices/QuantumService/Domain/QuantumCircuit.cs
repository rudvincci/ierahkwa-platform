namespace Ierahkwa.QuantumService.Domain;

public class QuantumCircuit
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
    public int QubitCount { get; set; }
    public int GateDepth { get; set; }
    public string CircuitDefinition { get; set; } = string.Empty;
    public string Backend { get; set; } = string.Empty;
}
