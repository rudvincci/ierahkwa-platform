using Mamey.Web3.Services;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;

namespace Mamey.Web3.ViewModels;

public class SendTransactionViewModel : SendTransactionBaseViewModel
{
    public SendTransactionViewModel(IAccountsService accountsService) : base(accountsService)
    {

    }

    public async Task<string> SendTransactionAsync()
    {
        var transactionInput =
            new TransactionInput
            {
                Value = new HexBigInteger(Nethereum.Web3.Web3.Convert.ToWei(AmountInEther)),
                To = AddressTo,
                From = Account
            };
        if (Gas != null)
            transactionInput.Gas = new HexBigInteger(Gas.Value);
        if (!string.IsNullOrEmpty(GasPrice))
        {
            var parsed = decimal.Parse(GasPrice);
            transactionInput.GasPrice = new HexBigInteger(Nethereum.Web3.Web3.Convert.ToWei(GasPrice, UnitConversion.EthUnit.Gwei));
        }

        if (Nonce != null)
            transactionInput.Nonce = new HexBigInteger(Nonce.Value);
        if (!string.IsNullOrEmpty(Data))
            transactionInput.Data = Data;

        return await AccountsService.SendTransactionAsync(transactionInput).ConfigureAwait(false);
    }
}