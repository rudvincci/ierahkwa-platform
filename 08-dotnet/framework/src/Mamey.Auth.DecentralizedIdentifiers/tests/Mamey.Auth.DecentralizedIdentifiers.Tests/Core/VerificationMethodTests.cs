using FluentAssertions;
using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.Tests.Core;

public class VerificationMethodTests
{
    [Fact]
    public void Constructor_WithAllFields_ShouldAssignProperties()
    {
        var jwk = new Dictionary<string, object>
            { { "kty", "EC" }, { "crv", "secp256k1" }, { "x", "Base64X" }, { "y", "Base64Y" } };
        var additional = new Dictionary<string, object> { { "foo", 42 } };

        var method = new VerificationMethod(
            id: "did:example:123#key-1",
            type: "EcdsaSecp256k1VerificationKey2019",
            controller: "did:example:123",
            publicKeyJwk: jwk,
            publicKeyBase58: "3S7tyr9hQdYcXGuA8n7b2U5Ew5f9v7dNyk",
            publicKeyMultibase: "z6MkXJ3sUZ8Sx8PpoDgm2BHTMe9n4Pqkt8P4N",
            additionalProperties: additional);

        method.Id.Should().Be("did:example:123#key-1");
        method.Type.Should().Be("EcdsaSecp256k1VerificationKey2019");
        method.Controller.Should().Be("did:example:123");
        method.PublicKeyJwk.Should().ContainKey("kty").And.ContainKey("x");
        method.PublicKeyBase58.Should().Be("3S7tyr9hQdYcXGuA8n7b2U5Ew5f9v7dNyk");
        method.PublicKeyMultibase.Should().Be("z6MkXJ3sUZ8Sx8PpoDgm2BHTMe9n4Pqkt8P4N");
        method.AdditionalProperties.Should().ContainKey("foo").WhoseValue.Should().Be(42);
    }

    [Fact]
    public void Constructor_NullOptionals_ShouldDefaultToEmpty()
    {
        var method = new VerificationMethod("did:ex:1#k", "Ed25519VerificationKey2020", "did:ex:1");
        method.PublicKeyJwk.Should().BeEmpty();
        method.PublicKeyBase58.Should().BeNull();
        method.PublicKeyMultibase.Should().BeNull();
        method.AdditionalProperties.Should().BeEmpty();
    }

    [Fact]
    public void GetPublicKeyBytes_Multibase_ShouldDecode()
    {
        // Multibase: starts with 'z', valid base58btc content (simulate)
        var mbStr = "z3S7tyr9hQdYcXGuA8n7b2U5Ew5f9v7dNyk";
        var method = new VerificationMethod("did:ex:1#k", "Ed25519VerificationKey2020", "did:ex:1",
            publicKeyMultibase: mbStr);

        // Substitute Crypto.MultibaseUtil.Decode
        byte[] result = method.GetPublicKeyBytes();
        result.Should().NotBeNull().And.HaveCountGreaterThan(0);
    }

    [Fact]
    public void GetPublicKeyBytes_Base58_ShouldDecode()
    {
        var b58Str = "4v4rVfL8WTExxULF2g5DUSWJjSufWQas7GbGtp1DbJwy";
        var method = new VerificationMethod("did:ex:2#k", "Ed25519VerificationKey2020", "did:ex:2",
            publicKeyBase58: b58Str);

        byte[] result = method.GetPublicKeyBytes();
        result.Should().NotBeNull().And.HaveCountGreaterThan(0);
    }

    [Fact]
    public void GetPublicKeyBytes_JwkEd25519_ShouldExtractX()
    {
        var jwk = new Dictionary<string, object>
        {
            { "kty", "OKP" }, { "crv", "Ed25519" },
            { "x", "ZmFrZV9lZDI1NTE5X2tleQ" } // base64url for "fake_ed25519_key"
        };
        var method = new VerificationMethod("did:ex:3#k", "Ed25519VerificationKey2020", "did:ex:3",
            publicKeyJwk: jwk);

        var result = method.GetPublicKeyBytes();
        result.Should().BeEquivalentTo(Convert.FromBase64String("ZmFrZV9lZDI1NTE5X2tleQ==")); // b64url to b64 padding
    }

    [Fact]
    public void GetPublicKeyBytes_JwkUnknown_Throws()
    {
        var jwk = new Dictionary<string, object> { { "kty", "EC" }, { "crv", "P-256" } };
        var method = new VerificationMethod("did:ex:4#k", "EcdsaSecp256k1VerificationKey2019", "did:ex:4",
            publicKeyJwk: jwk);

        Action act = () => method.GetPublicKeyBytes();
        act.Should().Throw<NotSupportedException>().WithMessage("*JWK format present but not supported*");
    }

    [Fact]
    public void GetPublicKeyBytes_NoKey_Throws()
    {
        var method = new VerificationMethod("did:ex:5#k", "Ed25519VerificationKey2020", "did:ex:5");
        Action act = () => method.GetPublicKeyBytes();
        act.Should().Throw<InvalidOperationException>().WithMessage("*No usable public key found*");
    }

    [Fact]
    public void AdditionalProperties_AreImmutableCopy()
    {
        var props = new Dictionary<string, object> { { "one", 1 } };
        var method = new VerificationMethod("did:ex:7#k", "Ed25519VerificationKey2020", "did:ex:7",
            additionalProperties: props);

        props["one"] = 99;
        method.AdditionalProperties["one"].Should().Be(1);
    }

    [Fact]
    public void Constructor_WithUnicodeIdAndController_ShouldSucceed()
    {
        var method = new VerificationMethod(
            id: "did:example:🌿✨#ключ",
            type: "Ed25519VerificationKey2020",
            controller: "did:example:🌿✨");
        method.Id.Should().Be("did:example:🌿✨#ключ");
        method.Controller.Should().Be("did:example:🌿✨");
    }

    [Fact]
    public void Constructor_NoId_ShouldThrowOrAssignNull()
    {
        Action act = () => new VerificationMethod(null, "Ed25519VerificationKey2020", "did:example:1");
        act.Should().Throw<ArgumentNullException>().WithMessage("*id*");
    }

    [Fact]
    public void GetPublicKeyBytes_MultipleKeyFormats_PrefersMultibaseOverBase58()
    {
        var mbStr = "z3S7tyr9hQdYcXGuA8n7b2U5Ew5f9v7dNyk";
        var b58Str = "4v4rVfL8WTExxULF2g5DUSWJjSufWQas7GbGtp1DbJwy";
        var method = new VerificationMethod("did:ex:1#k", "Ed25519VerificationKey2020", "did:ex:1",
            publicKeyMultibase: mbStr, publicKeyBase58: b58Str);

        // Confirm multibase decoder used, not base58
        var mbBytes = method.GetPublicKeyBytes();
        mbBytes.Should().NotBeEquivalentTo(Crypto.Base58Util.Decode(b58Str));
    }

    [Fact]
    public void GetPublicKeyBytes_JwkMissingX_ShouldThrow()
    {
        var jwk = new Dictionary<string, object> { { "kty", "OKP" }, { "crv", "Ed25519" } };
        var method = new VerificationMethod("did:ex:3#k", "Ed25519VerificationKey2020", "did:ex:3", publicKeyJwk: jwk);

        Action act = () => method.GetPublicKeyBytes();
        act.Should().Throw<FormatException>().WithMessage("*'x'*");
    }

    [Fact]
    public void GetPublicKeyBytes_InvalidBase58_ShouldThrow()
    {
        var b58Str = "invalid_base58_key_!@#$%";
        var method = new VerificationMethod("did:ex:2#k", "Ed25519VerificationKey2020", "did:ex:2",
            publicKeyBase58: b58Str);

        Action act = () => method.GetPublicKeyBytes();
        act.Should().Throw<FormatException>().WithMessage("*base58*");
    }

    [Fact]
    public void RoundTrip_SerializeDeserialize_ShouldPreserveAllProperties()
    {
        var method = new VerificationMethod(
            id: "did:example:123#key-1",
            type: "EcdsaSecp256k1VerificationKey2019",
            controller: "did:example:123",
            publicKeyBase58: "4v4rVfL8WTExxULF2g5DUSWJjSufWQas7GbGtp1DbJwy",
            additionalProperties: new Dictionary<string, object> { { "custom", 999 } }
        );
        var json = method.ToJson();
        var parsed = VerificationMethod.Parse(json);
        parsed.Id.Should().Be(method.Id);
        parsed.Type.Should().Be(method.Type);
        parsed.PublicKeyBase58.Should().Be(method.PublicKeyBase58);
        parsed.AdditionalProperties.Should().ContainKey("custom").WhoseValue.Should().Be(999);
    }

    [Fact]
    public void Controller_AsArrayAndString_ShouldParseBoth()
    {
        // Controller as string
        var m1 = VerificationMethod.Parse(@"
    { ""id"": ""did:example:1#key-1"", ""type"": ""Ed25519VerificationKey2020"", ""controller"": ""did:example:1"" }");
        m1.Controller.Should().Be("did:example:1");

        // Controller as array
        var m2 = VerificationMethod.Parse(@"
    { ""id"": ""did:example:2#key-2"", ""type"": ""Ed25519VerificationKey2020"", ""controller"": [""did:example:2""] }");
        m2.Controller.Should().Be("[\"did:example:2\"]");
    }
}