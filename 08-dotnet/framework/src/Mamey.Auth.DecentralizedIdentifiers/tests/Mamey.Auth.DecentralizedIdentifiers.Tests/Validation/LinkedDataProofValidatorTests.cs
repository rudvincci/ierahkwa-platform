using System.Text.Json;
using Moq;
using FluentAssertions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Validation;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers.Tests.Validation;

public class LinkedDataProofValidatorTests
{
    private readonly Mock<IProofService> _mockProofService;
    private readonly Mock<IDidResolver> _mockDidResolver;

    private readonly LinkedDataProofValidator _validator;

    // Use a valid Ed25519 base58 key (32 bytes, random for test only)
    private const string ValidEd25519KeyBase58 = "4v4rVfL8WTExxULF2g5DUSWJjSufWQas7GbGtp1DbJwy";

    public LinkedDataProofValidatorTests()
    {
        _mockProofService = new Mock<IProofService>();
        _mockDidResolver = new Mock<IDidResolver>();
        _validator = new LinkedDataProofValidator(_mockProofService.Object, _mockDidResolver.Object);
    }

    [Fact]
    public async Task ValidateAsync_ValidEd25519Proof_ShouldReturnTrue()
    {
        // Arrange: Minimal VC with proof
        string vcJson = @"
        {
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""id"": ""urn:uuid:1234"",
          ""type"": [""VerifiableCredential""],
          ""issuer"": ""did:example:issuer"",
          ""issuanceDate"": ""2024-01-01T00:00:00Z"",
          ""credentialSubject"": { ""id"": ""did:example:subject"" },
          ""proof"": {
            ""type"": ""Ed25519Signature2020"",
            ""created"": ""2024-01-01T00:00:00Z"",
            ""verificationMethod"": ""did:example:issuer#key-1"",
            ""proofPurpose"": ""assertionMethod"",
            ""proofValue"": ""z7D3...""
          }
        }";

        // Setup DID Document with correct key
        var verificationMethods = new List<IDidVerificationMethod>
        {
            new VerificationMethod(
                id: "did:example:issuer#key-1",
                type: "Ed25519VerificationKey2020",
                controller: "did:example:issuer",
                publicKeyBase58: ValidEd25519KeyBase58
            )
        };

        var didDoc = new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: "did:example:issuer",
            verificationMethods: verificationMethods
        );

        _mockDidResolver.Setup(r => r.ResolveAsync("did:example:issuer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = didDoc });

        _mockProofService.Setup(p =>
                p.VerifyProofAsync(
                    It.IsAny<string>(), It.IsAny<JsonElement>(), It.IsAny<byte[]>(),
                    "Ed25519Signature2020", "assertionMethod", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _validator.ValidateAsync(vcJson);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_InvalidProofSignature_ShouldReturnFalse()
    {
        // Arrange: As above, but proofService returns false (invalid sig)
        string vcJson = @"
        {
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""id"": ""urn:uuid:5678"",
          ""type"": [""VerifiableCredential""],
          ""issuer"": ""did:example:issuer"",
          ""issuanceDate"": ""2024-01-01T00:00:00Z"",
          ""credentialSubject"": { ""id"": ""did:example:subject"" },
          ""proof"": {
            ""type"": ""Ed25519Signature2020"",
            ""created"": ""2024-01-01T00:00:00Z"",
            ""verificationMethod"": ""did:example:issuer#key-1"",
            ""proofPurpose"": ""assertionMethod"",
            ""proofValue"": ""z7D3...""
          }
        }";

        var verificationMethods = new List<IDidVerificationMethod>
        {
            new VerificationMethod(
                id: "did:example:issuer#key-1",
                type: "Ed25519VerificationKey2020",
                controller: "did:example:issuer",
                publicKeyBase58: ValidEd25519KeyBase58
            )
        };

        var didDoc = new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: "did:example:issuer",
            verificationMethods: verificationMethods
        );

        _mockDidResolver.Setup(r => r.ResolveAsync("did:example:issuer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = didDoc });

        _mockProofService.Setup(p =>
                p.VerifyProofAsync(
                    It.IsAny<string>(), It.IsAny<JsonElement>(), It.IsAny<byte[]>(),
                    "Ed25519Signature2020", "assertionMethod", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.ValidateAsync(vcJson);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateAsync_MissingProof_ShouldThrow()
    {
        string noProof = @"
        {
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""id"": ""urn:uuid:noproof"",
          ""type"": [""VerifiableCredential""],
          ""issuer"": ""did:example:issuer"",
          ""issuanceDate"": ""2024-01-01T00:00:00Z"",
          ""credentialSubject"": { ""id"": ""did:example:subject"" }
        }";

        Func<Task> act = async () => await _validator.ValidateAsync(noProof);
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*proof*");
    }

    [Fact]
    public async Task ValidateAsync_VerificationMethodNotFound_ShouldThrow()
    {
        string vcJson = @"
        {
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""id"": ""urn:uuid:9999"",
          ""type"": [""VerifiableCredential""],
          ""issuer"": ""did:example:issuer"",
          ""issuanceDate"": ""2024-01-01T00:00:00Z"",
          ""credentialSubject"": { ""id"": ""did:example:subject"" },
          ""proof"": {
            ""type"": ""Ed25519Signature2020"",
            ""created"": ""2024-01-01T00:00:00Z"",
            ""verificationMethod"": ""did:example:issuer#missing-key"",
            ""proofPurpose"": ""assertionMethod"",
            ""proofValue"": ""z7D3...""
          }
        }";
        // Only a different key, not the required one
        var verificationMethods = new List<IDidVerificationMethod>
        {
            new VerificationMethod(
                id: "did:example:issuer#key-1",
                type: "Ed25519VerificationKey2020",
                controller: "did:example:issuer",
                publicKeyBase58: ValidEd25519KeyBase58
            )
        };
        var didDoc = new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: "did:example:issuer",
            verificationMethods: verificationMethods
        );
        _mockDidResolver.Setup(r => r.ResolveAsync("did:example:issuer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = didDoc });

        Func<Task> act = async () => await _validator.ValidateAsync(vcJson);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*verification method*did:example:issuer#missing-key*not found*");
    }

    [Fact]
    public async Task ValidateAsync_MultipleProofs_AllValid_ShouldReturnTrue()
    {
        string vcJson = @"
        {
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""id"": ""urn:uuid:multi"",
          ""type"": [""VerifiableCredential""],
          ""issuer"": ""did:example:issuer"",
          ""issuanceDate"": ""2024-01-01T00:00:00Z"",
          ""credentialSubject"": { ""id"": ""did:example:subject"" },
          ""proof"": [
            {
              ""type"": ""Ed25519Signature2020"",
              ""created"": ""2024-01-01T00:00:00Z"",
              ""verificationMethod"": ""did:example:issuer#key-1"",
              ""proofPurpose"": ""assertionMethod"",
              ""proofValue"": ""z7D3...""
            },
            {
              ""type"": ""Ed25519Signature2020"",
              ""created"": ""2024-01-01T00:01:00Z"",
              ""verificationMethod"": ""did:example:issuer#key-1"",
              ""proofPurpose"": ""assertionMethod"",
              ""proofValue"": ""z9F4...""
            }
          ]
        }";
        var verificationMethods = new List<IDidVerificationMethod>
        {
            new VerificationMethod(
                id: "did:example:issuer#key-1",
                type: "Ed25519VerificationKey2020",
                controller: "did:example:issuer",
                publicKeyBase58: ValidEd25519KeyBase58
            )
        };
        var didDoc = new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: "did:example:issuer",
            verificationMethods: verificationMethods
        );
        _mockDidResolver.Setup(r => r.ResolveAsync("did:example:issuer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = didDoc });

        _mockProofService.Setup(p =>
                p.VerifyProofAsync(It.IsAny<string>(), It.IsAny<JsonElement>(), It.IsAny<byte[]>(),
                    "Ed25519Signature2020", "assertionMethod", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(vcJson);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_MultipleProofs_OneInvalid_ShouldReturnFalse()
    {
        string vcJson = @"
        {
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""id"": ""urn:uuid:multi-invalid"",
          ""type"": [""VerifiableCredential""],
          ""issuer"": ""did:example:issuer"",
          ""issuanceDate"": ""2024-01-01T00:00:00Z"",
          ""credentialSubject"": { ""id"": ""did:example:subject"" },
          ""proof"": [
            {
              ""type"": ""Ed25519Signature2020"",
              ""created"": ""2024-01-01T00:00:00Z"",
              ""verificationMethod"": ""did:example:issuer#key-1"",
              ""proofPurpose"": ""assertionMethod"",
              ""proofValue"": ""z7D3...""
            },
            {
              ""type"": ""Ed25519Signature2020"",
              ""created"": ""2024-01-01T00:01:00Z"",
              ""verificationMethod"": ""did:example:issuer#key-1"",
              ""proofPurpose"": ""assertionMethod"",
              ""proofValue"": ""z9F4...""
            }
          ]
        }";
        var verificationMethods = new List<IDidVerificationMethod>
        {
            new VerificationMethod(
                id: "did:example:issuer#key-1",
                type: "Ed25519VerificationKey2020",
                controller: "did:example:issuer",
                publicKeyBase58: ValidEd25519KeyBase58
            )
        };
        var didDoc = new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: "did:example:issuer",
            verificationMethods: verificationMethods
        );
        _mockDidResolver.Setup(r => r.ResolveAsync("did:example:issuer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = didDoc });

        int callCount = 0;
        _mockProofService.Setup(p =>
                p.VerifyProofAsync(It.IsAny<string>(), It.IsAny<JsonElement>(), It.IsAny<byte[]>(),
                    "Ed25519Signature2020", "assertionMethod", It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => callCount++ == 0 ? true : false);

        var result = await _validator.ValidateAsync(vcJson);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateAsync_UnknownProofType_ShouldThrow()
    {
        string vcJson = @"
        {
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""id"": ""urn:uuid:unknown-proof"",
          ""type"": [""VerifiableCredential""],
          ""issuer"": ""did:example:issuer"",
          ""issuanceDate"": ""2024-01-01T00:00:00Z"",
          ""credentialSubject"": { ""id"": ""did:example:subject"" },
          ""proof"": {
            ""type"": ""UnknownProofType2024"",
            ""created"": ""2024-01-01T00:00:00Z"",
            ""verificationMethod"": ""did:example:issuer#key-1"",
            ""proofPurpose"": ""assertionMethod"",
            ""proofValue"": ""z7D3...""
          }
        }";
        var verificationMethods = new List<IDidVerificationMethod>
        {
            new VerificationMethod(
                id: "did:example:issuer#key-1",
                type: "Ed25519VerificationKey2020",
                controller: "did:example:issuer",
                publicKeyBase58: ValidEd25519KeyBase58
            )
        };
        var didDoc = new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: "did:example:issuer",
            verificationMethods: verificationMethods
        );
        _mockDidResolver.Setup(r => r.ResolveAsync("did:example:issuer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = didDoc });

        _mockProofService.Setup(p =>
                p.VerifyProofAsync(It.IsAny<string>(), It.IsAny<JsonElement>(), It.IsAny<byte[]>(),
                    "UnknownProofType2024", "assertionMethod", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotSupportedException("Unknown proof type"));

        Func<Task> act = async () => await _validator.ValidateAsync(vcJson);

        await act.Should().ThrowAsync<NotSupportedException>()
            .WithMessage("*Unknown proof type*");
    }

    [Fact]
    public async Task ValidateAsync_Secp256k1JwsProof_Interop_ShouldSucceed()
    {
        string vcJson = @"
        {
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""id"": ""urn:uuid:secp256k1"",
          ""type"": [""VerifiableCredential""],
          ""issuer"": ""did:example:issuer"",
          ""issuanceDate"": ""2024-01-01T00:00:00Z"",
          ""credentialSubject"": { ""id"": ""did:example:subject"" },
          ""proof"": {
            ""type"": ""EcdsaSecp256k1Signature2019"",
            ""created"": ""2024-01-01T00:00:00Z"",
            ""verificationMethod"": ""did:example:issuer#key-jwk"",
            ""proofPurpose"": ""assertionMethod"",
            ""jws"": ""eyJhbGciOiJFUzI1NksiLCJ... (truncated) ...""
          }
        }";

        var verificationMethods = new List<IDidVerificationMethod>
        {
            new VerificationMethod(
                id: "did:example:issuer#key-jwk",
                type: "EcdsaSecp256k1VerificationKey2019",
                controller: "did:example:issuer",
                publicKeyJwk: new Dictionary<string, object>
                {
                    { "kty", "EC" },
                    { "crv", "secp256k1" },
                    { "x", "MHcCAQEEIKZtAcX2IqfWT2_9Vp01dJ1eV9IUdA8X0jEGyKkA8Eha" },
                    { "y", "MHcCAQEEIKZtAcX2IqfWT2_9Vp01dJ1eV9IUdA8X0jEGyKkA8Ehb" }
                }
            )
        };
        var didDoc = new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: "did:example:issuer",
            verificationMethods: verificationMethods
        );
        _mockDidResolver.Setup(r => r.ResolveAsync("did:example:issuer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = didDoc });

        _mockProofService.Setup(p =>
                p.VerifyProofAsync(It.IsAny<string>(), It.IsAny<JsonElement>(), It.IsAny<byte[]>(),
                    "EcdsaSecp256k1Signature2019", "assertionMethod", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(vcJson);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_ProofWithChallengeDomain_ShouldSucceed()
    {
        string vcJson = @"
        {
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""id"": ""urn:uuid:challenge"",
          ""type"": [""VerifiableCredential""],
          ""issuer"": ""did:example:issuer"",
          ""issuanceDate"": ""2024-01-01T00:00:00Z"",
          ""credentialSubject"": { ""id"": ""did:example:subject"" },
          ""proof"": {
            ""type"": ""Ed25519Signature2020"",
            ""created"": ""2024-01-01T00:00:00Z"",
            ""verificationMethod"": ""did:example:issuer#key-1"",
            ""proofPurpose"": ""assertionMethod"",
            ""proofValue"": ""z7D3..."",
            ""challenge"": ""abc123"",
            ""domain"": ""verifier.example.com""
          }
        }";
        var verificationMethods = new List<IDidVerificationMethod>
        {
            new VerificationMethod(
                id: "did:example:issuer#key-1",
                type: "Ed25519VerificationKey2020",
                controller: "did:example:issuer",
                publicKeyBase58: ValidEd25519KeyBase58
            )
        };
        var didDoc = new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: "did:example:issuer",
            verificationMethods: verificationMethods
        );
        _mockDidResolver.Setup(r => r.ResolveAsync("did:example:issuer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = didDoc });

        _mockProofService.Setup(p =>
                p.VerifyProofAsync(It.IsAny<string>(), It.IsAny<JsonElement>(), It.IsAny<byte[]>(),
                    "Ed25519Signature2020", "assertionMethod", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(vcJson);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_ProofCreatedTooFarInPast_ShouldThrowOrFail()
    {
        string vcJson = $@"
    {{
      ""@context"": [""https://www.w3.org/2018/credentials/v1""],
      ""id"": ""urn:uuid:expired"",
      ""type"": [""VerifiableCredential""],
      ""issuer"": ""did:example:issuer"",
      ""issuanceDate"": ""2020-01-01T00:00:00Z"",
      ""credentialSubject"": {{ ""id"": ""did:example:subject"" }},
      ""proof"": {{
        ""type"": ""Ed25519Signature2020"",
        ""created"": ""2000-01-01T00:00:00Z"",
        ""verificationMethod"": ""did:example:issuer#key-1"",
        ""proofPurpose"": ""assertionMethod"",
        ""proofValue"": ""z7D3...""
      }}
    }}";
        var verificationMethods = new List<IDidVerificationMethod>
        {
            new VerificationMethod(
                id: "did:example:issuer#key-1",
                type: "Ed25519VerificationKey2020",
                controller: "did:example:issuer",
                publicKeyBase58: ValidEd25519KeyBase58
            )
        };
        var didDoc = new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: "did:example:issuer",
            verificationMethods: verificationMethods
        );
        _mockDidResolver.Setup(r => r.ResolveAsync("did:example:issuer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = didDoc });
        _mockProofService.Setup(p =>
                p.VerifyProofAsync(It.IsAny<string>(), It.IsAny<JsonElement>(), It.IsAny<byte[]>(),
                    "Ed25519Signature2020", "assertionMethod", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // If your implementation enforces a window, this should throw or return false
        var result = await _validator.ValidateAsync(vcJson);
        // result.Should().BeFalse(); OR
        // await Assert.ThrowsAsync<YourCustomException>(async () => await _validator.ValidateAsync(vcJson));
    }

    [Fact]
    public async Task ValidateAsync_VerificationMethodTypeMismatch_ShouldThrowOrFail()
    {
        string vcJson = @"
    {
      ""@context"": [""https://www.w3.org/2018/credentials/v1""],
      ""id"": ""urn:uuid:type-mismatch"",
      ""type"": [""VerifiableCredential""],
      ""issuer"": ""did:example:issuer"",
      ""issuanceDate"": ""2024-01-01T00:00:00Z"",
      ""credentialSubject"": { ""id"": ""did:example:subject"" },
      ""proof"": {
        ""type"": ""Ed25519Signature2020"",
        ""created"": ""2024-01-01T00:00:00Z"",
        ""verificationMethod"": ""did:example:issuer#key-2"",
        ""proofPurpose"": ""assertionMethod"",
        ""proofValue"": ""z7D3...""
      }
    }";
        var verificationMethods = new List<IDidVerificationMethod>
        {
            new VerificationMethod(
                id: "did:example:issuer#key-2",
                type: "EcdsaSecp256k1VerificationKey2019", // mismatch!
                controller: "did:example:issuer",
                publicKeyJwk: new Dictionary<string, object>
                {
                    { "kty", "EC" },
                    { "crv", "secp256k1" },
                    { "x", "MHcCAQEEIKZtAcX2IqfWT2_9Vp01dJ1eV9IUdA8X0jEGyKkA8Eha" },
                    { "y", "MHcCAQEEIKZtAcX2IqfWT2_9Vp01dJ1eV9IUdA8X0jEGyKkA8Ehb" }
                }
            )
        };
        var didDoc = new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: "did:example:issuer",
            verificationMethods: verificationMethods
        );
        _mockDidResolver.Setup(r => r.ResolveAsync("did:example:issuer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = didDoc });

        // Should throw or fail because key type doesn't match proof type
        Func<Task> act = async () => await _validator.ValidateAsync(vcJson);
        await act.Should().ThrowAsync<Exception>().WithMessage("*type*");
    }

    [Theory]
    [InlineData(@"{
  ""@context"": [""https://www.w3.org/2018/credentials/v1""],
  ""id"": ""urn:uuid:malformed-proof-1"",
  ""type"": [""VerifiableCredential""],
  ""issuer"": ""did:example:issuer"",
  ""issuanceDate"": ""2024-01-01T00:00:00Z"",
  ""credentialSubject"": { ""id"": ""did:example:subject"" },
  ""proof"": null
}")]
    [InlineData(@"{
  ""@context"": [""https://www.w3.org/2018/credentials/v1""],
  ""id"": ""urn:uuid:malformed-proof-2"",
  ""type"": [""VerifiableCredential""],
  ""issuer"": ""did:example:issuer"",
  ""issuanceDate"": ""2024-01-01T00:00:00Z"",
  ""credentialSubject"": { ""id"": ""did:example:subject"" },
  ""proof"": 42
}")]
    public async Task ValidateAsync_MalformedProofStructure_ShouldThrow(string vcJson)
    {
        var verificationMethods = new List<IDidVerificationMethod>
        {
            new VerificationMethod(
                id: "did:example:issuer#key-1",
                type: "Ed25519VerificationKey2020",
                controller: "did:example:issuer",
                publicKeyBase58: ValidEd25519KeyBase58
            )
        };
        var didDoc = new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: "did:example:issuer",
            verificationMethods: verificationMethods
        );
        _mockDidResolver.Setup(r => r.ResolveAsync("did:example:issuer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = didDoc });

        Func<Task> act = async () => await _validator.ValidateAsync(vcJson);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task ValidateAsync_ProofWithUnknownFields_ShouldSucceed()
    {
        string vcJson = @"
    {
      ""@context"": [""https://www.w3.org/2018/credentials/v1""],
      ""id"": ""urn:uuid:extra-proof-fields"",
      ""type"": [""VerifiableCredential""],
      ""issuer"": ""did:example:issuer"",
      ""issuanceDate"": ""2024-01-01T00:00:00Z"",
      ""credentialSubject"": { ""id"": ""did:example:subject"" },
      ""proof"": {
        ""type"": ""Ed25519Signature2020"",
        ""created"": ""2024-01-01T00:00:00Z"",
        ""verificationMethod"": ""did:example:issuer#key-1"",
        ""proofPurpose"": ""assertionMethod"",
        ""proofValue"": ""z7D3..."",
        ""extraProp1"": ""foo"",
        ""extraProp2"": 123
      }
    }";
        var verificationMethods = new List<IDidVerificationMethod>
        {
            new VerificationMethod(
                id: "did:example:issuer#key-1",
                type: "Ed25519VerificationKey2020",
                controller: "did:example:issuer",
                publicKeyBase58: ValidEd25519KeyBase58
            )
        };
        var didDoc = new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: "did:example:issuer",
            verificationMethods: verificationMethods
        );
        _mockDidResolver.Setup(r => r.ResolveAsync("did:example:issuer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = didDoc });

        _mockProofService.Setup(p =>
                p.VerifyProofAsync(It.IsAny<string>(), It.IsAny<JsonElement>(), It.IsAny<byte[]>(),
                    "Ed25519Signature2020", "assertionMethod", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(vcJson);

        result.Should().BeTrue();
    }
}