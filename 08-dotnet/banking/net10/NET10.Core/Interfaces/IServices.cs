using NET10.Core.Models;

namespace NET10.Core.Interfaces;

/// <summary>
/// Token management service
/// </summary>
public interface ITokenService
{
    Task<List<Token>> GetAllTokensAsync();
    Task<Token?> GetTokenByIdAsync(string tokenId);
    Task<Token?> GetTokenBySymbolAsync(string symbol);
    Task<Token?> GetTokenByAddressAsync(string address);
    Task<List<Token>> SearchTokensAsync(string query);
    Task<Token> AddTokenAsync(Token token);
    Task<Token> UpdateTokenAsync(Token token);
    Task<bool> RemoveTokenAsync(string tokenId);
    Task<List<TokenBalance>> GetUserBalancesAsync(string userId);
    Task<decimal> GetTokenPriceAsync(string tokenId);
}

/// <summary>
/// Swap service for token exchanges
/// </summary>
public interface ISwapService
{
    Task<SwapQuote> GetQuoteAsync(string tokenInId, string tokenOutId, decimal amountIn);
    Task<SwapQuote> GetQuoteExactOutAsync(string tokenInId, string tokenOutId, decimal amountOut);
    Task<SwapTransaction> ExecuteSwapAsync(SwapRequest request);
    Task<List<SwapTransaction>> GetUserSwapHistoryAsync(string userId, int limit = 50);
    Task<List<SwapTransaction>> GetRecentSwapsAsync(int limit = 100);
    Task<string[]> FindBestRouteAsync(string tokenInId, string tokenOutId);
    Task<decimal> GetRateAsync(string tokenInId, string tokenOutId);
    Task<decimal> CalculatePriceImpactAsync(string poolId, decimal amountIn, bool isBuy);
}

/// <summary>
/// Liquidity pool service
/// </summary>
public interface IPoolService
{
    Task<List<LiquidityPool>> GetAllPoolsAsync();
    Task<LiquidityPool?> GetPoolByIdAsync(string poolId);
    Task<LiquidityPool?> GetPoolByPairAsync(string token0Id, string token1Id);
    Task<LiquidityPool> CreatePoolAsync(string token0Id, string token1Id, decimal fee);
    Task<LiquidityPool> UpdatePoolAsync(LiquidityPool pool);
    
    // Liquidity operations
    Task<LiquidityTransaction> AddLiquidityAsync(LiquidityRequest request);
    Task<LiquidityTransaction> RemoveLiquidityAsync(LiquidityRequest request);
    Task<(decimal token0Amount, decimal token1Amount)> CalculateAddLiquidityAsync(string poolId, decimal token0Amount);
    Task<(decimal token0Amount, decimal token1Amount)> CalculateRemoveLiquidityAsync(string poolId, decimal lpTokenAmount);
    
    // User positions
    Task<List<LiquidityPosition>> GetUserPositionsAsync(string userId);
    Task<LiquidityPosition?> GetUserPositionAsync(string userId, string poolId);
    Task<List<LiquidityTransaction>> GetUserLiquidityHistoryAsync(string userId, int limit = 50);
    
    // Analytics
    Task<List<LiquidityPool>> GetTopPoolsByTVLAsync(int limit = 10);
    Task<List<LiquidityPool>> GetTopPoolsByVolumeAsync(int limit = 10);
}

/// <summary>
/// Farming service for yield farming
/// </summary>
public interface IFarmService
{
    Task<List<Farm>> GetAllFarmsAsync();
    Task<List<Farm>> GetActiveFarmsAsync();
    Task<Farm?> GetFarmByIdAsync(string farmId);
    Task<Farm> CreateFarmAsync(Farm farm);
    Task<Farm> UpdateFarmAsync(Farm farm);
    Task<bool> EndFarmAsync(string farmId);
    
    // User operations
    Task<FarmTransaction> StakeAsync(FarmActionRequest request);
    Task<FarmTransaction> UnstakeAsync(FarmActionRequest request);
    Task<FarmTransaction> HarvestAsync(FarmActionRequest request);
    Task<FarmTransaction> CompoundAsync(FarmActionRequest request);
    
    // User positions
    Task<List<FarmPosition>> GetUserPositionsAsync(string userId);
    Task<FarmPosition?> GetUserPositionAsync(string userId, string farmId);
    Task<decimal> CalculatePendingRewardsAsync(string userId, string farmId);
    Task<List<FarmTransaction>> GetUserFarmHistoryAsync(string userId, int limit = 50);
    
    // Analytics
    Task<List<Farm>> GetTopFarmsByAPRAsync(int limit = 10);
    Task<List<Farm>> GetTopFarmsByTVLAsync(int limit = 10);
}

/// <summary>
/// Admin configuration service
/// </summary>
public interface IConfigService
{
    Task<NET10Config> GetConfigAsync();
    Task<NET10Config> UpdateConfigAsync(NET10Config config);
    Task<AdminStats> GetStatsAsync();
    Task<List<DeployedContract>> GetDeployedContractsAsync();
    Task<DeployedContract> DeployContractAsync(ContractDeployRequest request);
}

/// <summary>
/// Price feed and analytics service
/// </summary>
public interface IPriceService
{
    Task<decimal> GetPriceUsdAsync(string tokenId);
    Task<Dictionary<string, decimal>> GetPricesUsdAsync(string[] tokenIds);
    Task<List<PricePoint>> GetPriceHistoryAsync(string tokenId, string interval, int limit);
    Task<decimal> GetTVLAsync();
    Task<decimal> GetVolume24hAsync();
}

/// <summary>
/// Contribution graph service (GitHub-style activity tracking)
/// </summary>
public interface IContributionService
{
    // Get contribution graph data
    Task<ContributionGraph> GetContributionGraphAsync(string userId, int year);
    Task<ContributionGraph> GetContributionGraphAsync(string userId);
    Task<ContributionStats> GetContributionStatsAsync(string userId);
    
    // Daily contributions
    Task<List<DailyContribution>> GetDailyContributionsAsync(string userId, DateTime startDate, DateTime endDate);
    Task<DailyContribution> GetContributionsForDateAsync(string userId, DateTime date);
    
    // Individual contributions
    Task<List<Contribution>> GetUserContributionsAsync(string userId, int limit = 100);
    Task<List<Contribution>> GetProjectContributionsAsync(string projectId, int limit = 100);
    Task<Contribution?> GetContributionByIdAsync(string contributionId);
    Task<Contribution> AddContributionAsync(Contribution contribution);
    Task<List<Contribution>> AddContributionsBatchAsync(List<Contribution> contributions);
    Task<bool> DeleteContributionAsync(string contributionId);
    
    // Project activity
    Task<List<ProjectActivity>> GetUserProjectsAsync(string userId);
    Task<ProjectActivity?> GetProjectActivityAsync(string projectId);
    
    // Leaderboards
    Task<List<ContributionStats>> GetTopContributorsAsync(int limit = 10);
    Task<List<ContributionStats>> GetTopContributorsThisMonthAsync(int limit = 10);
    Task<List<ContributionStats>> GetTopContributorsThisWeekAsync(int limit = 10);
}
