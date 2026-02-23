// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Application Service: FraudDetectionService
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

using Ierahkwa.ForexInvestment.Application.DTOs;
using Ierahkwa.ForexInvestment.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Ierahkwa.ForexInvestment.Application.Services;

/// <summary>
/// Advanced fraud detection service with AI-powered risk analysis
/// </summary>
public class FraudDetectionService : IFraudDetectionService
{
    private readonly IForexRepository _repository;
    private readonly ILogger<FraudDetectionService> _logger;
    
    // Thresholds
    private const decimal HIGH_FREQUENCY_THRESHOLD_MINUTES = 5;
    private const int HIGH_FREQUENCY_COUNT = 5;
    private const decimal LARGE_TRANSACTION_THRESHOLD = 50000;
    private const decimal UNUSUAL_PATTERN_THRESHOLD = 500; // % increase
    
    public FraudDetectionService(
        IForexRepository repository,
        ILogger<FraudDetectionService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    /// <summary>
    /// Analyzes a deposit for fraud indicators
    /// </summary>
    public async Task<FraudAnalysisResult> AnalyzeDepositAsync(FraudAnalysisRequest request)
    {
        var result = new FraudAnalysisResult
        {
            TransactionType = "Deposit",
            AnalyzedAt = DateTime.UtcNow
        };
        
        var factors = new List<RiskFactor>();
        
        try
        {
            // 1. High-frequency deposit check
            var recentDeposits = await _repository.GetRecentDepositsAsync(
                request.UserId, 
                TimeSpan.FromMinutes(HIGH_FREQUENCY_THRESHOLD_MINUTES));
            
            if (recentDeposits.Count() >= HIGH_FREQUENCY_COUNT)
            {
                factors.Add(new RiskFactor
                {
                    Name = "HighFrequencyDeposit",
                    Description = $"User made {recentDeposits.Count()} deposits in last {HIGH_FREQUENCY_THRESHOLD_MINUTES} minutes",
                    Score = 25,
                    Severity = "High"
                });
            }
            
            // 2. Large transaction check
            if (request.Amount > LARGE_TRANSACTION_THRESHOLD)
            {
                factors.Add(new RiskFactor
                {
                    Name = "LargeTransaction",
                    Description = $"Transaction amount ${request.Amount:N2} exceeds threshold",
                    Score = 20,
                    Severity = "Medium"
                });
            }
            
            // 3. Unusual amount pattern
            var userAverage = await _repository.GetUserAverageDepositAsync(request.UserId);
            if (userAverage > 0 && request.Amount > userAverage * (UNUSUAL_PATTERN_THRESHOLD / 100))
            {
                factors.Add(new RiskFactor
                {
                    Name = "UnusualAmount",
                    Description = $"Amount is {(request.Amount / userAverage * 100):N0}% of user's average",
                    Score = 15,
                    Severity = "Medium"
                });
            }
            
            // 4. New account deposit
            var account = await _repository.GetAccountByIdAsync(request.AccountId);
            if (account != null && (DateTime.UtcNow - account.CreatedAt).TotalDays < 1)
            {
                factors.Add(new RiskFactor
                {
                    Name = "NewAccountDeposit",
                    Description = "Deposit to account created less than 24 hours ago",
                    Score = 10,
                    Severity = "Low"
                });
            }
            
            // 5. IP geolocation check
            if (!string.IsNullOrEmpty(request.IpAddress))
            {
                var knownIps = await _repository.GetUserKnownIpsAsync(request.UserId);
                if (knownIps.Any() && !knownIps.Contains(request.IpAddress))
                {
                    factors.Add(new RiskFactor
                    {
                        Name = "NewIPAddress",
                        Description = "Transaction from new IP address",
                        Score = 15,
                        Severity = "Medium"
                    });
                }
            }
            
            // 6. Device fingerprint check
            if (!string.IsNullOrEmpty(request.DeviceFingerprint))
            {
                var knownDevices = await _repository.GetUserKnownDevicesAsync(request.UserId);
                if (knownDevices.Any() && !knownDevices.Contains(request.DeviceFingerprint))
                {
                    factors.Add(new RiskFactor
                    {
                        Name = "NewDevice",
                        Description = "Transaction from new device",
                        Score = 10,
                        Severity = "Low"
                    });
                }
            }
            
            // Calculate total score
            result.RiskScore = Math.Min(100, factors.Sum(f => f.Score));
            result.RiskFactors = factors;
            result.RequiresReview = result.RiskScore > 50;
            result.ShouldBlock = result.RiskScore > 80;
            result.Recommendation = GetRecommendation(result.RiskScore);
            
            _logger.LogInformation(
                "Deposit fraud analysis for user {UserId}: Score={Score}, Factors={Factors}",
                request.UserId, result.RiskScore, factors.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing deposit for fraud");
            result.RiskScore = 0;
            result.Error = ex.Message;
        }
        
        return result;
    }
    
    /// <summary>
    /// Analyzes a withdrawal for fraud indicators
    /// </summary>
    public async Task<FraudAnalysisResult> AnalyzeWithdrawalAsync(FraudAnalysisRequest request)
    {
        var result = new FraudAnalysisResult
        {
            TransactionType = "Withdrawal",
            AnalyzedAt = DateTime.UtcNow
        };
        
        var factors = new List<RiskFactor>();
        
        try
        {
            // 1. High-frequency withdrawal check
            var recentWithdrawals = await _repository.GetRecentWithdrawalsAsync(
                request.UserId, 
                TimeSpan.FromMinutes(HIGH_FREQUENCY_THRESHOLD_MINUTES));
            
            if (recentWithdrawals.Count() >= 3)
            {
                factors.Add(new RiskFactor
                {
                    Name = "HighFrequencyWithdrawal",
                    Description = $"User made {recentWithdrawals.Count()} withdrawals in last {HIGH_FREQUENCY_THRESHOLD_MINUTES} minutes",
                    Score = 30,
                    Severity = "High"
                });
            }
            
            // 2. Large withdrawal check
            if (request.Amount > LARGE_TRANSACTION_THRESHOLD)
            {
                factors.Add(new RiskFactor
                {
                    Name = "LargeWithdrawal",
                    Description = $"Withdrawal amount ${request.Amount:N2} exceeds threshold",
                    Score = 25,
                    Severity = "High"
                });
            }
            
            // 3. Full balance withdrawal
            var account = await _repository.GetAccountByIdAsync(request.AccountId);
            if (account != null && request.Amount >= account.Balance * 0.9m)
            {
                factors.Add(new RiskFactor
                {
                    Name = "FullBalanceWithdrawal",
                    Description = "Withdrawal of 90% or more of account balance",
                    Score = 20,
                    Severity = "Medium"
                });
            }
            
            // 4. New withdrawal address
            if (!string.IsNullOrEmpty(request.WithdrawalAddress))
            {
                var knownAddresses = await _repository.GetUserKnownAddressesAsync(request.UserId);
                if (knownAddresses.Any() && !knownAddresses.Contains(request.WithdrawalAddress))
                {
                    factors.Add(new RiskFactor
                    {
                        Name = "NewWithdrawalAddress",
                        Description = "Withdrawal to new address",
                        Score = 20,
                        Severity = "Medium"
                    });
                }
            }
            
            // 5. Quick deposit-withdrawal pattern
            var recentDeposits = await _repository.GetRecentDepositsAsync(
                request.UserId, 
                TimeSpan.FromHours(24));
            var totalDeposited = recentDeposits.Sum(d => d.Amount);
            
            if (totalDeposited > 0 && request.Amount >= totalDeposited * 0.8m)
            {
                factors.Add(new RiskFactor
                {
                    Name = "QuickDepositWithdrawal",
                    Description = "Withdrawal of 80%+ of recent deposits within 24 hours",
                    Score = 35,
                    Severity = "Critical"
                });
            }
            
            // 6. Withdrawal limit check
            var limits = await _repository.GetWithdrawalLimitsAsync(request.UserId);
            if (limits != null)
            {
                if (limits.DailyUsed + request.Amount > limits.DailyLimit)
                {
                    factors.Add(new RiskFactor
                    {
                        Name = "DailyLimitExceeded",
                        Description = "Withdrawal would exceed daily limit",
                        Score = 40,
                        Severity = "Critical"
                    });
                }
            }
            
            // Calculate total score
            result.RiskScore = Math.Min(100, factors.Sum(f => f.Score));
            result.RiskFactors = factors;
            result.RequiresReview = result.RiskScore > 40;
            result.ShouldBlock = result.RiskScore > 70;
            result.Recommendation = GetRecommendation(result.RiskScore);
            
            _logger.LogInformation(
                "Withdrawal fraud analysis for user {UserId}: Score={Score}, Factors={Factors}",
                request.UserId, result.RiskScore, factors.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing withdrawal for fraud");
            result.RiskScore = 0;
            result.Error = ex.Message;
        }
        
        return result;
    }
    
    /// <summary>
    /// Analyzes an investment for fraud indicators
    /// </summary>
    public async Task<FraudAnalysisResult> AnalyzeInvestmentAsync(FraudAnalysisRequest request)
    {
        var result = new FraudAnalysisResult
        {
            TransactionType = "Investment",
            AnalyzedAt = DateTime.UtcNow
        };
        
        var factors = new List<RiskFactor>();
        
        try
        {
            // 1. High-frequency investment check
            var recentInvestments = await _repository.GetRecentInvestmentsAsync(
                request.UserId, 
                TimeSpan.FromHours(1));
            
            if (recentInvestments.Count() >= 10)
            {
                factors.Add(new RiskFactor
                {
                    Name = "HighFrequencyInvestment",
                    Description = $"User made {recentInvestments.Count()} investments in last hour",
                    Score = 25,
                    Severity = "High"
                });
            }
            
            // 2. Large investment check
            if (request.Amount > LARGE_TRANSACTION_THRESHOLD)
            {
                factors.Add(new RiskFactor
                {
                    Name = "LargeInvestment",
                    Description = $"Investment amount ${request.Amount:N2} exceeds threshold",
                    Score = 15,
                    Severity = "Medium"
                });
            }
            
            // 3. Multiple active investments check
            var activeInvestments = await _repository.GetActiveInvestmentsAsync(request.UserId);
            if (activeInvestments.Count() >= 20)
            {
                factors.Add(new RiskFactor
                {
                    Name = "TooManyActiveInvestments",
                    Description = $"User has {activeInvestments.Count()} active investments",
                    Score = 10,
                    Severity = "Low"
                });
            }
            
            // 4. New user rapid investment
            var account = await _repository.GetAccountByIdAsync(request.AccountId);
            if (account != null && (DateTime.UtcNow - account.CreatedAt).TotalDays < 7)
            {
                var userTotalInvested = recentInvestments.Sum(i => i.Amount) + request.Amount;
                if (userTotalInvested > 10000)
                {
                    factors.Add(new RiskFactor
                    {
                        Name = "NewUserHighVolume",
                        Description = "New user with high investment volume",
                        Score = 20,
                        Severity = "Medium"
                    });
                }
            }
            
            // Calculate total score
            result.RiskScore = Math.Min(100, factors.Sum(f => f.Score));
            result.RiskFactors = factors;
            result.RequiresReview = result.RiskScore > 50;
            result.ShouldBlock = result.RiskScore > 80;
            result.Recommendation = GetRecommendation(result.RiskScore);
            
            _logger.LogInformation(
                "Investment fraud analysis for user {UserId}: Score={Score}, Factors={Factors}",
                request.UserId, result.RiskScore, factors.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing investment for fraud");
            result.RiskScore = 0;
            result.Error = ex.Message;
        }
        
        return result;
    }
    
    /// <summary>
    /// Gets comprehensive user risk profile
    /// </summary>
    public async Task<UserRiskProfile> GetUserRiskProfileAsync(Guid userId)
    {
        var profile = new UserRiskProfile
        {
            UserId = userId,
            AnalyzedAt = DateTime.UtcNow
        };
        
        try
        {
            var transactions = await _repository.GetTransactionsByUserIdAsync(userId);
            var investments = await _repository.GetInvestmentsByUserIdAsync(userId);
            
            // Calculate metrics
            profile.TotalTransactions = transactions.Count();
            profile.FlaggedTransactions = transactions.Count(t => t.FraudFlagged);
            profile.TotalVolume = transactions.Sum(t => t.Amount);
            profile.AverageTransactionSize = profile.TotalTransactions > 0 
                ? profile.TotalVolume / profile.TotalTransactions 
                : 0;
            
            // Calculate overall risk score
            var flaggedRatio = profile.TotalTransactions > 0 
                ? (decimal)profile.FlaggedTransactions / profile.TotalTransactions 
                : 0;
            
            profile.OverallRiskScore = Math.Min(100, flaggedRatio * 100 + 
                (profile.TotalVolume > 100000 ? 20 : 0) +
                (profile.TotalTransactions > 1000 ? 10 : 0));
            
            profile.RiskLevel = profile.OverallRiskScore switch
            {
                > 70 => "Critical",
                > 50 => "High",
                > 30 => "Medium",
                > 10 => "Low",
                _ => "Minimal"
            };
            
            // Account age factor
            var accounts = await _repository.GetAccountsByUserIdAsync(userId);
            var oldestAccount = accounts.OrderBy(a => a.CreatedAt).FirstOrDefault();
            if (oldestAccount != null)
            {
                profile.AccountAge = DateTime.UtcNow - oldestAccount.CreatedAt;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting risk profile for user {UserId}", userId);
        }
        
        return profile;
    }
    
    private string GetRecommendation(decimal riskScore)
    {
        return riskScore switch
        {
            > 80 => "BLOCK: Transaction should be blocked and reviewed by compliance team",
            > 60 => "HOLD: Transaction requires manual approval before processing",
            > 40 => "REVIEW: Transaction should be flagged for post-processing review",
            > 20 => "MONITOR: Enhanced monitoring recommended for this user",
            _ => "APPROVE: Transaction can be processed normally"
        };
    }
}
