using Mamey.Web3.Messages;
using Mamey.Web3.Services;
using ReactiveUI;

namespace Mamey.Web3.ViewModels;

public class LatestBlockTransactionsViewModel : BlockTransactionsViewModel
{
    public LatestBlockTransactionsViewModel(IWeb3ProviderService web3ProviderService):base(web3ProviderService)
    {
        MessageBus.Current.Listen<NewBlock>().Subscribe(x =>
            {
                if (x.BlockNumber != BlockNumber)
                {
                    BlockNumber = x.BlockNumber;
                }
            }
        );
    }
}