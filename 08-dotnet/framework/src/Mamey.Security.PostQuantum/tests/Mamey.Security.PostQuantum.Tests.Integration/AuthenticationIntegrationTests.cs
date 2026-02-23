using System.Threading.Tasks;
using Xunit;

namespace Mamey.Security.PostQuantum.Tests.Integration;

/// <summary>
/// Skeleton integration tests for PQC JWT / hybrid JWT authentication
/// flows across services. These are skipped here because they require
/// a full FutureWampum / MameyNode environment.
/// </summary>
public class AuthenticationIntegrationTests
{
    [Fact(Skip = "Requires running auth and portal services with PQC JWT enabled.")]
    public async Task Login_And_UsePqcJwtToAccessApi_ShouldSucceed()
    {
        await Task.CompletedTask;
    }

    [Fact(Skip = "Requires running portal with hybrid JWT support.")]
    public async Task HybridJwt_ShouldBeAcceptedByPortalAuth()
    {
        await Task.CompletedTask;
    }
}


