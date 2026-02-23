using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Services;

public interface IWalletService
{
    Task<Wallet> CreateWalletAsync(CreateWalletRequest request);
    Task<Wallet?> GetWalletAsync(string id);
    Task<WalletBalance> GetBalanceAsync(string id);
    Task<List<Wallet>> GetUserWalletsAsync(string userId);
    Task<Transaction> DepositAsync(string walletId, DepositRequest request);
    Task<Transaction> WithdrawAsync(string walletId, WithdrawRequest request);
    Task<List<TokenBalance>> GetTokensAsync(string walletId);
    Task<TokenBalance> AddTokenAsync(string walletId, AddTokenRequest request);
    Task<string> ExportWalletAsync(string walletId, string password);
    Task<Wallet> ImportWalletAsync(ImportWalletRequest request);
}

public class WalletService : IWalletService
{
    private readonly ILogger<WalletService> _logger;
    private readonly ITransactionService _transactionService;
    private static readonly List<Wallet> _wallets = new();
    private static readonly object _lock = new();

    public WalletService(ILogger<WalletService> logger, ITransactionService transactionService)
    {
        _logger = logger;
        _transactionService = transactionService;
        InitializeSampleWallets();
    }

    private void InitializeSampleWallets()
    {
        lock (_lock)
        {
            if (_wallets.Count == 0)
            {
                _wallets.AddRange(new[]
                {
                    new Wallet
                    {
                        Id = "wallet_pm_001",
                        Address = GenerateAddress(),
                        UserId = "pm_chief",
                        Name = "Prime Minister Wallet",
                        Type = "multisig",
                        Balances = new List<TokenBalance>
                        {
                            new() { Symbol = "IGT", Name = "IERAHKWA Governance Token", Balance = 1000000, UsdValue = 1000000 },
                            new() { Symbol = "IGT-PM", Name = "Prime Minister Token", Balance = 777777, UsdValue = 777777 },
                            new() { Symbol = "IGT-GOV", Name = "Governance Token", Balance = 500000, UsdValue = 500000 }
                        },
                        IsActive = true
                    },
                    new Wallet
                    {
                        Id = "wallet_treasury_001",
                        Address = GenerateAddress(),
                        UserId = "treasury",
                        Name = "Treasury Wallet",
                        Type = "multisig",
                        Balances = new List<TokenBalance>
                        {
                            new() { Symbol = "IGT", Name = "IERAHKWA Governance Token", Balance = 50000000, UsdValue = 50000000 },
                            new() { Symbol = "IGT-RESERVE", Name = "Reserve Token", Balance = 10000000, UsdValue = 10000000 }
                        },
                        IsActive = true
                    },
                    new Wallet
                    {
                        Id = "wallet_gov_001",
                        Address = GenerateAddress(),
                        UserId = "gov_operations",
                        Name = "Government Operations",
                        Type = "standard",
                        Balances = new List<TokenBalance>
                        {
                            new() { Symbol = "IGT", Name = "IERAHKWA Governance Token", Balance = 5000000, UsdValue = 5000000 },
                            new() { Symbol = "IGT-GOV", Name = "Governance Token", Balance = 2500000, UsdValue = 2500000 }
                        },
                        IsActive = true
                    },
                    new Wallet
                    {
                        Id = "wallet_tradex_001",
                        Address = GenerateAddress(),
                        UserId = "tradex_exchange",
                        Name = "TradeX Hot Wallet",
                        Type = "standard",
                        Balances = new List<TokenBalance>
                        {
                            new() { Symbol = "IGT", Name = "IERAHKWA Governance Token", Balance = 2000000, UsdValue = 2000000 },
                            new() { Symbol = "USDT", Name = "Tether", Balance = 1000000, UsdValue = 1000000, ContractAddress = "0xdac17f958d2ee523a2206206994597c13d831ec7" }
                        },
                        IsActive = true
                    }
                });
            }
        }
    }

    public async Task<Wallet> CreateWalletAsync(CreateWalletRequest request)
    {
        await Task.Delay(100);

        var wallet = new Wallet
        {
            Id = $"wallet_{Guid.NewGuid().ToString("N")[..8]}",
            Address = GenerateAddress(),
            UserId = request.UserId,
            Name = request.Name,
            Type = request.Type,
            Balances = new List<TokenBalance>
            {
                new() { Symbol = "IGT", Name = "IERAHKWA Governance Token", Balance = 100, UsdValue = 100 } // Welcome bonus
            },
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        lock (_lock)
        {
            _wallets.Add(wallet);
        }

        _logger.LogInformation("Wallet created: {WalletId} for user {UserId}", wallet.Id, request.UserId);
        return wallet;
    }

    public async Task<Wallet?> GetWalletAsync(string id)
    {
        await Task.Delay(50);
        lock (_lock)
        {
            return _wallets.FirstOrDefault(w => w.Id == id || w.Address == id);
        }
    }

    public async Task<WalletBalance> GetBalanceAsync(string id)
    {
        await Task.Delay(50);
        
        Wallet? wallet;
        lock (_lock)
        {
            wallet = _wallets.FirstOrDefault(w => w.Id == id || w.Address == id);
        }

        if (wallet == null)
            throw new WalletNotFoundException();

        return new WalletBalance
        {
            WalletId = wallet.Id,
            Address = wallet.Address,
            TotalUsdValue = wallet.Balances.Sum(b => b.UsdValue),
            Tokens = wallet.Balances
        };
    }

    public async Task<List<Wallet>> GetUserWalletsAsync(string userId)
    {
        await Task.Delay(50);
        lock (_lock)
        {
            return _wallets.Where(w => w.UserId == userId).ToList();
        }
    }

    public async Task<Transaction> DepositAsync(string walletId, DepositRequest request)
    {
        await Task.Delay(100);

        Wallet? wallet;
        lock (_lock)
        {
            wallet = _wallets.FirstOrDefault(w => w.Id == walletId);
        }

        if (wallet == null)
            throw new WalletNotFoundException();

        // Actualizar balance
        lock (_lock)
        {
            var token = wallet.Balances.FirstOrDefault(b => b.Symbol == request.Currency);
            if (token != null)
            {
                token.Balance += request.Amount;
                token.UsdValue = token.Balance; // Simplified: 1:1 with USD
            }
            else
            {
                wallet.Balances.Add(new TokenBalance
                {
                    Symbol = request.Currency,
                    Name = request.Currency,
                    Balance = request.Amount,
                    UsdValue = request.Amount
                });
            }
            wallet.LastActivityAt = DateTime.UtcNow;
        }

        // Crear transacción de depósito
        var tx = await _transactionService.CreateTransactionAsync(new TransactionRequest
        {
            FromWallet = request.FromAddress ?? "external",
            ToWallet = walletId,
            Amount = request.Amount,
            Currency = request.Currency,
            Type = "deposit"
        });

        _logger.LogInformation("Deposit: {Amount} {Currency} to wallet {WalletId}", 
            request.Amount, request.Currency, walletId);

        return tx;
    }

    public async Task<Transaction> WithdrawAsync(string walletId, WithdrawRequest request)
    {
        await Task.Delay(100);

        Wallet? wallet;
        lock (_lock)
        {
            wallet = _wallets.FirstOrDefault(w => w.Id == walletId);
        }

        if (wallet == null)
            throw new WalletNotFoundException();

        // Verificar balance
        var token = wallet.Balances.FirstOrDefault(b => b.Symbol == request.Currency);
        if (token == null || token.Balance < request.Amount)
            throw new InsufficientFundsException();

        // Actualizar balance
        lock (_lock)
        {
            token.Balance -= request.Amount;
            token.UsdValue = token.Balance;
            wallet.LastActivityAt = DateTime.UtcNow;
        }

        // Crear transacción de retiro
        var tx = await _transactionService.CreateTransactionAsync(new TransactionRequest
        {
            FromWallet = walletId,
            ToWallet = request.ToAddress,
            Amount = request.Amount,
            Currency = request.Currency,
            Type = "withdrawal"
        });

        _logger.LogInformation("Withdrawal: {Amount} {Currency} from wallet {WalletId} to {Address}",
            request.Amount, request.Currency, walletId, request.ToAddress);

        return tx;
    }

    public async Task<List<TokenBalance>> GetTokensAsync(string walletId)
    {
        await Task.Delay(50);
        
        Wallet? wallet;
        lock (_lock)
        {
            wallet = _wallets.FirstOrDefault(w => w.Id == walletId);
        }

        if (wallet == null)
            throw new WalletNotFoundException();

        return wallet.Balances;
    }

    public async Task<TokenBalance> AddTokenAsync(string walletId, AddTokenRequest request)
    {
        await Task.Delay(100);

        Wallet? wallet;
        lock (_lock)
        {
            wallet = _wallets.FirstOrDefault(w => w.Id == walletId);
        }

        if (wallet == null)
            throw new WalletNotFoundException();

        var newToken = new TokenBalance
        {
            Symbol = request.Symbol,
            Name = request.Name,
            Balance = 0,
            ContractAddress = request.ContractAddress,
            Decimals = request.Decimals,
            UsdValue = 0
        };

        lock (_lock)
        {
            if (!wallet.Balances.Any(b => b.Symbol == request.Symbol))
            {
                wallet.Balances.Add(newToken);
            }
        }

        return newToken;
    }

    public async Task<string> ExportWalletAsync(string walletId, string password)
    {
        await Task.Delay(100);

        Wallet? wallet;
        lock (_lock)
        {
            wallet = _wallets.FirstOrDefault(w => w.Id == walletId);
        }

        if (wallet == null)
            throw new WalletNotFoundException();

        // Exportar datos del wallet encriptados
        var walletData = JsonSerializer.Serialize(new
        {
            wallet.Id,
            wallet.Address,
            wallet.Name,
            wallet.Type,
            ExportedAt = DateTime.UtcNow
        });

        // Encriptar con password (simplificado - en producción usar AES-256)
        var encrypted = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(walletData + ":" + password)
        );

        return encrypted;
    }

    public async Task<Wallet> ImportWalletAsync(ImportWalletRequest request)
    {
        await Task.Delay(100);

        try
        {
            // Desencriptar (simplificado)
            var decrypted = Encoding.UTF8.GetString(
                Convert.FromBase64String(request.EncryptedData)
            );

            var parts = decrypted.Split(':');
            if (parts.Length < 2 || parts[^1] != request.Password)
                throw new ArgumentException("Invalid password");

            // Crear nuevo wallet desde datos importados
            var wallet = new Wallet
            {
                Id = $"wallet_{Guid.NewGuid().ToString("N")[..8]}",
                Address = GenerateAddress(),
                UserId = request.UserId,
                Name = "Imported Wallet",
                Type = "standard",
                Balances = new List<TokenBalance>
                {
                    new() { Symbol = "IGT", Name = "IERAHKWA Governance Token", Balance = 0, UsdValue = 0 }
                },
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            lock (_lock)
            {
                _wallets.Add(wallet);
            }

            return wallet;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing wallet");
            throw new ArgumentException("Failed to import wallet");
        }
    }

    private static string GenerateAddress()
    {
        var bytes = RandomNumberGenerator.GetBytes(20);
        return "0x" + Convert.ToHexString(bytes).ToLower();
    }
}
