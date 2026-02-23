using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using TradeX.Core.Interfaces;

namespace TradeX.Infrastructure.Services;

/// <summary>
/// Ierahkwa Futurehead Mamey Node Service
/// Sovereign blockchain integration - 100% owned by Ierahkwa Government
/// </summary>
public class IerahkwaNodeService : IIerahkwaNodeService
{
    private readonly HttpClient _httpClient;
    private readonly IerahkwaNodeConfig _config;
    
    public IerahkwaNodeService(HttpClient httpClient, IerahkwaNodeConfig config)
    {
        _httpClient = httpClient;
        _config = config;
        _httpClient.BaseAddress = new Uri(config.NodeEndpoint);
        _httpClient.DefaultRequestHeaders.Add("X-Ierahkwa-API-Key", config.ApiKey);
    }
    
    public async Task<bool> IsConnectedAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<decimal> GetBalanceAsync(string address, string assetSymbol)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<BalanceResponse>(
                $"/api/v1/balance/{address}/{assetSymbol}");
            return response?.Balance ?? 0;
        }
        catch
        {
            return 0;
        }
    }
    
    public async Task<string> TransferAsync(string fromAddress, string toAddress, decimal amount, string assetSymbol)
    {
        var request = new TransferRequest(fromAddress, toAddress, amount, assetSymbol);
        
        var response = await _httpClient.PostAsJsonAsync("/api/v1/transfer", request);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<TransferResponse>();
        return result?.TxHash ?? throw new Exception("Transfer failed");
    }
    
    public async Task<string> GetTransactionStatusAsync(string txHash)
    {
        var response = await _httpClient.GetFromJsonAsync<TxStatusResponse>(
            $"/api/v1/tx/{txHash}");
        return response?.Status ?? "unknown";
    }
    
    public async Task<decimal> GetAssetPriceAsync(string assetSymbol)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<PriceResponse>(
                $"/api/v1/price/{assetSymbol}");
            return response?.Price ?? 0;
        }
        catch
        {
            return 0;
        }
    }
    
    public async Task<string> GenerateWalletAsync()
    {
        var response = await _httpClient.PostAsync("/api/v1/wallet/generate", null);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<WalletResponse>();
        return result?.Address ?? throw new Exception("Wallet generation failed");
    }
    
    public async Task<bool> ValidateAddressAsync(string address)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ValidationResponse>(
                $"/api/v1/validate/{address}");
            return response?.IsValid ?? false;
        }
        catch
        {
            return false;
        }
    }
}

// Configuration
public class IerahkwaNodeConfig
{
    public string NodeEndpoint { get; set; } = "https://node.ierahkwa.gov";
    public string ApiKey { get; set; } = string.Empty;
    public string NetworkId { get; set; } = "ierahkwa-mainnet";
    public int ChainId { get; set; } = 777777;
    public string Currency { get; set; } = "IGT";
}

// Response DTOs
public record BalanceResponse(decimal Balance, string Asset);
public record TransferRequest(string From, string To, decimal Amount, string Asset);
public record TransferResponse(string TxHash, string Status);
public record TxStatusResponse(string TxHash, string Status, int Confirmations);
public record PriceResponse(string Asset, decimal Price, decimal Change24h);
public record WalletResponse(string Address, string PublicKey);
public record ValidationResponse(bool IsValid, string AddressType);
