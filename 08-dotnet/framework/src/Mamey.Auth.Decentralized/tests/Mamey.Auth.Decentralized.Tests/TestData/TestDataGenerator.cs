using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.VerifiableCredentials;
using Bogus;

namespace Mamey.Auth.Decentralized.Tests.TestData;

/// <summary>
/// Generates test data for the Mamey.Auth.Decentralized library tests.
/// </summary>
public static class TestDataGenerator
{
    private static readonly Faker _faker = new();
    private static readonly Random _random = new();

    /// <summary>
    /// Generates a valid DID for testing.
    /// </summary>
    /// <param name="method">The DID method (default: "web").</param>
    /// <param name="identifier">The identifier (default: random).</param>
    /// <returns>A valid DID.</returns>
    public static Did GenerateValidDid(string method = "web", string? identifier = null)
    {
        identifier ??= _faker.Internet.DomainName();
        return Did.Parse($"did:{method}:{identifier}");
    }

    /// <summary>
    /// Generates a valid DID URL for testing.
    /// </summary>
    /// <param name="method">The DID method (default: "web").</param>
    /// <param name="identifier">The identifier (default: random).</param>
    /// <param name="path">The path (optional).</param>
    /// <param name="query">The query string (optional).</param>
    /// <param name="fragment">The fragment (optional).</param>
    /// <returns>A valid DID URL.</returns>
    public static DidUrl GenerateValidDidUrl(string method = "web", string? identifier = null, 
        string? path = null, string? query = null, string? fragment = null)
    {
        identifier ??= _faker.Internet.DomainName();
        var didUrl = $"did:{method}:{identifier}";
        
        if (!string.IsNullOrEmpty(path))
            didUrl += $"/{path.TrimStart('/')}";
        if (!string.IsNullOrEmpty(query))
            didUrl += $"?{query}";
        if (!string.IsNullOrEmpty(fragment))
            didUrl += $"#{fragment}";
            
        return DidUrl.Parse(didUrl);
    }

    /// <summary>
    /// Generates a valid verification method for testing.
    /// </summary>
    /// <param name="did">The DID (default: generated).</param>
    /// <param name="type">The verification method type (default: "Ed25519VerificationKey2020").</param>
    /// <returns>A valid verification method.</returns>
    public static VerificationMethod GenerateValidVerificationMethod(Did? did = null, string type = "Ed25519VerificationKey2020")
    {
        did ??= GenerateValidDid();
        var keyId = $"{did}#key-{_faker.Random.AlphaNumeric(8)}";
        
        return new VerificationMethod
        {
            Id = keyId,
            Type = type,
            Controller = did.ToString(),
            PublicKeyJwk = GenerateValidJwk(type),
            PublicKeyMultibase = GenerateValidMultibase()
        };
    }

    /// <summary>
    /// Generates a valid service endpoint for testing.
    /// </summary>
    /// <param name="did">The DID (default: generated).</param>
    /// <param name="type">The service type (default: "DIDCommMessaging").</param>
    /// <returns>A valid service endpoint.</returns>
    public static ServiceEndpoint GenerateValidServiceEndpoint(Did? did = null, string type = "DIDCommMessaging")
    {
        did ??= GenerateValidDid();
        var serviceId = $"{did}#service-{_faker.Random.AlphaNumeric(8)}";
        
        return new ServiceEndpoint
        {
            Id = serviceId,
            Type = type,
            ServiceEndpointUrl = _faker.Internet.Url(),
            Properties = GenerateValidServiceProperties()
        };
    }

    /// <summary>
    /// Generates a valid DID document for testing.
    /// </summary>
    /// <param name="did">The DID (default: generated).</param>
    /// <param name="includeVerificationMethods">Whether to include verification methods (default: true).</param>
    /// <param name="includeServices">Whether to include services (default: true).</param>
    /// <returns>A valid DID document.</returns>
    public static DidDocument GenerateValidDidDocument(Did? did = null, bool includeVerificationMethods = true, bool includeServices = true)
    {
        did ??= GenerateValidDid();
        
        var document = new DidDocument
        {
            Id = did.ToString(),
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Controller = did.ToString(),
            AlsoKnownAs = GenerateValidAlsoKnownAs(),
            VerificationMethod = includeVerificationMethods ? GenerateValidVerificationMethods(did) : new List<VerificationMethod>(),
            Authentication = new List<string>(),
            AssertionMethod = new List<string>(),
            KeyAgreement = new List<string>(),
            CapabilityInvocation = new List<string>(),
            CapabilityDelegation = new List<string>(),
            Service = includeServices ? GenerateValidServices(did) : new List<ServiceEndpoint>()
        };

        // Add verification method references
        if (includeVerificationMethods && document.VerificationMethod.Any())
        {
            document.Authentication.Add(document.VerificationMethod.First().Id);
            document.AssertionMethod.Add(document.VerificationMethod.First().Id);
        }

        return document;
    }

    /// <summary>
    /// Generates a valid verifiable credential for testing.
    /// </summary>
    /// <param name="issuer">The issuer DID (default: generated).</param>
    /// <param name="subject">The subject DID (default: generated).</param>
    /// <returns>A valid verifiable credential.</returns>
    public static VerifiableCredential GenerateValidVerifiableCredential(Did? issuer = null, Did? subject = null)
    {
        issuer ??= GenerateValidDid();
        subject ??= GenerateValidDid();
        
        return new VerifiableCredential
        {
            Id = $"vc:{_faker.Random.AlphaNumeric(16)}",
            Type = new List<string> { "VerifiableCredential", "EmailCredential" },
            Issuer = issuer.ToString(),
            IssuanceDate = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            CredentialSubject = new CredentialSubject
            {
                Id = subject.ToString(),
                Properties = new Dictionary<string, object>
                {
                    ["email"] = _faker.Internet.Email(),
                    ["name"] = _faker.Name.FullName(),
                    ["verified"] = true
                }
            },
            Proof = GenerateValidProof()
        };
    }

    /// <summary>
    /// Generates a valid verifiable presentation for testing.
    /// </summary>
    /// <param name="holder">The holder DID (default: generated).</param>
    /// <param name="credentials">The credentials to include (default: generated).</param>
    /// <returns>A valid verifiable presentation.</returns>
    public static VerifiablePresentation GenerateValidVerifiablePresentation(Did? holder = null, List<VerifiableCredential>? credentials = null)
    {
        holder ??= GenerateValidDid();
        credentials ??= new List<VerifiableCredential> { GenerateValidVerifiableCredential(subject: holder) };
        
        return new VerifiablePresentation
        {
            Id = $"vp:{_faker.Random.AlphaNumeric(16)}",
            Type = new List<string> { "VerifiablePresentation" },
            Holder = holder.ToString(),
            VerifiableCredential = credentials,
            Proof = GenerateValidProof()
        };
    }

    /// <summary>
    /// Generates a valid proof for testing.
    /// </summary>
    /// <param name="type">The proof type (default: "Ed25519Signature2020").</param>
    /// <returns>A valid proof.</returns>
    public static Proof GenerateValidProof(string type = "Ed25519Signature2020")
    {
        return new Proof
        {
            Type = type,
            Created = DateTime.UtcNow,
            VerificationMethod = $"did:web:example.com#key-{_faker.Random.AlphaNumeric(8)}",
            ProofPurpose = "assertionMethod",
            ProofValue = _faker.Random.AlphaNumeric(64)
        };
    }

    /// <summary>
    /// Generates a valid credential subject for testing.
    /// </summary>
    /// <param name="id">The subject ID (default: generated DID).</param>
    /// <returns>A valid credential subject.</returns>
    public static CredentialSubject GenerateValidCredentialSubject(string? id = null)
    {
        id ??= GenerateValidDid().ToString();
        
        return new CredentialSubject
        {
            Id = id,
            Properties = new Dictionary<string, object>
            {
                ["email"] = _faker.Internet.Email(),
                ["name"] = _faker.Name.FullName(),
                ["age"] = _faker.Random.Int(18, 100),
                ["verified"] = _faker.Random.Bool()
            }
        };
    }

    /// <summary>
    /// Generates a valid JWK for testing.
    /// </summary>
    /// <param name="keyType">The key type (default: "Ed25519").</param>
    /// <returns>A valid JWK dictionary.</returns>
    public static Dictionary<string, object> GenerateValidJwk(string keyType = "Ed25519")
    {
        return keyType switch
        {
            "Ed25519" => new Dictionary<string, object>
            {
                ["kty"] = "OKP",
                ["crv"] = "Ed25519",
                ["x"] = _faker.Random.AlphaNumeric(43) // Base64url encoded 32-byte key
            },
            "RSA" => new Dictionary<string, object>
            {
                ["kty"] = "RSA",
                ["n"] = _faker.Random.AlphaNumeric(256), // Base64url encoded modulus
                ["e"] = "AQAB" // Base64url encoded exponent
            },
            "EC" => new Dictionary<string, object>
            {
                ["kty"] = "EC",
                ["crv"] = "P-256",
                ["x"] = _faker.Random.AlphaNumeric(43),
                ["y"] = _faker.Random.AlphaNumeric(43)
            },
            _ => new Dictionary<string, object>
            {
                ["kty"] = "OKP",
                ["crv"] = "Ed25519",
                ["x"] = _faker.Random.AlphaNumeric(43)
            }
        };
    }

    /// <summary>
    /// Generates a valid multibase string for testing.
    /// </summary>
    /// <returns>A valid multibase string.</returns>
    public static string GenerateValidMultibase()
    {
        // Generate a valid multibase string (z prefix for base58btc)
        var randomBytes = new byte[32];
        _random.NextBytes(randomBytes);
        return "z" + Convert.ToBase64String(randomBytes).Replace("+", "").Replace("/", "").Replace("=", "");
    }

    /// <summary>
    /// Generates a valid also known as list for testing.
    /// </summary>
    /// <returns>A valid also known as list.</returns>
    public static List<string> GenerateValidAlsoKnownAs()
    {
        return new List<string>
        {
            _faker.Internet.Url(),
            _faker.Internet.Url(),
            _faker.Internet.Url()
        };
    }

    /// <summary>
    /// Generates a valid list of verification methods for testing.
    /// </summary>
    /// <param name="did">The DID.</param>
    /// <param name="count">The number of verification methods to generate (default: 2).</param>
    /// <returns>A valid list of verification methods.</returns>
    public static List<VerificationMethod> GenerateValidVerificationMethods(Did did, int count = 2)
    {
        var methods = new List<VerificationMethod>();
        var types = new[] { "Ed25519VerificationKey2020", "RsaVerificationKey2018", "EcdsaSecp256k1VerificationKey2019" };
        
        for (int i = 0; i < count; i++)
        {
            methods.Add(GenerateValidVerificationMethod(did, types[i % types.Length]));
        }
        
        return methods;
    }

    /// <summary>
    /// Generates a valid list of services for testing.
    /// </summary>
    /// <param name="did">The DID.</param>
    /// <param name="count">The number of services to generate (default: 2).</param>
    /// <returns>A valid list of services.</returns>
    public static List<ServiceEndpoint> GenerateValidServices(Did did, int count = 2)
    {
        var services = new List<ServiceEndpoint>();
        var types = new[] { "DIDCommMessaging", "LinkedDomains", "DIDCommV2" };
        
        for (int i = 0; i < count; i++)
        {
            services.Add(GenerateValidServiceEndpoint(did, types[i % types.Length]));
        }
        
        return services;
    }

    /// <summary>
    /// Generates valid service properties for testing.
    /// </summary>
    /// <returns>A valid dictionary of service properties.</returns>
    public static Dictionary<string, JsonElement> GenerateValidServiceProperties()
    {
        var properties = new Dictionary<string, object>
        {
            ["description"] = _faker.Lorem.Sentence(),
            ["priority"] = _faker.Random.Int(1, 10),
            ["enabled"] = _faker.Random.Bool()
        };
        
        return properties.ToDictionary(
            kvp => kvp.Key,
            kvp => JsonElement.Parse(JsonSerializer.Serialize(kvp.Value))
        );
    }

    /// <summary>
    /// Generates invalid test data for negative testing.
    /// </summary>
    public static class Invalid
    {
        /// <summary>
        /// Generates an invalid DID for testing.
        /// </summary>
        /// <returns>An invalid DID string.</returns>
        public static string GenerateInvalidDid()
        {
            var invalidDids = new[]
            {
                "invalid-did",
                "did:",
                "did:invalid:",
                "did:web:",
                "did:web:invalid@character",
                "did:web:invalid space",
                "did:web:invalid/slash",
                "did:web:invalid#fragment",
                "did:web:invalid?query",
                ""
            };
            
            return _faker.PickRandom(invalidDids);
        }

        /// <summary>
        /// Generates an invalid DID URL for testing.
        /// </summary>
        /// <returns>An invalid DID URL string.</returns>
        public static string GenerateInvalidDidUrl()
        {
            var invalidUrls = new[]
            {
                "invalid-url",
                "did:",
                "did:invalid:",
                "did:web:invalid@character/path",
                "did:web:invalid space/path",
                "did:web:invalid#fragment?query",
                ""
            };
            
            return _faker.PickRandom(invalidUrls);
        }

        /// <summary>
        /// Generates an invalid verification method for testing.
        /// </summary>
        /// <returns>An invalid verification method.</returns>
        public static VerificationMethod GenerateInvalidVerificationMethod()
        {
            return new VerificationMethod
            {
                Id = "invalid-id",
                Type = "InvalidType",
                Controller = "invalid-controller",
                PublicKeyJwk = new Dictionary<string, object>(),
                PublicKeyMultibase = "invalid-multibase"
            };
        }

        /// <summary>
        /// Generates an invalid DID document for testing.
        /// </summary>
        /// <returns>An invalid DID document.</returns>
        public static DidDocument GenerateInvalidDidDocument()
        {
            return new DidDocument
            {
                Id = "invalid-did",
                Context = new List<string>(),
                Controller = "invalid-controller",
                VerificationMethod = new List<VerificationMethod>(),
                Service = new List<ServiceEndpoint>()
            };
        }
    }
}
