using FluentValidation;
using Mamey.Blockchain.Types;
using Nethereum.UI.Validation;

namespace Mamey.Blockchain.Validators;

public class ERC20ContractValidator<T> : AbstractValidator<T>
    where T : IERC20Contract
{
    public ERC20ContractValidator()
    {
        RuleFor(t => t.Address).IsEthereumAddress();
        RuleFor(t => t.DecimalPlaces).GreaterThan(0).WithMessage("Decimal Places must be greater than 0");
    }
}
