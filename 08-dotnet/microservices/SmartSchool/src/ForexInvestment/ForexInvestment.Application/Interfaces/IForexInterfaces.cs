// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Application Interfaces
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

using Ierahkwa.ForexInvestment.Application.DTOs;
using Ierahkwa.ForexInvestment.Domain.Entities;

namespace Ierahkwa.ForexInvestment.Application.Interfaces;

#region Repository Interfaces

public interface IForexRepository
{
    // Accounts
    Task<ForexAccount?> GetAccountByIdAsync(Guid accountId);
    Task<IEnumerable<ForexAccount>> GetAccountsByUserIdAsync(Guid userId);
    Task AddAccountAsync(ForexAccount account);
    Task UpdateAccountAsync(ForexAccount account);
    
    // Investments
    Task<Investment?> GetInvestmentByIdAsync(Guid investmentId);
    Task<IEnumerable<Investment>> GetInvestmentsByUserIdAsync(Guid userId, InvestmentQueryParams? queryParams = null);
    Task<IEnumerable<Investment>> GetActiveInvestmentsAsync(Guid userId);
    Task<IEnumerable<Investment>> GetRecentInvestmentsAsync(Guid userId, TimeSpan period);
    Task AddInvestmentAsync(Investment investment);
    Task UpdateInvestmentAsync(Investment investment);
    
    // Plans
    Task<InvestmentPlan?> GetPlanByIdAsync(Guid planId);
    Task<IEnumerable<InvestmentPlan>> GetAllPlansAsync(bool activeOnly = true);
    Task<IEnumerable<InvestmentPlan>> GetTrendingPlansAsync();
    Task AddPlanAsync(InvestmentPlan plan);
    Task UpdatePlanAsync(InvestmentPlan plan);
    
    // Durations
    Task<InvestmentDuration?> GetDurationByIdAsync(Guid durationId);
    Task<IEnumerable<InvestmentDuration>> GetAllDurationsAsync(bool activeOnly = true);
    Task<PlanDurationMapping?> GetPlanDurationMappingAsync(Guid planId, Guid durationId);
    
    // Transactions
    Task<ForexTransaction?> GetTransactionByIdAsync(Guid transactionId);
    Task<IEnumerable<ForexTransaction>> GetTransactionsByUserIdAsync(Guid userId, int? limit = null);
    Task<IEnumerable<ForexTransaction>> GetRecentDepositsAsync(Guid userId, TimeSpan period);
    Task<IEnumerable<ForexTransaction>> GetRecentWithdrawalsAsync(Guid userId, TimeSpan period);
    Task<decimal> GetUserAverageDepositAsync(Guid userId);
    Task AddTransactionAsync(ForexTransaction transaction);
    Task UpdateTransactionAsync(ForexTransaction transaction);
    
    // Signals
    Task<SignalProvider?> GetSignalProviderByIdAsync(Guid providerId);
    Task<IEnumerable<SignalProvider>> GetAllSignalProvidersAsync(bool activeOnly = true);
    Task<IEnumerable<TradingSignal>> GetActiveSignalsAsync(Guid providerId);
    Task AddSignalSubscriptionAsync(SignalSubscription subscription);
    
    // Withdrawal Limits
    Task<WithdrawalLimit?> GetWithdrawalLimitsAsync(Guid userId);
    Task UpdateWithdrawalLimitsAsync(WithdrawalLimit limits);
    
    // Fraud Detection
    Task<IEnumerable<string>> GetUserKnownIpsAsync(Guid userId);
    Task<IEnumerable<string>> GetUserKnownDevicesAsync(Guid userId);
    Task<IEnumerable<string>> GetUserKnownAddressesAsync(Guid userId);
    
    // Brokers
    Task<Broker?> GetBrokerByIdAsync(Guid brokerId);
    Task<IEnumerable<Broker>> GetAllBrokersAsync(bool activeOnly = true);
    
    // Save Changes
    Task<int> SaveChangesAsync();
}

#endregion

#region Service Interfaces

public interface IForexAccountService
{
    Task<ServiceResult<ForexAccountDto>> CreateAccountAsync(CreateAccountRequest request);
    Task<ServiceResult<IEnumerable<ForexAccountDto>>> GetUserAccountsAsync(Guid userId);
    Task<ServiceResult<ForexAccountDto>> GetAccountByIdAsync(Guid accountId);
    Task<ServiceResult<ForexAccountDto>> SyncAccountAsync(Guid accountId);
    Task<ServiceResult<ForexAccountDto>> UpdateAccountAsync(Guid accountId, UpdateAccountRequest request);
    Task<ServiceResult<bool>> SubscribeToSignalAsync(Guid accountId, Guid providerId, SignalSubscriptionRequest request);
}

public interface IInvestmentService
{
    Task<ServiceResult<InvestmentDto>> CreateInvestmentAsync(CreateInvestmentRequest request);
    Task<ServiceResult<IEnumerable<InvestmentDto>>> GetUserInvestmentsAsync(Guid userId, InvestmentQueryParams? queryParams = null);
    Task<ServiceResult<InvestmentDto>> GetInvestmentByIdAsync(Guid investmentId);
    Task<ServiceResult<InvestmentDto>> CompleteInvestmentAsync(Guid investmentId, CompleteInvestmentRequest request);
    Task<ServiceResult<bool>> CancelInvestmentAsync(Guid investmentId, string reason);
    Task<ServiceResult<UserAnalyticsDto>> GetUserAnalyticsAsync(Guid userId);
}

public interface IInvestmentPlanService
{
    Task<ServiceResult<IEnumerable<InvestmentPlanDto>>> GetAllPlansAsync();
    Task<ServiceResult<IEnumerable<InvestmentPlanDto>>> GetTrendingPlansAsync();
    Task<ServiceResult<InvestmentPlanDto>> GetPlanByIdAsync(Guid planId);
    Task<ServiceResult<InvestmentPlanDto>> CreatePlanAsync(CreatePlanRequest request);
    Task<ServiceResult<InvestmentPlanDto>> UpdatePlanAsync(Guid planId, UpdatePlanRequest request);
    Task<ServiceResult<bool>> DeletePlanAsync(Guid planId);
}

public interface ISignalService
{
    Task<ServiceResult<IEnumerable<SignalProviderDto>>> GetAllProvidersAsync();
    Task<ServiceResult<SignalProviderDto>> GetProviderByIdAsync(Guid providerId);
    Task<ServiceResult<IEnumerable<TradingSignalDto>>> GetActiveSignalsAsync(Guid providerId);
    Task<ServiceResult<TradingSignalDto>> CreateSignalAsync(CreateSignalRequest request);
    Task<ServiceResult<bool>> CloseSignalAsync(Guid signalId, CloseSignalRequest request);
}

public interface ITransactionService
{
    Task<ServiceResult<ForexTransactionDto>> ProcessDepositAsync(DepositRequest request);
    Task<ServiceResult<ForexTransactionDto>> ProcessWithdrawalAsync(WithdrawalRequest request);
    Task<ServiceResult<IEnumerable<ForexTransactionDto>>> GetUserTransactionsAsync(Guid userId, int? limit = null);
    Task<ServiceResult<ForexTransactionDto>> GetTransactionByIdAsync(Guid transactionId);
}

public interface IFraudDetectionService
{
    Task<FraudAnalysisResult> AnalyzeDepositAsync(FraudAnalysisRequest request);
    Task<FraudAnalysisResult> AnalyzeWithdrawalAsync(FraudAnalysisRequest request);
    Task<FraudAnalysisResult> AnalyzeInvestmentAsync(FraudAnalysisRequest request);
    Task<UserRiskProfile> GetUserRiskProfileAsync(Guid userId);
}

public interface IBrokerIntegrationService
{
    Task<ServiceResult<BrokerAccountData>> CreateAccountAsync(CreateAccountRequest request);
    Task<ServiceResult<BrokerAccountData>> GetAccountDataAsync(string accountNumber, string brokerId);
    Task<ServiceResult<bool>> ExecuteTradeAsync(Guid accountId, TradeRequest request);
    Task<ServiceResult<bool>> CloseTradeAsync(Guid accountId, long ticketNumber);
}

public interface INotificationService
{
    Task SendInvestmentCreatedNotificationAsync(Investment investment);
    Task SendInvestmentCompletedNotificationAsync(Investment investment);
    Task SendDepositConfirmationAsync(ForexTransaction transaction);
    Task SendWithdrawalConfirmationAsync(ForexTransaction transaction);
    Task SendSignalNotificationAsync(TradingSignal signal, IEnumerable<Guid> subscriberAccountIds);
}

#endregion

#region Additional Request DTOs

public class CreatePlanRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal MinROI { get; set; }
    public decimal MaxROI { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsTrending { get; set; }
    public string RiskLevel { get; set; } = "Medium";
    public List<Guid> DurationIds { get; set; } = new();
}

public class UpdatePlanRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public decimal? MinROI { get; set; }
    public decimal? MaxROI { get; set; }
    public bool? IsTrending { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateSignalRequest
{
    public Guid ProviderId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public SignalAction Action { get; set; }
    public decimal EntryPrice { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit1 { get; set; }
    public decimal? TakeProfit2 { get; set; }
    public decimal? TakeProfit3 { get; set; }
    public decimal LotSize { get; set; }
    public string? Analysis { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class CloseSignalRequest
{
    public SignalResult Result { get; set; }
    public decimal ExitPrice { get; set; }
    public decimal? ProfitPips { get; set; }
}

public class TradeRequest
{
    public string Symbol { get; set; } = string.Empty;
    public TradeType Type { get; set; }
    public decimal LotSize { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
    public string? Comment { get; set; }
    public int MagicNumber { get; set; }
}

#endregion
