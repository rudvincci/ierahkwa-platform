using Microsoft.AspNetCore.Mvc;

namespace NET10.API.Controllers;

/// <summary>
/// Blockchain Controller - Ierahkwa Sovereign Blockchain Explorer
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BlockchainController : ControllerBase
{
    private static readonly ChainInfo _chainInfo = InitializeChainInfo();
    private static readonly List<Block> _blocks = InitializeBlocks();
    private static readonly List<BlockTransaction> _transactions = InitializeBlockTransactions();
    
    /// <summary>
    /// Get blockchain info
    /// </summary>
    [HttpGet]
    public ActionResult<ChainInfo> GetChainInfo()
    {
        _chainInfo.LatestBlock = _blocks.First().Number;
        _chainInfo.TransactionsToday = _transactions.Count(t => t.Timestamp.Date == DateTime.UtcNow.Date);
        return Ok(_chainInfo);
    }
    
    /// <summary>
    /// Get latest blocks
    /// </summary>
    [HttpGet("blocks")]
    public ActionResult<List<Block>> GetBlocks([FromQuery] int limit = 10)
    {
        return Ok(_blocks.Take(Math.Min(limit, 100)).ToList());
    }
    
    /// <summary>
    /// Get block by number or hash
    /// </summary>
    [HttpGet("blocks/{identifier}")]
    public ActionResult<Block> GetBlock(string identifier)
    {
        Block? block;
        
        if (long.TryParse(identifier, out var blockNumber))
            block = _blocks.FirstOrDefault(b => b.Number == blockNumber);
        else
            block = _blocks.FirstOrDefault(b => b.Hash.Equals(identifier, StringComparison.OrdinalIgnoreCase));
        
        if (block == null) return NotFound(new { error = "Block not found" });
        return Ok(block);
    }
    
    /// <summary>
    /// Get transaction by hash
    /// </summary>
    [HttpGet("tx/{hash}")]
    public ActionResult<BlockTransaction> GetTransaction(string hash)
    {
        var tx = _transactions.FirstOrDefault(t => t.Hash.Equals(hash, StringComparison.OrdinalIgnoreCase));
        if (tx == null) return NotFound(new { error = "Transaction not found" });
        return Ok(tx);
    }
    
    /// <summary>
    /// Get latest transactions
    /// </summary>
    [HttpGet("transactions")]
    public ActionResult<List<BlockTransaction>> GetTransactions([FromQuery] int limit = 20)
    {
        return Ok(_transactions.Take(Math.Min(limit, 100)).ToList());
    }
    
    /// <summary>
    /// Get account info
    /// </summary>
    [HttpGet("account/{address}")]
    public ActionResult<AccountInfo> GetAccount(string address)
    {
        var txCount = _transactions.Count(t => 
            t.From.Equals(address, StringComparison.OrdinalIgnoreCase) ||
            t.To.Equals(address, StringComparison.OrdinalIgnoreCase));
        
        return Ok(new AccountInfo
        {
            Address = address,
            Balance = 12_500.75m,
            NativeBalance = "12,500.75 IGT",
            TransactionCount = txCount,
            FirstSeen = DateTime.UtcNow.AddDays(-365),
            LastSeen = DateTime.UtcNow.AddMinutes(-5),
            IsContract = address.StartsWith("0xC", StringComparison.OrdinalIgnoreCase),
            TokenBalances = new List<AccountTokenBalance>
            {
                new() { Token = "IGT-PM", Balance = 12_500.75m, Symbol = "IGT-PM" },
                new() { Token = "USDT", Balance = 5_000m, Symbol = "USDT" },
                new() { Token = "IGT-BDET", Balance = 1_000m, Symbol = "IGT-BDET" }
            }
        });
    }
    
    /// <summary>
    /// Get contract info
    /// </summary>
    [HttpGet("contract/{address}")]
    public ActionResult<ContractInfo> GetContract(string address)
    {
        return Ok(new ContractInfo
        {
            Address = address,
            Name = "Ierahkwa DEX Router",
            Symbol = "ROUTER",
            Type = ContractType.DEX,
            Creator = "0x1234567890abcdef1234567890abcdef12345678",
            CreatedAt = DateTime.UtcNow.AddDays(-180),
            TransactionCount = 125_678,
            IsVerified = true,
            SourceCodeUrl = "https://github.com/ierahkwa/contracts",
            ABI = "[{\"type\":\"function\",\"name\":\"swap\",...}]"
        });
    }
    
    /// <summary>
    /// Get gas prices
    /// </summary>
    [HttpGet("gas")]
    public ActionResult<GasPrices> GetGasPrices()
    {
        return Ok(new GasPrices
        {
            Slow = 0.0001m,
            Standard = 0.0005m,
            Fast = 0.001m,
            Instant = 0.002m,
            BaseFee = 0.00005m,
            LastUpdated = DateTime.UtcNow
        });
    }
    
    /// <summary>
    /// Get network stats
    /// </summary>
    [HttpGet("stats")]
    public ActionResult<NetworkStats> GetNetworkStats()
    {
        return Ok(new NetworkStats
        {
            TotalTransactions = 15_678_234,
            TotalAccounts = 125_678,
            TotalContracts = 1_234,
            TPS = 1_500,
            AverageBlockTime = 2.5m,
            TotalValueLocked = 500_000_000m,
            ValidatorCount = 21,
            StakedAmount = 250_000_000m,
            CirculatingSupply = 500_000_000m,
            MarketCap = 1_250_000_000m
        });
    }
    
    /// <summary>
    /// Get validators
    /// </summary>
    [HttpGet("validators")]
    public ActionResult<List<Validator>> GetValidators()
    {
        var validators = new List<Validator>
        {
            new() { Address = "0xV001...1234", Name = "Ierahkwa Foundation", Stake = 50_000_000, Uptime = 99.99m, BlocksProduced = 125_000 },
            new() { Address = "0xV002...5678", Name = "Akwesasne Node", Stake = 25_000_000, Uptime = 99.95m, BlocksProduced = 62_500 },
            new() { Address = "0xV003...9abc", Name = "Sovereign Staking", Stake = 20_000_000, Uptime = 99.90m, BlocksProduced = 50_000 },
            new() { Address = "0xV004...def0", Name = "Community Pool", Stake = 15_000_000, Uptime = 99.85m, BlocksProduced = 37_500 },
            new() { Address = "0xV005...1111", Name = "Treasury Validator", Stake = 10_000_000, Uptime = 99.80m, BlocksProduced = 25_000 }
        };
        
        return Ok(validators);
    }
    
    /// <summary>
    /// Search blockchain
    /// </summary>
    [HttpGet("search")]
    public ActionResult<SearchResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrEmpty(q))
            return BadRequest(new { error = "Query required" });
        
        // Determine search type
        if (q.StartsWith("0x") && q.Length == 66) // TX hash
            return Ok(new SearchResult { Type = "transaction", Redirect = $"/api/blockchain/tx/{q}" });
        
        if (q.StartsWith("0x") && q.Length == 42) // Address
            return Ok(new SearchResult { Type = "address", Redirect = $"/api/blockchain/account/{q}" });
        
        if (long.TryParse(q, out _)) // Block number
            return Ok(new SearchResult { Type = "block", Redirect = $"/api/blockchain/blocks/{q}" });
        
        return NotFound(new { error = "No results found" });
    }
    
    // Initialization helpers
    private static ChainInfo InitializeChainInfo()
    {
        return new ChainInfo
        {
            ChainId = 777777,
            Name = "Ierahkwa Sovereign Blockchain",
            Symbol = "IGT",
            NativeCurrency = "Ierahkwa Governance Token",
            Decimals = 18,
            RpcUrl = "https://rpc.ierahkwa.network",
            ExplorerUrl = "https://explorer.ierahkwa.network",
            LogoUrl = "/images/ierahkwa-logo.png",
            LatestBlock = 12_345_678,
            TotalSupply = 1_000_000_000m,
            CirculatingSupply = 500_000_000m,
            Price = 2.50m,
            MarketCap = 1_250_000_000m
        };
    }
    
    private static List<Block> InitializeBlocks()
    {
        var blocks = new List<Block>();
        var baseBlock = 12_345_678;
        var random = new Random(42);
        
        for (int i = 0; i < 50; i++)
        {
            blocks.Add(new Block
            {
                Number = baseBlock - i,
                Hash = "0x" + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N")[..24],
                ParentHash = "0x" + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N")[..24],
                Timestamp = DateTime.UtcNow.AddSeconds(-i * 2.5),
                Validator = $"0xV00{random.Next(1, 6)}...{random.Next(1000, 9999)}",
                TransactionCount = random.Next(50, 500),
                GasUsed = random.Next(1_000_000, 10_000_000),
                GasLimit = 30_000_000,
                Size = random.Next(10_000, 100_000)
            });
        }
        
        return blocks;
    }
    
    private static List<BlockTransaction> InitializeBlockTransactions()
    {
        var transactions = new List<BlockTransaction>();
        var random = new Random(42);
        var methods = new[] { "transfer", "swap", "addLiquidity", "removeLiquidity", "stake", "claim", "approve", "mint" };
        
        for (int i = 0; i < 100; i++)
        {
            transactions.Add(new BlockTransaction
            {
                Hash = "0x" + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N")[..24],
                BlockNumber = 12_345_678 - random.Next(0, 50),
                From = "0x" + Guid.NewGuid().ToString("N")[..40],
                To = "0x" + Guid.NewGuid().ToString("N")[..40],
                Value = random.Next(0, 10000),
                GasPrice = 0.0005m,
                GasUsed = random.Next(21000, 500000),
                Method = methods[random.Next(methods.Length)],
                Status = true,
                Timestamp = DateTime.UtcNow.AddMinutes(-random.Next(1, 1000))
            });
        }
        
        return transactions.OrderByDescending(t => t.Timestamp).ToList();
    }
}

// ═══════════════════════════════════════════════════════════════
// BLOCKCHAIN MODELS
// ═══════════════════════════════════════════════════════════════

public class ChainInfo
{
    public int ChainId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string NativeCurrency { get; set; } = string.Empty;
    public int Decimals { get; set; }
    public string RpcUrl { get; set; } = string.Empty;
    public string ExplorerUrl { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public long LatestBlock { get; set; }
    public decimal TotalSupply { get; set; }
    public decimal CirculatingSupply { get; set; }
    public decimal Price { get; set; }
    public decimal MarketCap { get; set; }
    public int TransactionsToday { get; set; }
}

public class Block
{
    public long Number { get; set; }
    public string Hash { get; set; } = string.Empty;
    public string ParentHash { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Validator { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public long GasUsed { get; set; }
    public long GasLimit { get; set; }
    public long Size { get; set; }
}

public class BlockTransaction
{
    public string Hash { get; set; } = string.Empty;
    public long BlockNumber { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal GasPrice { get; set; }
    public long GasUsed { get; set; }
    public string Method { get; set; } = string.Empty;
    public bool Status { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Input { get; set; }
}

public class AccountInfo
{
    public string Address { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string NativeBalance { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
    public bool IsContract { get; set; }
    public List<AccountTokenBalance> TokenBalances { get; set; } = new();
}

public class AccountTokenBalance
{
    public string Token { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}

public class ContractInfo
{
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public ContractType Type { get; set; }
    public string Creator { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public long TransactionCount { get; set; }
    public bool IsVerified { get; set; }
    public string? SourceCodeUrl { get; set; }
    public string? ABI { get; set; }
}

public enum ContractType
{
    Token,
    NFT,
    DEX,
    Staking,
    Governance,
    Bridge,
    Other
}

public class GasPrices
{
    public decimal Slow { get; set; }
    public decimal Standard { get; set; }
    public decimal Fast { get; set; }
    public decimal Instant { get; set; }
    public decimal BaseFee { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class NetworkStats
{
    public long TotalTransactions { get; set; }
    public long TotalAccounts { get; set; }
    public long TotalContracts { get; set; }
    public int TPS { get; set; }
    public decimal AverageBlockTime { get; set; }
    public decimal TotalValueLocked { get; set; }
    public int ValidatorCount { get; set; }
    public decimal StakedAmount { get; set; }
    public decimal CirculatingSupply { get; set; }
    public decimal MarketCap { get; set; }
}

public class Validator
{
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Stake { get; set; }
    public decimal Uptime { get; set; }
    public long BlocksProduced { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SearchResult
{
    public string Type { get; set; } = string.Empty;
    public string Redirect { get; set; } = string.Empty;
}
