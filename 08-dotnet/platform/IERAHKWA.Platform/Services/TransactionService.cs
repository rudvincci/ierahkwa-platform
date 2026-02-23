using System.Security.Cryptography;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Services;

public interface ITransactionService
{
    Task<Transaction> CreateTransactionAsync(TransactionRequest request);
    Task<Transaction?> GetTransactionAsync(string id);
    Task<List<Transaction>> GetWalletTransactionsAsync(string walletId, int page, int limit);
    Task<Transaction> TransferAsync(TransferRequest request);
    Task<List<Transaction>> GetHistoryAsync(DateTime? from, DateTime? to, int limit);
    Task<TransactionStats> GetStatsAsync();
    Task<TransactionStatus> GetStatusAsync(string txHash);
    Task<Transaction> CancelAsync(string id);
}

public class TransactionService : ITransactionService
{
    private readonly ILogger<TransactionService> _logger;
    private static readonly List<Transaction> _transactions = new();
    private static readonly object _lock = new();

    public TransactionService(ILogger<TransactionService> logger)
    {
        _logger = logger;
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        lock (_lock)
        {
            if (_transactions.Count == 0)
            {
                // Transacciones de ejemplo
                _transactions.AddRange(new[]
                {
                    new Transaction
                    {
                        Id = "tx_001",
                        TxHash = GenerateTxHash(),
                        FromWallet = "wallet_pm_001",
                        ToWallet = "wallet_treasury_001",
                        Amount = 50000,
                        Currency = "IGT",
                        Status = "confirmed",
                        Type = "transfer",
                        Confirmations = 24,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        ConfirmedAt = DateTime.UtcNow.AddDays(-5),
                        BlockNumber = "1234567"
                    },
                    new Transaction
                    {
                        Id = "tx_002",
                        TxHash = GenerateTxHash(),
                        FromWallet = "wallet_gov_001",
                        ToWallet = "wallet_pm_001",
                        Amount = 25000,
                        Currency = "IGT-GOV",
                        Status = "confirmed",
                        Type = "payment",
                        Confirmations = 18,
                        CreatedAt = DateTime.UtcNow.AddDays(-3),
                        ConfirmedAt = DateTime.UtcNow.AddDays(-3),
                        BlockNumber = "1234890"
                    },
                    new Transaction
                    {
                        Id = "tx_003",
                        TxHash = GenerateTxHash(),
                        FromWallet = "wallet_user_001",
                        ToWallet = "wallet_tradex_001",
                        Amount = 1000,
                        Currency = "IGT",
                        Status = "pending",
                        Type = "swap",
                        Confirmations = 3,
                        CreatedAt = DateTime.UtcNow.AddHours(-1)
                    }
                });
            }
        }
    }

    public async Task<Transaction> CreateTransactionAsync(TransactionRequest request)
    {
        await Task.Delay(100); // Simular latencia de red

        var transaction = new Transaction
        {
            Id = $"tx_{Guid.NewGuid().ToString("N")[..8]}",
            TxHash = GenerateTxHash(),
            FromWallet = request.FromWallet,
            ToWallet = request.ToWallet,
            Amount = request.Amount,
            Currency = request.Currency,
            Fee = CalculateFee(request.Amount),
            Status = "pending",
            Type = request.Type,
            Memo = request.Memo,
            Confirmations = 0,
            CreatedAt = DateTime.UtcNow
        };

        lock (_lock)
        {
            _transactions.Add(transaction);
        }

        _logger.LogInformation("Transaction created: {TxId} - {Amount} {Currency}", 
            transaction.Id, transaction.Amount, transaction.Currency);

        // Simular confirmación automática después de crear
        _ = SimulateConfirmation(transaction.Id);

        return transaction;
    }

    public async Task<Transaction?> GetTransactionAsync(string id)
    {
        await Task.Delay(50);
        lock (_lock)
        {
            return _transactions.FirstOrDefault(t => t.Id == id || t.TxHash == id);
        }
    }

    public async Task<List<Transaction>> GetWalletTransactionsAsync(string walletId, int page, int limit)
    {
        await Task.Delay(50);
        lock (_lock)
        {
            return _transactions
                .Where(t => t.FromWallet == walletId || t.ToWallet == walletId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();
        }
    }

    public async Task<Transaction> TransferAsync(TransferRequest request)
    {
        await Task.Delay(150); // Simular procesamiento

        // Validar fondos (en producción se verificaría contra el balance real)
        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be positive");

        var transaction = new Transaction
        {
            Id = $"tx_{Guid.NewGuid().ToString("N")[..8]}",
            TxHash = GenerateTxHash(),
            FromWallet = request.FromWalletId,
            ToWallet = request.ToAddress,
            Amount = request.Amount,
            Currency = request.Currency,
            Fee = CalculateFee(request.Amount),
            Status = "processing",
            Type = "transfer",
            Memo = request.Memo,
            Confirmations = 0,
            CreatedAt = DateTime.UtcNow
        };

        lock (_lock)
        {
            _transactions.Add(transaction);
        }

        _logger.LogInformation("Transfer initiated: {TxId} - {Amount} {Currency} from {From} to {To}",
            transaction.Id, transaction.Amount, transaction.Currency, 
            request.FromWalletId, request.ToAddress);

        // Simular confirmación
        _ = SimulateConfirmation(transaction.Id);

        return transaction;
    }

    public async Task<List<Transaction>> GetHistoryAsync(DateTime? from, DateTime? to, int limit)
    {
        await Task.Delay(50);
        
        from ??= DateTime.UtcNow.AddMonths(-1);
        to ??= DateTime.UtcNow;

        lock (_lock)
        {
            return _transactions
                .Where(t => t.CreatedAt >= from && t.CreatedAt <= to)
                .OrderByDescending(t => t.CreatedAt)
                .Take(limit)
                .ToList();
        }
    }

    public async Task<TransactionStats> GetStatsAsync()
    {
        await Task.Delay(50);
        
        var today = DateTime.UtcNow.Date;
        
        lock (_lock)
        {
            var todayTxs = _transactions.Where(t => t.CreatedAt.Date == today).ToList();
            
            return new TransactionStats
            {
                TotalTransactions = _transactions.Count,
                TotalVolume = _transactions.Sum(t => t.Amount),
                TodayTransactions = todayTxs.Count,
                TodayVolume = todayTxs.Sum(t => t.Amount),
                AverageAmount = _transactions.Any() ? _transactions.Average(t => t.Amount) : 0,
                PendingCount = _transactions.Count(t => t.Status == "pending" || t.Status == "processing"),
                ConfirmedCount = _transactions.Count(t => t.Status == "confirmed"),
                FailedCount = _transactions.Count(t => t.Status == "failed")
            };
        }
    }

    public async Task<TransactionStatus> GetStatusAsync(string txHash)
    {
        await Task.Delay(50);
        
        Transaction? tx;
        lock (_lock)
        {
            tx = _transactions.FirstOrDefault(t => t.TxHash == txHash || t.Id == txHash);
        }

        if (tx == null)
        {
            return new TransactionStatus
            {
                TxHash = txHash,
                Status = "not_found",
                Confirmations = 0
            };
        }

        return new TransactionStatus
        {
            TxHash = tx.TxHash,
            Status = tx.Status,
            Confirmations = tx.Confirmations,
            RequiredConfirmations = 12,
            ConfirmedAt = tx.ConfirmedAt,
            BlockNumber = tx.BlockNumber
        };
    }

    public async Task<Transaction> CancelAsync(string id)
    {
        await Task.Delay(50);
        
        lock (_lock)
        {
            var tx = _transactions.FirstOrDefault(t => t.Id == id);
            if (tx == null)
                throw new ArgumentException("Transaction not found");

            if (tx.Status != "pending")
                throw new InvalidOperationException("Only pending transactions can be cancelled");

            tx.Status = "cancelled";
            return tx;
        }
    }

    private async Task SimulateConfirmation(string txId)
    {
        // Simular proceso de confirmación en blockchain
        await Task.Delay(3000); // 3 segundos para "confirmar"
        
        lock (_lock)
        {
            var tx = _transactions.FirstOrDefault(t => t.Id == txId);
            if (tx != null && tx.Status != "cancelled")
            {
                tx.Status = "confirmed";
                tx.Confirmations = 12;
                tx.ConfirmedAt = DateTime.UtcNow;
                tx.BlockNumber = new Random().Next(1000000, 9999999).ToString();
            }
        }
    }

    private static string GenerateTxHash()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return "0x" + Convert.ToHexString(bytes).ToLower();
    }

    private static decimal CalculateFee(decimal amount)
    {
        // Fee del 0.1% con mínimo de 0.01 IGT
        var fee = amount * 0.001m;
        return Math.Max(fee, 0.01m);
    }
}
