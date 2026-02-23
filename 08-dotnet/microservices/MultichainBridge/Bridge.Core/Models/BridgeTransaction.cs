namespace Bridge.Core.Models;

public class BridgeTransaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string SourceChain { get; set; } = string.Empty;
    public string DestinationChain { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public BridgeStatus Status { get; set; } = BridgeStatus.Pending;
    public string SourceTxHash { get; set; } = string.Empty;
    public string? DestinationTxHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}

public enum BridgeStatus { Pending, Confirming, Bridging, Completed, Failed }

public class SupportedChain
{
    public int ChainId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int Confirmations { get; set; }
}

public class BridgeStats
{
    public decimal TotalVolume { get; set; }
    public int TotalTransactions { get; set; }
    public int ActiveChains { get; set; }
    public List<string> SupportedTokens { get; set; } = new();
}
