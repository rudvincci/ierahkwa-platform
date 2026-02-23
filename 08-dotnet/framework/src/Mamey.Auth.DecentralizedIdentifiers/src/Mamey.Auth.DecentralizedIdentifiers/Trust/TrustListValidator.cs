namespace Mamey.Auth.DecentralizedIdentifiers.Trust;

/// <summary>
/// Validates trust for credentials, issuers, or operations using a TrustRegistry.
/// </summary>
public class TrustListValidator
{
    private readonly TrustRegistry _trustRegistry;

    public TrustListValidator(TrustRegistry trustRegistry)
    {
        _trustRegistry = trustRegistry ?? throw new ArgumentNullException(nameof(trustRegistry));
    }

    /// <summary>
    /// Checks if the issuer of a credential/DID is trusted.
    /// </summary>
    public void ValidateIssuer(string issuerDid)
    {
        if (!_trustRegistry.IsTrusted(issuerDid))
            throw new UnauthorizedAccessException($"Issuer {issuerDid} is not trusted in the current trust registry.");
    }

    /// <summary>
    /// Checks trust, returns true/false.
    /// </summary>
    public bool IsTrusted(string issuerDid) => _trustRegistry.IsTrusted(issuerDid);
}