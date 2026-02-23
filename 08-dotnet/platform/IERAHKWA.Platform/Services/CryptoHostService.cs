using IERAHKWA.Platform.Models;
using System.Collections.Concurrent;

namespace IERAHKWA.Platform.Services;

// ============================================================================
//                    CRYPTOHOST AI CRYPTO EXCHANGE
//                    Powered by Mamey Node Technology
// ============================================================================

public interface ICryptoHostService
{
    // Market Data
    Task<List<CryptoAsset>> GetAllAssetsAsync();
    Task<CryptoAsset?> GetAssetAsync(string symbol);
    Task<List<CryptoAsset>> GetTrendingAsync(int limit);
    Task<MarketOverview> GetMarketOverviewAsync();
    
    // Trading
    Task<SwapQuote> GetSwapQuoteAsync(string fromSymbol, string toSymbol, decimal amount);
    Task<SwapResult> ExecuteSwapAsync(string userId, SwapQuote quote);
    Task<List<SwapHistory>> GetSwapHistoryAsync(string userId);
    
    // Portfolio
    Task<CryptoPortfolio> GetPortfolioAsync(string userId);
    Task<PortfolioAnalytics> GetPortfolioAnalyticsAsync(string userId);
    
    // AI Insights
    Task<List<AIInsight>> GetAIInsightsAsync(string? symbol = null);
    Task<PricePredictor> GetPricePredictionAsync(string symbol, string timeframe);
    Task<string> AskCryptoAIAsync(string userId, string question);
    
    // Staking
    Task<List<StakingPool>> GetStakingPoolsAsync();
    Task<StakeResult> StakeAsync(string userId, string poolId, decimal amount);
    Task<UnstakeResult> UnstakeAsync(string userId, string stakeId);
}

public class CryptoHostService : ICryptoHostService
{
    private readonly ILogger<CryptoHostService> _logger;
    private readonly Random _random = new();
    private readonly ConcurrentDictionary<string, CryptoPortfolio> _portfolios = new();
    private readonly ConcurrentDictionary<string, SwapHistory> _swapHistory = new();
    private readonly ConcurrentDictionary<string, StakePosition> _stakes = new();

    private readonly List<CryptoAsset> _assets;

    public CryptoHostService(ILogger<CryptoHostService> logger)
    {
        _logger = logger;
        _assets = InitializeAssets();
    }

    private List<CryptoAsset> InitializeAssets()
    {
        return new List<CryptoAsset>
        {
            // IERAHKWA Native Tokens
            new() { Symbol = "WPM", Name = "Wampum", Price = 0.0125m, Change24h = 3.5m, Volume24h = 25000000, MarketCap = 10625000000, Icon = "🏛️", Category = "native", IsNative = true },
            new() { Symbol = "IGT", Name = "Ierahkwa Gov Token", Price = 0.85m, Change24h = 5.8m, Volume24h = 12000000, MarketCap = 425000000000, Icon = "⚖️", Category = "native", IsNative = true },
            new() { Symbol = "IGT-S", Name = "IGT Stablecoin", Price = 1.00m, Change24h = 0.01m, Volume24h = 50000000, MarketCap = 50000000000, Icon = "💵", Category = "stablecoin", IsNative = true },
            new() { Symbol = "FHT", Name = "Futurehead Token", Price = 12.50m, Change24h = 8.2m, Volume24h = 8500000, MarketCap = 312500000, Icon = "🔮", Category = "native", IsNative = true },
            
            // Major Cryptocurrencies
            new() { Symbol = "BTC", Name = "Bitcoin", Price = 105000m, Change24h = 2.1m, Volume24h = 45000000000, MarketCap = 2100000000000, Icon = "₿", Category = "major" },
            new() { Symbol = "ETH", Name = "Ethereum", Price = 3800m, Change24h = 3.2m, Volume24h = 25000000000, MarketCap = 456000000000, Icon = "⟠", Category = "major" },
            new() { Symbol = "SOL", Name = "Solana", Price = 245m, Change24h = 6.5m, Volume24h = 8000000000, MarketCap = 108000000000, Icon = "◎", Category = "major" },
            new() { Symbol = "BNB", Name = "BNB", Price = 720m, Change24h = 1.8m, Volume24h = 2500000000, MarketCap = 108000000000, Icon = "🔶", Category = "major" },
            new() { Symbol = "XRP", Name = "Ripple", Price = 2.85m, Change24h = 4.2m, Volume24h = 3500000000, MarketCap = 163000000000, Icon = "✕", Category = "major" },
            new() { Symbol = "ADA", Name = "Cardano", Price = 1.12m, Change24h = 2.8m, Volume24h = 1200000000, MarketCap = 40000000000, Icon = "₳", Category = "major" },
            new() { Symbol = "AVAX", Name = "Avalanche", Price = 52m, Change24h = 5.1m, Volume24h = 850000000, MarketCap = 21000000000, Icon = "🔺", Category = "major" },
            new() { Symbol = "DOT", Name = "Polkadot", Price = 9.50m, Change24h = 3.7m, Volume24h = 650000000, MarketCap = 14000000000, Icon = "●", Category = "major" },
            new() { Symbol = "MATIC", Name = "Polygon", Price = 1.25m, Change24h = 4.5m, Volume24h = 750000000, MarketCap = 12500000000, Icon = "🟣", Category = "major" },
            new() { Symbol = "LINK", Name = "Chainlink", Price = 22m, Change24h = 3.9m, Volume24h = 550000000, MarketCap = 13200000000, Icon = "⬡", Category = "defi" },
            
            // Stablecoins
            new() { Symbol = "USDT", Name = "Tether", Price = 1.00m, Change24h = 0.0m, Volume24h = 85000000000, MarketCap = 140000000000, Icon = "₮", Category = "stablecoin" },
            new() { Symbol = "USDC", Name = "USD Coin", Price = 1.00m, Change24h = 0.0m, Volume24h = 12000000000, MarketCap = 45000000000, Icon = "💲", Category = "stablecoin" },
            new() { Symbol = "DAI", Name = "Dai", Price = 1.00m, Change24h = 0.01m, Volume24h = 450000000, MarketCap = 5300000000, Icon = "◆", Category = "stablecoin" },
            
            // DeFi Tokens
            new() { Symbol = "UNI", Name = "Uniswap", Price = 12.50m, Change24h = 4.8m, Volume24h = 320000000, MarketCap = 9400000000, Icon = "🦄", Category = "defi" },
            new() { Symbol = "AAVE", Name = "Aave", Price = 185m, Change24h = 3.2m, Volume24h = 180000000, MarketCap = 2800000000, Icon = "👻", Category = "defi" },
            new() { Symbol = "MKR", Name = "Maker", Price = 2100m, Change24h = 2.5m, Volume24h = 120000000, MarketCap = 1900000000, Icon = "Ⓜ", Category = "defi" },
            new() { Symbol = "CRV", Name = "Curve", Price = 0.85m, Change24h = 5.2m, Volume24h = 95000000, MarketCap = 1100000000, Icon = "〰️", Category = "defi" },
            
            // Meme Coins
            new() { Symbol = "DOGE", Name = "Dogecoin", Price = 0.42m, Change24h = 8.5m, Volume24h = 2500000000, MarketCap = 62000000000, Icon = "🐕", Category = "meme" },
            new() { Symbol = "SHIB", Name = "Shiba Inu", Price = 0.000035m, Change24h = 12.3m, Volume24h = 850000000, MarketCap = 20000000000, Icon = "🐕‍🦺", Category = "meme" },
            new() { Symbol = "PEPE", Name = "Pepe", Price = 0.0000125m, Change24h = 15.8m, Volume24h = 450000000, MarketCap = 5200000000, Icon = "🐸", Category = "meme" },
        };
    }

    // ==================== MARKET DATA ====================

    public async Task<List<CryptoAsset>> GetAllAssetsAsync()
    {
        await Task.Delay(20);
        foreach (var asset in _assets)
        {
            // Simulate price movement
            var change = (decimal)(_random.NextDouble() * 0.004 - 0.002);
            asset.Price *= (1 + change);
            asset.Change24h += (decimal)(_random.NextDouble() * 0.2 - 0.1);
        }
        return _assets.OrderByDescending(a => a.MarketCap).ToList();
    }

    public async Task<CryptoAsset?> GetAssetAsync(string symbol)
    {
        await Task.Delay(10);
        return _assets.FirstOrDefault(a => a.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<CryptoAsset>> GetTrendingAsync(int limit)
    {
        await Task.Delay(10);
        return _assets.OrderByDescending(a => Math.Abs(a.Change24h)).Take(limit).ToList();
    }

    public async Task<MarketOverview> GetMarketOverviewAsync()
    {
        await Task.Delay(20);
        return new MarketOverview
        {
            TotalMarketCap = _assets.Sum(a => a.MarketCap),
            Total24hVolume = _assets.Sum(a => a.Volume24h),
            BtcDominance = 52.3m + (decimal)(_random.NextDouble() * 2 - 1),
            EthDominance = 16.8m + (decimal)(_random.NextDouble() * 1 - 0.5),
            ActiveCoins = _assets.Count,
            GainersCount = _assets.Count(a => a.Change24h > 0),
            LosersCount = _assets.Count(a => a.Change24h < 0),
            FearGreedIndex = 65 + _random.Next(-10, 10),
            FearGreedLabel = "Greed"
        };
    }

    // ==================== TRADING ====================

    public async Task<SwapQuote> GetSwapQuoteAsync(string fromSymbol, string toSymbol, decimal amount)
    {
        await Task.Delay(50);
        
        var fromAsset = _assets.FirstOrDefault(a => a.Symbol.Equals(fromSymbol, StringComparison.OrdinalIgnoreCase));
        var toAsset = _assets.FirstOrDefault(a => a.Symbol.Equals(toSymbol, StringComparison.OrdinalIgnoreCase));

        if (fromAsset == null || toAsset == null)
            throw new ArgumentException("Invalid asset symbol");

        var rate = fromAsset.Price / toAsset.Price;
        var outputAmount = amount * rate;
        
        // Calculate fees
        var networkFee = fromAsset.IsNative ? 0.001m : 0.002m;
        var platformFee = outputAmount * 0.001m; // 0.1% fee
        var slippage = outputAmount * 0.002m; // 0.2% slippage estimate

        return new SwapQuote
        {
            FromSymbol = fromSymbol,
            ToSymbol = toSymbol,
            FromAmount = amount,
            ToAmount = outputAmount - platformFee - slippage,
            Rate = rate,
            RateInverse = toAsset.Price / fromAsset.Price,
            NetworkFee = networkFee,
            PlatformFee = platformFee,
            Slippage = slippage,
            PriceImpact = amount > 10000 ? 0.5m : 0.1m,
            ValidUntil = DateTime.UtcNow.AddMinutes(2),
            Route = new[] { fromSymbol, "WPM", toSymbol }
        };
    }

    public async Task<SwapResult> ExecuteSwapAsync(string userId, SwapQuote quote)
    {
        await Task.Delay(200);

        if (DateTime.UtcNow > quote.ValidUntil)
            return new SwapResult { Success = false, Error = "Quote expired" };

        var swap = new SwapHistory
        {
            UserId = userId,
            FromSymbol = quote.FromSymbol,
            ToSymbol = quote.ToSymbol,
            FromAmount = quote.FromAmount,
            ToAmount = quote.ToAmount,
            Rate = quote.Rate,
            Fee = quote.PlatformFee,
            TxHash = $"0x{Guid.NewGuid():N}",
            Status = "completed"
        };

        _swapHistory[swap.Id] = swap;

        // Update portfolio
        if (!_portfolios.TryGetValue(userId, out var portfolio))
        {
            portfolio = new CryptoPortfolio { UserId = userId };
            _portfolios[userId] = portfolio;
        }

        // Add to holdings
        var holding = portfolio.Holdings.FirstOrDefault(h => h.Symbol == quote.ToSymbol);
        if (holding != null)
            holding.Amount += quote.ToAmount;
        else
            portfolio.Holdings.Add(new CryptoHolding { Symbol = quote.ToSymbol, Amount = quote.ToAmount });

        _logger.LogInformation("Swap executed: {From} {FromSymbol} -> {To} {ToSymbol}", 
            quote.FromAmount, quote.FromSymbol, quote.ToAmount, quote.ToSymbol);

        return new SwapResult
        {
            Success = true,
            SwapId = swap.Id,
            TxHash = swap.TxHash,
            FromAmount = swap.FromAmount,
            ToAmount = swap.ToAmount,
            Fee = swap.Fee
        };
    }

    public async Task<List<SwapHistory>> GetSwapHistoryAsync(string userId)
    {
        await Task.Delay(10);
        return _swapHistory.Values.Where(s => s.UserId == userId).OrderByDescending(s => s.ExecutedAt).ToList();
    }

    // ==================== PORTFOLIO ====================

    public async Task<CryptoPortfolio> GetPortfolioAsync(string userId)
    {
        await Task.Delay(20);

        if (!_portfolios.TryGetValue(userId, out var portfolio))
        {
            // Initialize with sample holdings
            portfolio = new CryptoPortfolio
            {
                UserId = userId,
                Holdings = new List<CryptoHolding>
                {
                    new() { Symbol = "WPM", Amount = 1000000, AveragePrice = 0.01m },
                    new() { Symbol = "IGT", Amount = 50000, AveragePrice = 0.75m },
                    new() { Symbol = "BTC", Amount = 0.5m, AveragePrice = 95000m },
                    new() { Symbol = "ETH", Amount = 5m, AveragePrice = 3500m },
                    new() { Symbol = "USDT", Amount = 10000, AveragePrice = 1m }
                }
            };
            _portfolios[userId] = portfolio;
        }

        // Calculate current values
        decimal totalValue = 0;
        decimal totalCost = 0;
        foreach (var holding in portfolio.Holdings)
        {
            var asset = _assets.FirstOrDefault(a => a.Symbol == holding.Symbol);
            if (asset != null)
            {
                holding.CurrentPrice = asset.Price;
                holding.CurrentValue = holding.Amount * asset.Price;
                holding.ProfitLoss = holding.CurrentValue - (holding.Amount * holding.AveragePrice);
                holding.ProfitLossPercent = holding.AveragePrice > 0 
                    ? ((holding.CurrentPrice - holding.AveragePrice) / holding.AveragePrice) * 100 
                    : 0;
                holding.Icon = asset.Icon;
                totalValue += holding.CurrentValue;
                totalCost += holding.Amount * holding.AveragePrice;
            }
        }

        portfolio.TotalValue = totalValue;
        portfolio.TotalProfitLoss = totalValue - totalCost;
        portfolio.TotalProfitLossPercent = totalCost > 0 ? ((totalValue - totalCost) / totalCost) * 100 : 0;

        return portfolio;
    }

    public async Task<PortfolioAnalytics> GetPortfolioAnalyticsAsync(string userId)
    {
        var portfolio = await GetPortfolioAsync(userId);
        await Task.Delay(50);

        return new PortfolioAnalytics
        {
            TotalValue = portfolio.TotalValue,
            DailyChange = portfolio.TotalValue * 0.025m,
            DailyChangePercent = 2.5m,
            WeeklyChange = portfolio.TotalValue * 0.08m,
            WeeklyChangePercent = 8m,
            MonthlyChange = portfolio.TotalValue * 0.15m,
            MonthlyChangePercent = 15m,
            BestPerformer = portfolio.Holdings.OrderByDescending(h => h.ProfitLossPercent).FirstOrDefault()?.Symbol ?? "N/A",
            WorstPerformer = portfolio.Holdings.OrderBy(h => h.ProfitLossPercent).FirstOrDefault()?.Symbol ?? "N/A",
            Diversification = new Dictionary<string, decimal>
            {
                ["Native"] = portfolio.Holdings.Where(h => new[] { "WPM", "IGT", "FHT" }.Contains(h.Symbol)).Sum(h => h.CurrentValue) / portfolio.TotalValue * 100,
                ["Major"] = portfolio.Holdings.Where(h => new[] { "BTC", "ETH", "SOL", "BNB" }.Contains(h.Symbol)).Sum(h => h.CurrentValue) / portfolio.TotalValue * 100,
                ["Stablecoin"] = portfolio.Holdings.Where(h => new[] { "USDT", "USDC", "IGT-S" }.Contains(h.Symbol)).Sum(h => h.CurrentValue) / portfolio.TotalValue * 100,
                ["Other"] = 100 - (portfolio.Holdings.Where(h => new[] { "WPM", "IGT", "FHT", "BTC", "ETH", "SOL", "BNB", "USDT", "USDC", "IGT-S" }.Contains(h.Symbol)).Sum(h => h.CurrentValue) / portfolio.TotalValue * 100)
            }
        };
    }

    // ==================== AI INSIGHTS ====================

    public async Task<List<AIInsight>> GetAIInsightsAsync(string? symbol = null)
    {
        await Task.Delay(100);

        var insights = new List<AIInsight>
        {
            new() { Type = "bullish", Symbol = "WPM", Title = "WAMPUM Strong Momentum", 
                Message = "WAMPUM showing strong buying pressure with increasing volume. Technical indicators suggest continuation of uptrend.", 
                Confidence = 85, Priority = "high" },
            new() { Type = "alert", Symbol = "BTC", Title = "Bitcoin Key Level", 
                Message = "BTC approaching major resistance at $108,000. Watch for breakout confirmation with volume.", 
                Confidence = 78, Priority = "medium" },
            new() { Type = "opportunity", Symbol = "ETH", Title = "ETH DeFi Activity Surge", 
                Message = "Ethereum network activity increasing significantly. DeFi TVL up 12% this week.", 
                Confidence = 82, Priority = "high" },
            new() { Type = "warning", Symbol = "SHIB", Title = "High Volatility Alert", 
                Message = "SHIB showing extreme volatility. Consider risk management strategies.", 
                Confidence = 90, Priority = "medium" },
            new() { Type = "bullish", Symbol = "IGT", Title = "Governance Token Accumulation", 
                Message = "Large wallets accumulating IGT. Governance proposals driving interest.", 
                Confidence = 75, Priority = "medium" }
        };

        if (!string.IsNullOrEmpty(symbol))
            return insights.Where(i => i.Symbol?.Equals(symbol, StringComparison.OrdinalIgnoreCase) == true).ToList();

        return insights;
    }

    public async Task<PricePredictor> GetPricePredictionAsync(string symbol, string timeframe)
    {
        await Task.Delay(150);

        var asset = _assets.FirstOrDefault(a => a.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
        if (asset == null) throw new ArgumentException("Asset not found");

        var multiplier = timeframe switch
        {
            "1h" => 1.005m,
            "24h" => 1.02m,
            "7d" => 1.08m,
            "30d" => 1.15m,
            _ => 1.02m
        };

        return new PricePredictor
        {
            Symbol = symbol,
            CurrentPrice = asset.Price,
            PredictedPrice = asset.Price * multiplier * (1 + (decimal)(_random.NextDouble() * 0.1 - 0.05)),
            Timeframe = timeframe,
            Confidence = 70 + _random.Next(20),
            Sentiment = "bullish",
            SupportLevel = asset.Price * 0.95m,
            ResistanceLevel = asset.Price * 1.08m,
            Indicators = new Dictionary<string, string>
            {
                ["RSI"] = "58 (Neutral)",
                ["MACD"] = "Bullish Crossover",
                ["MA200"] = "Above (Bullish)",
                ["Volume"] = "Increasing"
            }
        };
    }

    public async Task<string> AskCryptoAIAsync(string userId, string question)
    {
        await Task.Delay(200);

        var lowerQ = question.ToLower();

        if (lowerQ.Contains("buy") || lowerQ.Contains("comprar"))
            return "🤖 Based on current market analysis, I recommend dollar-cost averaging (DCA) for major assets like BTC and ETH. For IERAHKWA native tokens (WPM, IGT), the current price levels show strong support. Always invest what you can afford to lose.";

        if (lowerQ.Contains("sell") || lowerQ.Contains("vender"))
            return "🤖 Consider taking partial profits on positions showing 50%+ gains. Use trailing stop-losses to protect profits. For long-term holds in WPM and IGT, the fundamentals remain strong.";

        if (lowerQ.Contains("stake") || lowerQ.Contains("staking"))
            return "🤖 IERAHKWA staking pools offer competitive APY: WPM Pool (12% APY), IGT Pool (18% APY), and FHT Pool (25% APY). Staking locks funds but provides passive income. I recommend the IGT pool for balanced risk/reward.";

        if (lowerQ.Contains("market") || lowerQ.Contains("mercado"))
            return "🤖 Current market sentiment is 'Greed' (Fear & Greed Index: 65). BTC dominance at 52% suggests altcoin season may be approaching. IERAHKWA ecosystem tokens showing stronger performance than market average.";

        return "🤖 I'm your CRYPTOHOST AI assistant. I can help you with: market analysis, trading strategies, portfolio optimization, staking recommendations, and price predictions. What specific aspect would you like to explore?";
    }

    // ==================== STAKING ====================

    public async Task<List<StakingPool>> GetStakingPoolsAsync()
    {
        await Task.Delay(20);
        return new List<StakingPool>
        {
            new() { Id = "wpm-stake", Name = "WAMPUM Staking", Symbol = "WPM", APY = 12m, TotalStaked = 250000000m, MinStake = 1000, LockPeriod = 7, Icon = "🏛️" },
            new() { Id = "igt-stake", Name = "IGT Governance", Symbol = "IGT", APY = 18m, TotalStaked = 85000000m, MinStake = 100, LockPeriod = 14, Icon = "⚖️" },
            new() { Id = "fht-stake", Name = "Futurehead Pool", Symbol = "FHT", APY = 25m, TotalStaked = 12000000m, MinStake = 10, LockPeriod = 30, Icon = "🔮" },
            new() { Id = "lp-wpm-usdt", Name = "WPM/USDT LP", Symbol = "LP", APY = 45m, TotalStaked = 15000000m, MinStake = 100, LockPeriod = 0, Icon = "💧" },
            new() { Id = "lp-igt-eth", Name = "IGT/ETH LP", Symbol = "LP", APY = 38m, TotalStaked = 8500000m, MinStake = 50, LockPeriod = 0, Icon = "💧" }
        };
    }

    public async Task<StakeResult> StakeAsync(string userId, string poolId, decimal amount)
    {
        await Task.Delay(100);

        var pools = await GetStakingPoolsAsync();
        var pool = pools.FirstOrDefault(p => p.Id == poolId);
        if (pool == null) return new StakeResult { Success = false, Error = "Pool not found" };
        if (amount < pool.MinStake) return new StakeResult { Success = false, Error = $"Minimum stake is {pool.MinStake}" };

        var stake = new StakePosition
        {
            UserId = userId,
            PoolId = poolId,
            Amount = amount,
            APY = pool.APY,
            UnlockDate = DateTime.UtcNow.AddDays(pool.LockPeriod)
        };

        _stakes[stake.Id] = stake;
        _logger.LogInformation("Staked {Amount} in {Pool}", amount, pool.Name);

        return new StakeResult
        {
            Success = true,
            StakeId = stake.Id,
            Amount = amount,
            APY = pool.APY,
            EstimatedRewards = amount * pool.APY / 100 / 365 * pool.LockPeriod,
            UnlockDate = stake.UnlockDate
        };
    }

    public async Task<UnstakeResult> UnstakeAsync(string userId, string stakeId)
    {
        await Task.Delay(100);

        if (!_stakes.TryGetValue(stakeId, out var stake))
            return new UnstakeResult { Success = false, Error = "Stake not found" };

        if (stake.UserId != userId)
            return new UnstakeResult { Success = false, Error = "Unauthorized" };

        if (DateTime.UtcNow < stake.UnlockDate)
            return new UnstakeResult { Success = false, Error = $"Locked until {stake.UnlockDate:yyyy-MM-dd}" };

        var rewards = stake.Amount * stake.APY / 100 / 365 * (decimal)(DateTime.UtcNow - stake.StakedAt).TotalDays;

        _stakes.TryRemove(stakeId, out _);

        return new UnstakeResult
        {
            Success = true,
            AmountReturned = stake.Amount,
            RewardsEarned = rewards,
            TxHash = $"0x{Guid.NewGuid():N}"
        };
    }
}

// ==================== MODELS ====================

public class CryptoAsset
{
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public decimal Change24h { get; set; }
    public decimal Volume24h { get; set; }
    public decimal MarketCap { get; set; }
    public string Icon { get; set; } = "";
    public string Category { get; set; } = "";
    public bool IsNative { get; set; }
}

public class MarketOverview
{
    public decimal TotalMarketCap { get; set; }
    public decimal Total24hVolume { get; set; }
    public decimal BtcDominance { get; set; }
    public decimal EthDominance { get; set; }
    public int ActiveCoins { get; set; }
    public int GainersCount { get; set; }
    public int LosersCount { get; set; }
    public int FearGreedIndex { get; set; }
    public string FearGreedLabel { get; set; } = "";
}

public class SwapQuote
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FromSymbol { get; set; } = "";
    public string ToSymbol { get; set; } = "";
    public decimal FromAmount { get; set; }
    public decimal ToAmount { get; set; }
    public decimal Rate { get; set; }
    public decimal RateInverse { get; set; }
    public decimal NetworkFee { get; set; }
    public decimal PlatformFee { get; set; }
    public decimal Slippage { get; set; }
    public decimal PriceImpact { get; set; }
    public DateTime ValidUntil { get; set; }
    public string[] Route { get; set; } = Array.Empty<string>();
}

public class SwapResult
{
    public bool Success { get; set; }
    public string? SwapId { get; set; }
    public string? TxHash { get; set; }
    public decimal FromAmount { get; set; }
    public decimal ToAmount { get; set; }
    public decimal Fee { get; set; }
    public string? Error { get; set; }
}

public class SwapHistory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string FromSymbol { get; set; } = "";
    public string ToSymbol { get; set; } = "";
    public decimal FromAmount { get; set; }
    public decimal ToAmount { get; set; }
    public decimal Rate { get; set; }
    public decimal Fee { get; set; }
    public string TxHash { get; set; } = "";
    public string Status { get; set; } = "pending";
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
}

public class CryptoPortfolio
{
    public string UserId { get; set; } = "";
    public List<CryptoHolding> Holdings { get; set; } = new();
    public decimal TotalValue { get; set; }
    public decimal TotalProfitLoss { get; set; }
    public decimal TotalProfitLossPercent { get; set; }
}

public class CryptoHolding
{
    public string Symbol { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal ProfitLoss { get; set; }
    public decimal ProfitLossPercent { get; set; }
    public string Icon { get; set; } = "";
}

public class PortfolioAnalytics
{
    public decimal TotalValue { get; set; }
    public decimal DailyChange { get; set; }
    public decimal DailyChangePercent { get; set; }
    public decimal WeeklyChange { get; set; }
    public decimal WeeklyChangePercent { get; set; }
    public decimal MonthlyChange { get; set; }
    public decimal MonthlyChangePercent { get; set; }
    public string BestPerformer { get; set; } = "";
    public string WorstPerformer { get; set; } = "";
    public Dictionary<string, decimal> Diversification { get; set; } = new();
}

public class AIInsight
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = ""; // bullish, bearish, alert, opportunity, warning
    public string? Symbol { get; set; }
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public int Confidence { get; set; }
    public string Priority { get; set; } = "medium";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class PricePredictor
{
    public string Symbol { get; set; } = "";
    public decimal CurrentPrice { get; set; }
    public decimal PredictedPrice { get; set; }
    public string Timeframe { get; set; } = "";
    public int Confidence { get; set; }
    public string Sentiment { get; set; } = "";
    public decimal SupportLevel { get; set; }
    public decimal ResistanceLevel { get; set; }
    public Dictionary<string, string> Indicators { get; set; } = new();
}

public class StakingPool
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Symbol { get; set; } = "";
    public decimal APY { get; set; }
    public decimal TotalStaked { get; set; }
    public decimal MinStake { get; set; }
    public int LockPeriod { get; set; } // days
    public string Icon { get; set; } = "";
}

public class StakePosition
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string PoolId { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal APY { get; set; }
    public DateTime StakedAt { get; set; } = DateTime.UtcNow;
    public DateTime UnlockDate { get; set; }
}

public class StakeResult
{
    public bool Success { get; set; }
    public string? StakeId { get; set; }
    public decimal Amount { get; set; }
    public decimal APY { get; set; }
    public decimal EstimatedRewards { get; set; }
    public DateTime UnlockDate { get; set; }
    public string? Error { get; set; }
}

public class UnstakeResult
{
    public bool Success { get; set; }
    public decimal AmountReturned { get; set; }
    public decimal RewardsEarned { get; set; }
    public string? TxHash { get; set; }
    public string? Error { get; set; }
}
