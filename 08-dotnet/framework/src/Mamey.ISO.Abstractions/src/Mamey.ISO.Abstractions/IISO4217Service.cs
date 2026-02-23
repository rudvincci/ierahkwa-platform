using System.Threading.Tasks;

namespace Mamey.ISO.Abstractions;

public interface IISO4217Service
{
    Task InitializeAsync();
    Task<IISOCurrency?> GetCurrencyAsync(string currencyCode);
    IISOCurrency GetCurrency(string currencyCode);
}
