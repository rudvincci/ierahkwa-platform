using Mamey.Types;

namespace Mamey.Exceptions;

public class UnsupportedCurrencyException : MameyException
{
    public UnsupportedCurrencyException(string currency) : base($"Currency: '{currency}' is unsupported.")
    {
        Currency = currency;
    }

    public UnsupportedCurrencyException(Currency currency) : this(currency.Code)
    {
    }
    public string Currency { get; }
}