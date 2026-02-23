// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Application Service: InvestmentService
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

using Ierahkwa.ForexInvestment.Application.DTOs;
using Ierahkwa.ForexInvestment.Application.Interfaces;
using Ierahkwa.ForexInvestment.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Ierahkwa.ForexInvestment.Application.Services;

/// <summary>
/// Service for managing Forex investments
/// </summary>
public class InvestmentService : IInvestmentService
{
    private readonly IForexRepository _repository;
    private readonly IFraudDetectionService _fraudDetection;
    private readonly INotificationService _notificationService;
    private readonly ILogger<InvestmentService> _logger;
    
    // Rate limiting: max 10 investments per hour
    private static readonly Dictionary<Guid, Queue<DateTime>> _rateLimits = new();
    private const int MAX_INVESTMENTS_PER_HOUR = 10;
    
    public InvestmentService(
        IForexRepository repository,
        IFraudDetectionService fraudDetection,
        INotificationService notificationService,
        ILogger<InvestmentService> logger)
    {
        _repository = repository;
        _fraudDetection = fraudDetection;
        _notificationService = notificationService;
        _logger = logger;
    }
    
    /// <summary>
    /// Creates a new investment
    /// </summary>
    public async Task<ServiceResult<InvestmentDto>> CreateInvestmentAsync(CreateInvestmentRequest request)
    {
        try
        {
            _logger.LogInformation("Creating investment for user {UserId}, amount: {Amount}", 
                request.UserId, request.Amount);
            
            // Rate limiting check
            if (!CheckRateLimit(request.UserId))
            {
                return ServiceResult<InvestmentDto>.Failure("Rate limit exceeded. Maximum 10 investments per hour.");
            }
            
            // Get account
            var account = await _repository.GetAccountByIdAsync(request.AccountId);
            if (account == null || account.UserId != request.UserId)
            {
                return ServiceResult<InvestmentDto>.Failure("Account not found or unauthorized");
            }
            
            if (account.Status != AccountStatus.Active)
            {
                return ServiceResult<InvestmentDto>.Failure("Account is not active");
            }
            
            // Get plan
            var plan = await _repository.GetPlanByIdAsync(request.PlanId);
            if (plan == null || !plan.IsActive)
            {
                return ServiceResult<InvestmentDto>.Failure("Investment plan not found or inactive");
            }
            
            // Validate amount
            if (request.Amount < plan.MinAmount || request.Amount > plan.MaxAmount)
            {
                return ServiceResult<InvestmentDto>.Failure(
                    $"Amount must be between {plan.MinAmount} and {plan.MaxAmount} {plan.Currency}");
            }
            
            // Check account balance
            if (account.Type == AccountType.Live && account.Balance < request.Amount)
            {
                return ServiceResult<InvestmentDto>.Failure("Insufficient account balance");
            }
            
            // Get duration
            var duration = await _repository.GetDurationByIdAsync(request.DurationId);
            if (duration == null || !duration.IsActive)
            {
                return ServiceResult<InvestmentDto>.Failure("Duration not found or inactive");
            }
            
            // Validate plan-duration mapping
            var mapping = await _repository.GetPlanDurationMappingAsync(request.PlanId, request.DurationId);
            if (mapping == null || !mapping.IsActive)
            {
                return ServiceResult<InvestmentDto>.Failure("Selected duration is not available for this plan");
            }
            
            // Calculate ROI
            var roi = mapping.SpecificROI > 0 ? mapping.SpecificROI : 
                      (plan.MinROI + plan.MaxROI) / 2 + duration.ROIBonus;
            var expectedProfit = request.Amount * roi / 100;
            
            // Fraud detection
            var fraudResult = await _fraudDetection.AnalyzeInvestmentAsync(new FraudAnalysisRequest
            {
                UserId = request.UserId,
                AccountId = request.AccountId,
                Amount = request.Amount,
                IpAddress = request.IpAddress,
                DeviceFingerprint = request.DeviceFingerprint
            });
            
            if (fraudResult.RiskScore > 80)
            {
                _logger.LogWarning("High risk investment detected for user {UserId}, score: {Score}", 
                    request.UserId, fraudResult.RiskScore);
                return ServiceResult<InvestmentDto>.Failure("Transaction flagged for review");
            }
            
            // Create investment
            var investment = new Investment
            {
                UserId = request.UserId,
                AccountId = request.AccountId,
                PlanId = request.PlanId,
                DurationId = request.DurationId,
                InvestmentNumber = GenerateInvestmentNumber(),
                Amount = request.Amount,
                Currency = plan.Currency,
                AmountInUSD = request.Amount, // TODO: Convert if different currency
                ROIPercentage = roi,
                ExpectedProfit = expectedProfit,
                Status = InvestmentStatus.Active,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(duration.TotalHours),
                TermsAccepted = request.TermsAccepted,
                TermsAcceptedAt = request.TermsAccepted ? DateTime.UtcNow : null,
                TermsVersion = plan.TermsVersion,
                AutoReinvest = request.AutoReinvest,
                RiskScore = fraudResult.RiskScore,
                FraudFlagged = fraudResult.RiskScore > 50
            };
            
            // Deduct from account balance
            if (account.Type == AccountType.Live)
            {
                account.Balance -= request.Amount;
                account.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAccountAsync(account);
            }
            
            // Create transaction record
            var transaction = new ForexTransaction
            {
                UserId = request.UserId,
                AccountId = request.AccountId,
                TransactionNumber = GenerateTransactionNumber(),
                Type = TransactionType.InvestmentDeposit,
                Amount = request.Amount,
                Currency = plan.Currency,
                AmountInUSD = request.Amount,
                NetAmount = request.Amount,
                BalanceBefore = account.Balance + request.Amount,
                BalanceAfter = account.Balance,
                Status = TransactionStatus.Completed,
                InvestmentId = investment.Id,
                Description = $"Investment in {plan.Name}",
                IpAddress = request.IpAddress,
                RiskScore = fraudResult.RiskScore,
                CompletedAt = DateTime.UtcNow
            };
            
            await _repository.AddInvestmentAsync(investment);
            await _repository.AddTransactionAsync(transaction);
            await _repository.SaveChangesAsync();
            
            // Update rate limit
            UpdateRateLimit(request.UserId);
            
            // Send notification
            await _notificationService.SendInvestmentCreatedNotificationAsync(investment);
            
            _logger.LogInformation("Created investment {InvestmentNumber} for user {UserId}", 
                investment.InvestmentNumber, request.UserId);
            
            return ServiceResult<InvestmentDto>.Success(MapToDto(investment, plan, duration));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating investment");
            return ServiceResult<InvestmentDto>.Failure("Failed to create investment");
        }
    }
    
    /// <summary>
    /// Gets all investments for a user
    /// </summary>
    public async Task<ServiceResult<IEnumerable<InvestmentDto>>> GetUserInvestmentsAsync(
        Guid userId, InvestmentQueryParams? queryParams = null)
    {
        try
        {
            var investments = await _repository.GetInvestmentsByUserIdAsync(userId, queryParams);
            var plans = await _repository.GetAllPlansAsync();
            var durations = await _repository.GetAllDurationsAsync();
            
            var dtos = investments.Select(inv =>
            {
                var plan = plans.FirstOrDefault(p => p.Id == inv.PlanId);
                var duration = durations.FirstOrDefault(d => d.Id == inv.DurationId);
                return MapToDto(inv, plan, duration);
            });
            
            return ServiceResult<IEnumerable<InvestmentDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting investments for user {UserId}", userId);
            return ServiceResult<IEnumerable<InvestmentDto>>.Failure("Failed to retrieve investments");
        }
    }
    
    /// <summary>
    /// Gets investment by ID
    /// </summary>
    public async Task<ServiceResult<InvestmentDto>> GetInvestmentByIdAsync(Guid investmentId)
    {
        try
        {
            var investment = await _repository.GetInvestmentByIdAsync(investmentId);
            if (investment == null)
            {
                return ServiceResult<InvestmentDto>.Failure("Investment not found");
            }
            
            var plan = await _repository.GetPlanByIdAsync(investment.PlanId);
            var duration = await _repository.GetDurationByIdAsync(investment.DurationId);
            
            return ServiceResult<InvestmentDto>.Success(MapToDto(investment, plan, duration));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting investment {InvestmentId}", investmentId);
            return ServiceResult<InvestmentDto>.Failure("Failed to retrieve investment");
        }
    }
    
    /// <summary>
    /// Completes an investment and records the result
    /// </summary>
    public async Task<ServiceResult<InvestmentDto>> CompleteInvestmentAsync(
        Guid investmentId, CompleteInvestmentRequest request)
    {
        try
        {
            var investment = await _repository.GetInvestmentByIdAsync(investmentId);
            if (investment == null)
            {
                return ServiceResult<InvestmentDto>.Failure("Investment not found");
            }
            
            if (investment.Status != InvestmentStatus.Active)
            {
                return ServiceResult<InvestmentDto>.Failure("Investment is not active");
            }
            
            investment.Result = request.Result;
            investment.ActualProfit = request.ActualProfit;
            investment.TotalReturn = investment.Amount + request.ActualProfit;
            investment.Status = InvestmentStatus.Completed;
            investment.CompletedAt = DateTime.UtcNow;
            investment.UpdatedAt = DateTime.UtcNow;
            
            // Credit return to account
            var account = await _repository.GetAccountByIdAsync(investment.AccountId);
            if (account != null && account.Type == AccountType.Live)
            {
                account.Balance += investment.TotalReturn;
                account.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAccountAsync(account);
                
                // Create return transaction
                var transaction = new ForexTransaction
                {
                    UserId = investment.UserId,
                    AccountId = investment.AccountId,
                    TransactionNumber = GenerateTransactionNumber(),
                    Type = TransactionType.InvestmentReturn,
                    Amount = investment.TotalReturn,
                    Currency = investment.Currency,
                    AmountInUSD = investment.TotalReturn,
                    NetAmount = investment.TotalReturn,
                    BalanceBefore = account.Balance - investment.TotalReturn,
                    BalanceAfter = account.Balance,
                    Status = TransactionStatus.Completed,
                    InvestmentId = investment.Id,
                    Description = $"Investment return - {investment.Result}",
                    CompletedAt = DateTime.UtcNow
                };
                
                await _repository.AddTransactionAsync(transaction);
            }
            
            // Handle auto-reinvest
            if (investment.AutoReinvest && investment.Result == InvestmentResult.Win)
            {
                await HandleAutoReinvestAsync(investment);
            }
            
            await _repository.UpdateInvestmentAsync(investment);
            await _repository.SaveChangesAsync();
            
            // Send notification
            await _notificationService.SendInvestmentCompletedNotificationAsync(investment);
            
            var plan = await _repository.GetPlanByIdAsync(investment.PlanId);
            var duration = await _repository.GetDurationByIdAsync(investment.DurationId);
            
            return ServiceResult<InvestmentDto>.Success(MapToDto(investment, plan, duration));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing investment {InvestmentId}", investmentId);
            return ServiceResult<InvestmentDto>.Failure("Failed to complete investment");
        }
    }
    
    /// <summary>
    /// Cancels an investment
    /// </summary>
    public async Task<ServiceResult<bool>> CancelInvestmentAsync(Guid investmentId, string reason)
    {
        try
        {
            var investment = await _repository.GetInvestmentByIdAsync(investmentId);
            if (investment == null)
            {
                return ServiceResult<bool>.Failure("Investment not found");
            }
            
            if (investment.Status != InvestmentStatus.Pending && investment.Status != InvestmentStatus.Active)
            {
                return ServiceResult<bool>.Failure("Investment cannot be cancelled");
            }
            
            investment.Status = InvestmentStatus.Cancelled;
            investment.CancelledAt = DateTime.UtcNow;
            investment.CancellationReason = reason;
            investment.UpdatedAt = DateTime.UtcNow;
            
            // Refund to account if active
            if (investment.Status == InvestmentStatus.Active)
            {
                var account = await _repository.GetAccountByIdAsync(investment.AccountId);
                if (account != null && account.Type == AccountType.Live)
                {
                    account.Balance += investment.Amount;
                    account.UpdatedAt = DateTime.UtcNow;
                    await _repository.UpdateAccountAsync(account);
                    
                    var transaction = new ForexTransaction
                    {
                        UserId = investment.UserId,
                        AccountId = investment.AccountId,
                        TransactionNumber = GenerateTransactionNumber(),
                        Type = TransactionType.Refund,
                        Amount = investment.Amount,
                        Currency = investment.Currency,
                        AmountInUSD = investment.Amount,
                        NetAmount = investment.Amount,
                        BalanceBefore = account.Balance - investment.Amount,
                        BalanceAfter = account.Balance,
                        Status = TransactionStatus.Completed,
                        InvestmentId = investment.Id,
                        Description = $"Investment cancelled - {reason}",
                        CompletedAt = DateTime.UtcNow
                    };
                    
                    await _repository.AddTransactionAsync(transaction);
                }
            }
            
            await _repository.UpdateInvestmentAsync(investment);
            await _repository.SaveChangesAsync();
            
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling investment {InvestmentId}", investmentId);
            return ServiceResult<bool>.Failure("Failed to cancel investment");
        }
    }
    
    /// <summary>
    /// Gets user investment analytics
    /// </summary>
    public async Task<ServiceResult<UserAnalyticsDto>> GetUserAnalyticsAsync(Guid userId)
    {
        try
        {
            var investments = await _repository.GetInvestmentsByUserIdAsync(userId);
            var transactions = await _repository.GetTransactionsByUserIdAsync(userId);
            
            var analytics = new UserAnalyticsDto
            {
                UserId = userId,
                TotalInvested = investments.Sum(i => i.Amount),
                TotalProfit = investments.Where(i => i.Status == InvestmentStatus.Completed)
                                        .Sum(i => i.ActualProfit),
                TotalReturn = investments.Where(i => i.Status == InvestmentStatus.Completed)
                                        .Sum(i => i.TotalReturn),
                ActiveInvestments = investments.Count(i => i.Status == InvestmentStatus.Active),
                CompletedInvestments = investments.Count(i => i.Status == InvestmentStatus.Completed),
                WinCount = investments.Count(i => i.Result == InvestmentResult.Win),
                LossCount = investments.Count(i => i.Result == InvestmentResult.Loss),
                TotalDeposited = transactions.Where(t => t.Type == TransactionType.Deposit)
                                            .Sum(t => t.NetAmount),
                TotalWithdrawn = transactions.Where(t => t.Type == TransactionType.Withdrawal)
                                            .Sum(t => t.NetAmount)
            };
            
            analytics.ROIPercentage = analytics.TotalInvested > 0 
                ? (analytics.TotalProfit / analytics.TotalInvested) * 100 
                : 0;
            analytics.WinRate = (analytics.WinCount + analytics.LossCount) > 0
                ? (decimal)analytics.WinCount / (analytics.WinCount + analytics.LossCount) * 100
                : 0;
            
            return ServiceResult<UserAnalyticsDto>.Success(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting analytics for user {UserId}", userId);
            return ServiceResult<UserAnalyticsDto>.Failure("Failed to retrieve analytics");
        }
    }
    
    private async Task HandleAutoReinvestAsync(Investment originalInvestment)
    {
        // Create a new investment with the same parameters
        originalInvestment.ReinvestCount++;
        
        _logger.LogInformation("Auto-reinvesting investment {InvestmentNumber}, count: {Count}",
            originalInvestment.InvestmentNumber, originalInvestment.ReinvestCount);
    }
    
    private bool CheckRateLimit(Guid userId)
    {
        if (!_rateLimits.ContainsKey(userId))
        {
            return true;
        }
        
        var timestamps = _rateLimits[userId];
        var oneHourAgo = DateTime.UtcNow.AddHours(-1);
        
        while (timestamps.Count > 0 && timestamps.Peek() < oneHourAgo)
        {
            timestamps.Dequeue();
        }
        
        return timestamps.Count < MAX_INVESTMENTS_PER_HOUR;
    }
    
    private void UpdateRateLimit(Guid userId)
    {
        if (!_rateLimits.ContainsKey(userId))
        {
            _rateLimits[userId] = new Queue<DateTime>();
        }
        _rateLimits[userId].Enqueue(DateTime.UtcNow);
    }
    
    private string GenerateInvestmentNumber()
    {
        return $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
    
    private string GenerateTransactionNumber()
    {
        return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
    }
    
    private InvestmentDto MapToDto(Investment investment, InvestmentPlan? plan, InvestmentDuration? duration)
    {
        return new InvestmentDto
        {
            Id = investment.Id,
            InvestmentNumber = investment.InvestmentNumber,
            UserId = investment.UserId,
            AccountId = investment.AccountId,
            PlanId = investment.PlanId,
            PlanName = plan?.Name ?? "Unknown",
            DurationId = investment.DurationId,
            DurationName = duration?.Name ?? "Unknown",
            Amount = investment.Amount,
            Currency = investment.Currency,
            ROIPercentage = investment.ROIPercentage,
            ExpectedProfit = investment.ExpectedProfit,
            ActualProfit = investment.ActualProfit,
            TotalReturn = investment.TotalReturn,
            Status = investment.Status.ToString(),
            Result = investment.Result?.ToString(),
            StartDate = investment.StartDate,
            EndDate = investment.EndDate,
            CompletedAt = investment.CompletedAt,
            CreatedAt = investment.CreatedAt,
            Progress = CalculateProgress(investment)
        };
    }
    
    private decimal CalculateProgress(Investment investment)
    {
        if (investment.Status == InvestmentStatus.Completed) return 100;
        if (investment.Status != InvestmentStatus.Active) return 0;
        
        var totalDuration = (investment.EndDate - investment.StartDate).TotalMinutes;
        var elapsed = (DateTime.UtcNow - investment.StartDate).TotalMinutes;
        
        return Math.Min(100, (decimal)(elapsed / totalDuration * 100));
    }
}
