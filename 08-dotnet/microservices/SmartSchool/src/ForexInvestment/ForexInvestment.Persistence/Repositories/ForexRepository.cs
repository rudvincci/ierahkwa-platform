// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Repository Implementation
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

using Microsoft.EntityFrameworkCore;
using Ierahkwa.ForexInvestment.Application.DTOs;
using Ierahkwa.ForexInvestment.Application.Interfaces;
using Ierahkwa.ForexInvestment.Domain.Entities;

namespace Ierahkwa.ForexInvestment.Persistence.Repositories;

/// <summary>
/// Repository implementation for Forex Investment System
/// </summary>
public class ForexRepository : IForexRepository
{
    private readonly ForexDbContext _context;
    
    public ForexRepository(ForexDbContext context)
    {
        _context = context;
    }
    
    #region Accounts
    
    public async Task<ForexAccount?> GetAccountByIdAsync(Guid accountId)
    {
        return await _context.ForexAccounts
            .FirstOrDefaultAsync(a => a.Id == accountId);
    }
    
    public async Task<IEnumerable<ForexAccount>> GetAccountsByUserIdAsync(Guid userId)
    {
        return await _context.ForexAccounts
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
    
    public async Task AddAccountAsync(ForexAccount account)
    {
        await _context.ForexAccounts.AddAsync(account);
    }
    
    public async Task UpdateAccountAsync(ForexAccount account)
    {
        _context.ForexAccounts.Update(account);
        await Task.CompletedTask;
    }
    
    #endregion
    
    #region Investments
    
    public async Task<Investment?> GetInvestmentByIdAsync(Guid investmentId)
    {
        return await _context.Investments
            .Include(i => i.Plan)
            .Include(i => i.Duration)
            .FirstOrDefaultAsync(i => i.Id == investmentId);
    }
    
    public async Task<IEnumerable<Investment>> GetInvestmentsByUserIdAsync(Guid userId, InvestmentQueryParams? queryParams = null)
    {
        var query = _context.Investments
            .Where(i => i.UserId == userId);
        
        if (queryParams != null)
        {
            if (!string.IsNullOrEmpty(queryParams.Status) && Enum.TryParse<InvestmentStatus>(queryParams.Status, out var status))
            {
                query = query.Where(i => i.Status == status);
            }
            
            if (queryParams.StartDate.HasValue)
            {
                query = query.Where(i => i.CreatedAt >= queryParams.StartDate.Value);
            }
            
            if (queryParams.EndDate.HasValue)
            {
                query = query.Where(i => i.CreatedAt <= queryParams.EndDate.Value);
            }
        }
        
        return await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip(((queryParams?.Page ?? 1) - 1) * (queryParams?.PageSize ?? 20))
            .Take(queryParams?.PageSize ?? 20)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Investment>> GetActiveInvestmentsAsync(Guid userId)
    {
        return await _context.Investments
            .Where(i => i.UserId == userId && i.Status == InvestmentStatus.Active)
            .OrderByDescending(i => i.StartDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Investment>> GetRecentInvestmentsAsync(Guid userId, TimeSpan period)
    {
        var cutoff = DateTime.UtcNow.Subtract(period);
        return await _context.Investments
            .Where(i => i.UserId == userId && i.CreatedAt >= cutoff)
            .ToListAsync();
    }
    
    public async Task AddInvestmentAsync(Investment investment)
    {
        await _context.Investments.AddAsync(investment);
    }
    
    public async Task UpdateInvestmentAsync(Investment investment)
    {
        _context.Investments.Update(investment);
        await Task.CompletedTask;
    }
    
    #endregion
    
    #region Plans
    
    public async Task<InvestmentPlan?> GetPlanByIdAsync(Guid planId)
    {
        return await _context.InvestmentPlans
            .Include(p => p.DurationMappings)
            .ThenInclude(m => m.Duration)
            .FirstOrDefaultAsync(p => p.Id == planId);
    }
    
    public async Task<IEnumerable<InvestmentPlan>> GetAllPlansAsync(bool activeOnly = true)
    {
        var query = _context.InvestmentPlans
            .Include(p => p.DurationMappings)
            .ThenInclude(m => m.Duration)
            .AsQueryable();
        
        if (activeOnly)
        {
            query = query.Where(p => p.IsActive);
        }
        
        return await query.OrderBy(p => p.SortOrder).ToListAsync();
    }
    
    public async Task<IEnumerable<InvestmentPlan>> GetTrendingPlansAsync()
    {
        return await _context.InvestmentPlans
            .Include(p => p.DurationMappings)
            .ThenInclude(m => m.Duration)
            .Where(p => p.IsActive && p.IsTrending)
            .OrderBy(p => p.SortOrder)
            .ToListAsync();
    }
    
    public async Task AddPlanAsync(InvestmentPlan plan)
    {
        await _context.InvestmentPlans.AddAsync(plan);
    }
    
    public async Task UpdatePlanAsync(InvestmentPlan plan)
    {
        _context.InvestmentPlans.Update(plan);
        await Task.CompletedTask;
    }
    
    #endregion
    
    #region Durations
    
    public async Task<InvestmentDuration?> GetDurationByIdAsync(Guid durationId)
    {
        return await _context.InvestmentDurations
            .FirstOrDefaultAsync(d => d.Id == durationId);
    }
    
    public async Task<IEnumerable<InvestmentDuration>> GetAllDurationsAsync(bool activeOnly = true)
    {
        var query = _context.InvestmentDurations.AsQueryable();
        
        if (activeOnly)
        {
            query = query.Where(d => d.IsActive);
        }
        
        return await query.OrderBy(d => d.SortOrder).ToListAsync();
    }
    
    public async Task<PlanDurationMapping?> GetPlanDurationMappingAsync(Guid planId, Guid durationId)
    {
        return await _context.PlanDurationMappings
            .FirstOrDefaultAsync(m => m.PlanId == planId && m.DurationId == durationId);
    }
    
    #endregion
    
    #region Transactions
    
    public async Task<ForexTransaction?> GetTransactionByIdAsync(Guid transactionId)
    {
        return await _context.ForexTransactions
            .FirstOrDefaultAsync(t => t.Id == transactionId);
    }
    
    public async Task<IEnumerable<ForexTransaction>> GetTransactionsByUserIdAsync(Guid userId, int? limit = null)
    {
        var query = _context.ForexTransactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt);
        
        if (limit.HasValue)
        {
            return await query.Take(limit.Value).ToListAsync();
        }
        
        return await query.ToListAsync();
    }
    
    public async Task<IEnumerable<ForexTransaction>> GetRecentDepositsAsync(Guid userId, TimeSpan period)
    {
        var cutoff = DateTime.UtcNow.Subtract(period);
        return await _context.ForexTransactions
            .Where(t => t.UserId == userId && 
                       t.Type == TransactionType.Deposit && 
                       t.CreatedAt >= cutoff)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<ForexTransaction>> GetRecentWithdrawalsAsync(Guid userId, TimeSpan period)
    {
        var cutoff = DateTime.UtcNow.Subtract(period);
        return await _context.ForexTransactions
            .Where(t => t.UserId == userId && 
                       t.Type == TransactionType.Withdrawal && 
                       t.CreatedAt >= cutoff)
            .ToListAsync();
    }
    
    public async Task<decimal> GetUserAverageDepositAsync(Guid userId)
    {
        var deposits = await _context.ForexTransactions
            .Where(t => t.UserId == userId && t.Type == TransactionType.Deposit)
            .Select(t => t.Amount)
            .ToListAsync();
        
        return deposits.Any() ? deposits.Average() : 0;
    }
    
    public async Task AddTransactionAsync(ForexTransaction transaction)
    {
        await _context.ForexTransactions.AddAsync(transaction);
    }
    
    public async Task UpdateTransactionAsync(ForexTransaction transaction)
    {
        _context.ForexTransactions.Update(transaction);
        await Task.CompletedTask;
    }
    
    #endregion
    
    #region Signals
    
    public async Task<SignalProvider?> GetSignalProviderByIdAsync(Guid providerId)
    {
        return await _context.SignalProviders
            .FirstOrDefaultAsync(p => p.Id == providerId);
    }
    
    public async Task<IEnumerable<SignalProvider>> GetAllSignalProvidersAsync(bool activeOnly = true)
    {
        var query = _context.SignalProviders.AsQueryable();
        
        if (activeOnly)
        {
            query = query.Where(p => p.IsActive);
        }
        
        return await query.OrderByDescending(p => p.Rating).ToListAsync();
    }
    
    public async Task<IEnumerable<TradingSignal>> GetActiveSignalsAsync(Guid providerId)
    {
        return await _context.TradingSignals
            .Where(s => s.ProviderId == providerId && s.Status == SignalStatus.Active)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }
    
    public async Task AddSignalSubscriptionAsync(SignalSubscription subscription)
    {
        await _context.SignalSubscriptions.AddAsync(subscription);
    }
    
    #endregion
    
    #region Withdrawal Limits
    
    public async Task<WithdrawalLimit?> GetWithdrawalLimitsAsync(Guid userId)
    {
        return await _context.WithdrawalLimits
            .FirstOrDefaultAsync(w => w.UserId == userId && w.IsActive);
    }
    
    public async Task UpdateWithdrawalLimitsAsync(WithdrawalLimit limits)
    {
        _context.WithdrawalLimits.Update(limits);
        await Task.CompletedTask;
    }
    
    #endregion
    
    #region Fraud Detection
    
    public async Task<IEnumerable<string>> GetUserKnownIpsAsync(Guid userId)
    {
        return await _context.ForexTransactions
            .Where(t => t.UserId == userId && t.IpAddress != null)
            .Select(t => t.IpAddress!)
            .Distinct()
            .ToListAsync();
    }
    
    public async Task<IEnumerable<string>> GetUserKnownDevicesAsync(Guid userId)
    {
        return await _context.ForexTransactions
            .Where(t => t.UserId == userId && t.DeviceFingerprint != null)
            .Select(t => t.DeviceFingerprint!)
            .Distinct()
            .ToListAsync();
    }
    
    public async Task<IEnumerable<string>> GetUserKnownAddressesAsync(Guid userId)
    {
        return await _context.ForexTransactions
            .Where(t => t.UserId == userId && t.WithdrawalAddress != null)
            .Select(t => t.WithdrawalAddress!)
            .Distinct()
            .ToListAsync();
    }
    
    #endregion
    
    #region Brokers
    
    public async Task<Broker?> GetBrokerByIdAsync(Guid brokerId)
    {
        return await _context.Brokers
            .FirstOrDefaultAsync(b => b.Id == brokerId);
    }
    
    public async Task<IEnumerable<Broker>> GetAllBrokersAsync(bool activeOnly = true)
    {
        var query = _context.Brokers.AsQueryable();
        
        if (activeOnly)
        {
            query = query.Where(b => b.IsActive);
        }
        
        return await query.OrderBy(b => b.SortOrder).ToListAsync();
    }
    
    #endregion
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
