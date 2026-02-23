namespace Mamey.Identity.Decentralized.Core;

/// <summary>
/// Well-known context URIs for DID Documents.
/// </summary>
public static class DidContextConstants
{
    /// <summary>
    /// The primary context URI for all W3C DID Documents.
    /// </summary>
    public const string W3cDidContext = "https://www.w3.org/ns/did/v1";

    /// <summary>
    /// Context for verifiable credentials.
    /// </summary>
    public const string VerifiableCredentialContext = "https://www.w3.org/2018/credentials/v1";

    /// <summary>
    /// Add any additional well-known or supported extension contexts here.
    /// </summary>
}