using Grpc.Core;
using Mamey.Government;

namespace Mamey.Blockchain.Government;

public interface IMameyGovernmentClient
{
    /// <summary>
    /// Gets the underlying gRPC client
    /// </summary>
    GovernmentService.GovernmentServiceClient Client { get; }

    /// <summary>
    /// Create CallOptions with metadata from credential provider
    /// </summary>
    CallOptions CreateCallOptions(CredentialProvider? credentialProvider = null, string? correlationId = null, CancellationToken cancellationToken = default);
}









