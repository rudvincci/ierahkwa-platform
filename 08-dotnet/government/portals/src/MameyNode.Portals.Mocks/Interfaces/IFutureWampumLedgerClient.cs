using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks.Interfaces;

public interface IFutureWampumLedgerClient
{
    Task<List<TransactionInfo>> GetTransactionsAsync(int limit = 50);
    Task<TransactionInfo?> GetTransactionAsync(string transactionId);
    Task<List<LedgerBlockInfo>> GetBlocksAsync(int limit = 50);
    Task<LedgerBlockInfo?> GetBlockAsync(string blockHash);
    Task<TransparencyDashboardInfo> GetTransparencyDashboardAsync();
    Task<List<AuditTrailInfo>> GetAuditTrailAsync(string entityType, string entityId);
    Task<List<SynchronizationInfo>> GetSynchronizationsAsync();
    Task<List<TransactionLogInfo>> GetTransactionLogsAsync();
    Task<List<TransactionFlagInfo>> GetTransactionFlagsAsync();
    Task<List<CurrencyInfo>> GetCurrenciesAsync();
    Task<List<CreditEntryInfo>> GetCreditHistoryAsync(string accountId);
    Task<CreditSummaryInfo?> GetCreditSummaryAsync(string accountId);
}

