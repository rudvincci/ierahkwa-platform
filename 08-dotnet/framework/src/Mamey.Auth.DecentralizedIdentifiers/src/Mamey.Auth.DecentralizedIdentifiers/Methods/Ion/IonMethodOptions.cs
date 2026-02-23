namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Ion;

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

    /// <summary>
    /// Bitcoin anchoring configuration for the DID.
    /// </summary>
    public BitcoinAnchoringOptions BitcoinAnchoring { get; set; } = new();
}

/// <summary>
/// Bitcoin anchoring configuration for ION DIDs.
/// </summary>
public class BitcoinAnchoringOptions
{
    /// <summary>
    /// Whether to enable Bitcoin anchoring for this DID.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Bitcoin network to use (mainnet, testnet, regtest).
    /// </summary>
    public string Network { get; set; } = "testnet";

    /// <summary>
    /// Bitcoin node RPC endpoint for anchoring operations.
    /// </summary>
    public string BitcoinNodeEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Bitcoin node RPC credentials.
    /// </summary>
    public BitcoinNodeCredentials Credentials { get; set; } = new();

    /// <summary>
    /// Fee rate for Bitcoin transactions (satoshis per byte).
    /// </summary>
    public int FeeRate { get; set; } = 10;

    /// <summary>
    /// Minimum confirmations required for anchoring.
    /// </summary>
    public int MinConfirmations { get; set; } = 1;
}

/// <summary>
/// Bitcoin node RPC credentials.
/// </summary>
public class BitcoinNodeCredentials
{
    /// <summary>
    /// RPC username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// RPC password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
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

    /// <summary>
    /// Bitcoin anchoring configuration for the update operation.
    /// </summary>
    public BitcoinAnchoringOptions BitcoinAnchoring { get; set; } = new();
}