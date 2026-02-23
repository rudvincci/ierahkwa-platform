using TradeX.Core.Models;

namespace TradeX.Core.Interfaces;

/// <summary>
/// Crypto Wallet Config - IERAHKWA TradeX
/// CodeCanyon-style: settings, tokens (ERC20/BEP20/Matic), fiat on-ramp, swap quote with admin fee.
/// </summary>
public interface ICryptoWalletConfigService
{
    CryptoWalletSettings GetSettings();
    FiatOnRampConfig GetFiatOnRampConfig();
    /// <param name="network">ETH | BSC | POLYGON | BITCOIN | IERAHKWA | null for all</param>
    IReadOnlyList<CryptoWalletToken> GetTokens(string? network = null);
    /// <summary>Quote for swap: fromAmount, rate, baseFee, adminFee, totalFee, youReceive.</summary>
    SwapQuoteResult GetSwapQuote(string fromSymbol, string toSymbol, decimal fromAmount);
}

public class SwapQuoteResult
{
    public decimal FromAmount { get; set; }
    public decimal ToAmount { get; set; }
    public decimal Rate { get; set; }
    public decimal BaseFeePercent { get; set; }
    public decimal AdminFeePercent { get; set; }
    public decimal TotalFeeAmount { get; set; }
    public decimal YouReceive { get; set; }
}
