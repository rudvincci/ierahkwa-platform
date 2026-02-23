using Mamey.ISO.Abstractions;

namespace Mamey.ISO.ISO4217;

public class ISOCurrency : IISOCurrency
{
    public ISOCurrency()
    {
    }
    public string Entity { get; private set; }
    public string Currency { get; private set; }
    public string AlphabeticCode { get; private set; }
    public string MinorUnit { get; private set; }
    public string? Fund { get; private set; }

}
