namespace Mamey.Auth.DecentralizedIdentifiers.Utilities;

/// <summary>
/// Constants for well-known DID document JSON-LD vocabularies, property names, and types.
/// </summary>
public static class SchemaConstants
{
    public const string DidContext = "https://www.w3.org/ns/credentials/v2";
    public const string VerificationMethodEd25519Type = "Ed25519VerificationKey2020";
    public const string AssertionMethod = "assertionMethod";
    public const string Authentication = "authentication";
    public const string KeyAgreement = "keyAgreement";
    public const string CapabilityDelegation = "capabilityDelegation";
    public const string CapabilityInvocation = "capabilityInvocation";
    public const string Service = "service";
    public const string Id = "id";
    public const string Type = "type";
    public const string Controller = "controller";
    public const string PublicKeyBase58 = "publicKeyBase58";
    public const string PublicKeyJwk = "publicKeyJwk";
    public const string PublicKeyMultibase = "publicKeyMultibase";
    // Add others as the W3C/VC/DID/JSON-LD specs expand.
}