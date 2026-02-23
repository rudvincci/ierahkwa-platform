using System.Text.Json;
using FluentAssertions;
using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.Tests.Core;

public class VerifiablePresentationTests
{
    private readonly string MinimalValidJson = @"
    {
      ""@context"": [""https://www.w3.org/2018/credentials/v1""],
      ""type"": [""VerifiablePresentation""],
      ""verifiableCredential"": {
        ""@context"": [""https://www.w3.org/2018/credentials/v1""],
        ""type"": [""VerifiableCredential""],
        ""issuer"": ""did:example:issuer"",
        ""issuanceDate"": ""2023-12-01T00:00:00Z"",
        ""credentialSubject"": { ""id"": ""did:example:subject"" },
        ""proof"": {
          ""type"": ""Ed25519Signature2020"",
          ""created"": ""2023-12-01T00:00:00Z"",
          ""verificationMethod"": ""did:example:issuer#key-1"",
          ""proofPurpose"": ""assertionMethod"",
          ""proofValue"": ""z7D3abcdef1234567890""
        }
      },
      ""proof"": {
        ""type"": ""Ed25519Signature2020"",
        ""created"": ""2023-12-01T12:34:56Z"",
        ""verificationMethod"": ""did:example:holder#key-1"",
        ""proofPurpose"": ""authentication"",
        ""proofValue"": ""z9F3abcdef9876543210""
      }
    }";

    private readonly string FullValidJson = @"
    {
      ""@context"": [
        ""https://www.w3.org/2018/credentials/v1"",
        ""https://w3id.org/presentation-exchange/v1""
      ],
      ""type"": [""VerifiablePresentation"", ""CustomPresentation""],
      ""holder"": ""did:example:holder123"",
      ""verifiableCredential"": [
        {
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""type"": [""VerifiableCredential"", ""AlumniCredential""],
          ""issuer"": ""did:example:issuer1"",
          ""issuanceDate"": ""2023-12-01T00:00:00Z"",
          ""credentialSubject"": { ""id"": ""did:example:subject1"" },
          ""proof"": {
            ""type"": ""Ed25519Signature2020"",
            ""created"": ""2023-12-01T00:00:00Z"",
            ""verificationMethod"": ""did:example:issuer1#key-1"",
            ""proofPurpose"": ""assertionMethod"",
            ""proofValue"": ""z7D3abcdef1234567890""
          }
        },
        {
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""type"": [""VerifiableCredential"", ""DegreeCredential""],
          ""issuer"": ""did:example:issuer2"",
          ""issuanceDate"": ""2023-01-01T00:00:00Z"",
          ""credentialSubject"": { ""id"": ""did:example:subject2"" },
          ""proof"": {
            ""type"": ""Ed25519Signature2020"",
            ""created"": ""2023-01-01T00:00:00Z"",
            ""verificationMethod"": ""did:example:issuer2#key-2"",
            ""proofPurpose"": ""assertionMethod"",
            ""proofValue"": ""z8G4abcdef9876543210""
          }
        }
      ],
      ""proof"": [
        {
          ""type"": ""Ed25519Signature2020"",
          ""created"": ""2023-12-01T12:34:56Z"",
          ""verificationMethod"": ""did:example:holder123#key-1"",
          ""proofPurpose"": ""authentication"",
          ""proofValue"": ""z9F3abcdef9876543210""
        },
        {
          ""type"": ""EcdsaSecp256k1Signature2019"",
          ""created"": ""2023-12-01T12:35:56Z"",
          ""verificationMethod"": ""did:example:holder123#key-2"",
          ""proofPurpose"": ""authentication"",
          ""jws"": ""eyJhbGciOiJFUzI1Nksi...""
        }
      ]
    }";

    [Fact]
    public void Parse_MinimalValidJson_ShouldSucceed()
    {
        var vp = VerifiablePresentation.Parse(MinimalValidJson);
        vp.Should().NotBeNull();
        vp.Type.Should().Contain("VerifiablePresentation");
        ((IEnumerable<VerifiableCredential>)vp.VerifiableCredential).Should().HaveCount(1);
        vp.Proof.Should().NotBeNull();
        if (vp.Proof is JsonElement proofElem && proofElem.ValueKind == JsonValueKind.Object)
        {
            proofElem.GetProperty("type").GetString().Should().Be("Ed25519Signature2020");
            proofElem.GetProperty("proofValue").GetString().Should().NotBeNull();
        }
        else if (vp.Proof is Dictionary<string, object> dict)
        {
            dict["type"].Should().Be("Ed25519Signature2020");
            dict["proofValue"].Should().NotBeNull();
        }
        else
        {
            throw new Xunit.Sdk.XunitException("Unexpected proof type: " + vp.Proof?.GetType());
        }
    }

    [Fact]
    public void Parse_FullValidJson_ShouldSucceed_AllFieldsCorrect()
    {
        var vp = VerifiablePresentation.Parse(FullValidJson);
        vp.Should().NotBeNull();
        vp.Context.Should().Contain("https://w3id.org/presentation-exchange/v1");
        vp.Type.Should().Contain("CustomPresentation");
        vp.Holder.Should().Be("did:example:holder123");
        ((IEnumerable<object>)vp.VerifiableCredential).Should().HaveCount(2);
        (((IEnumerable<object>)vp.VerifiableCredential).ElementAt(1) as VerifiableCredential)?.Issuer.Should().Be("did:example:issuer2");
        vp.Proof.Should().NotBeNull();
    }

    [Fact]
    public void Parse_WithMultipleProofs_ShouldSucceed()
    {
        var vp = VerifiablePresentation.Parse(FullValidJson);
        var proofs = vp.GetProofs().ToList();
        proofs.Should().HaveCount(2);

        var secondProof = ToProof(proofs[1]);
        secondProof.Should().NotBeNull();
        secondProof.Type.Should().Be("EcdsaSecp256k1Signature2019");
    }

    private static Proof ToProof(object obj)
    {
        if (obj is Proof proof)
            return proof;
        if (obj is JsonElement elem)
            return JsonSerializer.Deserialize<Proof>(elem.GetRawText());
        if (obj is Dictionary<string, object> dict)
            return JsonSerializer.Deserialize<Proof>(JsonSerializer.Serialize(dict));
        if (obj is string str && !string.IsNullOrWhiteSpace(str))
            return JsonSerializer.Deserialize<Proof>(str);
        throw new InvalidOperationException("Cannot convert proof to Proof type.");
    }

    [Fact]
    public void Parse_WithMultipleCredentials_ShouldSucceed()
    {
        // Use your FullValidJson string from above
        var vp = VerifiablePresentation.Parse(FullValidJson);

        // Your custom converter will make this a List<object>
        var creds = (vp.VerifiableCredential as List<object>)!;
        creds.Should().HaveCount(2);

        // Helper to turn List<object> into VerifiableCredential
        VerifiableCredential AsVerifiableCredential(object obj)
        {
            if (obj is VerifiableCredential vc)
                return vc;
            if (obj is JsonElement elem)
                return JsonSerializer.Deserialize<VerifiableCredential>(elem.GetRawText());
            if (obj is Dictionary<string, object> dict)
                return JsonSerializer.Deserialize<VerifiableCredential>(JsonSerializer.Serialize(dict));
            if (obj is string s && !string.IsNullOrWhiteSpace(s))
                return JsonSerializer.Deserialize<VerifiableCredential>(s);
            throw new InvalidOperationException("Cannot convert object to VerifiableCredential");
        }

        // Test the first credential
        var vc1 = AsVerifiableCredential(creds[0]);
        vc1.Id.Should().Be("did:example:subject1");

        // Test the second credential
        var vc2 = AsVerifiableCredential(creds[1]);
        vc2.Id.Should().Be("did:example:subject2");
    }
    
    [Fact]
    public void Parse_MalformedJson_ShouldThrow()
    {
        string malformed = @"{ ""@context"": [""https://www.w3.org/2018/credentials/v1"", ";
        Action act = () => VerifiablePresentation.Parse(malformed);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Parse_InvalidCredential_ShouldThrow()
    {
        string invalidVc = @"
        {
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""type"": [""VerifiablePresentation""],
          ""verifiableCredential"": { ""issuer"": 42 }
        }";
        Action act = () => VerifiablePresentation.Parse(invalidVc);
        act.Should().Throw<Exception>().WithMessage("*verifiableCredential*");
    }

    


    [Fact]
    public void RoundTrip_SerializeDeserialize_ShouldPreserveFields()
    {
        var vp = VerifiablePresentation.Parse(FullValidJson);
        var json = vp.ToJson();
        var reparsed = VerifiablePresentation.Parse(json);
        reparsed.Holder.Should().Be(vp.Holder);
        ((IEnumerable<CredentialSubject>)((IEnumerable<VerifiableCredential>)reparsed.VerifiableCredential).ElementAt(1)
            .CredentialSubject).ElementAt(0)
            .Id.Should().Be("did:example:subject2");
        // Proof roundtrip check
        reparsed.Type.Should().BeEquivalentTo(vp.Type);
    }

    [Fact]
    public void Parse_SupportsUnicodeAndNonAsciiFields()
    {
        string unicodeJson = @"
    {
      ""@context"": [""https://www.w3.org/2018/credentials/v1""],
      ""type"": [""VerifiablePresentation""],
      ""verifiableCredential"": [],
      ""holder"": ""did:example:🌿✨""
    }";
        var vp = VerifiablePresentation.Parse(unicodeJson);
        vp.Holder.Should().Be("did:example:🌿✨");
        // Optionally check: proofs should be empty
        vp.GetProofs().Should().BeEmpty();
    }

    [Fact]
    public void Parse_ProofObjectOrArray_ShouldBeSupported()
    {
        // As object (minimal)
        var vp1 = VerifiablePresentation.Parse(MinimalValidJson);
        var proofs1 = vp1.GetProofs();
        proofs1.Should().HaveCount(1);

        // As array (full)
        var vp2 = VerifiablePresentation.Parse(FullValidJson);
        var proofs2 = vp2.GetProofs();
        proofs2.Should().HaveCount(2); // <--- Should now pass
    }


    [Fact]
    public void Parse_EmptyCredentialsArray_ShouldNotThrow()
    {
        string emptyArray = @"{
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""type"": [""VerifiablePresentation""],
          ""verifiableCredential"": []
        }";
        var vp = VerifiablePresentation.Parse(emptyArray);
        ((List<VerifiableCredential>)vp.VerifiableCredential).Should().BeEmpty();
    }
}
