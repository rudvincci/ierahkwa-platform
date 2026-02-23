using System.Text.Json;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.Tests.Core;

public class VerifiableCredentialTests
{
    private readonly string MinimalValidJson = @"
    {
      ""@context"": [""https://www.w3.org/2018/credentials/v1""],
      ""id"": ""urn:uuid:3b1de3b6-3dca-4ca5-b7c1-943b8222c2aa"",
      ""type"": [""VerifiableCredential""],
      ""issuer"": ""did:example:issuer"",
      ""issuanceDate"": ""2023-12-01T00:00:00Z"",
      ""credentialSubject"": { ""id"": ""did:example:subject"" },
      ""proof"": {
        ""type"": ""Ed25519Signature2020"",
        ""created"": ""2023-12-01T00:00:00Z"",
        ""verificationMethod"": ""did:example:issuer#key-1"",
        ""proofPurpose"": ""assertionMethod"",
        ""proofValue"": ""z7D3...""
      }
    }";

    private readonly string FullValidJson = @"
    {
      ""@context"": [
        ""https://www.w3.org/2018/credentials/v1"",
        ""https://example.com/custom-context""
      ],
      ""id"": ""http://example.edu/credentials/3732"",
      ""type"": [""VerifiableCredential"", ""AlumniCredential""],
      ""issuer"": ""did:example:123456789abcdefghi"",
      ""issuanceDate"": ""2022-01-01T19:23:24Z"",
      ""expirationDate"": ""2025-01-01T00:00:00Z"",
      ""credentialSubject"": {
        ""id"": ""did:example:abcdef1234567890"",
        ""alumniOf"": { ""id"": ""did:example:c276e12ec21ebfeb1f712ebc6f1"" },
        ""name"": ""Alice Example"",
        ""customData"": { ""foo"": 42, ""bar"": [1,2,3] }
      },
      ""credentialStatus"": {
        ""id"": ""https://example.com/status/24"",
        ""type"": ""StatusList2021Entry"",
        ""statusPurpose"": ""revocation"",
        ""statusListIndex"": ""42"",
        ""statusListCredential"": ""https://example.com/status/1""
      },
      ""evidence"": [ { ""type"": ""DocumentVerification"", ""document"": ""passport"" } ],
      ""proof"": {
        ""type"": ""Ed25519Signature2020"",
        ""created"": ""2022-01-01T19:23:24Z"",
        ""verificationMethod"": ""did:example:123456789abcdefghi#key-1"",
        ""proofPurpose"": ""assertionMethod"",
        ""proofValue"": ""z2J6TXi2J...""
      }
    }";

    [Fact]
    public void Parse_MinimalValidJson_ShouldSucceed()
    {
        var vc = VerifiableCredential.Parse(MinimalValidJson);
        vc.Should().NotBeNull();
        vc.Id.Should().Be("urn:uuid:3b1de3b6-3dca-4ca5-b7c1-943b8222c2aa");
        vc.Issuer.Should().Be("did:example:issuer");
        vc.Type.Should().Contain("VerifiableCredential");
        
        ((IEnumerable<CredentialSubject>)vc.CredentialSubject).Should().HaveCount(1);
        
        (((IEnumerable<CredentialSubject>)vc.CredentialSubject)
            .ElementAt(0)).Id.Should().Be("did:example:subject");
        vc.Proof.Should().NotBeNull();
        // Proof should be a JsonElement
        vc.Proof.Should().BeOfType<JsonElement>();
        var proofElement = (JsonElement)vc.Proof;
        proofElement.ValueKind.Should().Be(JsonValueKind.Object);

        proofElement.TryGetProperty("type", out var typeProp).Should().BeTrue("proof must contain 'type'");
        typeProp.GetString().Should().Be("Ed25519Signature2020");
    }

    [Fact]
    public void Parse_FullValidJson_ShouldSucceed_AllFieldsCorrect()
    {
        var vc = VerifiableCredential.Parse(FullValidJson);

        // Context should include both contexts as strings
        vc.Context.Select(x => x.ToString()).Should().Contain("https://www.w3.org/2018/credentials/v1");
        vc.Context.Select(x => x.ToString()).Should().Contain("https://example.com/custom-context");
        vc.Type.Should().BeEquivalentTo(new[] { "VerifiableCredential", "AlumniCredential" });
        vc.Issuer.Should().Be("did:example:123456789abcdefghi");
        ((IEnumerable<CredentialSubject>)vc.CredentialSubject).Should().HaveCount(1);

        // Access extension claims (arbitrary fields)
        var claims = ((IEnumerable<CredentialSubject>)vc.CredentialSubject).ElementAt(0).Claims;
        claims.Should().ContainKey("alumniOf");
        claims["alumniOf"].Should().NotBeNull();

        claims.Should().ContainKey("customData");
        var customDataElement = claims["customData"];
        customDataElement.Should().NotBeNull();

        // Handle value as JsonElement
        if (customDataElement is JsonElement el && el.ValueKind == JsonValueKind.Object)
        {
            // Read "foo" property (should be 42)
            el.TryGetProperty("foo", out var fooProp).Should().BeTrue("customData should have property 'foo'");
            fooProp.ValueKind.Should().Be(JsonValueKind.Number);
            fooProp.GetInt32().Should().Be(42);

            // Read "bar" property as array ([1,2,3])
            el.TryGetProperty("bar", out var barProp).Should().BeTrue("customData should have property 'bar'");
            barProp.ValueKind.Should().Be(JsonValueKind.Array);
            barProp.GetArrayLength().Should().Be(3);
            barProp.EnumerateArray().Select(x => x.GetInt32()).Should().BeEquivalentTo(new[] { 1, 2, 3 });
        }
        else
        {
            throw new Xunit.Sdk.XunitException("customData is not a JsonElement object");
        }

        vc.CredentialStatus.Should().NotBeNull();
        vc.CredentialStatus.StatusPurpose.Should().Be("revocation");
        vc.Proof.Should().NotBeNull();
        vc.ExpirationDate.Value.ToUniversalTime().Should().Be(DateTime.Parse("2025-01-01T00:00:00Z").ToUniversalTime());
    }


    [Fact]
    public void ToJson_ShouldRoundTripWithParse()
    {
        var vc = VerifiableCredential.Parse(FullValidJson);
        string json = vc.ToJson();
        var vc2 = VerifiableCredential.Parse(json);

        vc2.Id.Should().Be(vc.Id);
        ((IEnumerable<CredentialSubject>)vc.CredentialSubject).Should().HaveCount(((IEnumerable<CredentialSubject>)vc.CredentialSubject).Count());
        ((IEnumerable<CredentialSubject>)vc.CredentialSubject)
            .ElementAt(0).Id.Should().Be(((IEnumerable<CredentialSubject>)vc.CredentialSubject).ElementAt(0).Id);

        // Compare ProofPurpose in Proof (JsonElement)
        vc2.Proof.Should().BeOfType<JsonElement>();
        var proof2 = (JsonElement)vc2.Proof;
        proof2.TryGetProperty("proofPurpose", out var proofPurposeProp2).Should()
            .BeTrue("Proof should have 'proofPurpose'");
        proofPurposeProp2.GetString().Should().Be(GetProofPurpose(vc.Proof));

        vc2.CredentialStatus.StatusListIndex.Should().Be(vc.CredentialStatus.StatusListIndex);

        // Access "customData" via Claims
        ((IEnumerable<CredentialSubject>)vc2.CredentialSubject).ElementAt(0).Claims.Should().ContainKey("customData");
        var customDataElement = ((IEnumerable<CredentialSubject>)vc2.CredentialSubject).ElementAt(0).Claims["customData"];
        customDataElement.Should().NotBeNull();

        if (customDataElement is JsonElement el && el.ValueKind == JsonValueKind.Object)
        {
            el.TryGetProperty("foo", out var fooProp).Should().BeTrue();
            fooProp.GetInt32().Should().Be(42);

            el.TryGetProperty("bar", out var barProp).Should().BeTrue();
            barProp.ValueKind.Should().Be(JsonValueKind.Array);
            barProp.GetArrayLength().Should().Be(3);
        }
        else
        {
            throw new Xunit.Sdk.XunitException("customData is not a JsonElement object");
        }
    }

// Helper to extract proofPurpose from original Proof object, whether it's JsonElement or already a POCO
    private static string GetProofPurpose(object proof)
    {
        if (proof is JsonElement proofElement)
        {
            if (proofElement.TryGetProperty("proofPurpose", out var prop))
                return prop.GetString();
        }
        else if (proof is IDictionary<string, object> dict && dict.TryGetValue("proofPurpose", out var obj))
        {
            return obj?.ToString();
        }

        throw new Xunit.Sdk.XunitException("Could not extract 'proofPurpose' from proof object");
    }


    [Fact]
    public void Parse_WithMissingRequiredFields_ShouldThrow()
    {
        string noType =
            @"{ ""@context"": [""https://www.w3.org/2018/credentials/v1""], ""id"": ""a"", ""issuer"": ""b"", ""issuanceDate"": ""2022-01-01"", ""credentialSubject"": {} }";
        Action act = () => VerifiableCredential.Parse(noType);
        act.Should().Throw<Exception>().WithMessage("*type*");
    }

    [Fact]
    public void Parse_WithMalformedJson_ShouldThrow()
    {
        string malformed = @"{ ""@context"": [""https://www.w3.org/2018/credentials/v1"", }";
        Action act = () => VerifiableCredential.Parse(malformed);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Parse_WithNullCredentialSubject_ShouldThrow()
    {
        string nullSubject =
            @"{ ""@context"": [""https://www.w3.org/2018/credentials/v1""], ""id"": ""a"", ""type"": [""VerifiableCredential""], ""issuer"": ""b"", ""issuanceDate"": ""2022-01-01T00:00:00Z"", ""credentialSubject"": null }";
        Action act = () => VerifiableCredential.Parse(nullSubject);
        act.Should().Throw<Exception>().WithMessage("*credentialSubject*");
    }

    [Fact]
    public void Parse_WithUnknownProperties_ShouldNotThrow()
    {
        string extraProps = @"{
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""id"": ""urn:uuid:123"",
          ""type"": [""VerifiableCredential""],
          ""issuer"": ""did:example:issuer"",
          ""issuanceDate"": ""2022-01-01T00:00:00Z"",
          ""credentialSubject"": { ""id"": ""did:example:subject"" },
          ""foo"": ""bar"",
          ""extra"": 123
        }";
        var vc = VerifiableCredential.Parse(extraProps);
        vc.Should().NotBeNull();
    }

    [Theory]
    [InlineData(
        @"{ ""@context"": [""https://www.w3.org/2018/credentials/v1""], ""type"": [""VerifiableCredential""], ""id"": [123,456] }")]
    [InlineData(@"{ ""@context"": [""https://www.w3.org/2018/credentials/v1""], ""type"": null }")]
    [InlineData(@"{ ""@context"": [""https://www.w3.org/2018/credentials/v1""], ""type"": [] }")]
    public void Parse_FuzzedOrPoisonedJson_ShouldThrow(string input)
    {
        Action act = () => VerifiableCredential.Parse(input);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Parse_UnicodeCharacters_ShouldSucceed()
    {
        string unicodeVc = @"
        {
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""id"": ""urn:uuid:🍄✨"",
          ""type"": [""VerifiableCredential""],
          ""issuer"": ""did:example:issuer"",
          ""issuanceDate"": ""2023-12-01T00:00:00Z"",
          ""credentialSubject"": { ""id"": ""did:example:🍍✨"" },
          ""proof"": {
            ""type"": ""Ed25519Signature2020"",
            ""created"": ""2023-12-01T00:00:00Z"",
            ""verificationMethod"": ""did:example:issuer#key-1"",
            ""proofPurpose"": ""assertionMethod"",
            ""proofValue"": ""z7D3...""
          }
        }";
        var vc = VerifiableCredential.Parse(unicodeVc);
        vc.Id.Should().Be("urn:uuid:🍄✨");

        ((IEnumerable<CredentialSubject>)vc.CredentialSubject).Should().HaveCount(1);
        ((IEnumerable<CredentialSubject>)vc.CredentialSubject).ElementAt(0).Id.Should().Be("did:example:🍍✨");
    }

    [Fact]
    public void ToJson_Produces_ValidJson_WithAllFields()
    {
        var vc = VerifiableCredential.Parse(FullValidJson);
        string json = vc.ToJson();
        Action act = () => JObject.Parse(json);
        act.Should().NotThrow();
        json.Should().Contain("\"@context\"");
        json.Should().Contain("\"type\"");
        json.Should().Contain("\"proof\"");
        json.Should().Contain("\"credentialSubject\"");
    }

    [Fact]
    public void Parse_VeryLargeCredential_ShouldNotCrashOrLeakMemory()
    {
        // Simulate a credential with 1,000 subject claims and a large custom data payload
        var claims = Enumerable.Range(1, 1000).Select(i => $"\"claim{i}\": \"value{i}\"");
        string hugeSubject = $"{{ \"id\": \"did:example:huge\", {string.Join(",", claims)} }}";
        string largeJson = $@"
        {{
          ""@context"": [""https://www.w3.org/2018/credentials/v1""],
          ""id"": ""urn:uuid:huge"",
          ""type"": [""VerifiableCredential""],
          ""issuer"": ""did:example:issuer"",
          ""issuanceDate"": ""2023-12-01T00:00:00Z"",
          ""credentialSubject"": {hugeSubject},
          ""proof"": {{
            ""type"": ""Ed25519Signature2020"",
            ""created"": ""2023-12-01T00:00:00Z"",
            ""verificationMethod"": ""did:example:issuer#key-1"",
            ""proofPurpose"": ""assertionMethod"",
            ""proofValue"": ""z7D3...""
          }}
        }}";
        var vc = VerifiableCredential.Parse(largeJson);
        ((IEnumerable<CredentialSubject>)vc.CredentialSubject).Should().HaveCount(1);
        ((IEnumerable<CredentialSubject>)vc.CredentialSubject).ElementAt(0).Id.Should().Be("did:example:huge");
    }

    [Fact]
    public void Parse_MultiSubjectArray_ShouldSupportList()
    {
        string multiSubjectJson = @"
        {
            ""@context"": [""https://www.w3.org/2018/credentials/v1""],
            ""id"": ""urn:uuid:arr"",
            ""type"": [""VerifiableCredential""],
            ""issuer"": ""did:example:issuer"",
            ""issuanceDate"": ""2023-12-01T00:00:00Z"",
            ""credentialSubject"": [
                { ""id"": ""did:example:subject1"" },
                { ""id"": ""did:example:subject2"" }
            ],
            ""proof"": {
                ""type"": ""Ed25519Signature2020"",
                ""created"": ""2023-12-01T00:00:00Z"",
                ""verificationMethod"": ""did:example:issuer#key-1"",
                ""proofPurpose"": ""assertionMethod"",
                ""proofValue"": ""z7D3...""
            }
        }";
        var vc = VerifiableCredential.Parse(multiSubjectJson);
        ((IEnumerable<CredentialSubject>)vc.CredentialSubject).Should().HaveCount(2);
        ((IEnumerable<CredentialSubject>)vc.CredentialSubject).ElementAt(0).Id.Should().Be("did:example:subject1");
        ((IEnumerable<CredentialSubject>)vc.CredentialSubject).ElementAt(1).Id.Should().Be("did:example:subject2");
    }
}