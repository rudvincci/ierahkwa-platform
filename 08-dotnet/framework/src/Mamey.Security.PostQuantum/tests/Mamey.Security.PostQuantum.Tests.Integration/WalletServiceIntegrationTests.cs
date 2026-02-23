using System.Threading.Tasks;
using Xunit;

namespace Mamey.Security.PostQuantum.Tests.Integration;

/// <summary>
/// Skeleton integration tests for the PQC-enabled Wallet gRPC service.
/// These are skipped here because they require live infrastructure
/// (database + WalletService host) and native liboqs runtime.
/// </summary>
public class WalletServiceIntegrationTests
{
    [Fact(Skip = "Requires running WalletService gRPC endpoint and backing database.")]
    public async Task GenerateAndStorePqcWalletKey_ShouldSucceed()
    {
        await Task.CompletedTask;
    }

    [Fact(Skip = "Requires running WalletService gRPC endpoint and backing database.")]
    public async Task MigrateRsaKeyToMldsa_ShouldCreateHybridLink()
    {
        await Task.CompletedTask;
    }
}


