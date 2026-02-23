using TradeX.Core.Interfaces;
using TradeX.Core.Models;

namespace TradeX.Infrastructure.Services;

/// <summary>
/// Crypto Wallet Config Service - IERAHKWA TradeX
/// CodeCanyon-style: Bitcoin, Ethereum, ERC20, BEP20, Matic. Admin fee, fiat on-ramp, token list.
/// </summary>
public class CryptoWalletConfigService : ICryptoWalletConfigService
{
    private readonly CryptoWalletSettings _settings;
    private readonly FiatOnRampConfig _fiat;
    private readonly IReadOnlyList<CryptoWalletToken> _tokens;
    private readonly Dictionary<string, decimal> _prices = new(StringComparer.OrdinalIgnoreCase)
    {
        ["BTC"] = 65000m, ["ETH"] = 3500m, ["BNB"] = 550m, ["MATIC"] = 0.9m, ["USDT"] = 1m,
        ["IGT-PM"] = 1m, ["IGT-SOVEREIGN"] = 1m, ["USDC"] = 1m, ["DAI"] = 1m,
        ["WETH"] = 3500m, ["WBTC"] = 65000m
    };

    public CryptoWalletConfigService(CryptoWalletSettings settings, FiatOnRampConfig fiat)
    {
        _settings = settings;
        _fiat = fiat;
        _tokens = BuildDefaultTokens();
    }

    public CryptoWalletSettings GetSettings() => _settings;
    public FiatOnRampConfig GetFiatOnRampConfig() => _fiat;

    public IReadOnlyList<CryptoWalletToken> GetTokens(string? network = null)
    {
        if (string.IsNullOrEmpty(network)) return _tokens;
        return _tokens.Where(t => string.Equals(t.Network, network, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public SwapQuoteResult GetSwapQuote(string fromSymbol, string toSymbol, decimal fromAmount)
    {
        var fromPrice = _prices.GetValueOrDefault(fromSymbol, 1m);
        var toPrice = _prices.GetValueOrDefault(toSymbol, 1m);
        var rate = fromPrice / toPrice;
        var rawTo = fromAmount * rate;
        var basePct = _settings.BaseExchangeFeePercent / 100m;
        var adminPct = _settings.AdminExchangeFeePercent / 100m;
        var totalPct = basePct + adminPct;
        var feeAmount = rawTo * totalPct;
        var youReceive = rawTo - feeAmount;

        return new SwapQuoteResult
        {
            FromAmount = fromAmount,
            ToAmount = rawTo,
            Rate = rate,
            BaseFeePercent = _settings.BaseExchangeFeePercent,
            AdminFeePercent = _settings.AdminExchangeFeePercent,
            TotalFeeAmount = feeAmount,
            YouReceive = youReceive
        };
    }

    private static IReadOnlyList<CryptoWalletToken> BuildDefaultTokens()
    {
        return new List<CryptoWalletToken>
        {
            new() { Symbol = "BTC", Name = "Bitcoin", Network = "BITCOIN", Decimals = 8, IsNative = true },
            new() { Symbol = "ETH", Name = "Ethereum", Network = "ETH", Decimals = 18, IsNative = true },
            new() { Symbol = "USDT", Name = "Tether", ContractAddress = "0xdAC17F958D2ee523a2206206994597C13D831ec7", Network = "ETH", Decimals = 6 },
            new() { Symbol = "USDC", Name = "USD Coin", ContractAddress = "0xA0b86991c6218b36c1d19D4a2e9Eb0cE3606eB48", Network = "ETH", Decimals = 6 },
            new() { Symbol = "DAI", Name = "Dai", ContractAddress = "0x6B175474E89094C44Da98b954Eedeac495271d0F", Network = "ETH", Decimals = 18 },
            new() { Symbol = "WETH", Name = "Wrapped Ether", ContractAddress = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2", Network = "ETH", Decimals = 18 },
            new() { Symbol = "WBTC", Name = "Wrapped Bitcoin", ContractAddress = "0x2260FAC5E5542a773Aa44fBCfeDf7C193bc2C599", Network = "ETH", Decimals = 8 },
            new() { Symbol = "BNB", Name = "BNB", Network = "BSC", Decimals = 18, IsNative = true },
            new() { Symbol = "USDT", Name = "Tether (BEP20)", ContractAddress = "0x55d398326f99059fF775485246999027B3197955", Network = "BSC", Decimals = 18 },
            new() { Symbol = "USDC", Name = "USD Coin (BEP20)", ContractAddress = "0x8AC76a51cc950d9822D68b83fE1Ad97B32Cd580d", Network = "BSC", Decimals = 18 },
            new() { Symbol = "MATIC", Name = "Polygon", Network = "POLYGON", Decimals = 18, IsNative = true },
            new() { Symbol = "USDT", Name = "Tether (Polygon)", ContractAddress = "0xc2132D05D31c914a87C6611C10748AEb04B58e8F", Network = "POLYGON", Decimals = 6 },
            new() { Symbol = "USDC", Name = "USD Coin (Polygon)", ContractAddress = "0x2791Bca1f2de4661ED88A30C99A7a9449Aa84174", Network = "POLYGON", Decimals = 6 },
            new() { Symbol = "IGT-PM", Name = "Ierahkwa Prime Minister", Network = "IERAHKWA", Decimals = 9, RecommendedPrice = 1m },
            new() { Symbol = "IGT-SOVEREIGN", Name = "Ierahkwa Sovereign", Network = "IERAHKWA", Decimals = 9, RecommendedPrice = 1m }
        };
    }
}
