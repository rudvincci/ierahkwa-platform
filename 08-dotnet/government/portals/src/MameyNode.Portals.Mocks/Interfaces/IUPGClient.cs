using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks.Interfaces;

public interface IUPGClient
{
    Task<List<ProtocolAdapterInfo>> GetProtocolAdaptersAsync();
    Task<ProtocolAdapterInfo?> GetProtocolAdapterAsync(string adapterId);
    Task<List<RouteOption>> GetRouteOptionsAsync(string fromAccount, string toAccount, string amount, string currency);
    Task<List<POSTransactionInfo>> GetPOSTransactionsAsync();
    Task<List<MerchantSettlementInfo>> GetMerchantSettlementsAsync();
}

