namespace MameyFramework.Blockchain;

/// <summary>
/// Client interface for interacting with MameyNode blockchain
/// </summary>
public interface IBlockchainClient
{
    // Connection
    Task<bool> ConnectAsync(string nodeUrl, CancellationToken cancellationToken = default);
    Task DisconnectAsync(CancellationToken cancellationToken = default);
    bool IsConnected { get; }
    
    // Chain info
    Task<ChainInfo> GetChainInfoAsync(CancellationToken cancellationToken = default);
    Task<ulong> GetBlockNumberAsync(CancellationToken cancellationToken = default);
    Task<Block?> GetBlockAsync(ulong blockNumber, CancellationToken cancellationToken = default);
    Task<Block?> GetBlockByHashAsync(string hash, CancellationToken cancellationToken = default);
    
    // Transactions
    Task<string> SendTransactionAsync(TransactionRequest request, CancellationToken cancellationToken = default);
    Task<Transaction?> GetTransactionAsync(string hash, CancellationToken cancellationToken = default);
    Task<TransactionReceipt?> GetTransactionReceiptAsync(string hash, CancellationToken cancellationToken = default);
    Task<TransactionReceipt> WaitForTransactionAsync(string hash, CancellationToken cancellationToken = default);
    
    // Accounts
    Task<AccountInfo> GetAccountAsync(string address, CancellationToken cancellationToken = default);
    Task<string> GetBalanceAsync(string address, string token = "WAMPUM", CancellationToken cancellationToken = default);
    Task<ulong> GetNonceAsync(string address, CancellationToken cancellationToken = default);
    
    // Tokens
    Task<IReadOnlyList<TokenInfo>> GetTokensAsync(CancellationToken cancellationToken = default);
    Task<TokenInfo?> GetTokenAsync(string symbol, CancellationToken cancellationToken = default);
    Task<string> CreateTokenAsync(CreateTokenRequest request, CancellationToken cancellationToken = default);
    Task<string> TransferTokenAsync(TransferRequest request, CancellationToken cancellationToken = default);
}

// DTOs
public record ChainInfo(
    ulong ChainId,
    string Name,
    ulong BlockHeight,
    ulong TotalTransactions,
    int TotalTokens,
    int TotalAccounts
);

public record Block(
    ulong Number,
    string Hash,
    string ParentHash,
    DateTime Timestamp,
    string Miner,
    ulong GasUsed,
    ulong GasLimit,
    IReadOnlyList<string> TransactionHashes
);

public record Transaction(
    string Hash,
    string From,
    string To,
    string Value,
    string? Data,
    ulong Nonce,
    ulong GasPrice,
    ulong GasLimit,
    string Status
);

public record TransactionReceipt(
    string TransactionHash,
    ulong BlockNumber,
    string BlockHash,
    string Status,
    ulong GasUsed
);

public record TransactionRequest(
    string From,
    string To,
    string Value,
    string? Data = null,
    ulong? GasLimit = null
);

public record AccountInfo(
    string Address,
    Dictionary<string, string> Balances,
    ulong Nonce,
    string? FutureWampumId = null
);

public record TokenInfo(
    string Symbol,
    string Name,
    int Decimals,
    string TotalSupply,
    string Owner,
    bool Mintable,
    bool Burnable
);

public record CreateTokenRequest(
    string Symbol,
    string Name,
    int Decimals,
    string InitialSupply,
    string Owner
);

public record TransferRequest(
    string From,
    string To,
    string Token,
    string Amount
);
