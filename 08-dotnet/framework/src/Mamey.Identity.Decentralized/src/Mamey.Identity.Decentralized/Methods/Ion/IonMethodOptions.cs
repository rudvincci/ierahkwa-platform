namespace Mamey.Identity.Decentralized.Methods.Ion;

/// <summary>
/// Options for creating a new did:ion identifier.
/// </summary>
public class IonMethodOptions
{
    /// <summary>
    /// The public key to use for recovery operations (JWK format, strongly recommended).
    /// </summary>
    public IDictionary<string, object> RecoveryPublicKeyJwk { get; set; }

    /// <summary>
    /// The public key for updates (JWK format, strongly recommended).
    /// </summary>
    public IDictionary<string, object> UpdatePublicKeyJwk { get; set; }

    /// <summary>
    /// The DID Document public keys (in ION "publicKeys" array format).
    /// </summary>
    public IList<IDictionary<string, object>> PublicKeys { get; set; }

    /// <summary>
    /// The DID Document service endpoints (in ION "services" array format).
    /// </summary>
    public IList<IDictionary<string, object>> Services { get; set; }

    /// <summary>
    /// Optional: Additional operation metadata.
    /// </summary>
    public IDictionary<string, object> Metadata { get; set; }
}

/// <summary>
/// Options for updating a did:ion identifier.
/// </summary>
public class IonUpdateOptions
{
    /// <summary>
    /// The current update private key (for signing the update operation).
    /// </summary>
    public string UpdatePrivateKeyJwk { get; set; }

    /// <summary>
    /// Patches to apply (add/remove keys/services, per ION "patches" array).
    /// </summary>
    public IList<IDictionary<string, object>> Patches { get; set; }

    /// <summary>
    /// If rotating update key: new public key JWK.
    /// </summary>
    public IDictionary<string, object> NextUpdatePublicKeyJwk { get; set; }
}