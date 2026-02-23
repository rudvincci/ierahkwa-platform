using Grpc.Net.Client;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.Government;
using MameyGov = Mamey.Government;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.FWID.Identities.Infrastructure.Blockchain;

/// <summary>
/// Client implementation for Government Identity blockchain operations via MameyNode GovernmentService.
/// 
/// TDD Reference: Line 1594-1703 (Feature Domain 1: Biometric Identity)
/// BDD Reference: Lines 86-112 (I.1-I.3 Executive Summary - Sovereign Identity)
/// </summary>
internal sealed class GovernmentIdentityClient : IGovernmentIdentityClient, IDisposable
{
    private readonly GovernmentService.GovernmentServiceClient _client;
    private readonly GrpcChannel _channel;
    private readonly GovernmentIdentityOptions _options;
    private readonly ILogger<GovernmentIdentityClient> _logger;
    private bool _disposed;

    public GovernmentIdentityClient(
        IOptions<GovernmentIdentityOptions> options,
        ILogger<GovernmentIdentityClient> logger)
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

        _client = new GovernmentService.GovernmentServiceClient(_channel);

        _logger.LogInformation(
            "Government identity client initialized with endpoint: {NodeUrl}",
            _options.NodeUrl);
    }

    public async Task<CreateIdentityResult> CreateIdentityAsync(
        Application.Clients.CreateIdentityRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            _logger.LogDebug("Government identity integration disabled, skipping identity creation");
            return new CreateIdentityResult(false, null, null, "Government identity integration disabled");
        }

        try
        {
            _logger.LogDebug(
                "Creating sovereign identity on blockchain: CitizenId={CitizenId}, Name={FirstName} {LastName}",
                request.CitizenId, request.FirstName, request.LastName);

            var grpcRequest = new Mamey.Government.CreateIdentityRequest
            {
                CitizenId = request.CitizenId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Nationality = request.Nationality
            };

            if (request.Metadata != null)
            {
                foreach (var (key, value) in request.Metadata)
                {
                    grpcRequest.Metadata.Add(key, value);
                }
            }

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.CreateIdentityAsync(
                grpcRequest,
                deadline: deadline,
                cancellationToken: cancellationToken);

            if (response.Success)
            {
                _logger.LogDebug(
                    "Successfully created sovereign identity: IdentityId={IdentityId}, BlockchainAccount={BlockchainAccount}",
                    response.IdentityId, response.BlockchainAccount);

                return new CreateIdentityResult(
                    Success: true,
                    IdentityId: response.IdentityId,
                    BlockchainAccount: response.BlockchainAccount,
                    ErrorMessage: null);
            }

            _logger.LogWarning(
                "Failed to create sovereign identity: Error={Error}",
                response.ErrorMessage);

            return new CreateIdentityResult(
                Success: false,
                IdentityId: null,
                BlockchainAccount: null,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sovereign identity on blockchain");
            return new CreateIdentityResult(
                Success: false,
                IdentityId: null,
                BlockchainAccount: null,
                ErrorMessage: ex.Message);
        }
    }

    public async Task<GetIdentityResult?> GetIdentityAsync(
        string identityId,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return null;
        }

        try
        {
            _logger.LogDebug("Getting sovereign identity from blockchain: IdentityId={IdentityId}", identityId);

            var grpcRequest = new GetIdentityRequest { IdentityId = identityId };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.GetIdentityAsync(
                grpcRequest,
                deadline: deadline,
                cancellationToken: cancellationToken);

            if (response.Success)
            {
                return new GetIdentityResult(
                    IdentityId: response.IdentityId,
                    CitizenId: response.CitizenId,
                    FirstName: response.FirstName,
                    LastName: response.LastName,
                    DateOfBirth: response.DateOfBirth,
                    Nationality: response.Nationality,
                    Status: response.Status.ToString(),
                    Success: true,
                    ErrorMessage: null);
            }

            return new GetIdentityResult(
                IdentityId: identityId,
                CitizenId: "",
                FirstName: "",
                LastName: "",
                DateOfBirth: "",
                Nationality: "",
                Status: "Unknown",
                Success: false,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sovereign identity from blockchain: IdentityId={IdentityId}", identityId);
            return null;
        }
    }

    public async Task<UpdateIdentityResult> UpdateIdentityAsync(
        string identityId,
        Dictionary<string, string> updates,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            _logger.LogDebug("Government identity integration disabled, skipping identity update");
            return new UpdateIdentityResult(false, "Government identity integration disabled");
        }

        try
        {
            _logger.LogDebug(
                "Updating sovereign identity on blockchain: IdentityId={IdentityId}",
                identityId);

            var grpcRequest = new UpdateIdentityRequest { IdentityId = identityId };
            foreach (var (key, value) in updates)
            {
                grpcRequest.Updates.Add(key, value);
            }

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.UpdateIdentityAsync(
                grpcRequest,
                deadline: deadline,
                cancellationToken: cancellationToken);

            if (response.Success)
            {
                _logger.LogDebug(
                    "Successfully updated sovereign identity: IdentityId={IdentityId}",
                    identityId);

                return new UpdateIdentityResult(Success: true, ErrorMessage: null);
            }

            _logger.LogWarning(
                "Failed to update sovereign identity: IdentityId={IdentityId}, Error={Error}",
                identityId, response.ErrorMessage);

            return new UpdateIdentityResult(Success: false, ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sovereign identity on blockchain: IdentityId={IdentityId}", identityId);
            return new UpdateIdentityResult(Success: false, ErrorMessage: ex.Message);
        }
    }

    public async Task<Application.Clients.VerifyIdentityResult> VerifyIdentityAsync(
        string identityId,
        string verificationType,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return new Application.Clients.VerifyIdentityResult(false, null, false, "Government identity integration disabled");
        }

        try
        {
            _logger.LogDebug(
                "Verifying sovereign identity on blockchain: IdentityId={IdentityId}, Type={Type}",
                identityId, verificationType);

            var grpcRequest = new Mamey.Government.VerifyIdentityRequest
            {
                IdentityId = identityId,
                VerificationType = verificationType
            };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.VerifyIdentityAsync(
                grpcRequest,
                deadline: deadline,
                cancellationToken: cancellationToken);

            return new Application.Clients.VerifyIdentityResult(
                Verified: response.Verified,
                VerificationResult: response.VerificationResult,
                Success: response.Success,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying sovereign identity on blockchain: IdentityId={IdentityId}", identityId);
            return new Application.Clients.VerifyIdentityResult(false, null, false, ex.Message);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _channel.Dispose();
        _disposed = true;

        _logger.LogDebug("Government identity client disposed");
    }
}
