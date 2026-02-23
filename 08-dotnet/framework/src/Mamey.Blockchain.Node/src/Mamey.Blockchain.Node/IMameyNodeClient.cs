using Grpc.Core;
using Mamey.Node;
using Mamey.Rpc;

namespace Mamey.Blockchain.Node;

public interface IMameyNodeClient
{
    /// <summary>
    /// Gets the underlying NodeService gRPC client
    /// </summary>
    NodeService.NodeServiceClient Client { get; }

    /// <summary>
    /// Gets the underlying RpcService gRPC client
    /// </summary>
    RpcService.RpcServiceClient RpcClient { get; }

    /// <summary>
    /// Create CallOptions with metadata from credential provider
    /// </summary>
    CallOptions CreateCallOptions(CredentialProvider? credentialProvider = null, string? correlationId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get node information (staging smoke test method)
    /// </summary>
    Task<GetNodeInfoResponse> GetNodeInfoAsync(
        GetNodeInfoRequest? request = null,
        CredentialProvider? credentialProvider = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get RPC version (staging smoke test method)
    /// </summary>
    Task<VersionResponse> GetVersionAsync(
        VersionRequest? request = null,
        CredentialProvider? credentialProvider = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default);
}








