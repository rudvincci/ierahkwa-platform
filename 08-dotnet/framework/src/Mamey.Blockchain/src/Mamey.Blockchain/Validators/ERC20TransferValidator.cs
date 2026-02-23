using FluentValidation;
using Mamey.Blockchain.Types;
using Nethereum.UI.Validation;

namespace Mamey.Blockchain.Validators;

public class ERC20TransferValidator<T> : AbstractValidator<ERC20TransferModel<T>>
    where T : IERC20Contract, new()
{
    public ERC20TransferValidator()
    {
        RuleFor(t => t.To).IsEthereumAddress();
        RuleFor(t => t.Value).GreaterThan(0).WithMessage("Amount must be greater than 0");
        RuleFor(t => t.ERC20Contract).SetValidator(new ERC20ContractValidator<T>());
    }
}