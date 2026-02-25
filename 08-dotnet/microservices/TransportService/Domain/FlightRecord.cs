namespace Ierahkwa.TransportService.Domain;

public class FlightRecord
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
    public string FlightNumber { get; set; } = string.Empty;
    public string AircraftType { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public string AirportCode { get; set; } = string.Empty;
}
