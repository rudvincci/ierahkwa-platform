namespace Ierahkwa.IoTRoboticsService.Domain;

public class NeuralInterface
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
    public string InterfaceType { get; set; } = string.Empty;
    public int ChannelCount { get; set; }
    public double SamplingRateHz { get; set; }
    public string CalibrationState { get; set; } = string.Empty;
}
