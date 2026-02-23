using Mamey.Exceptions;

namespace Mamey.ISO.ISO4217;

public class CurrencyNotFoundException : MameyException
{
    public CurrencyNotFoundException(string currency)
        : base($"Currency '{currency}' does not exist")
        => Currency = currency;

    public string Currency { get; set; }
}

