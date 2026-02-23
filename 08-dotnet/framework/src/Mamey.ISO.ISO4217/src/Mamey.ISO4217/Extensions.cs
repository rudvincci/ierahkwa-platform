using Mamey.ISO.Abstractions;
using Mamey.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ISO.ISO4217;

public static class Extensions
{
    public static IServiceCollection AddISO4217(this IServiceCollection services)
    {
        services.AddSingleton<IISO4217Service, ISO4217Service>();
        services.AddScoped<CurrencyConverter>();
        return services;
    }
    public static IApplicationBuilder UseISO4217(this IApplicationBuilder builder)
    {
        var iso3166Service = builder.ApplicationServices.GetRequiredService<IISO4217Service>();
        iso3166Service.InitializeAsync();

        return builder;
    }
    public static Money Convert(Money money, Currency targetCurrency, decimal exchangeRate)
    {
        if (money == null) throw new ArgumentNullException(nameof(money));
        if (exchangeRate <= 0) throw new ArgumentException("Exchange rate must be positive.", nameof(exchangeRate));

        var convertedAmount = money.Amount.Value * exchangeRate;
        return new Money(new Amount(convertedAmount), targetCurrency);
    }
    
}