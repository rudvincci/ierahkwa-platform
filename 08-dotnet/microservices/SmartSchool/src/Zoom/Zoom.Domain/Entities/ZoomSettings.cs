using Common.Domain.Entities;

namespace Zoom.Domain.Entities;

public class ZoomSettings : TenantEntity
{
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string? AccountId { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? WebhookSecret { get; set; }
    public bool IsActive { get; set; } = true;
    public bool AutoRecord { get; set; } = false;
    public bool EnableWaitingRoom { get; set; } = true;
    public bool MuteParticipantsOnEntry { get; set; } = true;
}
