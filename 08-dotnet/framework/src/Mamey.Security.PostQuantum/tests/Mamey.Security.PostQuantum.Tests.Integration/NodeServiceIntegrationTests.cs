using System.Threading.Tasks;
using Xunit;

namespace Mamey.Security.PostQuantum.Tests.Integration;

/// <summary>
/// Skeleton integration tests for PQC-aware NodeService behaviours.
/// These are skipped here because they require a running MameyNode
/// instance with PQC consensus enabled.
/// </summary>
public class NodeServiceIntegrationTests
{
    [Fact(Skip = "Requires running MameyNode instance with PQC enabled.")]
    public async Task PublishAndVerifyPqcSignedBlock_ShouldSucceed()
    {
        await Task.CompletedTask;
    }

    [Fact(Skip = "Requires running MameyNode instance with PQC enabled.")]
    public async Task Validator_ShouldRejectBlock_WithInvalidPqcSignature()
    {
        await Task.CompletedTask;
    }
}


