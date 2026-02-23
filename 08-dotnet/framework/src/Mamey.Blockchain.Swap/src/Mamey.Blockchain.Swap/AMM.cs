using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.Swap;

/// <summary>
/// Automated Market Maker (AMM) calculator
/// </summary>
public class ConstantProductAMM
{
    private readonly decimal _feeRate;

    /// <summary>
    /// Initializes a new instance with fee rate (default: 0.3%)
    /// </summary>
    public ConstantProductAMM(decimal feeRate = 0.003m)
    {
        _feeRate = feeRate;
    }

    /// <summary>
    /// Calculate output amount for a given input (constant product formula)
    /// Formula: amountOut = (reserveOut * amountInWithFee) / (reserveIn + amountInWithFee)
    /// </summary>
    public decimal CalculateOutput(
        decimal amountIn,
        decimal reserveIn,
        decimal reserveOut)
    {
        if (reserveIn == 0 || reserveOut == 0)
        {
            throw new InvalidOperationException("Pool has no liquidity");
        }

        // Apply fee: amountInWithFee = amountIn * (1 - feeRate)
        var amountInWithFee = amountIn * (1 - _feeRate);

        // Calculate output: amountOut = (reserveOut * amountInWithFee) / (reserveIn + amountInWithFee)
        var numerator = reserveOut * amountInWithFee;
        var denominator = reserveIn + amountInWithFee;

        return numerator / denominator;
    }

    /// <summary>
    /// Calculate input amount needed for a given output
    /// </summary>
    public decimal CalculateInput(
        decimal amountOut,
        decimal reserveIn,
        decimal reserveOut)
    {
        if (reserveIn == 0 || reserveOut == 0)
        {
            throw new InvalidOperationException("Pool has no liquidity");
        }

        if (amountOut >= reserveOut)
        {
            throw new InvalidOperationException("Insufficient reserve for output amount");
        }

        // Reverse calculation: amountIn = (reserveIn * amountOut) / ((reserveOut - amountOut) * (1 - feeRate))
        var numerator = reserveIn * amountOut;
        var denominator = (reserveOut - amountOut) * (1 - _feeRate);

        return numerator / denominator;
    }

    /// <summary>
    /// Calculate price impact (in percentage)
    /// </summary>
    public decimal CalculatePriceImpact(
        decimal amountIn,
        decimal reserveIn,
        decimal reserveOut)
    {
        if (reserveIn == 0 || reserveOut == 0)
        {
            throw new InvalidOperationException("Pool has no liquidity");
        }

        // Price before swap: reserveOut / reserveIn
        var priceBefore = reserveOut / reserveIn;

        // Price after swap: (reserveOut - amountOut) / (reserveIn + amountIn)
        var amountOut = CalculateOutput(amountIn, reserveIn, reserveOut);
        var reserveInAfter = reserveIn + amountIn;
        var reserveOutAfter = reserveOut - amountOut;

        if (reserveOutAfter == 0)
        {
            return 1.0m; // 100% price impact
        }

        var priceAfter = reserveOutAfter / reserveInAfter;

        // Price impact = ((priceBefore - priceAfter) / priceBefore) * 100
        var priceImpact = ((priceBefore - priceAfter) / priceBefore);
        return Math.Max(0, Math.Min(1, priceImpact)); // Cap at 100%
    }
}

/// <summary>
/// Swap router for finding optimal routes
/// </summary>
public class SwapRouter
{
    private readonly ConstantProductAMM _amm;
    private readonly ILogger<SwapRouter>? _logger;

    /// <summary>
    /// Initializes a new instance
    /// </summary>
    public SwapRouter(ILogger<SwapRouter>? logger = null)
    {
        _amm = new ConstantProductAMM();
        _logger = logger;
    }

    /// <summary>
    /// Find the best route for a swap
    /// </summary>
    public async Task<SwapRoute?> FindBestRouteAsync(
        string tokenIn,
        string tokenOut,
        decimal amountIn,
        List<PoolInfo> availablePools,
        CancellationToken cancellationToken = default)
    {
        // Try direct route first
        var directPool = availablePools.FirstOrDefault(p =>
            (p.TokenA == tokenIn && p.TokenB == tokenOut) ||
            (p.TokenA == tokenOut && p.TokenB == tokenIn));

        if (directPool != null)
        {
            var (reserveIn, reserveOut) = directPool.TokenA == tokenIn
                ? (directPool.ReserveA, directPool.ReserveB)
                : (directPool.ReserveB, directPool.ReserveA);

            var amountOut = _amm.CalculateOutput(amountIn, reserveIn, reserveOut);
            var priceImpact = _amm.CalculatePriceImpact(amountIn, reserveIn, reserveOut);

            return new SwapRoute
            {
                Steps = new List<RouteStep>
                {
                    new RouteStep
                    {
                        TokenIn = tokenIn,
                        TokenOut = tokenOut,
                        Pool = directPool
                    }
                },
                ExpectedOutput = amountOut,
                PriceImpact = priceImpact
            };
        }

        // TODO: Implement multi-hop routing
        // For now, return null if no direct route
        return null;
    }
}

/// <summary>
/// Swap route
/// </summary>
public class SwapRoute
{
    public List<RouteStep> Steps { get; set; } = new();
    public decimal ExpectedOutput { get; set; }
    public decimal PriceImpact { get; set; }
}

/// <summary>
/// Route step
/// </summary>
public class RouteStep
{
    public string TokenIn { get; set; } = string.Empty;
    public string TokenOut { get; set; } = string.Empty;
    public PoolInfo Pool { get; set; } = new();
}


























