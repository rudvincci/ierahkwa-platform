namespace IDOFactory.Core.Models;

/// <summary>
/// Platform configuration for the IDO Factory
/// Admin can edit: Logo, title, colors, social links, admin addresses
/// </summary>
public class PlatformConfig
{
    public string Id { get; set; } = "default";
    
    // Branding
    public string PlatformName { get; set; } = "Ierahkwa Futurehead Launchpad";
    public string LogoUrl { get; set; } = "/assets/logo.png";
    public string FaviconUrl { get; set; } = "/favicon.ico";
    public string Title { get; set; } = "Ierahkwa IDO Factory | Sovereign Token Launchpad";
    public string Description { get; set; } = "Launch your token on the Ierahkwa Sovereign Blockchain. Create IDO pools with token lockers.";
    
    // Colors (CSS variables)
    public string PrimaryColor { get; set; } = "#FFD700";
    public string SecondaryColor { get; set; } = "#00FF41";
    public string AccentColor { get; set; } = "#00FFFF";
    public string BackgroundColor { get; set; } = "#0a0e17";
    public string CardColor { get; set; } = "#111827";
    public string TextColor { get; set; } = "#ffffff";
    
    // Social Links
    public string Website { get; set; } = "https://launchpad.ierahkwa.gov";
    public string Twitter { get; set; } = "https://twitter.com/ierahkwa";
    public string Telegram { get; set; } = "https://t.me/ierahkwa";
    public string Discord { get; set; } = "https://discord.gg/ierahkwa";
    public string Medium { get; set; } = "https://medium.com/@ierahkwa";
    public string Email { get; set; } = "launchpad@ierahkwa.gov";
    
    // Admin Addresses
    public List<string> AdminAddresses { get; set; } = new() { "IERAHKWA_ADMIN_ADDRESS" };
    public string FeeReceiverAddress { get; set; } = "IERAHKWA_FEE_ADDRESS";
    public string TreasuryAddress { get; set; } = "IERAHKWA_TREASURY_ADDRESS";
    
    // Fees
    public decimal PoolCreationFee { get; set; } = 0.1m; // in native token
    public decimal PlatformFeePercentage { get; set; } = 3.0m; // 3% of funds raised
    public decimal TokenLockFee { get; set; } = 0.05m; // in native token
    public decimal EmergencyWithdrawFee { get; set; } = 10.0m; // 10% penalty
    
    // Supported Networks
    public List<SupportedNetwork> Networks { get; set; } = new()
    {
        new SupportedNetwork { ChainId = 777777, Name = "Ierahkwa Sovereign Blockchain", Symbol = "ISB", RpcUrl = "https://rpc.ierahkwa.gov", Explorer = "https://explorer.ierahkwa.gov", IsDefault = true },
        new SupportedNetwork { ChainId = 1, Name = "Ethereum Mainnet", Symbol = "ETH", RpcUrl = "https://mainnet.infura.io/v3/", Explorer = "https://etherscan.io" },
        new SupportedNetwork { ChainId = 56, Name = "BNB Smart Chain", Symbol = "BNB", RpcUrl = "https://bsc-dataseed.binance.org/", Explorer = "https://bscscan.com" },
        new SupportedNetwork { ChainId = 137, Name = "Polygon", Symbol = "MATIC", RpcUrl = "https://polygon-rpc.com/", Explorer = "https://polygonscan.com" },
        new SupportedNetwork { ChainId = 43114, Name = "Avalanche C-Chain", Symbol = "AVAX", RpcUrl = "https://api.avax.network/ext/bc/C/rpc", Explorer = "https://snowtrace.io" },
        new SupportedNetwork { ChainId = 250, Name = "Fantom Opera", Symbol = "FTM", RpcUrl = "https://rpc.ftm.tools/", Explorer = "https://ftmscan.com" },
        new SupportedNetwork { ChainId = 42161, Name = "Arbitrum One", Symbol = "ETH", RpcUrl = "https://arb1.arbitrum.io/rpc", Explorer = "https://arbiscan.io" }
    };
    
    // Pool Limits
    public decimal MinSoftCap { get; set; } = 100;
    public decimal MaxHardCap { get; set; } = 10000000;
    public int MaxPoolDurationDays { get; set; } = 90;
    public int MinRegistrationPeriodHours { get; set; } = 24;
    
    // Lock Limits
    public int MinLockDurationDays { get; set; } = 30;
    public int MaxLockDurationDays { get; set; } = 3650; // 10 years
    
    // Features
    public bool EnableKYC { get; set; } = false;
    public bool EnableWhitelist { get; set; } = true;
    public bool EnableTieredPools { get; set; } = true;
    public bool EnableLotteryPools { get; set; } = true;
    public bool EnableTokenLocker { get; set; } = true;
    public bool EnableStaking { get; set; } = true;
    public bool RequireAudit { get; set; } = false;
    
    // Smart Contract Addresses
    public string IDOFactoryContract { get; set; } = string.Empty;
    public string TokenLockerContract { get; set; } = string.Empty;
    public string StakingContract { get; set; } = string.Empty;
    
    // Timestamps
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class SupportedNetwork
{
    public int ChainId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string RpcUrl { get; set; } = string.Empty;
    public string Explorer { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsEnabled { get; set; } = true;
}
