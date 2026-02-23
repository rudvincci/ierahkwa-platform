using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks.Interfaces;

public interface IBIISClient
{
    Task<List<LiquidityPoolInfo>> GetLiquidityPoolsAsync();
    Task<LiquidityPoolInfo?> GetLiquidityPoolAsync(string poolId);
    Task<List<CurrencyExchangeInfo>> GetCurrencyExchangesAsync();
    Task<List<CrossBorderSettlementInfo>> GetCrossBorderSettlementsAsync();
    Task<List<InterbankChannelInfo>> GetInterbankChannelsAsync();
    Task<List<BlockchainTransparencyInfo>> GetBlockchainTransactionsAsync(int limit = 50);
    Task<List<AssetCollateralizationInfo>> GetCollateralizedAssetsAsync();
    Task<List<IdentityComplianceInfo>> GetIdentityComplianceChecksAsync();
    Task<List<ZKPPrivacyInfo>> GetZKPProofsAsync();
    Task<List<TreatyEnforcementInfo>> GetTreatyEnforcementsAsync();
    Task<List<LiquidityRiskInfo>> GetLiquidityRisksAsync();
}

