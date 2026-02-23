namespace Mamey.ISO.Abstractions
{
    public interface IISOCurrency
    {
        string Entity { get; }
        string Currency { get; }
        string AlphabeticCode { get; }
        string MinorUnit { get; }
        string? Fund { get; }
    }
}