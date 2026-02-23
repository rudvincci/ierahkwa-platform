using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks.Interfaces;

public interface IFutureWampumXClient
{
    Task<List<PoolInfo>> GetPoolsAsync();
    Task<PoolInfo?> GetPoolAsync(string poolId);
    Task<List<SwapInfo>> GetSwapsAsync();
    Task<List<MultiCurrencyWalletInfo>> GetMultiCurrencyWalletsAsync();
    Task<List<ExchangeOrderInfo>> GetExchangeOrdersAsync();
    Task<List<TradingPairInfo>> GetTradingPairsAsync();
    Task<List<ExchangeRateOracleInfo>> GetExchangeRateOraclesAsync();
    Task<List<CryptoOrderInfo>> GetCryptoOrdersAsync();
    Task<List<CustodyAccountInfo>> GetCustodyAccountsAsync();
    Task<List<StakingInfo>> GetStakingInfoAsync();
    Task<List<AccountMappingInfo>> GetAccountMappingsAsync();
    Task<List<BridgedIdentityInfo>> GetBridgedIdentitiesAsync();
    Task<List<BridgedTransactionInfo>> GetBridgedTransactionsAsync();
    Task<List<TravelRuleInfo>> GetTravelRuleTransactionsAsync();
    Task<List<VASPDirectoryInfo>> GetVASPDirectoryAsync();
    Task<List<TrustLineInfo>> GetTrustLinesAsync();
}

