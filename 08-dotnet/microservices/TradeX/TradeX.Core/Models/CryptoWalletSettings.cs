namespace TradeX.Core.Models;

/// <summary>
/// Crypto Wallet Settings - IERAHKWA TradeX
/// CodeCanyon-style: theme, registration, exchange-only mode, admin commission.
/// Non-custodial: keys in user's browser (localStorage). Bitcoin, Ethereum, ERC20, BEP20, Matic.
/// </summary>
public class CryptoWalletSettings
{
    /// <summary>light | dark | both</summary>
    public string Theme { get; set; } = "both";

    /// <summary>Enable user registration (create account).</summary>
    public bool RegistrationEnabled { get; set; } = true;

    /// <summary>Dashboard and exchange only: no "Create wallet"; users use MetaMask / WalletConnect.</summary>
    public bool ExchangeOnlyMode { get; set; } = false;

    /// <summary>Admin fee on exchange/swap (e.g. 0.5 = 0.5%).</summary>
    public decimal AdminExchangeFeePercent { get; set; } = 0.1m;

    /// <summary>Base swap/exchange fee before admin (e.g. 0.1 = 0.1%).</summary>
    public decimal BaseExchangeFeePercent { get; set; } = 0.1m;
}

/// <summary>
/// Fiat on-ramp: Visa/MC to buy BTC, ETH, USDT. Transak, itez.com, etc.
/// </summary>
public class FiatOnRampConfig
{
    public bool Enabled { get; set; } = true;
    public string Provider { get; set; } = "transak"; // transak | itez
    public string? ApiKey { get; set; }
    public string? BaseUrl { get; set; }
    /// <summary>BTC, ETH, USDT</summary>
    public string[] SupportedAssets { get; set; } = { "BTC", "ETH", "USDT" };
}

/// <summary>
/// Token for ERC20/BEP20/Matic. From Etherscan, BscScan or custom.
/// </summary>
public class CryptoWalletToken
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ContractAddress { get; set; }
    public string Network { get; set; } = string.Empty; // ETH | BSC | POLYGON | BITCOIN | IERAHKWA
    public int Decimals { get; set; } = 18;
    public decimal? RecommendedPrice { get; set; }
    public bool IsNative { get; set; } // ETH, BNB, MATIC, BTC
}
