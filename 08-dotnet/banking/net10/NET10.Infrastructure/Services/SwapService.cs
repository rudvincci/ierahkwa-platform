using NET10.Core.Interfaces;
using NET10.Core.Models;
using TransactionStatus = NET10.Core.Models.TransactionStatus;

namespace NET10.Infrastructure.Services;

public class SwapService : ISwapService
{
    private readonly ITokenService _tokenService;
    private readonly IPoolService _poolService;
    private readonly List<SwapTransaction> _swapHistory;
    private readonly NET10Config _config;

    public SwapService(ITokenService tokenService, IPoolService poolService, NET10Config config)
    {
        _tokenService = tokenService;
        _poolService = poolService;
        _config = config;
        _swapHistory = new List<SwapTransaction>();
    }

    public async Task<SwapQuote> GetQuoteAsync(string tokenInId, string tokenOutId, decimal amountIn)
    {
        var tokenIn = await _tokenService.GetTokenByIdAsync(tokenInId);
        var tokenOut = await _tokenService.GetTokenByIdAsync(tokenOutId);

        if (tokenIn == null || tokenOut == null)
            throw new ArgumentException("Invalid token pair");

        // Find pool and calculate swap
        var pool = await _poolService.GetPoolByPairAsync(tokenInId, tokenOutId);
        
        decimal rate = tokenIn.Price / tokenOut.Price;
        decimal amountOut = amountIn * rate;
        
        // Calculate fees
        decimal swapFee = amountOut * _config.DefaultSwapFee;
        decimal adminFee = amountOut * (_config.AdminFeePercent / 100);
        decimal totalFee = swapFee + adminFee;
        
        // Calculate price impact (simplified)
        decimal priceImpact = pool != null ? (amountIn / pool.Reserve0) * 100 : 0.1m;
        priceImpact = Math.Min(priceImpact, 50); // Cap at 50%

        // Apply slippage
        decimal minimumReceived = amountOut - totalFee - (amountOut * (_config.DefaultSlippage / 100));

        return new SwapQuote
        {
            TokenInId = tokenInId,
            TokenOutId = tokenOutId,
            TokenInSymbol = tokenIn.Symbol,
            TokenOutSymbol = tokenOut.Symbol,
            AmountIn = amountIn,
            AmountOut = amountOut - totalFee,
            Rate = rate,
            PriceImpact = priceImpact,
            SwapFee = swapFee,
            AdminFee = adminFee,
            TotalFee = totalFee,
            MinimumReceived = minimumReceived,
            Route = new[] { tokenIn.Symbol, tokenOut.Symbol },
            UsdValueIn = amountIn * tokenIn.Price,
            UsdValueOut = (amountOut - totalFee) * tokenOut.Price,
            ValidUntil = DateTime.UtcNow.AddMinutes(1)
        };
    }

    public async Task<SwapQuote> GetQuoteExactOutAsync(string tokenInId, string tokenOutId, decimal amountOut)
    {
        var tokenIn = await _tokenService.GetTokenByIdAsync(tokenInId);
        var tokenOut = await _tokenService.GetTokenByIdAsync(tokenOutId);

        if (tokenIn == null || tokenOut == null)
            throw new ArgumentException("Invalid token pair");

        decimal rate = tokenIn.Price / tokenOut.Price;
        
        // Calculate input needed including fees
        decimal totalFeePercent = _config.DefaultSwapFee + (_config.AdminFeePercent / 100);
        decimal amountIn = (amountOut / (1 - totalFeePercent)) / rate;
        
        decimal swapFee = amountOut * _config.DefaultSwapFee;
        decimal adminFee = amountOut * (_config.AdminFeePercent / 100);

        return new SwapQuote
        {
            TokenInId = tokenInId,
            TokenOutId = tokenOutId,
            TokenInSymbol = tokenIn.Symbol,
            TokenOutSymbol = tokenOut.Symbol,
            AmountIn = amountIn,
            AmountOut = amountOut,
            Rate = rate,
            PriceImpact = 0.1m,
            SwapFee = swapFee,
            AdminFee = adminFee,
            TotalFee = swapFee + adminFee,
            MinimumReceived = amountOut,
            Route = new[] { tokenIn.Symbol, tokenOut.Symbol },
            UsdValueIn = amountIn * tokenIn.Price,
            UsdValueOut = amountOut * tokenOut.Price,
            ValidUntil = DateTime.UtcNow.AddMinutes(1)
        };
    }

    public async Task<SwapTransaction> ExecuteSwapAsync(SwapRequest request)
    {
        var quote = await GetQuoteAsync(request.TokenInId, request.TokenOutId, request.AmountIn);

        if (quote.AmountOut < request.AmountOutMin)
            throw new InvalidOperationException("Slippage tolerance exceeded");

        if (DateTime.UtcNow > request.Deadline)
            throw new InvalidOperationException("Transaction deadline exceeded");

        var transaction = new SwapTransaction
        {
            Id = Guid.NewGuid().ToString(),
            TxHash = $"0x{Guid.NewGuid():N}",
            UserId = request.UserId,
            WalletAddress = request.WalletAddress,
            TokenInId = request.TokenInId,
            TokenOutId = request.TokenOutId,
            TokenInSymbol = quote.TokenInSymbol,
            TokenOutSymbol = quote.TokenOutSymbol,
            AmountIn = request.AmountIn,
            AmountOut = quote.AmountOut,
            Rate = quote.Rate,
            SwapFee = quote.SwapFee,
            AdminFee = quote.AdminFee,
            TotalFee = quote.TotalFee,
            UsdValue = quote.UsdValueIn,
            PriceImpact = quote.PriceImpact,
            Route = quote.Route,
            Status = TransactionStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            ConfirmedAt = DateTime.UtcNow
        };

        _swapHistory.Add(transaction);
        return transaction;
    }

    public Task<List<SwapTransaction>> GetUserSwapHistoryAsync(string userId, int limit = 50)
    {
        var history = _swapHistory
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Take(limit)
            .ToList();
        return Task.FromResult(history);
    }

    public Task<List<SwapTransaction>> GetRecentSwapsAsync(int limit = 100)
    {
        var recent = _swapHistory
            .OrderByDescending(s => s.CreatedAt)
            .Take(limit)
            .ToList();
        return Task.FromResult(recent);
    }

    public Task<string[]> FindBestRouteAsync(string tokenInId, string tokenOutId)
    {
        // Simplified: direct route
        return Task.FromResult(new[] { tokenInId, tokenOutId });
    }

    public async Task<decimal> GetRateAsync(string tokenInId, string tokenOutId)
    {
        var tokenIn = await _tokenService.GetTokenByIdAsync(tokenInId);
        var tokenOut = await _tokenService.GetTokenByIdAsync(tokenOutId);
        
        if (tokenIn == null || tokenOut == null) return 0;
        return tokenIn.Price / tokenOut.Price;
    }

    public Task<decimal> CalculatePriceImpactAsync(string poolId, decimal amountIn, bool isBuy)
    {
        // Simplified price impact calculation
        decimal impact = Math.Min(amountIn / 100000 * 100, 50);
        return Task.FromResult(impact);
    }
}
