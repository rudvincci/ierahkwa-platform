namespace Mamey.Identity.Decentralized.Configuration;

public class BlockchainTrustRegistryOptions
{
    public string RpcUrl { get; set; } = "https://mainnet.infura.io/v3/YOUR_API_KEY";
    public string ContractAddress { get; set; } = "0xYourSmartContractAddress";
}