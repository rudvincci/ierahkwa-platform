namespace NET10.Core.Models;

/// <summary>
/// Admin configuration for the DeFi platform
/// </summary>
public class NET10Config
{
    // General
    public string PlatformName { get; set; } = "Ierahkwa NET10 DeFi";
    public string LogoUrl { get; set; } = "/assets/logo.png";
    public string Theme { get; set; } = "dark"; // dark, light, both
    public string PrimaryColor { get; set; } = "#FFD700"; // Gold
    public string SecondaryColor { get; set; } = "#00FF41"; // Neon Green
    
    // Network
    public string DefaultChainId { get; set; } = "777777"; // Ierahkwa Sovereign Blockchain
    public string[] SupportedChains { get; set; } = new[] { "777777", "1", "56", "137", "43114" };
    public string NodeEndpoint { get; set; } = "https://node.ierahkwa.gov";
    
    // Fees
    public decimal DefaultSwapFee { get; set; } = 0.003m; // 0.3%
    public decimal AdminFeePercent { get; set; } = 0.1m; // 0.1% admin fee
    public decimal LPFeePercent { get; set; } = 0.2m; // 0.2% to LPs
    public string FeeRecipient { get; set; } = "0x...AdminWallet";
    
    // Slippage
    public decimal DefaultSlippage { get; set; } = 0.5m;
    public decimal MaxSlippage { get; set; } = 50m;
    
    // Trading
    public decimal MinTradeAmount { get; set; } = 0.0001m;
    public decimal MaxTradeAmount { get; set; } = 1000000m;
    public int TransactionDeadlineMinutes { get; set; } = 20;
    
    // Liquidity
    public decimal MinLiquidityAmount { get; set; } = 100m;
    public bool RequireLiquidityApproval { get; set; } = false;
    
    // Farming
    public bool FarmingEnabled { get; set; } = true;
    public decimal MaxFarmAPR { get; set; } = 500m;
    
    // Features
    public bool SwapEnabled { get; set; } = true;
    public bool LiquidityEnabled { get; set; } = true;
    public bool ChartsEnabled { get; set; } = true;
    public bool AnalyticsEnabled { get; set; } = true;
    
    // Smart Contracts
    public string RouterAddress { get; set; } = "0x...Router";
    public string FactoryAddress { get; set; } = "0x...Factory";
    public string MasterChefAddress { get; set; } = "0x...MasterChef";
    
    // Fiat On-Ramp
    public bool FiatOnRampEnabled { get; set; } = false;
    public string FiatProvider { get; set; } = "transak";
    public string FiatApiKey { get; set; } = string.Empty;
}

/// <summary>
/// Admin statistics dashboard
/// </summary>
public class AdminStats
{
    // Volume
    public decimal TotalVolume { get; set; }
    public decimal Volume24h { get; set; }
    public decimal Volume7d { get; set; }
    public decimal Volume30d { get; set; }
    
    // TVL
    public decimal TotalTVL { get; set; }
    public decimal PoolsTVL { get; set; }
    public decimal FarmsTVL { get; set; }
    
    // Fees
    public decimal TotalFees { get; set; }
    public decimal AdminFees { get; set; }
    public decimal LPFees { get; set; }
    public decimal Fees24h { get; set; }
    
    // Counts
    public int TotalSwaps { get; set; }
    public int Swaps24h { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveUsers24h { get; set; }
    public int TotalPools { get; set; }
    public int TotalFarms { get; set; }
    public int TotalTokens { get; set; }
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Smart contract deployment request
/// </summary>
public class ContractDeployRequest
{
    public string ContractType { get; set; } = string.Empty; // Router, Factory, Pair, Farm
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string ChainId { get; set; } = "777777";
    public string DeployerAddress { get; set; } = string.Empty;
}

/// <summary>
/// Deployed contract info
/// </summary>
public class DeployedContract
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ContractType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string TxHash { get; set; } = string.Empty;
    public string ChainId { get; set; } = string.Empty;
    public string ABI { get; set; } = string.Empty;
    public string Bytecode { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime DeployedAt { get; set; } = DateTime.UtcNow;
}
