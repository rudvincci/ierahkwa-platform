using System.Net.Http.Json;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Infrastructure.Clients;

/// <summary>
/// HTTP client implementation for FutureWampumLedger.Transaction service
/// </summary>
public class LedgerTransactionClient : ILedgerTransactionClient
{
    private readonly IHttpClient _httpClient;
    private readonly ILogger<LedgerTransactionClient> _logger;
    private readonly string _baseUrl;

    public LedgerTransactionClient(
        IHttpClient httpClient,
        ILogger<LedgerTransactionClient> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration.GetValue<string>("ledger:baseUrl") ?? "https://localhost:5501";
    }

    public async Task<bool> LogTransactionAsync(TransactionLogRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug(
                "Logging transaction to ledger: Type={TransactionType}, EntityType={EntityType}, EntityId={EntityId}",
                request.TransactionType,
                request.EntityType,
                request.EntityId
            );

            var response = await _httpClient.PostAsync(
                $"{_baseUrl}/api/transactions",
                request
            );

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "Successfully logged transaction to ledger: Type={TransactionType}, EntityId={EntityId}",
                    request.TransactionType,
                    request.EntityId
                );
                return true;
            }

            _logger.LogWarning(
                "Failed to log transaction to ledger: StatusCode={StatusCode}, Type={TransactionType}, EntityId={EntityId}",
                response.StatusCode,
                request.TransactionType,
                request.EntityId
            );
            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "HTTP error logging transaction to ledger: Type={TransactionType}, EntityId={EntityId}",
                request.TransactionType,
                request.EntityId
            );
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error logging transaction to ledger: Type={TransactionType}, EntityId={EntityId}",
                request.TransactionType,
                request.EntityId
            );
            return false;
        }
    }
}


