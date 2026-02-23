using System.Collections.Concurrent;
using System.Text.Json;
using System.Reflection;
using Mamey.ISO.Abstractions;

namespace Mamey.ISO.ISO4217;

public class ISO4217Service : IISO4217Service
{
    private ConcurrentDictionary<string, IISOCurrency> _currencies = new ConcurrentDictionary<string, IISOCurrency>();
    public ISO4217Service()
    {

    }
    public async Task InitializeAsync()
        => await Task.WhenAll(LoadCurrenciesAsync());

    // TODO: Change to Redis
    private Task LoadCurrenciesAsync() => Task.FromResult(LoadCurrencies);

    private async void LoadCurrencies()
    {
        var resourceName = $"Mamey.ISO.ISO4217.Data.iso4217-20230101";
        var assembly = typeof(ISOCurrency).GetTypeInfo().Assembly;
        var resources = assembly?.GetManifestResourceNames();

        var jsonResources = resources?.Where(c => c.StartsWith(resourceName) && c.EndsWith(".json"));
        if (jsonResources is not null && jsonResources.Any())
        {
            foreach (var resource in jsonResources)
            {
                Stream? stream = assembly?.GetManifestResourceStream(resource);
                if (stream is not null)
                {
                    var currencyKV = JsonSerializer.Deserialize<Dictionary<string, IISOCurrency>>(stream,
                        JsonExtensions.SerializerOptions);
                    if (currencyKV is not null && currencyKV.Any())
                    {
                        _currencies.TryAdd(currencyKV.First().Value.AlphabeticCode, currencyKV.First().Value);
                    }
                }
            }
        }
    }
    
    public Task<IISOCurrency?> GetCurrencyAsync(string currencyCode)
        => Task.FromResult(GetCurrency(currencyCode));
    public IISOCurrency? GetCurrency(string currencyCode)
    {
        if (string.IsNullOrEmpty(currencyCode))
        {
            throw new ArgumentNullException(currencyCode);
        }
        _currencies.TryGetValue(currencyCode, out IISOCurrency? currencyInfo);
        return currencyInfo;
    }
}
