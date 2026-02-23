using IDOFactory.Core.Interfaces;
using IDOFactory.Core.Models;

namespace IDOFactory.Infrastructure.Services;

public class TokenLockerService : ITokenLockerService
{
    private readonly List<TokenLocker> _lockers = new();

    public TokenLockerService()
    {
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        _lockers.Add(new TokenLocker
        {
            Id = "lock-001",
            Name = "IGT-GOV Team Lock",
            Description = "Team tokens locked for 24 months",
            TokenSymbol = "IGT-GOV",
            TokenName = "Ierahkwa Governance Token",
            TokenAddress = "0x1234567890abcdef1234567890abcdef12345678",
            TokenDecimals = 18,
            Amount = 10000000,
            AmountUnlocked = 0,
            LockDate = DateTime.UtcNow.AddDays(-30),
            UnlockDate = DateTime.UtcNow.AddMonths(24),
            IsLinearVesting = true,
            VestingPeriodDays = 730,
            LockType = TokenLockType.Team,
            OwnerAddress = "0xOwner123",
            BeneficiaryAddress = "0xBeneficiary123",
            ChainId = 777777,
            Network = "ierahkwa-mainnet",
            Status = TokenLockStatus.Active
        });

        _lockers.Add(new TokenLocker
        {
            Id = "lock-002",
            Name = "IDEFI Liquidity Lock",
            Description = "Liquidity tokens locked for 12 months",
            TokenSymbol = "IDEFI-LP",
            TokenName = "Ierahkwa DeFi LP",
            TokenAddress = "0xabcdef1234567890abcdef1234567890abcdef12",
            TokenDecimals = 18,
            Amount = 5000000,
            AmountUnlocked = 0,
            LockDate = DateTime.UtcNow.AddDays(-10),
            UnlockDate = DateTime.UtcNow.AddMonths(12),
            IsLinearVesting = false,
            LockType = TokenLockType.Liquidity,
            OwnerAddress = "0xOwner456",
            BeneficiaryAddress = "0xOwner456",
            ChainId = 777777,
            Network = "ierahkwa-mainnet",
            Status = TokenLockStatus.Active
        });

        _lockers.Add(new TokenLocker
        {
            Id = "lock-003",
            Name = "Marketing Lock",
            Description = "Marketing tokens with 6-month cliff",
            TokenSymbol = "IGT-MAIN",
            TokenName = "Ierahkwa Main Token",
            TokenAddress = "0x567890abcdef1234567890abcdef1234567890ab",
            TokenDecimals = 18,
            Amount = 2000000,
            AmountUnlocked = 500000,
            LockDate = DateTime.UtcNow.AddMonths(-8),
            UnlockDate = DateTime.UtcNow.AddMonths(4),
            IsLinearVesting = true,
            VestingPeriodDays = 365,
            LockType = TokenLockType.Marketing,
            OwnerAddress = "0xOwner789",
            BeneficiaryAddress = "0xMarketing789",
            ChainId = 777777,
            Network = "ierahkwa-mainnet",
            Status = TokenLockStatus.PartiallyUnlocked
        });
    }

    public Task<IEnumerable<TokenLocker>> GetAllLockersAsync() =>
        Task.FromResult<IEnumerable<TokenLocker>>(_lockers.ToList());

    public Task<IEnumerable<TokenLocker>> GetLockersByOwnerAsync(string ownerAddress) =>
        Task.FromResult<IEnumerable<TokenLocker>>(_lockers.Where(l => l.OwnerAddress == ownerAddress).ToList());

    public Task<IEnumerable<TokenLocker>> GetLockersByTokenAsync(string tokenAddress) =>
        Task.FromResult<IEnumerable<TokenLocker>>(_lockers.Where(l => l.TokenAddress == tokenAddress).ToList());

    public Task<TokenLocker?> GetLockerByIdAsync(string lockerId) =>
        Task.FromResult(_lockers.FirstOrDefault(l => l.Id == lockerId));

    public Task<TokenLocker> CreateLockAsync(TokenLocker locker)
    {
        locker.Id = Guid.NewGuid().ToString();
        locker.CreatedAt = DateTime.UtcNow;
        locker.UpdatedAt = DateTime.UtcNow;
        locker.Status = TokenLockStatus.Active;
        locker.LockerContractAddress = $"0xLocker{Guid.NewGuid():N}".Substring(0, 42);
        
        locker.Transactions.Add(new TokenLockTransaction
        {
            LockerId = locker.Id,
            Type = TokenLockTransactionType.Lock,
            Amount = locker.Amount,
            TransactionHash = $"0x{Guid.NewGuid():N}"
        });
        
        _lockers.Add(locker);
        return Task.FromResult(locker);
    }

    public Task<TokenLocker> UnlockTokensAsync(string lockerId, string ownerAddress, decimal amount)
    {
        var locker = _lockers.FirstOrDefault(l => l.Id == lockerId && l.OwnerAddress == ownerAddress)
            ?? throw new ArgumentException("Locker not found or unauthorized");
        
        var unlockable = GetUnlockableAmountSync(locker);
        if (amount > unlockable)
            throw new InvalidOperationException($"Can only unlock {unlockable} tokens at this time");
        
        locker.AmountUnlocked += amount;
        locker.UpdatedAt = DateTime.UtcNow;
        
        if (locker.AmountUnlocked >= locker.Amount)
        {
            locker.Status = TokenLockStatus.FullyUnlocked;
            locker.WithdrawnAt = DateTime.UtcNow;
        }
        else
        {
            locker.Status = TokenLockStatus.PartiallyUnlocked;
        }
        
        locker.Transactions.Add(new TokenLockTransaction
        {
            LockerId = locker.Id,
            Type = TokenLockTransactionType.Unlock,
            Amount = amount,
            TransactionHash = $"0x{Guid.NewGuid():N}"
        });
        
        return Task.FromResult(locker);
    }

    public Task<TokenLocker> TransferOwnershipAsync(string lockerId, string currentOwner, string newOwner)
    {
        var locker = _lockers.FirstOrDefault(l => l.Id == lockerId && l.OwnerAddress == currentOwner)
            ?? throw new ArgumentException("Locker not found or unauthorized");
        
        locker.OwnerAddress = newOwner;
        locker.UpdatedAt = DateTime.UtcNow;
        
        locker.Transactions.Add(new TokenLockTransaction
        {
            LockerId = locker.Id,
            Type = TokenLockTransactionType.TransferOwnership,
            Amount = 0,
            TransactionHash = $"0x{Guid.NewGuid():N}"
        });
        
        return Task.FromResult(locker);
    }

    public Task<TokenLocker> EmergencyWithdrawAsync(string lockerId, string ownerAddress)
    {
        var locker = _lockers.FirstOrDefault(l => l.Id == lockerId && l.OwnerAddress == ownerAddress)
            ?? throw new ArgumentException("Locker not found or unauthorized");
        
        // Emergency withdraw with 10% penalty
        var remaining = locker.Amount - locker.AmountUnlocked;
        var penalty = remaining * 0.10m;
        var withdrawAmount = remaining - penalty;
        
        locker.AmountUnlocked = locker.Amount;
        locker.Status = TokenLockStatus.FullyUnlocked;
        locker.WithdrawnAt = DateTime.UtcNow;
        locker.UpdatedAt = DateTime.UtcNow;
        
        locker.Transactions.Add(new TokenLockTransaction
        {
            LockerId = locker.Id,
            Type = TokenLockTransactionType.EmergencyWithdraw,
            Amount = withdrawAmount,
            TransactionHash = $"0x{Guid.NewGuid():N}"
        });
        
        return Task.FromResult(locker);
    }

    public Task<decimal> GetUnlockableAmountAsync(string lockerId)
    {
        var locker = _lockers.FirstOrDefault(l => l.Id == lockerId)
            ?? throw new ArgumentException("Locker not found");
        return Task.FromResult(GetUnlockableAmountSync(locker));
    }

    private decimal GetUnlockableAmountSync(TokenLocker locker)
    {
        if (DateTime.UtcNow < locker.LockDate)
            return 0;
        
        if (DateTime.UtcNow >= locker.UnlockDate)
            return locker.Amount - locker.AmountUnlocked;
        
        if (!locker.IsLinearVesting)
            return 0; // Cliff lock - nothing until unlock date
        
        // Linear vesting calculation
        var totalDuration = (locker.UnlockDate - locker.LockDate).TotalDays;
        var elapsed = (DateTime.UtcNow - locker.LockDate).TotalDays;
        var vestedPercentage = (decimal)(elapsed / totalDuration);
        var totalVested = locker.Amount * vestedPercentage;
        
        return Math.Max(0, totalVested - locker.AmountUnlocked);
    }

    public Task<DateTime> GetNextUnlockDateAsync(string lockerId)
    {
        var locker = _lockers.FirstOrDefault(l => l.Id == lockerId)
            ?? throw new ArgumentException("Locker not found");
        
        if (locker.Status == TokenLockStatus.FullyUnlocked)
            return Task.FromResult(DateTime.MinValue);
        
        if (!locker.IsLinearVesting)
            return Task.FromResult(locker.UnlockDate);
        
        // For linear vesting, next unlock is immediate if there's unlockable amount
        var unlockable = GetUnlockableAmountSync(locker);
        if (unlockable > 0)
            return Task.FromResult(DateTime.UtcNow);
        
        return Task.FromResult(locker.UnlockDate);
    }

    public Task<TokenLockerStatistics> GetStatisticsAsync()
    {
        return Task.FromResult(new TokenLockerStatistics
        {
            TotalLocks = _lockers.Count,
            TotalValueLocked = _lockers.Sum(l => l.Amount - l.AmountUnlocked),
            ActiveLocks = _lockers.Count(l => l.Status == TokenLockStatus.Active || l.Status == TokenLockStatus.PartiallyUnlocked),
            UniqueTokensLocked = _lockers.Select(l => l.TokenAddress).Distinct().Count()
        });
    }
}
