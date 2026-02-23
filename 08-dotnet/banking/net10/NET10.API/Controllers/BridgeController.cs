using Microsoft.AspNetCore.Mvc;

namespace NET10.API.Controllers;

/// <summary>
/// Bridge Controller - Cross-chain asset transfers
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BridgeController : ControllerBase
{
    private static readonly List<SupportedChain> _chains = InitializeChains();
    private static readonly List<BridgeTransaction> _transactions = InitializeBridgeTransactions();
    private static readonly List<BridgeToken> _tokens = InitializeBridgeTokens();
    
    /// <summary>
    /// Get supported chains
    /// </summary>
    [HttpGet("chains")]
    public ActionResult<List<SupportedChain>> GetChains()
    {
        return Ok(_chains.Where(c => c.IsActive).ToList());
    }
    
    /// <summary>
    /// Get bridgeable tokens
    /// </summary>
    [HttpGet("tokens")]
    public ActionResult<List<BridgeToken>> GetTokens([FromQuery] int? sourceChainId = null)
    {
        var tokens = _tokens.AsQueryable();
        
        if (sourceChainId.HasValue)
            tokens = tokens.Where(t => t.SupportedChains.Contains(sourceChainId.Value));
        
        return Ok(tokens.ToList());
    }
    
    /// <summary>
    /// Get bridge quote
    /// </summary>
    [HttpPost("quote")]
    public ActionResult<BridgeQuote> GetQuote([FromBody] QuoteRequest request)
    {
        var sourceChain = _chains.FirstOrDefault(c => c.ChainId == request.SourceChainId);
        var destChain = _chains.FirstOrDefault(c => c.ChainId == request.DestinationChainId);
        var token = _tokens.FirstOrDefault(t => t.Symbol == request.TokenSymbol);
        
        if (sourceChain == null || destChain == null || token == null)
            return BadRequest(new { error = "Invalid chain or token" });
        
        // Calculate fees
        var bridgeFee = request.Amount * 0.001m; // 0.1%
        var gasFee = destChain.EstimatedGas;
        var totalFee = bridgeFee + gasFee;
        var receiveAmount = request.Amount - totalFee;
        
        return Ok(new BridgeQuote
        {
            QuoteId = Guid.NewGuid().ToString(),
            SourceChain = sourceChain,
            DestinationChain = destChain,
            Token = token,
            SendAmount = request.Amount,
            ReceiveAmount = receiveAmount,
            BridgeFee = bridgeFee,
            GasFee = gasFee,
            TotalFee = totalFee,
            EstimatedTime = destChain.EstimatedTime,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            ExchangeRate = 1m
        });
    }
    
    /// <summary>
    /// Initiate bridge transfer
    /// </summary>
    [HttpPost("transfer")]
    public ActionResult<BridgeTransaction> InitiateTransfer([FromBody] BridgeTransferRequest request)
    {
        var sourceChain = _chains.FirstOrDefault(c => c.ChainId == request.SourceChainId);
        var destChain = _chains.FirstOrDefault(c => c.ChainId == request.DestinationChainId);
        
        if (sourceChain == null || destChain == null)
            return BadRequest(new { error = "Invalid chain" });
        
        var tx = new BridgeTransaction
        {
            Id = Guid.NewGuid(),
            SourceChainId = request.SourceChainId,
            SourceChainName = sourceChain.Name,
            DestinationChainId = request.DestinationChainId,
            DestinationChainName = destChain.Name,
            TokenSymbol = request.TokenSymbol,
            Amount = request.Amount,
            SenderAddress = request.SenderAddress,
            ReceiverAddress = request.ReceiverAddress,
            Status = BridgeStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            EstimatedCompletionAt = DateTime.UtcNow.AddMinutes(destChain.EstimatedTime),
            SourceTxHash = GenerateTxHash(),
            Fee = request.Amount * 0.001m
        };
        
        _transactions.Insert(0, tx);
        return Ok(tx);
    }
    
    /// <summary>
    /// Get transaction status
    /// </summary>
    [HttpGet("tx/{id}")]
    public ActionResult<BridgeTransaction> GetTransaction(Guid id)
    {
        var tx = _transactions.FirstOrDefault(t => t.Id == id);
        if (tx == null) return NotFound();
        return Ok(tx);
    }
    
    /// <summary>
    /// Get transaction by hash
    /// </summary>
    [HttpGet("tx/hash/{hash}")]
    public ActionResult<BridgeTransaction> GetTransactionByHash(string hash)
    {
        var tx = _transactions.FirstOrDefault(t => 
            t.SourceTxHash?.Equals(hash, StringComparison.OrdinalIgnoreCase) == true ||
            t.DestinationTxHash?.Equals(hash, StringComparison.OrdinalIgnoreCase) == true);
        if (tx == null) return NotFound();
        return Ok(tx);
    }
    
    /// <summary>
    /// Get user's bridge history
    /// </summary>
    [HttpGet("history/{address}")]
    public ActionResult<List<BridgeTransaction>> GetHistory(string address, [FromQuery] int limit = 20)
    {
        var transactions = _transactions
            .Where(t => 
                t.SenderAddress.Equals(address, StringComparison.OrdinalIgnoreCase) ||
                t.ReceiverAddress.Equals(address, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .ToList();
        
        return Ok(transactions);
    }
    
    /// <summary>
    /// Get bridge statistics
    /// </summary>
    [HttpGet("stats")]
    public ActionResult<BridgeStats> GetStats()
    {
        return Ok(new BridgeStats
        {
            TotalTransactions = _transactions.Count,
            TotalVolume = _transactions.Sum(t => t.Amount),
            TotalFeesCollected = _transactions.Sum(t => t.Fee),
            TransactionsToday = _transactions.Count(t => t.CreatedAt.Date == DateTime.UtcNow.Date),
            VolumeToday = _transactions.Where(t => t.CreatedAt.Date == DateTime.UtcNow.Date).Sum(t => t.Amount),
            AverageCompletionTime = 8.5m,
            SuccessRate = 99.8m,
            SupportedChains = _chains.Count(c => c.IsActive),
            SupportedTokens = _tokens.Count
        });
    }
    
    /// <summary>
    /// Get liquidity info
    /// </summary>
    [HttpGet("liquidity")]
    public ActionResult<List<BridgeLiquidity>> GetLiquidity()
    {
        var liquidity = _chains.Where(c => c.IsActive).Select(c => new BridgeLiquidity
        {
            ChainId = c.ChainId,
            ChainName = c.Name,
            Tokens = _tokens.Select(t => new TokenLiquidity
            {
                Symbol = t.Symbol,
                Available = 1_000_000m + new Random().Next(1, 1000000),
                Locked = 500_000m
            }).ToList()
        }).ToList();
        
        return Ok(liquidity);
    }
    
    // Helpers
    private static string GenerateTxHash() => "0x" + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N")[..24];
    
    private static List<SupportedChain> InitializeChains()
    {
        return new List<SupportedChain>
        {
            new() { ChainId = 777777, Name = "Ierahkwa Mainnet", Symbol = "IGT", LogoUrl = "/chains/ierahkwa.png", EstimatedTime = 2, EstimatedGas = 0.001m, IsActive = true },
            new() { ChainId = 1, Name = "Ethereum", Symbol = "ETH", LogoUrl = "/chains/ethereum.png", EstimatedTime = 15, EstimatedGas = 0.005m, IsActive = true },
            new() { ChainId = 137, Name = "Polygon", Symbol = "MATIC", LogoUrl = "/chains/polygon.png", EstimatedTime = 5, EstimatedGas = 0.001m, IsActive = true },
            new() { ChainId = 56, Name = "BNB Chain", Symbol = "BNB", LogoUrl = "/chains/bnb.png", EstimatedTime = 3, EstimatedGas = 0.0005m, IsActive = true },
            new() { ChainId = 42161, Name = "Arbitrum One", Symbol = "ETH", LogoUrl = "/chains/arbitrum.png", EstimatedTime = 10, EstimatedGas = 0.002m, IsActive = true },
            new() { ChainId = 43114, Name = "Avalanche C-Chain", Symbol = "AVAX", LogoUrl = "/chains/avalanche.png", EstimatedTime = 5, EstimatedGas = 0.001m, IsActive = true },
            new() { ChainId = 10, Name = "Optimism", Symbol = "ETH", LogoUrl = "/chains/optimism.png", EstimatedTime = 10, EstimatedGas = 0.002m, IsActive = true },
            new() { ChainId = 8453, Name = "Base", Symbol = "ETH", LogoUrl = "/chains/base.png", EstimatedTime = 10, EstimatedGas = 0.001m, IsActive = true }
        };
    }
    
    private static List<BridgeToken> InitializeBridgeTokens()
    {
        return new List<BridgeToken>
        {
            new() { Symbol = "USDT", Name = "Tether USD", LogoUrl = "/tokens/usdt.png", SupportedChains = new[] { 777777, 1, 137, 56, 42161, 43114 } },
            new() { Symbol = "USDC", Name = "USD Coin", LogoUrl = "/tokens/usdc.png", SupportedChains = new[] { 777777, 1, 137, 56, 42161, 43114, 10, 8453 } },
            new() { Symbol = "ETH", Name = "Ethereum", LogoUrl = "/tokens/eth.png", SupportedChains = new[] { 777777, 1, 42161, 10, 8453 } },
            new() { Symbol = "IGT-PM", Name = "Ierahkwa Governance Token", LogoUrl = "/tokens/igt.png", SupportedChains = new[] { 777777, 1, 137 } },
            new() { Symbol = "WBTC", Name = "Wrapped Bitcoin", LogoUrl = "/tokens/wbtc.png", SupportedChains = new[] { 777777, 1, 137, 42161 } }
        };
    }
    
    private static List<BridgeTransaction> InitializeBridgeTransactions()
    {
        var transactions = new List<BridgeTransaction>();
        var random = new Random(42);
        var statuses = new[] { BridgeStatus.Completed, BridgeStatus.Completed, BridgeStatus.Completed, BridgeStatus.Pending, BridgeStatus.InProgress };
        
        for (int i = 0; i < 50; i++)
        {
            var sourceChain = _chains[random.Next(_chains.Count)];
            var destChain = _chains[random.Next(_chains.Count)];
            var amount = random.Next(100, 100000);
            var status = statuses[random.Next(statuses.Length)];
            
            transactions.Add(new BridgeTransaction
            {
                SourceChainId = sourceChain.ChainId,
                SourceChainName = sourceChain.Name,
                DestinationChainId = destChain.ChainId,
                DestinationChainName = destChain.Name,
                TokenSymbol = _tokens[random.Next(_tokens.Count)].Symbol,
                Amount = amount,
                SenderAddress = "0x" + Guid.NewGuid().ToString("N")[..40],
                ReceiverAddress = "0x" + Guid.NewGuid().ToString("N")[..40],
                Status = status,
                CreatedAt = DateTime.UtcNow.AddMinutes(-random.Next(1, 10000)),
                SourceTxHash = GenerateTxHash(),
                DestinationTxHash = status == BridgeStatus.Completed ? GenerateTxHash() : null,
                Fee = amount * 0.001m,
                CompletedAt = status == BridgeStatus.Completed ? DateTime.UtcNow.AddMinutes(-random.Next(1, 5000)) : null
            });
        }
        
        return transactions.OrderByDescending(t => t.CreatedAt).ToList();
    }
}

// ═══════════════════════════════════════════════════════════════
// BRIDGE MODELS
// ═══════════════════════════════════════════════════════════════

public class SupportedChain
{
    public int ChainId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public int EstimatedTime { get; set; } // minutes
    public decimal EstimatedGas { get; set; }
    public bool IsActive { get; set; }
}

public class BridgeToken
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public int[] SupportedChains { get; set; } = Array.Empty<int>();
}

public class QuoteRequest
{
    public int SourceChainId { get; set; }
    public int DestinationChainId { get; set; }
    public string TokenSymbol { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class BridgeQuote
{
    public string QuoteId { get; set; } = string.Empty;
    public SupportedChain SourceChain { get; set; } = new();
    public SupportedChain DestinationChain { get; set; } = new();
    public BridgeToken Token { get; set; } = new();
    public decimal SendAmount { get; set; }
    public decimal ReceiveAmount { get; set; }
    public decimal BridgeFee { get; set; }
    public decimal GasFee { get; set; }
    public decimal TotalFee { get; set; }
    public int EstimatedTime { get; set; }
    public DateTime ExpiresAt { get; set; }
    public decimal ExchangeRate { get; set; }
}

public class BridgeTransferRequest
{
    public int SourceChainId { get; set; }
    public int DestinationChainId { get; set; }
    public string TokenSymbol { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string SenderAddress { get; set; } = string.Empty;
    public string ReceiverAddress { get; set; } = string.Empty;
}

public class BridgeTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int SourceChainId { get; set; }
    public string SourceChainName { get; set; } = string.Empty;
    public int DestinationChainId { get; set; }
    public string DestinationChainName { get; set; } = string.Empty;
    public string TokenSymbol { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string SenderAddress { get; set; } = string.Empty;
    public string ReceiverAddress { get; set; } = string.Empty;
    public BridgeStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EstimatedCompletionAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? SourceTxHash { get; set; }
    public string? DestinationTxHash { get; set; }
    public decimal Fee { get; set; }
    public string? Error { get; set; }
}

public enum BridgeStatus
{
    Pending,
    AwaitingConfirmation,
    InProgress,
    Completed,
    Failed,
    Refunded
}

public class BridgeStats
{
    public int TotalTransactions { get; set; }
    public decimal TotalVolume { get; set; }
    public decimal TotalFeesCollected { get; set; }
    public int TransactionsToday { get; set; }
    public decimal VolumeToday { get; set; }
    public decimal AverageCompletionTime { get; set; }
    public decimal SuccessRate { get; set; }
    public int SupportedChains { get; set; }
    public int SupportedTokens { get; set; }
}

public class BridgeLiquidity
{
    public int ChainId { get; set; }
    public string ChainName { get; set; } = string.Empty;
    public List<TokenLiquidity> Tokens { get; set; } = new();
}

public class TokenLiquidity
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Available { get; set; }
    public decimal Locked { get; set; }
}
