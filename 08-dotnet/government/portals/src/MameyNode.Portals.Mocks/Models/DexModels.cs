namespace MameyNode.Portals.Mocks.Models;

// Models based on dex.proto
public enum AMMModel
{
    Unknown = 0,
    ConstantProduct = 1,
    WeightedPool = 2,
    StablePool = 3
}

public enum PoolStatus
{
    Unknown = 0,
    Active = 1,
    Paused = 2,
    Closed = 3,
    Migrating = 4
}

public class PoolInfo
{
    public string PoolId { get; set; } = string.Empty;
    public string TokenA { get; set; } = string.Empty;
    public string TokenB { get; set; } = string.Empty;
    public string ReserveA { get; set; } = string.Empty;
    public string ReserveB { get; set; } = string.Empty;
    public string TotalLpSupply { get; set; } = string.Empty;
    public AMMModel Model { get; set; }
    public string FeeRate { get; set; } = "0.003";
    public PoolStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TotalVolume24h { get; set; } = "0";
    public string TotalFees24h { get; set; } = "0";
}

public class SwapRoute
{
    public List<string> Pools { get; set; } = new();
    public List<string> Tokens { get; set; } = new();
    public string ExpectedOutput { get; set; } = string.Empty;
    public string PriceImpact { get; set; } = string.Empty;
}

