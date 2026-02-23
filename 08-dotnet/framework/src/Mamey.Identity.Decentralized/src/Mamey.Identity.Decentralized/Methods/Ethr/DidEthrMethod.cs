using System.Text.Json;
using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Methods.MethodBase;
using System.Numerics;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;

namespace Mamey.Identity.Decentralized.Methods.Ethr;

public class DidEthrMethod : DidMethodBase
{
    public override string Name => "ethr";
    private readonly HttpClient _httpClient;
    private readonly string _ethrRegistryEndpoint;
    private readonly string _ethrRegistryContractAddress;
    private readonly string _ethNodeUrl;

    public DidEthrMethod(HttpClient httpClient, string ethrRegistryEndpoint, string ethrRegistryContractAddress,
        string ethNodeUrl)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ethrRegistryEndpoint = ethrRegistryEndpoint ?? throw new ArgumentNullException(nameof(ethrRegistryEndpoint));
        _ethrRegistryContractAddress = ethrRegistryContractAddress ??
                                       throw new ArgumentNullException(nameof(ethrRegistryContractAddress));
        _ethNodeUrl = ethNodeUrl ?? throw new ArgumentNullException(nameof(ethNodeUrl));
    }

    public override async Task<IDidDocument> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        ValidateDid(did);

        // Example: resolve via public resolver
        var url = $"{_ethrRegistryEndpoint.TrimEnd('/')}/identifiers/{did}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var resolveResponse =
            await JsonSerializer.DeserializeAsync<EthrResolveResponse>(stream, cancellationToken: cancellationToken);

        if (resolveResponse?.DidDocument == null)
            throw new Exception($"DID Document not found in ethr resolver response for {did}.");

        return resolveResponse.DidDocument;
    }

    public override async Task<IDidDocument> CreateAsync(object options, CancellationToken cancellationToken = default)
    {
        if (options is not EthrMethodOptions ethrOptions)
            throw new ArgumentException("Invalid options for did:ethr creation.");

        ValidateAddress(ethrOptions.ControllerAddress);

        // Create a Nethereum account
        var account = new Account(ethrOptions.PrivateKey);
        var web3 = new Web3(account, _ethNodeUrl);

        // Add the controller address as an owner of itself (main pattern for new DIDs)
        var contract = web3.Eth.GetContract(EthrDidRegistryAbi.ABI, _ethrRegistryContractAddress);

        // Optionally, add keys/services via setAttribute
        var gas = new HexBigInteger(600_000);

        // For demo, set a "did/pub/Ed25519/veriKey/hex" public key attribute
        if (ethrOptions.PublicKeys != null)
        {
            foreach (var key in ethrOptions.PublicKeys)
            {
                var keyHex = key.TryGetValue("hex", out var hex) ? hex.ToString() : null;
                var attrName = "did/pub/Ed25519/veriKey/hex"; // per Ethr DID spec; or customize
                var attrValue = System.Text.Encoding.UTF8.GetBytes(keyHex ?? "");
                var setAttrFunc = contract.GetFunction("setAttribute");
                var txReceipt = await setAttrFunc.SendTransactionAndWaitForReceiptAsync(account.Address, gas, null,
                    null,
                    account.Address, attrName, attrValue, BigInteger.Zero);
            }
        }

        // Optionally, add services as attributes (setAttribute with "did/svc/ServiceName" as attrName)
        if (ethrOptions.ServiceEndpoints != null)
        {
            foreach (var svc in ethrOptions.ServiceEndpoints)
            {
                var svcName = svc.TryGetValue("name", out var n) ? n.ToString() : "svc";
                var svcEndpoint = svc.TryGetValue("endpoint", out var ep) ? ep.ToString() : "";
                var attrName = $"did/svc/{svcName}";
                var attrValue = System.Text.Encoding.UTF8.GetBytes(svcEndpoint);
                var setAttrFunc = contract.GetFunction("setAttribute");
                var txReceipt = await setAttrFunc.SendTransactionAndWaitForReceiptAsync(account.Address, gas, null,
                    null,
                    account.Address, attrName, attrValue, BigInteger.Zero);
            }
        }

        // Wait for finality or just resolve
        string did = $"did:ethr:{ethrOptions.ControllerAddress}";
        return await ResolveAsync(did, cancellationToken);
    }

    public override async Task<IDidDocument> UpdateAsync(string did, object updateRequest,
        CancellationToken cancellationToken = default)
    {
        ValidateDid(did);

        if (updateRequest is not EthrUpdateOptions updateOptions)
            throw new ArgumentException("updateRequest must be EthrUpdateOptions.");

        ValidateAddress(updateOptions.ControllerAddress);

        var account = new Account(updateOptions.PrivateKey);
        var web3 = new Web3(account, _ethNodeUrl);
        var contract = web3.Eth.GetContract(EthrDidRegistryAbi.ABI, _ethrRegistryContractAddress);
        var gas = new HexBigInteger(600_000);

        if (updateOptions.Patches != null)
        {
            foreach (var patch in updateOptions.Patches)
            {
                var op = patch.TryGetValue("op", out var o) ? o.ToString() : null;
                var path = patch.TryGetValue("path", out var p) ? p.ToString() : null;
                var value = patch.TryGetValue("value", out var v) ? v : null;
                if (op == "add" && path != null)
                {
                    // Add a new attribute (key/service) as above
                    var attrValue = System.Text.Encoding.UTF8.GetBytes(value?.ToString() ?? "");
                    var setAttrFunc = contract.GetFunction("setAttribute");
                    var txReceipt = await setAttrFunc.SendTransactionAndWaitForReceiptAsync(account.Address, gas, null,
                        null,
                        account.Address, path, attrValue, BigInteger.Zero);
                }
                else if (op == "remove" && path != null)
                {
                    // Remove attribute (uses "revokeAttribute" in registry contract)
                    var attrValue = System.Text.Encoding.UTF8.GetBytes(value?.ToString() ?? "");
                    var revokeAttrFunc = contract.GetFunction("revokeAttribute");
                    var txReceipt = await revokeAttrFunc.SendTransactionAndWaitForReceiptAsync(account.Address, gas,
                        null, null,
                        account.Address, path, attrValue, BigInteger.Zero);
                }
            }
        }

        return await ResolveAsync(did, cancellationToken);
    }

    public override async Task DeactivateAsync(string did, CancellationToken cancellationToken = default)
    {
        ValidateDid(did);

        // To "deactivate", remove controller rights by transferring to null address
        // Or use setOwner to 0x000...0
        // Note: in production, set owner to Ethereum null address: 0x0000000000000000000000000000000000000000

        throw new NotImplementedException(
            "Deactivate not implemented in sample. To deactivate, transfer controller to 0x0.");
    }

    private static void ValidateAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address) || !address.StartsWith("0x") || address.Length != 42)
            throw new ArgumentException("Invalid Ethereum address.");
    }
}