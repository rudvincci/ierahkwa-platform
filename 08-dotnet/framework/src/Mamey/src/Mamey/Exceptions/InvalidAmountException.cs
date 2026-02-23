using Mamey.Types;

namespace Mamey.Exceptions;

public class InvalidAmountException : MameyException
{
    public InvalidAmountException(string message, decimal amount) 
        : base(message, "invalid_amount", "Amount is invalid.")
    {
        Amount = amount;
    }
    public InvalidAmountException(decimal amount) 
        : base($"Amount: '{amount}' is invalid.", "invalid_amount", "Amount is invalid.")
    {
        Amount = amount;
    }
    public InvalidAmountException(Amount amount) 
        : base($"Amount: '{amount.Value}' is invalid.", "invalid_amount", "Amount is invalid.")
    {
        Amount = amount.Value;
    }
    public decimal Amount { get; }
}