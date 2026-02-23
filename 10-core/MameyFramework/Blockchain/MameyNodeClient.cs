using System.Net.Http.Json;
using System.Text.Json;

namespace MameyFramework.Blockchain;

/// <summary>
/// HTTP client implementation for MameyNode
/// </summary>
public class MameyNodeClient : IBlockchainClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private string? _nodeUrl;
    
    public bool IsConnected => !string.IsNullOrEmpty(_nodeUrl);
    
    public MameyNodeClient(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
    }
    
    public Task<bool> ConnectAsync(string nodeUrl, CancellationToken cancellationToken = default)
    {
        _nodeUrl = nodeUrl.TrimEnd('/');
        return Task.FromResult(true);
    }
    
    public Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        _nodeUrl = null;
        return Task.CompletedTask;
    }
    
    public async Task<ChainInfo> GetChainInfoAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonDocument>(
            $"{_nodeUrl}/api/v1/stats", cancellationToken);
        
        var root = response!.RootElement;
        return new ChainInfo(
            root.GetProperty("chain_id").GetUInt64(),
            "Ierahkwa",
            root.GetProperty("block_height").GetUInt64(),
            root.GetProperty("total_transactions").GetUInt64(),
            root.GetProperty("total_tokens").GetInt32(),
            root.GetProperty("total_accounts").GetInt32()
        );
    }
    
    public async Task<ulong> GetBlockNumberAsync(CancellationToken cancellationToken = default)
    {
        var info = await GetChainInfoAsync(cancellationToken);
        return info.BlockHeight - 1;
    }
    
    public async Task<Block?> GetBlockAsync(ulong blockNumber, CancellationToken cancellationToken = default)
    {
        var response = await RpcCallAsync("eth_getBlockByNumber", 
            new object[] { $"0x{blockNumber:x}", true }, cancellationToken);
        
        if (response == null) return null;
        
        return ParseBlock(response);
    }
    
    public async Task<Block?> GetBlockByHashAsync(string hash, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonDocument>(
            $"{_nodeUrl}/api/v1/blocks/{hash}", cancellationToken);
        
        if (response == null) return null;
        
        return ParseBlock(response.RootElement.GetProperty("block"));
    }
    
    public async Task<string> SendTransactionAsync(TransactionRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{_nodeUrl}/api/v1/transactions", request, cancellationToken);
        
        var result = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken);
        return result!.RootElement.GetProperty("transaction_hash").GetString()!;
    }
    
    public async Task<Transaction?> GetTransactionAsync(string hash, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonDocument>(
            $"{_nodeUrl}/api/v1/transactions/{hash}", cancellationToken);
        
        if (response == null) return null;
        
        var tx = response.RootElement.GetProperty("transaction");
        return new Transaction(
            tx.GetProperty("hash").GetString()!,
            tx.GetProperty("from").GetString()!,
            tx.GetProperty("to").GetString()!,
            tx.GetProperty("value").GetRawText(),
            tx.TryGetProperty("data", out var data) ? data.GetString() : null,
            tx.GetProperty("nonce").GetUInt64(),
            tx.GetProperty("gas_price").GetUInt64(),
            tx.GetProperty("gas_limit").GetUInt64(),
            tx.GetProperty("status").GetString()!
        );
    }
    
    public async Task<TransactionReceipt?> GetTransactionReceiptAsync(string hash, CancellationToken cancellationToken = default)
    {
        var tx = await GetTransactionAsync(hash, cancellationToken);
        if (tx == null) return null;
        
        return new TransactionReceipt(hash, 0, "", tx.Status, 0);
    }
    
    public async Task<TransactionReceipt> WaitForTransactionAsync(string hash, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            var receipt = await GetTransactionReceiptAsync(hash, cancellationToken);
            if (receipt != null && receipt.Status != "pending")
                return receipt;
            
            await Task.Delay(1000, cancellationToken);
        }
    }
    
    public async Task<AccountInfo> GetAccountAsync(string address, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonDocument>(
            $"{_nodeUrl}/api/v1/accounts/{address}", cancellationToken);
        
        var acc = response!.RootElement.GetProperty("account");
        var balances = new Dictionary<string, string>();
        
        if (acc.TryGetProperty("balances", out var balancesElement))
        {
            foreach (var prop in balancesElement.EnumerateObject())
            {
                balances[prop.Name] = prop.Value.GetRawText();
            }
        }
        
        return new AccountInfo(
            address,
            balances,
            acc.TryGetProperty("nonce", out var nonce) ? nonce.GetUInt64() : 0,
            acc.TryGetProperty("fwid", out var fwid) ? fwid.GetString() : null
        );
    }
    
    public async Task<string> GetBalanceAsync(string address, string token = "WAMPUM", CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonDocument>(
            $"{_nodeUrl}/api/v1/accounts/{address}/balance", cancellationToken);
        
        var balances = response!.RootElement.GetProperty("balances");
        if (balances.TryGetProperty(token, out var balance))
            return balance.GetString()!;
        
        return "0";
    }
    
    public async Task<ulong> GetNonceAsync(string address, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonDocument>(
            $"{_nodeUrl}/api/v1/accounts/{address}/nonce", cancellationToken);
        
        return response!.RootElement.GetProperty("nonce").GetUInt64();
    }
    
    public async Task<IReadOnlyList<TokenInfo>> GetTokensAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonDocument>(
            $"{_nodeUrl}/api/v1/tokens", cancellationToken);
        
        var tokens = new List<TokenInfo>();
        foreach (var token in response!.RootElement.GetProperty("tokens").EnumerateArray())
        {
            tokens.Add(ParseToken(token));
        }
        
        return tokens;
    }
    
    public async Task<TokenInfo?> GetTokenAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonDocument>(
            $"{_nodeUrl}/api/v1/tokens/{symbol}", cancellationToken);
        
        if (response == null) return null;
        
        return ParseToken(response.RootElement.GetProperty("token"));
    }
    
    public async Task<string> CreateTokenAsync(CreateTokenRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{_nodeUrl}/api/v1/tokens", request, cancellationToken);
        
        var result = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken);
        return result!.RootElement.GetProperty("symbol").GetString()!;
    }
    
    public async Task<string> TransferTokenAsync(TransferRequest request, CancellationToken cancellationToken = default)
    {
        return await SendTransactionAsync(new TransactionRequest(
            request.From,
            request.To,
            request.Amount,
            $"transfer:{request.Token}"
        ), cancellationToken);
    }
    
    private async Task<JsonElement?> RpcCallAsync(string method, object[] parameters, CancellationToken cancellationToken)
    {
        var request = new
        {
            jsonrpc = "2.0",
            method,
            @params = parameters,
            id = 1
        };
        
        var response = await _httpClient.PostAsJsonAsync($"{_nodeUrl}/rpc", request, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken);
        
        if (result!.RootElement.TryGetProperty("result", out var resultElement))
            return resultElement;
        
        return null;
    }
    
    private static Block ParseBlock(JsonElement element)
    {
        var txHashes = new List<string>();
        if (element.TryGetProperty("transactions", out var transactions))
        {
            foreach (var tx in transactions.EnumerateArray())
            {
                txHashes.Add(tx.GetString() ?? tx.GetProperty("hash").GetString()!);
            }
        }
        
        return new Block(
            element.GetProperty("number").GetUInt64(),
            element.GetProperty("hash").GetString()!,
            element.GetProperty("parentHash").GetString()!,
            DateTime.UnixEpoch.AddSeconds(element.GetProperty("timestamp").GetInt64()),
            element.GetProperty("miner").GetString()!,
            element.GetProperty("gasUsed").GetUInt64(),
            element.GetProperty("gasLimit").GetUInt64(),
            txHashes
        );
    }
    
    private static TokenInfo ParseToken(JsonElement element)
    {
        return new TokenInfo(
            element.GetProperty("symbol").GetString()!,
            element.GetProperty("name").GetString()!,
            element.GetProperty("decimals").GetInt32(),
            element.GetProperty("total_supply").GetRawText(),
            element.GetProperty("owner").GetString()!,
            element.TryGetProperty("mintable", out var m) && m.GetBoolean(),
            element.TryGetProperty("burnable", out var b) && b.GetBoolean()
        );
    }
    
    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
