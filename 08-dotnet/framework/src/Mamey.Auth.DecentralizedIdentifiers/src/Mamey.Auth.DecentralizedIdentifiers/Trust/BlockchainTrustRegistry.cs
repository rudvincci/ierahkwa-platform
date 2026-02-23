using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Nethereum.Web3;

namespace Mamey.Auth.DecentralizedIdentifiers.Trust;

/// <summary>
/// Loads trusted DIDs or addresses from an on-chain smart contract registry (EVM/Ethereum).
/// The contract must expose a function to enumerate, query, or emit trusted issuer events.
/// </summary>
public class BlockchainTrustRegistry : ITrustRegistry
{
    private readonly Web3 _web3;
    private readonly string _contractAddress;
    private readonly string _network;
    private readonly HashSet<string> _trustedIssuers = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Creates a blockchain-based trust registry.
    /// </summary>
    /// <param name="web3">Nethereum Web3 instance, connected to the right network.</param>
    /// <param name="contractAddress">Smart contract address that manages the trust list.</param>
    /// <param name="network">Network name (for display/logging; e.g. "ethereum-mainnet").</param>
    public BlockchainTrustRegistry(Web3 web3, string contractAddress, string network = "ethereum-mainnet")
    {
        _web3 = web3 ?? throw new ArgumentNullException(nameof(web3));
        _contractAddress = contractAddress ?? throw new ArgumentNullException(nameof(contractAddress));
        _network = network ?? "ethereum-mainnet";
    }

    /// <inheritdoc />
    public void AddTrustedIssuer(string did) => _trustedIssuers.Add(did);

    /// <inheritdoc />
    public void RemoveTrustedIssuer(string did) => _trustedIssuers.Remove(did);

    /// <inheritdoc />
    public bool IsTrusted(string did) => _trustedIssuers.Contains(did);

    /// <inheritdoc />
    public IEnumerable<string> AllTrusted() => _trustedIssuers;

    /// <summary>
    /// Asynchronously fetches the trust list from the on-chain contract and updates local cache.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        // Assumption: Contract has a `getTrustedDids()` view function returning string[]
        var function = _web3.Eth.GetContract(QueryAbi, _contractAddress).GetFunction("getTrustedDids");
        var dids = await function.CallAsync<List<string>>();
        _trustedIssuers.Clear();
        foreach (var did in dids)
            _trustedIssuers.Add(did);
    }

    /// <summary>
    /// Sample contract ABI fragment for the `getTrustedDids()` function.
    /// If your contract exposes a mapping, event log, or different function, update as needed.
    /// </summary>
    public static string QueryAbi => @"[
        {
            ""constant"": true,
            ""inputs"": [],
            ""name"": ""getTrustedDids"",
            ""outputs"": [{""name"": """", ""type"": ""string[]""}],
            ""payable"": false,
            ""stateMutability"": ""view"",
            ""type"": ""function""
        }
    ]";
}