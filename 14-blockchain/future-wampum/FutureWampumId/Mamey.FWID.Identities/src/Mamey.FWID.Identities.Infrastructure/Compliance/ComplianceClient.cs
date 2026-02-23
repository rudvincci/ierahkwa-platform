using Grpc.Net.Client;
using Mamey.Compliance;
using Mamey.FWID.Identities.Application.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.FWID.Identities.Infrastructure.Compliance;

/// <summary>
/// gRPC client implementation for MameyNode ComplianceService.
/// 
/// TDD Reference: Lines 1476-1498 (Compliance Requirements)
/// BDD Reference: Lines 645-692 (VII. Compliance and Regulatory Framework)
/// </summary>
internal sealed class ComplianceClient : IComplianceClient, IDisposable
{
    private readonly ComplianceService.ComplianceServiceClient _client;
    private readonly GrpcChannel _channel;
    private readonly ComplianceOptions _options;
    private readonly ILogger<ComplianceClient> _logger;
    private bool _disposed;

    public ComplianceClient(
        IOptions<ComplianceOptions> options,
        ILogger<ComplianceClient> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _channel = GrpcChannel.ForAddress(_options.NodeUrl, new GrpcChannelOptions
        {
            HttpHandler = new SocketsHttpHandler
            {
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
                KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                EnableMultipleHttp2Connections = true
            }
        });

        _client = new ComplianceService.ComplianceServiceClient(_channel);

        _logger.LogInformation(
            "Compliance client initialized with endpoint: {NodeUrl}",
            _options.NodeUrl);
    }

    public async Task<Application.Clients.CreateAuditEntryResponse> CreateAuditEntryAsync(
        Application.Clients.CreateAuditEntryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            _logger.LogDebug("Compliance integration disabled, skipping audit entry creation");
            return new Application.Clients.CreateAuditEntryResponse(false, null, "Compliance integration disabled");
        }

        try
        {
            var grpcRequest = new Mamey.Compliance.CreateAuditEntryRequest
            {
                EntityType = request.EntityType,
                EntityId = request.EntityId,
                Action = request.Action,
                Actor = request.Actor
            };

            if (request.Details != null)
            {
                foreach (var (key, value) in request.Details)
                {
                    grpcRequest.Details.Add(key, value);
                }
            }

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.CreateAuditEntryAsync(
                grpcRequest,
                deadline: deadline,
                cancellationToken: cancellationToken);

            return new Application.Clients.CreateAuditEntryResponse(
                Success: response.Success,
                AuditEntryId: response.AuditEntryId,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating audit entry");
            return new Application.Clients.CreateAuditEntryResponse(false, null, ex.Message);
        }
    }

    public async Task<Application.Clients.GetAuditTrailResponse> GetAuditTrailAsync(
        string entityType,
        string entityId,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return new Application.Clients.GetAuditTrailResponse(Array.Empty<AuditEntryDto>(), 0, false, "Compliance integration disabled");
        }

        try
        {
            var grpcRequest = new Mamey.Compliance.GetAuditTrailRequest
            {
                EntityType = entityType,
                EntityId = entityId,
                Limit = (uint)limit,
                Offset = (uint)offset
            };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.GetAuditTrailAsync(
                grpcRequest,
                deadline: deadline,
                cancellationToken: cancellationToken);

            if (response.Success)
            {
                var entries = response.Entries.Select(e => new AuditEntryDto(
                    AuditEntryId: e.AuditEntryId,
                    EntityType: e.EntityType,
                    EntityId: e.EntityId,
                    Action: e.Action,
                    Actor: e.Actor,
                    Timestamp: (long)e.Timestamp,
                    Details: new Dictionary<string, string>(e.Details))).ToList();

                return new Application.Clients.GetAuditTrailResponse(
                    Entries: entries,
                    TotalCount: (int)response.TotalCount,
                    Success: true,
                    ErrorMessage: null);
            }

            return new Application.Clients.GetAuditTrailResponse(
                Entries: Array.Empty<AuditEntryDto>(),
                TotalCount: 0,
                Success: false,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting audit trail for {EntityType}:{EntityId}", entityType, entityId);
            return new Application.Clients.GetAuditTrailResponse(Array.Empty<AuditEntryDto>(), 0, false, ex.Message);
        }
    }

    public async Task<VerifyKycResponse> VerifyKycAsync(
        string accountId,
        string verificationType,
        Dictionary<string, string>? verificationData = null,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return new VerifyKycResponse(false, null, Array.Empty<string>(), false, "Compliance integration disabled");
        }

        try
        {
            var grpcRequest = new VerifyKYCRequest
            {
                AccountId = accountId,
                VerificationType = verificationType
            };

            if (verificationData != null)
            {
                foreach (var (key, value) in verificationData)
                {
                    grpcRequest.VerificationData.Add(key, value);
                }
            }

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.VerifyKYCAsync(
                grpcRequest,
                deadline: deadline,
                cancellationToken: cancellationToken);

            return new VerifyKycResponse(
                Verified: response.Verified,
                KycLevel: response.KycLevel,
                VerifiedAttributes: response.VerifiedAttributes.ToList(),
                Success: response.Success,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying KYC for {AccountId}", accountId);
            return new VerifyKycResponse(false, null, Array.Empty<string>(), false, ex.Message);
        }
    }

    public async Task<UpdateKycStatusResponse> UpdateKycStatusAsync(
        string accountId,
        string kycLevel,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return new UpdateKycStatusResponse(false, "Compliance integration disabled");
        }

        try
        {
            var grpcRequest = new UpdateKYCStatusRequest
            {
                AccountId = accountId,
                KycLevel = kycLevel,
                Status = status
            };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.UpdateKYCStatusAsync(
                grpcRequest,
                deadline: deadline,
                cancellationToken: cancellationToken);

            return new UpdateKycStatusResponse(
                Success: response.Success,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating KYC status for {AccountId}", accountId);
            return new UpdateKycStatusResponse(false, ex.Message);
        }
    }

    public async Task<GetKycStatusResponse?> GetKycStatusAsync(
        string accountId,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return null;
        }

        try
        {
            var grpcRequest = new GetKYCStatusRequest { AccountId = accountId };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.GetKYCStatusAsync(
                grpcRequest,
                deadline: deadline,
                cancellationToken: cancellationToken);

            if (response.Success)
            {
                return new GetKycStatusResponse(
                    KycLevel: response.KycLevel,
                    Status: response.Status,
                    VerifiedAt: response.VerifiedAt > 0 ? (long?)response.VerifiedAt : null,
                    ExpiresAt: response.ExpiresAt > 0 ? (long?)response.ExpiresAt : null,
                    Success: true,
                    ErrorMessage: null);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting KYC status for {AccountId}", accountId);
            return null;
        }
    }

    public async Task<CheckAmlResponse> CheckAmlAsync(
        string accountId,
        string? transactionId = null,
        string? amount = null,
        string? currency = null,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return new CheckAmlResponse(false, null, Array.Empty<string>(), false, "Compliance integration disabled");
        }

        try
        {
            var grpcRequest = new CheckAMLRequest
            {
                AccountId = accountId,
                TransactionId = transactionId ?? "",
                Amount = amount ?? "",
                Currency = currency ?? ""
            };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.CheckAMLAsync(
                grpcRequest,
                deadline: deadline,
                cancellationToken: cancellationToken);

            return new CheckAmlResponse(
                Flagged: response.Flagged,
                RiskLevel: response.RiskLevel,
                RiskFactors: response.RiskFactors.ToList(),
                Success: response.Success,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking AML for {AccountId}", accountId);
            return new CheckAmlResponse(false, null, Array.Empty<string>(), false, ex.Message);
        }
    }

    public async Task<Application.Clients.CreateRedFlagResponse> CreateRedFlagAsync(
        string accountId,
        string flagType,
        string severity,
        string description,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return new Application.Clients.CreateRedFlagResponse(false, null, "Compliance integration disabled");
        }

        try
        {
            var grpcRequest = new Mamey.Compliance.CreateRedFlagRequest
            {
                AccountId = accountId,
                FlagType = flagType,
                Severity = severity,
                Description = description
            };

            if (metadata != null)
            {
                foreach (var (key, value) in metadata)
                {
                    grpcRequest.Metadata.Add(key, value);
                }
            }

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.CreateRedFlagAsync(
                grpcRequest,
                deadline: deadline,
                cancellationToken: cancellationToken);

            return new Application.Clients.CreateRedFlagResponse(
                Success: response.Success,
                RedFlagId: response.RedFlagId,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating red flag for {AccountId}", accountId);
            return new Application.Clients.CreateRedFlagResponse(false, null, ex.Message);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _channel.Dispose();
        _disposed = true;

        _logger.LogDebug("Compliance client disposed");
    }
}
