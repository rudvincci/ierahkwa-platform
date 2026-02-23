// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Application Service: ForexAccountService
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

using Ierahkwa.ForexInvestment.Application.DTOs;
using Ierahkwa.ForexInvestment.Application.Interfaces;
using Ierahkwa.ForexInvestment.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Ierahkwa.ForexInvestment.Application.Services;

/// <summary>
/// Service for managing Forex trading accounts
/// </summary>
public class ForexAccountService : IForexAccountService
{
    private readonly IForexRepository _repository;
    private readonly IFraudDetectionService _fraudDetection;
    private readonly IBrokerIntegrationService _brokerService;
    private readonly ILogger<ForexAccountService> _logger;
    
    public ForexAccountService(
        IForexRepository repository,
        IFraudDetectionService fraudDetection,
        IBrokerIntegrationService brokerService,
        ILogger<ForexAccountService> logger)
    {
        _repository = repository;
        _fraudDetection = fraudDetection;
        _brokerService = brokerService;
        _logger = logger;
    }
    
    /// <summary>
    /// Creates a new Forex account for a user
    /// </summary>
    public async Task<ServiceResult<ForexAccountDto>> CreateAccountAsync(CreateAccountRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new Forex account for user {UserId}", request.UserId);
            
            // Check if user already has max accounts
            var existingAccounts = await _repository.GetAccountsByUserIdAsync(request.UserId);
            if (existingAccounts.Count() >= 5 && request.Type == AccountType.Live)
            {
                return ServiceResult<ForexAccountDto>.Failure("Maximum live accounts limit reached");
            }
            
            // Create account with broker
            var brokerAccount = await _brokerService.CreateAccountAsync(request);
            if (!brokerAccount.IsSuccess)
            {
                return ServiceResult<ForexAccountDto>.Failure(brokerAccount.Error);
            }
            
            var account = new ForexAccount
            {
                UserId = request.UserId,
                AccountNumber = brokerAccount.Data.AccountNumber,
                Type = request.Type,
                BrokerId = request.BrokerId,
                BrokerName = request.BrokerName,
                Platform = request.Platform ?? "MT5",
                ServerAddress = brokerAccount.Data.ServerAddress,
                Leverage = request.Leverage ?? "1:100",
                Currency = request.Currency ?? "USD",
                Balance = request.Type == AccountType.Demo ? 10000 : 0,
                Status = AccountStatus.Active
            };
            
            await _repository.AddAccountAsync(account);
            await _repository.SaveChangesAsync();
            
            _logger.LogInformation("Created account {AccountNumber} for user {UserId}", 
                account.AccountNumber, request.UserId);
            
            return ServiceResult<ForexAccountDto>.Success(MapToDto(account));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Forex account");
            return ServiceResult<ForexAccountDto>.Failure("Failed to create account");
        }
    }
    
    /// <summary>
    /// Gets all accounts for a user
    /// </summary>
    public async Task<ServiceResult<IEnumerable<ForexAccountDto>>> GetUserAccountsAsync(Guid userId)
    {
        try
        {
            var accounts = await _repository.GetAccountsByUserIdAsync(userId);
            var dtos = accounts.Select(MapToDto);
            return ServiceResult<IEnumerable<ForexAccountDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting accounts for user {UserId}", userId);
            return ServiceResult<IEnumerable<ForexAccountDto>>.Failure("Failed to retrieve accounts");
        }
    }
    
    /// <summary>
    /// Gets account by ID
    /// </summary>
    public async Task<ServiceResult<ForexAccountDto>> GetAccountByIdAsync(Guid accountId)
    {
        try
        {
            var account = await _repository.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return ServiceResult<ForexAccountDto>.Failure("Account not found");
            }
            return ServiceResult<ForexAccountDto>.Success(MapToDto(account));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account {AccountId}", accountId);
            return ServiceResult<ForexAccountDto>.Failure("Failed to retrieve account");
        }
    }
    
    /// <summary>
    /// Syncs account balance with broker
    /// </summary>
    public async Task<ServiceResult<ForexAccountDto>> SyncAccountAsync(Guid accountId)
    {
        try
        {
            var account = await _repository.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return ServiceResult<ForexAccountDto>.Failure("Account not found");
            }
            
            var brokerData = await _brokerService.GetAccountDataAsync(account.AccountNumber, account.BrokerId);
            if (!brokerData.IsSuccess)
            {
                return ServiceResult<ForexAccountDto>.Failure(brokerData.Error);
            }
            
            account.Balance = brokerData.Data.Balance;
            account.Equity = brokerData.Data.Equity;
            account.Margin = brokerData.Data.Margin;
            account.FreeMargin = brokerData.Data.FreeMargin;
            account.MarginLevel = brokerData.Data.MarginLevel;
            account.Profit = brokerData.Data.Profit;
            account.LastSyncAt = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;
            
            await _repository.UpdateAccountAsync(account);
            await _repository.SaveChangesAsync();
            
            return ServiceResult<ForexAccountDto>.Success(MapToDto(account));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing account {AccountId}", accountId);
            return ServiceResult<ForexAccountDto>.Failure("Failed to sync account");
        }
    }
    
    /// <summary>
    /// Updates account settings
    /// </summary>
    public async Task<ServiceResult<ForexAccountDto>> UpdateAccountAsync(Guid accountId, UpdateAccountRequest request)
    {
        try
        {
            var account = await _repository.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return ServiceResult<ForexAccountDto>.Failure("Account not found");
            }
            
            if (request.Leverage != null) account.Leverage = request.Leverage;
            if (request.RiskPerTrade.HasValue) account.RiskPerTrade = request.RiskPerTrade.Value;
            if (request.MaxOpenTrades.HasValue) account.MaxOpenTrades = request.MaxOpenTrades.Value;
            if (request.AutoTrading.HasValue) account.AutoTrading = request.AutoTrading.Value;
            if (request.CopyTrading.HasValue) account.CopyTrading = request.CopyTrading.Value;
            
            account.UpdatedAt = DateTime.UtcNow;
            
            await _repository.UpdateAccountAsync(account);
            await _repository.SaveChangesAsync();
            
            return ServiceResult<ForexAccountDto>.Success(MapToDto(account));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account {AccountId}", accountId);
            return ServiceResult<ForexAccountDto>.Failure("Failed to update account");
        }
    }
    
    /// <summary>
    /// Subscribes account to a signal provider
    /// </summary>
    public async Task<ServiceResult<bool>> SubscribeToSignalAsync(Guid accountId, Guid providerId, SignalSubscriptionRequest request)
    {
        try
        {
            var account = await _repository.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return ServiceResult<bool>.Failure("Account not found");
            }
            
            if (account.IsSignalSubscribed)
            {
                return ServiceResult<bool>.Failure("Account is already subscribed to a signal provider");
            }
            
            var subscription = new SignalSubscription
            {
                AccountId = accountId,
                ProviderId = providerId,
                AutoCopy = request.AutoCopy,
                LotMultiplier = request.LotMultiplier,
                MaxRiskPerTrade = request.MaxRiskPerTrade,
                MaxConcurrentTrades = request.MaxConcurrentTrades
            };
            
            await _repository.AddSignalSubscriptionAsync(subscription);
            
            account.IsSignalSubscribed = true;
            account.SignalProviderId = providerId;
            account.UpdatedAt = DateTime.UtcNow;
            
            await _repository.UpdateAccountAsync(account);
            await _repository.SaveChangesAsync();
            
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing account {AccountId} to provider {ProviderId}", 
                accountId, providerId);
            return ServiceResult<bool>.Failure("Failed to subscribe to signal");
        }
    }
    
    private ForexAccountDto MapToDto(ForexAccount account)
    {
        return new ForexAccountDto
        {
            Id = account.Id,
            UserId = account.UserId,
            AccountNumber = account.AccountNumber,
            Type = account.Type.ToString(),
            BrokerId = account.BrokerId,
            BrokerName = account.BrokerName,
            Platform = account.Platform,
            Leverage = account.Leverage,
            Currency = account.Currency,
            Balance = account.Balance,
            Equity = account.Equity,
            Margin = account.Margin,
            FreeMargin = account.FreeMargin,
            MarginLevel = account.MarginLevel,
            Profit = account.Profit,
            TotalDeposited = account.TotalDeposited,
            TotalWithdrawn = account.TotalWithdrawn,
            Status = account.Status.ToString(),
            IsSignalSubscribed = account.IsSignalSubscribed,
            SignalProviderId = account.SignalProviderId,
            RiskPerTrade = account.RiskPerTrade,
            MaxOpenTrades = account.MaxOpenTrades,
            AutoTrading = account.AutoTrading,
            CopyTrading = account.CopyTrading,
            CreatedAt = account.CreatedAt,
            LastSyncAt = account.LastSyncAt
        };
    }
}
