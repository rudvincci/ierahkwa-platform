using System.Text.Json.Serialization;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Ethr;

/// <summary>
/// Options for did:ethr method create/update operations.
/// </summary>
public class EthrMethodOptions
{
    /// <summary>
    /// The Ethereum network (e.g., "mainnet", "ropsten", or custom endpoint).
    /// </summary>
    [JsonPropertyName("network")]
    public string Network { get; set; }

    /// <summary>
    /// The Ethereum address controlling the DID.
    /// </summary>
    [JsonPropertyName("controllerAddress")]
    public string ControllerAddress { get; set; }

    /// <summary>
    /// The private key for the controller account.
    /// </summary>
    [JsonPropertyName("privateKey")]
    public string PrivateKey { get; set; }

    /// <summary>
    /// List of public keys to include in the DID document (JWK, hex, etc).
    /// </summary>
    [JsonPropertyName("publicKeys")]
    public IList<IDictionary<string, object>> PublicKeys { get; set; }

    /// <summary>
    /// Service endpoints to publish in the DID document.
    /// </summary>
    [JsonPropertyName("serviceEndpoints")]
    public IList<IDictionary<string, object>> ServiceEndpoints { get; set; }

    /// <summary>
    /// Any additional metadata or custom fields.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IDictionary<string, object> Metadata { get; set; }
}
public static class EthrDidRegistryAbi
{
    public const string ABI = @"[ ... (snip: use the actual ABI from https://github.com/decentralized-identity/ethr-did-registry/blob/develop/abi/EthereumDIDRegistry.json) ... ]";
}