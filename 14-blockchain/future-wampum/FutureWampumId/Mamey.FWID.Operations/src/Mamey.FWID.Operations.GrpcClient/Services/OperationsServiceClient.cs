using Grpc.Core;
using Mamey.FWID.Operations.GrpcClient.Protos;

namespace Mamey.FWID.Operations.GrpcClient.Services;

/// <summary>
/// Client wrapper for GrpcOperationsService gRPC calls.
/// </summary>
public class OperationsServiceClient
{
    private readonly GrpcOperationsService.GrpcOperationsServiceClient _client;
    private readonly ILogger<OperationsServiceClient> _logger;

    public OperationsServiceClient(
        GrpcOperationsService.GrpcOperationsServiceClient client,
        ILogger<OperationsServiceClient> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets an operation by ID.
    /// </summary>
    public async Task<GetOperationResponse> GetOperationAsync(
        GetOperationRequest request,
        CallOptions? callOptions = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting operation: {Id}", request.Id);
        
        try
        {
            var response = await _client.GetOperationAsync(
                request,
                callOptions ?? new CallOptions(),
                cancellationToken: cancellationToken);
            
            _logger.LogInformation(
                "Operation retrieved. Id: {Id}, Name: {Name}, State: {State}",
                response.Id,
                response.Name,
                response.State);
            
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting operation: {Id}", request.Id);
            throw;
        }
    }

    /// <summary>
    /// Subscribes to operations stream.
    /// </summary>
    public async Task<IAsyncEnumerable<GetOperationResponse>> SubscribeOperationsAsync(
        Empty request,
        CallOptions? callOptions = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Subscribing to operations stream");
        
        try
        {
            using var call = _client.SubscribeOperations(
                request,
                callOptions ?? new CallOptions(),
                cancellationToken: cancellationToken);
            
            return call.ResponseStream.ReadAllAsync(cancellationToken);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error in operations stream subscription");
            throw;
        }
    }
}


