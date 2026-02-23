// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Data Transfer Objects
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

namespace Ierahkwa.ForexInvestment.Application.DTOs;

#region Account DTOs

public class ForexAccountDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string BrokerId { get; set; } = string.Empty;
    public string BrokerName { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string Leverage { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal Equity { get; set; }
    public decimal Margin { get; set; }
    public decimal FreeMargin { get; set; }
    public decimal MarginLevel { get; set; }
    public decimal Profit { get; set; }
    public decimal TotalDeposited { get; set; }
    public decimal TotalWithdrawn { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsSignalSubscribed { get; set; }
    public Guid? SignalProviderId { get; set; }
    public decimal RiskPerTrade { get; set; }
    public int MaxOpenTrades { get; set; }
    public bool AutoTrading { get; set; }
    public bool CopyTrading { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastSyncAt { get; set; }
}

public class CreateAccountRequest
{
    public Guid UserId { get; set; }
    public string BrokerId { get; set; } = string.Empty;
    public string BrokerName { get; set; } = string.Empty;
    public Ierahkwa.ForexInvestment.Domain.Entities.AccountType Type { get; set; }
    public string? Platform { get; set; }
    public string? Leverage { get; set; }
    public string? Currency { get; set; }
}

public class UpdateAccountRequest
{
    public string? Leverage { get; set; }
    public decimal? RiskPerTrade { get; set; }
    public int? MaxOpenTrades { get; set; }
    public bool? AutoTrading { get; set; }
    public bool? CopyTrading { get; set; }
}

public class BrokerAccountData
{
    public string AccountNumber { get; set; } = string.Empty;
    public string ServerAddress { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal Equity { get; set; }
    public decimal Margin { get; set; }
    public decimal FreeMargin { get; set; }
    public decimal MarginLevel { get; set; }
    public decimal Profit { get; set; }
}

#endregion

#region Investment DTOs

public class InvestmentDto
{
    public Guid Id { get; set; }
    public string InvestmentNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
    public Guid PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public Guid DurationId { get; set; }
    public string DurationName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal ROIPercentage { get; set; }
    public decimal ExpectedProfit { get; set; }
    public decimal ActualProfit { get; set; }
    public decimal TotalReturn { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Result { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal Progress { get; set; }
}

public class CreateInvestmentRequest
{
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
    public Guid PlanId { get; set; }
    public Guid DurationId { get; set; }
    public decimal Amount { get; set; }
    public bool TermsAccepted { get; set; }
    public bool AutoReinvest { get; set; }
    public string? IpAddress { get; set; }
    public string? DeviceFingerprint { get; set; }
}

public class CompleteInvestmentRequest
{
    public Ierahkwa.ForexInvestment.Domain.Entities.InvestmentResult Result { get; set; }
    public decimal ActualProfit { get; set; }
    public string? Note { get; set; }
}

public class InvestmentQueryParams
{
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

#endregion

#region Plan DTOs

public class InvestmentPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal MinROI { get; set; }
    public decimal MaxROI { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsTrending { get; set; }
    public bool IsFeatured { get; set; }
    public string? ImageUrl { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public List<InvestmentDurationDto> Durations { get; set; } = new();
}

public class InvestmentDurationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Value { get; set; }
    public int TotalHours { get; set; }
    public decimal ROIBonus { get; set; }
}

#endregion

#region Signal DTOs

public class SignalProviderDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsVerified { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal WinRate { get; set; }
    public int TotalTrades { get; set; }
    public decimal AverageROI { get; set; }
    public decimal MaxDrawdown { get; set; }
    public decimal MonthlyFee { get; set; }
    public decimal PerformanceFee { get; set; }
    public string TradingStyle { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public int CurrentSubscribers { get; set; }
}

public class TradingSignalDto
{
    public Guid Id { get; set; }
    public Guid ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public decimal EntryPrice { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit1 { get; set; }
    public decimal? TakeProfit2 { get; set; }
    public decimal? TakeProfit3 { get; set; }
    public decimal RiskRewardRatio { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Result { get; set; }
    public string? Analysis { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class SignalSubscriptionRequest
{
    public bool AutoCopy { get; set; } = true;
    public decimal LotMultiplier { get; set; } = 1.0m;
    public decimal MaxRiskPerTrade { get; set; } = 2.0m;
    public int MaxConcurrentTrades { get; set; } = 5;
}

#endregion

#region Transaction DTOs

public class ForexTransactionDto
{
    public Guid Id { get; set; }
    public string TransactionNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal Fee { get; set; }
    public decimal NetAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class DepositRequest
{
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Method { get; set; } = string.Empty;
    public string? BlockchainNetwork { get; set; }
    public string? IpAddress { get; set; }
    public string? DeviceFingerprint { get; set; }
}

public class WithdrawalRequest
{
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? WithdrawalAddress { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankName { get; set; }
    public string? SwiftCode { get; set; }
    public string? IpAddress { get; set; }
    public string? DeviceFingerprint { get; set; }
}

#endregion

#region Analytics DTOs

public class UserAnalyticsDto
{
    public Guid UserId { get; set; }
    public decimal TotalInvested { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal TotalReturn { get; set; }
    public decimal ROIPercentage { get; set; }
    public int ActiveInvestments { get; set; }
    public int CompletedInvestments { get; set; }
    public int WinCount { get; set; }
    public int LossCount { get; set; }
    public decimal WinRate { get; set; }
    public decimal TotalDeposited { get; set; }
    public decimal TotalWithdrawn { get; set; }
}

public class AdminAnalyticsDto
{
    public decimal TotalInvestedPlatform { get; set; }
    public decimal TotalProfitPlatform { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalInvestments { get; set; }
    public int ActiveInvestments { get; set; }
    public decimal TotalDeposits { get; set; }
    public decimal TotalWithdrawals { get; set; }
    public Dictionary<string, decimal> InvestmentsByPlan { get; set; } = new();
    public List<TimeSeriesData> InvestmentGrowth { get; set; } = new();
}

public class TimeSeriesData
{
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public string Label { get; set; } = string.Empty;
}

#endregion

#region Fraud Detection DTOs

public class FraudAnalysisRequest
{
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string? IpAddress { get; set; }
    public string? DeviceFingerprint { get; set; }
    public string? WithdrawalAddress { get; set; }
}

public class FraudAnalysisResult
{
    public string TransactionType { get; set; } = string.Empty;
    public decimal RiskScore { get; set; }
    public List<RiskFactor> RiskFactors { get; set; } = new();
    public bool RequiresReview { get; set; }
    public bool ShouldBlock { get; set; }
    public string Recommendation { get; set; } = string.Empty;
    public DateTime AnalyzedAt { get; set; }
    public string? Error { get; set; }
}

public class RiskFactor
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string Severity { get; set; } = string.Empty;
}

public class UserRiskProfile
{
    public Guid UserId { get; set; }
    public decimal OverallRiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public int TotalTransactions { get; set; }
    public int FlaggedTransactions { get; set; }
    public decimal TotalVolume { get; set; }
    public decimal AverageTransactionSize { get; set; }
    public TimeSpan AccountAge { get; set; }
    public DateTime AnalyzedAt { get; set; }
}

#endregion

#region Service Result

public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string Error { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    
    public static ServiceResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static ServiceResult<T> Failure(string error) => new() { IsSuccess = false, Error = error };
    public static ServiceResult<T> Failure(List<string> errors) => new() { IsSuccess = false, Errors = errors };
}

#endregion
