using Microsoft.AspNetCore.Mvc;

namespace NET10.API.Controllers;

/// <summary>
/// Wallet Controller - Sovereign wallet management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WalletController : ControllerBase
{
    private static readonly List<Wallet> _wallets = InitializeWallets();
    private static readonly List<WalletTransaction> _transactions = InitializeTransactions();
    
    /// <summary>
    /// Get wallet by address
    /// </summary>
    [HttpGet("{address}")]
    public ActionResult<Wallet> GetWallet(string address)
    {
        var wallet = _wallets.FirstOrDefault(w => w.Address.Equals(address, StringComparison.OrdinalIgnoreCase));
        if (wallet == null) return NotFound(new { error = "Wallet not found" });
        return Ok(wallet);
    }
    
    /// <summary>
    /// Get wallet balances
    /// </summary>
    [HttpGet("{address}/balances")]
    public ActionResult<List<TokenBalance>> GetBalances(string address)
    {
        var wallet = _wallets.FirstOrDefault(w => w.Address.Equals(address, StringComparison.OrdinalIgnoreCase));
        if (wallet == null) return NotFound();
        return Ok(wallet.Balances);
    }
    
    /// <summary>
    /// Get wallet transactions
    /// </summary>
    [HttpGet("{address}/transactions")]
    public ActionResult<TransactionHistory> GetTransactions(
        string address,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? type = null)
    {
        var query = _transactions.Where(t => 
            t.FromAddress.Equals(address, StringComparison.OrdinalIgnoreCase) ||
            t.ToAddress.Equals(address, StringComparison.OrdinalIgnoreCase));
        
        if (!string.IsNullOrEmpty(type))
            query = query.Where(t => t.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
        
        var total = query.Count();
        var items = query
            .OrderByDescending(t => t.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        return Ok(new TransactionHistory
        {
            Address = address,
            Transactions = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        });
    }
    
    /// <summary>
    /// Create new wallet
    /// </summary>
    [HttpPost]
    public ActionResult<Wallet> CreateWallet([FromBody] CreateWalletRequest request)
    {
        var wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            Address = GenerateAddress(),
            Name = request.Name,
            Type = request.Type,
            OwnerId = request.OwnerId,
            CreatedAt = DateTime.UtcNow,
            Balances = new List<TokenBalance>
            {
                new() { Symbol = "IGT-PM", Name = "Ierahkwa Governance Token", Balance = 0, UsdValue = 0 },
                new() { Symbol = "USDT", Name = "Tether USD", Balance = 0, UsdValue = 0 }
            }
        };
        
        _wallets.Add(wallet);
        return CreatedAtAction(nameof(GetWallet), new { address = wallet.Address }, wallet);
    }
    
    /// <summary>
    /// Transfer tokens
    /// </summary>
    [HttpPost("transfer")]
    public ActionResult<WalletTransaction> Transfer([FromBody] WalletTransferRequest request)
    {
        var fromWallet = _wallets.FirstOrDefault(w => w.Address.Equals(request.FromAddress, StringComparison.OrdinalIgnoreCase));
        if (fromWallet == null) return NotFound(new { error = "Source wallet not found" });
        
        var fromBalance = fromWallet.Balances.FirstOrDefault(b => b.Symbol == request.TokenSymbol);
        if (fromBalance == null || fromBalance.Balance < request.Amount)
            return BadRequest(new { error = "Insufficient balance" });
        
        // Create transaction
        var tx = new WalletTransaction
        {
            Id = Guid.NewGuid(),
            Hash = GenerateTxHash(),
            Type = "transfer",
            FromAddress = request.FromAddress,
            ToAddress = request.ToAddress,
            TokenSymbol = request.TokenSymbol,
            Amount = request.Amount,
            Fee = 0.001m,
            Status = TransactionStatus.Confirmed,
            Timestamp = DateTime.UtcNow,
            BlockNumber = GetLatestBlock(),
            Confirmations = 1
        };
        
        // Update balances (simulated)
        fromBalance.Balance -= request.Amount;
        
        var toWallet = _wallets.FirstOrDefault(w => w.Address.Equals(request.ToAddress, StringComparison.OrdinalIgnoreCase));
        if (toWallet != null)
        {
            var toBalance = toWallet.Balances.FirstOrDefault(b => b.Symbol == request.TokenSymbol);
            if (toBalance != null)
                toBalance.Balance += request.Amount;
        }
        
        _transactions.Insert(0, tx);
        return Ok(tx);
    }
    
    /// <summary>
    /// Get wallet portfolio summary
    /// </summary>
    [HttpGet("{address}/portfolio")]
    public ActionResult<PortfolioSummary> GetPortfolio(string address)
    {
        var wallet = _wallets.FirstOrDefault(w => w.Address.Equals(address, StringComparison.OrdinalIgnoreCase));
        if (wallet == null) return NotFound();
        
        var totalValue = wallet.Balances.Sum(b => b.UsdValue);
        
        return Ok(new PortfolioSummary
        {
            Address = address,
            TotalValue = totalValue,
            Change24h = 5.25m,
            Change7d = 12.5m,
            Holdings = wallet.Balances.Select(b => new HoldingItem
            {
                Symbol = b.Symbol,
                Name = b.Name,
                Balance = b.Balance,
                UsdValue = b.UsdValue,
                Percentage = totalValue > 0 ? (b.UsdValue / totalValue) * 100 : 0
            }).ToList()
        });
    }
    
    /// <summary>
    /// Get NFTs owned by wallet
    /// </summary>
    [HttpGet("{address}/nfts")]
    public ActionResult<List<NFTItem>> GetNFTs(string address)
    {
        // Simulated NFTs
        var nfts = new List<NFTItem>
        {
            new() { TokenId = "1", Collection = "Ierahkwa Founders", Name = "Founder Badge #1", ImageUrl = "/nfts/founder-1.png", Rarity = "Legendary" },
            new() { TokenId = "42", Collection = "Sovereign Land Deeds", Name = "Plot #42 - Akwesasne", ImageUrl = "/nfts/land-42.png", Rarity = "Epic" }
        };
        
        return Ok(nfts);
    }
    
    // Helper methods
    private static string GenerateAddress() => "0x" + Guid.NewGuid().ToString("N")[..40];
    private static string GenerateTxHash() => "0x" + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N")[..24];
    private static long GetLatestBlock() => 12_345_678 + new Random().Next(1, 100);
    
    private static List<Wallet> InitializeWallets()
    {
        return new List<Wallet>
        {
            new()
            {
                Address = "0x1234567890abcdef1234567890abcdef12345678",
                Name = "Treasury Principal",
                Type = WalletType.Treasury,
                OwnerId = "system",
                Balances = new List<TokenBalance>
                {
                    new() { Symbol = "IGT-PM", Name = "Ierahkwa Governance Token", Balance = 100_000_000, UsdValue = 250_000_000 },
                    new() { Symbol = "USDT", Name = "Tether USD", Balance = 50_000_000, UsdValue = 50_000_000 },
                    new() { Symbol = "ETH", Name = "Ethereum", Balance = 10_000, UsdValue = 25_000_000 },
                    new() { Symbol = "IGT-BDET", Name = "Budget Execution Token", Balance = 25_000_000, UsdValue = 25_000_000 }
                }
            },
            new()
            {
                Address = "0xabcdef1234567890abcdef1234567890abcdef12",
                Name = "Staking Pool",
                Type = WalletType.Staking,
                OwnerId = "system",
                Balances = new List<TokenBalance>
                {
                    new() { Symbol = "IGT-PM", Name = "Ierahkwa Governance Token", Balance = 50_000_000, UsdValue = 125_000_000 }
                }
            }
        };
    }
    
    private static List<WalletTransaction> InitializeTransactions()
    {
        var transactions = new List<WalletTransaction>();
        var random = new Random(42);
        var types = new[] { "transfer", "swap", "stake", "unstake", "claim", "bridge" };
        var tokens = new[] { "IGT-PM", "USDT", "ETH", "IGT-BDET" };
        
        for (int i = 0; i < 100; i++)
        {
            transactions.Add(new WalletTransaction
            {
                Hash = GenerateTxHash(),
                Type = types[random.Next(types.Length)],
                FromAddress = "0x1234567890abcdef1234567890abcdef12345678",
                ToAddress = "0x" + Guid.NewGuid().ToString("N")[..40],
                TokenSymbol = tokens[random.Next(tokens.Length)],
                Amount = random.Next(100, 100000),
                Fee = 0.001m,
                Status = TransactionStatus.Confirmed,
                Timestamp = DateTime.UtcNow.AddMinutes(-random.Next(1, 10000)),
                BlockNumber = 12_345_678 - i,
                Confirmations = random.Next(1, 1000)
            });
        }
        
        return transactions;
    }
}

// ═══════════════════════════════════════════════════════════════
// WALLET MODELS
// ═══════════════════════════════════════════════════════════════

public class Wallet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public WalletType Type { get; set; } = WalletType.Personal;
    public string OwnerId { get; set; } = string.Empty;
    public List<TokenBalance> Balances { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsMultiSig { get; set; } = false;
    public int RequiredSignatures { get; set; } = 1;
}

public enum WalletType
{
    Personal,
    Business,
    Treasury,
    Staking,
    Escrow,
    MultiSig
}

public class TokenBalance
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal LockedBalance { get; set; }
    public decimal UsdValue { get; set; }
    public decimal Price { get; set; }
}

public class WalletTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Hash { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public string TokenSymbol { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime Timestamp { get; set; }
    public long BlockNumber { get; set; }
    public int Confirmations { get; set; }
    public string? Memo { get; set; }
}

public enum TransactionStatus
{
    Pending,
    Confirmed,
    Failed,
    Cancelled
}

public class TransactionHistory
{
    public string Address { get; set; } = string.Empty;
    public List<WalletTransaction> Transactions { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class CreateWalletRequest
{
    public string Name { get; set; } = string.Empty;
    public WalletType Type { get; set; } = WalletType.Personal;
    public string OwnerId { get; set; } = string.Empty;
}

public class WalletTransferRequest
{
    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public string TokenSymbol { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Memo { get; set; }
}

public class PortfolioSummary
{
    public string Address { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    public decimal Change24h { get; set; }
    public decimal Change7d { get; set; }
    public List<HoldingItem> Holdings { get; set; } = new();
}

public class HoldingItem
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal UsdValue { get; set; }
    public decimal Percentage { get; set; }
}

public class NFTItem
{
    public string TokenId { get; set; } = string.Empty;
    public string Collection { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Rarity { get; set; } = string.Empty;
    public decimal? FloorPrice { get; set; }
}
