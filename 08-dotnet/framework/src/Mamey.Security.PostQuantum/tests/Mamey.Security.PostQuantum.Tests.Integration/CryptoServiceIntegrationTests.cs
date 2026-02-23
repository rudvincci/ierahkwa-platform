using System.Threading.Tasks;
using Xunit;

namespace Mamey.Security.PostQuantum.Tests.Integration;

/// <summary>
/// Skeleton integration tests for the PQC-enabled Crypto gRPC service.
/// These are skipped in this environment because they require running
/// gRPC services and a native liboqs runtime.
/// </summary>
public class CryptoServiceIntegrationTests
{
    [Fact(Skip = "Requires running CryptoService gRPC endpoint and native liboqs runtime.")]
    public async Task GenerateKeypair_And_SignVerify_ShouldSucceed()
    {
        await Task.CompletedTask;
    }

    [Fact(Skip = "Requires running CryptoService gRPC endpoint and native liboqs runtime.")]
    public async Task GetSupportedAlgorithms_ShouldReturnPqcAlgorithms()
    {
        await Task.CompletedTask;
    }
}


