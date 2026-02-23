namespace Mamey.Blockchain.Types;

public abstract class ERC20Contract : IERC20Contract
{
    public const int DEFAULT_DECIMALS = 18;

    public ERC20Contract(string name, string symbol, string contractAddress, string contractUrl)
    {
        Name = name;
        Symbol = symbol;
        Address = contractAddress;
        ContractUrl = new Uri(contractUrl);
    }

    public string Name { get; }
    public string Symbol { get; }
    public string Address { get; set; }
    public int DecimalPlaces { get; set; } = DEFAULT_DECIMALS;
    public Uri ContractUrl { get; }
}
