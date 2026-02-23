using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.Web3;

namespace Mamey.Blockchain.Types;

public class ERC20TransferModel<T> where T : IERC20Contract, new()
{
    public ERC20TransferModel()
    {
    }
    public T ERC20Contract { get; } = new T();
    public string To { get; set; }
    public decimal Value { get; set; }

    public TransferFunction GetTransferFunction()
    {
        return new TransferFunction()
        {
            To = To,
            Value = Web3.Convert.ToWei(Value, ERC20Contract.DecimalPlaces),
            AmountToSend = 0
        };
    }
}
