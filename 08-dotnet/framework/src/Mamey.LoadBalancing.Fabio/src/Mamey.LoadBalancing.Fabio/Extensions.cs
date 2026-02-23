using Mamey.Discovery.Consul;
using Mamey.Discovery.Consul.Models;
using Mamey.Http;
using Mamey.LoadBalancing.Fabio.Builders;
using Mamey.LoadBalancing.Fabio.Http;
using Mamey.LoadBalancing.Fabio.MessageHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.LoadBalancing.Fabio;

public static class Extensions
{
    private const string SectionName = "fabio";
    private const string RegistryName = "loadBalancing.fabio";

    public static IMameyBuilder AddFabio(this IMameyBuilder builder, string sectionName = SectionName,
        string consulSectionName = "consul", string httpClientSectionName = "httpClient")
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }
            
        var fabioOptions = builder.GetOptions<FabioOptions>(sectionName);
        var consulOptions = builder.GetOptions<ConsulOptions>(consulSectionName);
        var httpClientOptions = builder.GetOptions<HttpClientOptions>(httpClientSectionName);
        return builder.AddFabio(fabioOptions, httpClientOptions,
            b => b.AddConsul(consulOptions, httpClientOptions));
    }

    public static IMameyBuilder AddFabio(this IMameyBuilder builder,
        Func<IFabioOptionsBuilder, IFabioOptionsBuilder> buildOptions,
        Func<IConsulOptionsBuilder, IConsulOptionsBuilder> buildConsulOptions,
        HttpClientOptions httpClientOptions)
    {
        var fabioOptions = buildOptions(new FabioOptionsBuilder()).Build();
        return builder.AddFabio(fabioOptions, httpClientOptions,
            b => b.AddConsul(buildConsulOptions, httpClientOptions));
    }

    public static IMameyBuilder AddFabio(this IMameyBuilder builder, FabioOptions fabioOptions,
        ConsulOptions consulOptions, HttpClientOptions httpClientOptions)
        => builder.AddFabio(fabioOptions, httpClientOptions, b => b.AddConsul(consulOptions, httpClientOptions));

    private static IMameyBuilder AddFabio(this IMameyBuilder builder, FabioOptions fabioOptions,
        HttpClientOptions httpClientOptions, Action<IMameyBuilder> registerConsul)
    {
        registerConsul(builder);
        builder.Services.AddSingleton(fabioOptions);

        if (!fabioOptions.Enabled || !builder.TryRegister(RegistryName))
        {
            return builder;
        }

        if (httpClientOptions.Type?.ToLowerInvariant() == "fabio")
        {
            builder.Services.AddTransient<FabioMessageHandler>();
            builder.Services.AddHttpClient<IFabioHttpClient, FabioHttpClient>("fabio-http")
                .AddHttpMessageHandler<FabioMessageHandler>();


            builder.RemoveHttpClient();
            builder.Services.AddHttpClient<IHttpClient, FabioHttpClient>("fabio")
                .AddHttpMessageHandler<FabioMessageHandler>();
        }

        using var serviceProvider = builder.Services.BuildServiceProvider();
        var registration = serviceProvider.GetRequiredService<ServiceRegistration>();
        var tags = GetFabioTags(registration.Name, fabioOptions.Service);
        if (registration.Tags is null)
        {
            registration.Tags = tags;
        }
        else
        {
            registration.Tags.AddRange(tags);
        }

        builder.Services.UpdateConsulRegistration(registration);

        return builder;
    }

    public static void AddFabioHttpClient(this IMameyBuilder builder, string clientName, string serviceName)
        => builder.Services.AddHttpClient<IHttpClient, FabioHttpClient>(clientName)
            .AddHttpMessageHandler(c => new FabioMessageHandler(c.GetRequiredService<FabioOptions>(), serviceName));

    private static void UpdateConsulRegistration(this IServiceCollection services,
        ServiceRegistration registration)
    {
        var serviceDescriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(ServiceRegistration));
        services.Remove(serviceDescriptor);
        services.AddSingleton(registration);
    }

    private static List<string> GetFabioTags(string consulService, string fabioService)
    {
        var service = (string.IsNullOrWhiteSpace(fabioService) ? consulService : fabioService)
            .ToLowerInvariant();

        return new List<string> {$"urlprefix-/{service} strip=/{service}"};
    }
}