using System.Collections.Concurrent;
using TradeX.Core.Interfaces;
using TradeX.Core.Models;

namespace TradeX.Infrastructure.Services;

/// <summary>
/// Wallet Service - Ierahkwa TradeX
/// Depósitos, retiros, transferencias y transacciones VIP (prioridad + descuento en fees)
/// </summary>
public class WalletService : IWalletService
{
    private readonly List<Wallet> _wallets = [];
    private readonly List<Transaction> _transactions = [];
    private readonly ConcurrentDictionary<Guid, VipLevel> _userVipLevels = new();
    private readonly object _lock = new();

    private static decimal GetVipDiscountPercent(VipLevel level) => level switch
    {
        VipLevel.Bronze => 5,
        VipLevel.Silver => 10,
        VipLevel.Gold => 15,
        VipLevel.Platinum => 20,
        VipLevel.Diamond => 25,
        VipLevel.Royal => 30,
        VipLevel.Sovereign => 50,
        _ => 0
    };

    public Task SetUserVipLevelAsync(Guid userId, VipLevel level)
    {
        _userVipLevels[userId] = level;
        return Task.CompletedTask;
    }

    public Task<Wallet> GetOrCreateWalletAsync(Guid userId, Guid assetId)
    {
        lock (_lock)
        {
            var w = _wallets.FirstOrDefault(x => x.UserId == userId && x.AssetId == assetId);
            if (w != null) return Task.FromResult(w);
            w = new Wallet
            {
                UserId = userId,
                AssetId = assetId,
                AvailableBalance = 0,
                LockedBalance = 0,
                DepositAddress = "0x" + Guid.NewGuid().ToString("N")[..40],
                IsActive = true
            };
            _wallets.Add(w);
            return Task.FromResult(w);
        }
    }

    public Task<IEnumerable<Wallet>> GetUserWalletsAsync(Guid userId)
    {
        var list = _wallets.Where(w => w.UserId == userId).ToList();
        return Task.FromResult<IEnumerable<Wallet>>(list);
    }

    public async Task<decimal> GetBalanceAsync(Guid userId, Guid assetId)
    {
        var w = await GetOrCreateWalletAsync(userId, assetId);
        return w.AvailableBalance;
    }

    public async Task<Transaction> DepositAsync(Guid userId, Guid assetId, decimal amount, string txHash)
    {
        var w = await GetOrCreateWalletAsync(userId, assetId);
        var vip = _userVipLevels.GetValueOrDefault(userId, VipLevel.None);
        var discount = GetVipDiscountPercent(vip);
        var fee = 0m; // depósito sin comisión estándar
        var isVip = vip != VipLevel.None;

        lock (_lock)
        {
            w.AvailableBalance += amount;
            w.LastTransactionAt = DateTime.UtcNow;
            var t = new Transaction
            {
                WalletId = w.Id,
                UserId = userId,
                Type = TransactionType.Deposit,
                Amount = amount,
                Fee = fee,
                TxHash = txHash,
                ToAddress = w.DepositAddress,
                Status = TransactionStatus.Confirmed,
                ConfirmedAt = DateTime.UtcNow,
                IsVipPriority = isVip,
                VipFeeDiscountPercent = discount
            };
            _transactions.Add(t);
            return t;
        }
    }

    public async Task<Transaction> WithdrawAsync(Guid userId, Guid assetId, decimal amount, string toAddress)
    {
        var w = await GetOrCreateWalletAsync(userId, assetId);
        var vip = _userVipLevels.GetValueOrDefault(userId, VipLevel.None);
        var discount = GetVipDiscountPercent(vip);
        var baseFee = amount * 0.001m; // 0.1% base
        var fee = Math.Round(baseFee * (1 - discount / 100m), 8);
        var isVip = vip != VipLevel.None;
        var total = amount + fee;

        lock (_lock)
        {
            if (w.AvailableBalance < total)
                throw new InvalidOperationException($"Saldo insuficiente. Disponible: {w.AvailableBalance}, necesario: {total} (monto + fee)");
            w.AvailableBalance -= total;
            w.LastTransactionAt = DateTime.UtcNow;
            var t = new Transaction
            {
                WalletId = w.Id,
                UserId = userId,
                Type = TransactionType.Withdrawal,
                Amount = amount,
                Fee = fee,
                ToAddress = toAddress,
                Status = TransactionStatus.Processing,
                IsVipPriority = isVip,
                VipFeeDiscountPercent = discount
            };
            _transactions.Add(t);
            return t;
        }
    }

    public async Task<Transaction> TransferAsync(Guid fromUserId, Guid toUserId, Guid assetId, decimal amount)
    {
        var wFrom = await GetOrCreateWalletAsync(fromUserId, assetId);
        var wTo = await GetOrCreateWalletAsync(toUserId, assetId);
        var vip = _userVipLevels.GetValueOrDefault(fromUserId, VipLevel.None);
        var discount = GetVipDiscountPercent(vip);
        var baseFee = amount * 0.001m;
        var fee = Math.Round(baseFee * (1 - discount / 100m), 8);
        var isVip = vip != VipLevel.None;
        var total = amount + fee;

        lock (_lock)
        {
            if (wFrom.AvailableBalance < total)
                throw new InvalidOperationException($"Saldo insuficiente para transferir. Disponible: {wFrom.AvailableBalance}, necesario: {total}");
            wFrom.AvailableBalance -= total;
            wTo.AvailableBalance += amount;
            wFrom.LastTransactionAt = DateTime.UtcNow;
            wTo.LastTransactionAt = DateTime.UtcNow;

            var tOut = new Transaction
            {
                WalletId = wFrom.Id,
                UserId = fromUserId,
                Type = TransactionType.Transfer,
                Amount = amount,
                Fee = fee,
                ToAddress = wTo.DepositAddress,
                Note = $"Transferencia a {toUserId}",
                Status = TransactionStatus.Confirmed,
                ConfirmedAt = DateTime.UtcNow,
                IsVipPriority = isVip,
                VipFeeDiscountPercent = discount
            };
            var tIn = new Transaction
            {
                WalletId = wTo.Id,
                UserId = toUserId,
                Type = TransactionType.Transfer,
                Amount = amount,
                Fee = 0,
                FromAddress = wFrom.DepositAddress,
                Note = $"Transferencia desde {fromUserId}",
                Status = TransactionStatus.Confirmed,
                ConfirmedAt = DateTime.UtcNow,
                IsVipPriority = false,
                VipFeeDiscountPercent = 0
            };
            _transactions.Add(tOut);
            _transactions.Add(tIn);
            return tOut;
        }
    }

    public Task<IEnumerable<Transaction>> GetTransactionHistoryAsync(Guid userId, int page = 1, int pageSize = 50, bool vipOnly = false)
    {
        var q = _transactions.Where(t => t.UserId == userId);
        if (vipOnly) q = q.Where(t => t.IsVipPriority);
        var list = q.OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return Task.FromResult<IEnumerable<Transaction>>(list);
    }

    public Task<IEnumerable<Transaction>> GetVipTransactionsAsync(Guid? userId, int page = 1, int pageSize = 50)
    {
        var q = _transactions.Where(t => t.IsVipPriority);
        if (userId.HasValue)
            q = q.Where(t => t.UserId == userId.Value);
        var list = q.OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return Task.FromResult<IEnumerable<Transaction>>(list);
    }
}
