namespace Mamey.Blockchain.Types;

public interface IERC20Contract
{
    public string Name { get; }
    public string Symbol { get; }
    public string Address { get; }
    public int DecimalPlaces { get; }
    public Uri ContractUrl { get; }
}
