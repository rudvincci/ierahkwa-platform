using Mamey.Barcode.Http;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Http;

namespace Mamey.Barcode;

public static class Extensions
{
    public static IMameyBuilder AddBarcode(this IMameyBuilder builder)
    {
        builder.Services.AddBarcode();
        return builder
            .AddGenericHttpClient<MameyBarcodeApiClient>("MameyBarcodeApiClient", configureClient: config =>
            {
                config.BaseAddress = new Uri("http://localhost:18648");
            }); 
    }
	private static IServiceCollection AddBarcode(this IServiceCollection services)
	{
        var options = services.GetOptions<BarcodeOptions>("app");
        //var httpClientOptions = services.GetOptions<HttpClientOptions>("httpClient");

        services
            .AddSingleton(options);

        services.AddScoped<IMameyBarcodeApiClientResponseHandler, MameyBarcodeApiClientResponseHandler>();
        services.AddScoped<MameyBarcodeApiClient>();
        services.AddScoped<IMameyBarcodeApiClient, MameyBarcodeApiClient>();
        services.AddScoped<IBarcodeService, BarcodeService>();
        
        return services;
	}
}
