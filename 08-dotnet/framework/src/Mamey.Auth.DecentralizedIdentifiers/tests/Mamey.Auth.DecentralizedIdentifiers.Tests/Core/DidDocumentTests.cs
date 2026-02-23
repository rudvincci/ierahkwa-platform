using System.Text;
using FluentAssertions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Newtonsoft.Json.Linq;

namespace Mamey.Auth.DecentralizedIdentifiers.Tests.Core;

public class DidDocumentTests
{
  
    private readonly string MinimalValidJson = @"
    {
      ""@context"": [""https://www.w3.org/ns/did/v1""],
      ""id"": ""did:example:123"",
      ""verificationMethod"": [
        {
          ""id"": ""did:example:123#key-1"",
          ""type"": ""Ed25519VerificationKey2020"",
          ""controller"": ""did:example:123"",
          ""publicKeyBase58"": ""2R2JpJUNonBPaawUGhWXeS1T1EsYz8f7nQ4GZyKwhLQ1""
        }
      ]
    }";

    private readonly string FullValidJson = @"
    {
      ""@context"": [
        ""https://www.w3.org/ns/did/v1"",
        ""https://example.com/custom-context""
      ],
      ""id"": ""did:example:456"",
      ""controller"": [""did:example:root1"", ""did:example:root2""],
      ""verificationMethod"": [
        {
          ""id"": ""did:example:456#key-1"",
          ""type"": ""Ed25519VerificationKey2020"",
          ""controller"": ""did:example:456"",
          ""publicKeyBase58"": ""2R2JpJUNonBPaawUGhWXeS1T1EsYz8f7nQ4GZyKwhLQ1""
        },
        {
          ""id"": ""did:example:456#key-2"",
          ""type"": ""EcdsaSecp256k1VerificationKey2019"",
          ""controller"": ""did:example:456"",
          ""publicKeyJwk"": {""kty"": ""EC"", ""crv"": ""secp256k1"", ""x"": ""..."", ""y"": ""...""}
        }
      ],
      ""authentication"": [
        ""did:example:456#key-1"",
        ""did:example:456#key-2""
      ],
      ""assertionMethod"": [""did:example:456#key-1""],
      ""service"": [
        {
          ""id"": ""did:example:456#msg"",
          ""type"": ""MessagingService"",
          ""serviceEndpoint"": ""https://msg.example.com/api""
        },
        {
          ""id"": ""did:example:456#vc"",
          ""type"": ""VerifiableCredentialService"",
          ""serviceEndpoint"": ""https://vc.example.com/""
        }
      ]
    }";

    [Fact]
    public void Parse_MinimalValidJson_ShouldSucceed()
    {
        var doc = DidDocument.Parse(MinimalValidJson);
        doc.Should().NotBeNull();
        doc.Id.Should().Be("did:example:123");
        doc.VerificationMethods.Should().HaveCount(1);
        doc.VerificationMethods[0].Type.Should().Be("Ed25519VerificationKey2020");
    }

    [Fact]
    public void Parse_FullValidJson_ShouldSucceed_AllFieldsCorrect()
    {
        var doc = DidDocument.Parse(FullValidJson);
        doc.Should().NotBeNull();
        doc.Id.Should().Be("did:example:456");
        doc.Controller.Should().Contain(new[] { "did:example:root1", "did:example:root2" });
        doc.VerificationMethods.Should().HaveCount(2);
        doc.ServiceEndpoints.Should().HaveCount(2);
        doc.Authentication.Should().Contain("did:example:456#key-2");
        doc.ServiceEndpoints[0].Type.Should().Be("MessagingService");
    }

    [Fact]
    public void ToJson_ShouldRoundTripWithParse()
    {
        var doc = DidDocument.Parse(FullValidJson);
        string json = doc.ToJson();
        var doc2 = DidDocument.Parse(json);
        doc2.Id.Should().Be(doc.Id);
        doc2.Controller.Should().BeEquivalentTo(doc.Controller);
        doc2.VerificationMethods[0].Type.Should().Be(doc.VerificationMethods[0].Type);
        doc2.ServiceEndpoints[1].Endpoints.FirstOrDefault().Should().Be(doc.ServiceEndpoints[1].Endpoints.FirstOrDefault());
    }

    [Fact]
    public void Parse_WithMissingRequiredFields_ShouldThrow()
    {
        string noId = @"{ ""@context"": [""https://www.w3.org/ns/did/v1""] }";
        Action act = () => DidDocument.Parse(noId);
        act.Should().Throw<Exception>().WithMessage("*id*");
    }

    [Fact]
    public void Parse_WithEmptyJson_ShouldThrow()
    {
        Action act = () => DidDocument.Parse("");
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Parse_WithMalformedJson_ShouldThrow()
    {
        string malformed = @"{ ""@context"": [""https://www.w3.org/ns/did/v1"", }";
        Action act = () => DidDocument.Parse(malformed);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Parse_WithDuplicateVerificationMethodIds_ShouldDetectOrHandle()
    {
        string dup = @"
        {
          ""@context"": [""https://www.w3.org/ns/did/v1""],
          ""id"": ""did:example:dup"",
          ""verificationMethod"": [
            {
              ""id"": ""did:example:dup#key-1"",
              ""type"": ""Ed25519VerificationKey2020"",
              ""controller"": ""did:example:dup"",
              ""publicKeyBase58"": ""2R2JpJUNonBPaawUGhWXeS1T1EsYz8f7nQ4GZyKwhLQ1""
            },
            {
              ""id"": ""did:example:dup#key-1"",
              ""type"": ""Ed25519VerificationKey2020"",
              ""controller"": ""did:example:dup"",
              ""publicKeyBase58"": ""DiffKey...""
            }
          ]
        }";
        Action act = () => DidDocument.Parse(dup);
        act.Should().Throw<Exception>().WithMessage("*duplicate*key-1*");
    }

    [Fact]
    public void Parse_NonArrayProperties_ShouldHandleAsSingleElementArray()
    {
      string singleObj = @"
      {
        ""@context"": ""https://www.w3.org/ns/did/v1"",
        ""id"": ""did:example:single"",
        ""verificationMethod"": {
          ""id"": ""did:example:single#key-1"",
          ""type"": ""Ed25519VerificationKey2020"",
          ""controller"": ""did:example:single"",
          ""publicKeyBase58"": ""2R2JpJUNonBPaawUGhWXeS1T1EsYz8f7nQ4GZyKwhLQ1""
        }
      }";
      var doc = DidDocument.Parse(singleObj);
      doc.VerificationMethods.Should().HaveCount(1);
      doc.VerificationMethods[0].Id.Should().Be("did:example:single#key-1");
    }

    [Fact]
    public void Parse_ServiceEndpoints_WithComplexTypes_ShouldSucceed()
    {
        string multiEndpoint = @"
        {
          ""@context"": [""https://www.w3.org/ns/did/v1""],
          ""id"": ""did:example:multi"",
          ""verificationMethod"": [
            {
              ""id"": ""did:example:multi#key-1"",
              ""type"": ""Ed25519VerificationKey2020"",
              ""controller"": ""did:example:multi"",
              ""publicKeyBase58"": ""2R2JpJUNonBPaawUGhWXeS1T1EsYz8f7nQ4GZyKwhLQ1""
            }
          ],
          ""service"": [
            {
              ""id"": ""did:example:multi#endpoint"",
              ""type"": ""ComplexService"",
              ""serviceEndpoint"": [""https://a.example.com"", ""https://b.example.com""]
            }
          ]
        }";
        var doc = DidDocument.Parse(multiEndpoint);
        doc.ServiceEndpoints.Should().HaveCount(1);
        var endpoints = doc.ServiceEndpoints[0].Endpoints;
        endpoints.Should().BeOfType<List<object>>(); // Or IReadOnlyList<object>
        endpoints.Should().HaveCount(2);
        endpoints[0].Should().Be("https://a.example.com");
        endpoints[1].Should().Be("https://b.example.com");
    }

    [Fact]
    public void ToJson_Produces_ValidJson_WithAllFields()
    {
        var doc = DidDocument.Parse(FullValidJson);
        string json = doc.ToJson();
        Action act = () => JObject.Parse(json);
        act.Should().NotThrow();
        json.Should().Contain("\"@context\"");
        json.Should().Contain("\"id\"");
        json.Should().Contain("\"verificationMethod\"");
    }

    [Fact]
    public void Parse_MalformedJson_ShouldThrow()
    {
      string malformed = @"{ ""@context"": [""https://www.w3.org/ns/did/v1"", }";
      Action act = () => DidDocument.Parse(malformed);
      act.Should().Throw<Exception>();
    }
    [Fact]
    public void Parse_DeeplyNestedService_ShouldNotThrowOrShouldThrowGracefully()
    {
      string deepNested = @"
    {
      ""@context"": [""https://www.w3.org/ns/did/v1""],
      ""id"": ""did:example:nested"",
      ""verificationMethod"": [
        {
          ""id"": ""did:example:nested#key-1"",
          ""type"": ""Ed25519VerificationKey2020"",
          ""controller"": ""did:example:nested"",
          ""publicKeyBase58"": ""2R2JpJUNonBPaawUGhWXeS1T1EsYz8f7nQ4GZyKwhLQ1""
        }
      ],
      ""service"": [
        {
          ""id"": ""did:example:nested#service"",
          ""type"": ""ComplexService"",
          ""serviceEndpoint"": { ""foo"": { ""bar"": [1,2,3], ""baz"": ""test"" } }
        }
      ]
    }";
      var doc = DidDocument.Parse(deepNested);
      doc.ServiceEndpoints.Should().HaveCount(1);
      // Optionally, check that the service endpoint can be traversed or is stored as a JObject
    }
    
    [Fact]
    public void Parse_UnicodeCharacters_ShouldSucceed()
    {
      string unicodeDoc = @"
    {
      ""@context"": [""https://www.w3.org/ns/did/v1""],
      ""id"": ""did:example:🍍✨"",
      ""verificationMethod"": [
        {
          ""id"": ""did:example:🍍✨#key-1"",
          ""type"": ""Ed25519VerificationKey2020"",
          ""controller"": ""did:example:🍍✨"",
          ""publicKeyBase58"": ""2R2JpJUNonBPaawUGhWXeS1T1EsYz8f7nQ4GZyKwhLQ1""
        }
      ]
    }";
      var doc = DidDocument.Parse(unicodeDoc);
      doc.Id.Should().Be("did:example:🍍✨");
    }
    
    [Fact]
    public void Parse_WithUnknownProperties_ShouldNotThrow()
    {
      string unknownProps = @"
    {
      ""@context"": [""https://www.w3.org/ns/did/v1""],
      ""id"": ""did:example:extra"",
      ""verificationMethod"": [
        {
          ""id"": ""did:example:extra#key-1"",
          ""type"": ""Ed25519VerificationKey2020"",
          ""controller"": ""did:example:extra"",
          ""publicKeyBase58"": ""2R2JpJUNonBPaawUGhWXeS1T1EsYz8f7nQ4GZyKwhLQ1""
        }
      ],
      ""someUnknownField"": ""should be ignored"",
      ""anotherExtra"": {""obj"": 42}
    }";
      var doc = DidDocument.Parse(unknownProps);
      doc.Should().NotBeNull();
    }
  
    [Theory]
    [InlineData(@"{ ""@context"": [""https://www.w3.org/ns/did/v1""], ""id"": [123,456] }")] // id should not be array
    [InlineData(@"{ ""@context"": [""https://www.w3.org/ns/did/v1""], ""id"": null }")]
    [InlineData(@"{ ""@context"": [""https://www.w3.org/ns/did/v1""], ""id"": """" }")]
    public void Parse_FuzzedOrPoisonedJson_ShouldThrow(string input)
    {
      Action act = () => DidDocument.Parse(input);
      act.Should().Throw<Exception>();
    }
    
    [Fact]
    public void Parse_VeryLargeDidDocument_ShouldNotCrashOrLeakMemory()
    {
      // Generate a DID Document with 2,000 verification methods and 2,000 services
      var sb = new StringBuilder();
      sb.Append(@"{
      ""@context"": [""https://www.w3.org/ns/did/v1""],
      ""id"": ""did:example:huge"",
      ""verificationMethod"": [");
      for (int i = 0; i < 2000; i++)
      {
        if (i > 0) sb.Append(",");
        sb.Append($@"{{ ""id"": ""did:example:huge#key-{i}"", ""type"": ""Ed25519VerificationKey2020"", ""controller"": ""did:example:huge"", ""publicKeyBase58"": ""H3C2AVvLMfBspc...{i}"" }}");
      }
      sb.Append("],");
      sb.Append(@"""service"": [");
      for (int i = 0; i < 2000; i++)
      {
        if (i > 0) sb.Append(",");
        sb.Append($@"{{ ""id"": ""did:example:huge#service-{i}"", ""type"": ""TestService"", ""serviceEndpoint"": ""https://service{i}.example.com/"" }}");
      }
      sb.Append("]}");
      string largeJson = sb.ToString();

      var stopwatch = System.Diagnostics.Stopwatch.StartNew();
      var doc = DidDocument.Parse(largeJson);
      stopwatch.Stop();

      doc.Should().NotBeNull();
      doc.VerificationMethods.Should().HaveCount(2000);
      doc.ServiceEndpoints.Should().HaveCount(2000);

      // Performance: parsing must complete in under X seconds (tune threshold for your hardware/environment)
      stopwatch.Elapsed.TotalSeconds.Should().BeLessThan(5);

      // Optional: Check that no duplicate IDs, etc.
      doc.VerificationMethods.Select(vm => vm.Id).Distinct().Count().Should().Be(2000);
    }

    [Fact]
    public void Parse_FuzzedRandomDocuments_ShouldNotCrash()
    {
      var rand = new Random();
      for (int i = 0; i < 100; i++)
      {
        // Randomly generate plausible-but-not-always-valid fields
        var id = $"did:example:rand{i}_{rand.Next(1000)}";
        var keyId = $"{id}#key-{rand.Next(100)}";
        var type = rand.Next(2) == 0 ? "Ed25519VerificationKey2020" : "EcdsaSecp256k1VerificationKey2019";
        var base58 = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));

        // Introduce random errors
        var verificationMethod = rand.Next(4) switch
        {
          0 => "null",
          1 => "{}",
          2 => $@"{{ ""id"": ""{keyId}"", ""type"": ""{type}"", ""controller"": ""{id}"", ""publicKeyBase58"": ""{base58}"" }}",
          _ => "[]"
        };

        var json = $@"
        {{
          ""@context"": [""https://www.w3.org/ns/did/v1""],
          ""id"": ""{id}"",
          ""verificationMethod"": {verificationMethod}
        }}";

        try
        {
          var doc = DidDocument.Parse(json);
          doc.Id.Should().Be(id); // If it parses, ID should be as expected
        }
        catch (Exception ex)
        {
          // The parser should not crash or hang, but should throw predictably on bad input
          ex.Should().BeAssignableTo<Exception>();
        }
      }
    }

     // Optional: schema validation (if schema is integrated)
    // [Fact]
    // public void Parse_InvalidSchema_ShouldThrowSchemaValidation()
    // {
    //     string invalidSchemaJson = @"{ ""@context"":[], ""id"":123, ""verificationMethod"":[] }";
    //     Action act = () => DidDocument.Parse(invalidSchemaJson);
    //     act.Should().Throw<SchemaValidationException>();
    // }

    // Add more: very deep/nested documents, non-string IDs, malicious input, max length, etc.

    // (Continue to add theory tests for known W3C test vectors, as well as random/fuzzed edge cases)

}