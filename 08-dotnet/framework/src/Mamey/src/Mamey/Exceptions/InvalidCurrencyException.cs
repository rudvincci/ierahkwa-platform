using Mamey.Types;

namespace Mamey.Exceptions;

public class InvalidCurrencyException(string currency, string message) : MameyException(message)
{
    public InvalidCurrencyException(string currency) : this(currency, $"Currency: '{currency}' is invalid.")
    {
    }
    public InvalidCurrencyException(Currency currency) : this(currency.Code, $"Currency: '{currency.Code}' is invalid.")
    {
    }
    public string Currency { get; } = currency;
}