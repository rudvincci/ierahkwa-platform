//using System.ComponentModel;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using Microsoft.Extensions.Http;
//using Mamey;
//using Microsoft.AspNetCore.Http;

using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace Mamey.Http;

public static class Extensions
{
    private const string SectionName = "httpClient";
    private const string RegistryName = "http.client";

    public static IMameyBuilder AddHttpClient<T>(this IMameyBuilder builder, string clientName = "Mamey",
        IEnumerable<string> maskedRequestUrlParts = null, string sectionName = SectionName,
        Action<IHttpClientBuilder> httpClientBuilder = null)
        where T : class, IHttpClient
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        if (string.IsNullOrWhiteSpace(clientName))
        {
            throw new ArgumentException("HTTP client name cannot be empty.", nameof(clientName));
        }

        var options = builder.GetOptions<HttpClientOptions>(sectionName);
        if (maskedRequestUrlParts is not null && options.RequestMasking is not null)
        {
            options.RequestMasking.UrlParts = maskedRequestUrlParts;
        }

        bool registerCorrelationContextFactory;
        bool registerCorrelationIdFactory;
        using (var scope = builder.Services.BuildServiceProvider().CreateScope())
        {
            registerCorrelationContextFactory = scope.ServiceProvider.GetService<ICorrelationContextFactory>() is null;
            registerCorrelationIdFactory = scope.ServiceProvider.GetService<ICorrelationIdFactory>() is null;
        }

        if (registerCorrelationContextFactory)
        {
            builder.Services.AddSingleton<ICorrelationContextFactory, EmptyCorrelationContextFactory>();
        }

        if (registerCorrelationIdFactory)
        {
            builder.Services.AddSingleton<ICorrelationIdFactory, EmptyCorrelationIdFactory>();
        }
        builder.Services.AddScoped<IApiResponseHandler, ApiResponseHandler>();
        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<IHttpClientSerializer, SystemTextJsonHttpClientSerializer>();
        var clientBuilder = builder.Services.AddHttpClient<IHttpClient, T>(clientName);
        httpClientBuilder?.Invoke(clientBuilder);

        if (options.RequestMasking?.Enabled == true)
        {
            builder.Services.Replace(ServiceDescriptor
                .Singleton<IHttpMessageHandlerBuilderFilter, MameyHttpLoggingFilter>());
        }

        return builder;
    }

    public static IMameyBuilder AddGenericHttpClient<T>(this IMameyBuilder builder, string clientName = "Mamey",
        IEnumerable<string> maskedRequestUrlParts = null, string sectionName = SectionName,
        Action<IHttpClientBuilder> httpClientBuilder = null, Action<HttpClient> configureClient = null)
        where T : MameyHttpClient
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        if (string.IsNullOrWhiteSpace(clientName))
        {
            throw new ArgumentException("HTTP client name cannot be empty.", nameof(clientName));
        }

        var options = builder.GetOptions<HttpClientOptions>(sectionName);
        if (maskedRequestUrlParts is not null && options.RequestMasking is not null)
        {
            options.RequestMasking.UrlParts = maskedRequestUrlParts;
        }

        bool registerCorrelationContextFactory;
        bool registerCorrelationIdFactory;
        using (var scope = builder.Services.BuildServiceProvider().CreateScope())
        {
            registerCorrelationContextFactory = scope.ServiceProvider.GetService<ICorrelationContextFactory>() is null;
            registerCorrelationIdFactory = scope.ServiceProvider.GetService<ICorrelationIdFactory>() is null;
        }

        if (registerCorrelationContextFactory)
        {
            builder.Services.AddSingleton<ICorrelationContextFactory, EmptyCorrelationContextFactory>();
        }

        if (registerCorrelationIdFactory)
        {
            builder.Services.AddSingleton<ICorrelationIdFactory, EmptyCorrelationIdFactory>();
        }

        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<IHttpClientSerializer, SystemTextJsonHttpClientSerializer>();
        var clientBuilder = builder.Services.AddHttpClient<IHttpClient, T>(clientName, configureClient);
        httpClientBuilder?.Invoke(clientBuilder);

        if (options.RequestMasking?.Enabled == true)
        {
            builder.Services.Replace(ServiceDescriptor
                .Singleton<IHttpMessageHandlerBuilderFilter, MameyHttpLoggingFilter>());
        }
        builder.Services.AddScoped<IApiResponseHandler, ApiResponseHandler>();

        return builder;
    }

    public static IMameyBuilder AddHttpClient(this IMameyBuilder builder, string clientName = "Mamey",
        IEnumerable<string> maskedRequestUrlParts = null, string sectionName = SectionName,
        Action<IHttpClientBuilder> httpClientBuilder = null)
    {
        AddHttpClient<MameyMicroserviceHttpClient>(builder, clientName, maskedRequestUrlParts, sectionName, httpClientBuilder);
        

        return builder;
    }

    [Description("This is a hack related to HttpClient issue: https://github.com/aspnet/AspNetCore/issues/13346")]
    public static void RemoveHttpClient(this IMameyBuilder builder)
    {
        var registryType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
            .SingleOrDefault(t => t.Name == "HttpClientMappingRegistry");
        var registry = builder.Services.SingleOrDefault(s => s.ServiceType == registryType)?.ImplementationInstance;
        var registrations = registry?.GetType().GetProperty("TypedClientRegistrations");
        var clientRegistrations = registrations?.GetValue(registry) as IDictionary<Type, string>;
        clientRegistrations?.Remove(typeof(IHttpClient));
    }
    [Description("This is a hack related to HttpClient issue: https://github.com/aspnet/AspNetCore/issues/13346")]
    public static void RemoveHttpClient<T>(this IMameyBuilder builder)
        where T : MameyMicroserviceHttpClient
    {
        var registryType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
            .SingleOrDefault(t => t.Name == "HttpClientMappingRegistry");
        var registry = builder.Services.SingleOrDefault(s => s.ServiceType == registryType)?.ImplementationInstance;
        var registrations = registry?.GetType().GetProperty("TypedClientRegistrations");
        var clientRegistrations = registrations?.GetValue(registry) as IDictionary<Type, string>;
        clientRegistrations?.Remove(typeof(T));
    }
    public static string ToQueryString<T>(this T obj) where T : class
    {
        var objectDictionary = obj.ToKeyValuePairList<T>();
        var queryObjects = (from kv in objectDictionary
                            where !string.IsNullOrEmpty(kv.Value)
                            select kv);
        return QueryString.Create(queryObjects).ToString();
    }
}
