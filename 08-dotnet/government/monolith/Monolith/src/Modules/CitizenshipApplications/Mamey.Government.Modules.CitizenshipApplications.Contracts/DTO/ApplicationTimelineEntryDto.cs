namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public class ApplicationTimelineEntryDto
{
    public string Event { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Actor { get; set; }
}
