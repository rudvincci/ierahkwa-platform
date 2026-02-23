using SpikeOffice.Core.Enums;

namespace SpikeOffice.Core.Entities;

/// <summary>Online & offline gateways. Offline = Cash, Bank, etc.</summary>
public class PaymentGateway : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public PaymentGatewayType Type { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ConfigJson { get; set; } // API keys, account numbers (encrypted)
    public bool IsOffline { get; set; }
}
