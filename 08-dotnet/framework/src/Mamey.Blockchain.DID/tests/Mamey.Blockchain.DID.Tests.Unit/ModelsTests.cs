using FluentAssertions;
using Xunit;

namespace Mamey.Blockchain.DID.Tests.Unit;

/// <summary>
/// Unit tests for DID Models.
/// </summary>
public class ModelsTests
{
    [Fact]
    public void DIDVerificationMethod_CanBeCreated()
    {
        // Arrange & Act
        var vm = new DIDVerificationMethod(
            Id: "did:futurewampum:123#key-1",
            Type: "Ed25519VerificationKey2020",
            Controller: "did:futurewampum:123",
            PublicKeyMultibase: "z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK");

        // Assert
        vm.Id.Should().Be("did:futurewampum:123#key-1");
        vm.Type.Should().Be("Ed25519VerificationKey2020");
        vm.Controller.Should().Be("did:futurewampum:123");
        vm.PublicKeyMultibase.Should().NotBeNull();
        vm.PublicKeyJwk.Should().BeNull();
    }

    [Fact]
    public void DIDService_CanBeCreated()
    {
        // Arrange & Act
        var service = new DIDService(
            Id: "did:futurewampum:123#linked-domain",
            Type: "LinkedDomains",
            ServiceEndpoint: "https://example.com");

        // Assert
        service.Id.Should().Be("did:futurewampum:123#linked-domain");
        service.Type.Should().Be("LinkedDomains");
        service.ServiceEndpoint.Should().Be("https://example.com");
    }

    [Fact]
    public void IssueDIDResult_Success()
    {
        // Arrange & Act
        var result = new IssueDIDResult(
            Success: true,
            DID: "did:futurewampum:abc123",
            DIDDocument: "{}",
            TransactionHash: "0x123",
            ErrorMessage: null);

        // Assert
        result.Success.Should().BeTrue();
        result.DID.Should().Be("did:futurewampum:abc123");
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void IssueDIDResult_Failure()
    {
        // Arrange & Act
        var result = new IssueDIDResult(
            Success: false,
            DID: null,
            DIDDocument: null,
            TransactionHash: null,
            ErrorMessage: "Controller not found");

        // Assert
        result.Success.Should().BeFalse();
        result.DID.Should().BeNull();
        result.ErrorMessage.Should().Be("Controller not found");
    }

    [Fact]
    public void DIDStatus_HasCorrectValues()
    {
        // Assert
        ((int)DIDStatus.Active).Should().Be(0);
        ((int)DIDStatus.Deactivated).Should().Be(1);
        ((int)DIDStatus.Suspended).Should().Be(2);
    }

    [Fact]
    public void DIDDocumentMetadata_CanBeCreated()
    {
        // Arrange & Act
        var metadata = new DIDDocumentMetadata(
            Created: DateTime.UtcNow.AddDays(-30),
            Updated: DateTime.UtcNow,
            VersionId: 5,
            Deactivated: false,
            CanonicalId: "did:futurewampum:abc123");

        // Assert
        metadata.VersionId.Should().Be(5);
        metadata.Deactivated.Should().BeFalse();
        metadata.CanonicalId.Should().Be("did:futurewampum:abc123");
    }

    [Fact]
    public void DIDHistoryEntry_CanBeCreated()
    {
        // Arrange & Act
        var entry = new DIDHistoryEntry(
            Version: 3,
            DIDDocument: "{}",
            Action: "updated",
            Actor: "did:futurewampum:controller",
            Timestamp: DateTime.UtcNow,
            TransactionHash: "0x456",
            Reason: "Key rotation");

        // Assert
        entry.Version.Should().Be(3);
        entry.Action.Should().Be("updated");
        entry.Reason.Should().Be("Key rotation");
    }
}
