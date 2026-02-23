using Grpc.Core;
using Mamey.FWID.Identities.Api.Protos;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.GrpcClient.Services;

/// <summary>
/// Client wrapper for BiometricService gRPC calls.
/// </summary>
public class BiometricServiceClient
{
    private readonly BiometricService.BiometricServiceClient _client;
    private readonly ILogger<BiometricServiceClient> _logger;

    public BiometricServiceClient(
        BiometricService.BiometricServiceClient client,
        ILogger<BiometricServiceClient> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Verifies biometric data for an identity.
    /// </summary>
    public async Task<VerifyBiometricResponse> VerifyBiometricAsync(
        VerifyBiometricRequest request,
        CallOptions? callOptions = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verifying biometric for identity: {IdentityId}", request.IdentityId);
        
        try
        {
            var options = callOptions ?? new CallOptions(cancellationToken: cancellationToken);
            var response = await _client.VerifyBiometricAsync(
                request,
                options);
            
            _logger.LogInformation(
                "Biometric verification completed. Verified: {IsVerified}, Score: {MatchScore}",
                response.IsVerified,
                response.MatchScore);
            
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error verifying biometric for identity: {IdentityId}", request.IdentityId);
            throw;
        }
    }

    /// <summary>
    /// Verifies biometric data using a streaming call.
    /// </summary>
    public async Task<IAsyncEnumerable<VerifyBiometricResponse>> VerifyBiometricStreamAsync(
        IAsyncEnumerable<VerifyBiometricRequest> requests,
        CallOptions? callOptions = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting biometric verification stream");
        
        try
        {
            using var call = _client.VerifyBiometricStream(
                cancellationToken: cancellationToken);
            
            await foreach (var request in requests.WithCancellation(cancellationToken))
            {
                await call.RequestStream.WriteAsync(request, cancellationToken);
            }
            
            await call.RequestStream.CompleteAsync();
            
            return call.ResponseStream.ReadAllAsync(cancellationToken);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error in biometric verification stream");
            throw;
        }
    }
}
