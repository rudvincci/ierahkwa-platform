namespace Mamey.Web3.Services.TokenList.Model;

public class Token
{
    public int ChainId { get; set; }
    public string Address { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    public int Decimals { get; set; }
    public string LogoURI { get; set; }
}