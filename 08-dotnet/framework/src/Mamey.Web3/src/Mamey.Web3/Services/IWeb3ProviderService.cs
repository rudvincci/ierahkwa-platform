using Mamey.Web3.Pages;

namespace Mamey.Web3.Services;

public interface IWeb3ProviderService
{
    string CurrentUrl { get; set; }
    string ChainId { get; }
    Nethereum.Web3.Web3 GetWeb3();
    Nethereum.Web3.Web3 GetWeb3(Balances.Account account);
}