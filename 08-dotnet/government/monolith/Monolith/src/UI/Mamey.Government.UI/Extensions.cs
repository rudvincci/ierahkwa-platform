using Mamey.Government.UI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.UI;

/// <summary>
/// Extension methods for registering UI services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds Government UI services to the service collection for Blazor Server.
    /// Uses IHttpContextAccessor to get the current request's base URL.
    /// </summary>
    public static IServiceCollection AddGovernmentUIServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        // Configure typed HttpClients that will use the current request's base URL
        services.AddHttpClient<ICitizensService, CitizensService>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                // Allow self-signed certs in development
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddHttpMessageHandler<BaseAddressHandler>();

        services.AddHttpClient<IApplicationsService, ApplicationsService>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddHttpMessageHandler<BaseAddressHandler>();

        services.AddHttpClient<IPassportsService, PassportsService>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddHttpMessageHandler<BaseAddressHandler>();

        services.AddHttpClient<ITravelIdentitiesService, TravelIdentitiesService>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddHttpMessageHandler<BaseAddressHandler>();

        services.AddHttpClient<ICertificatesService, CertificatesService>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddHttpMessageHandler<BaseAddressHandler>();

        // Register the base address handler
        services.AddTransient<BaseAddressHandler>();

        // Dashboard service uses other services, so it needs its own HttpClient
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }

    /// <summary>
    /// Adds Government UI services with a fixed base URL.
    /// </summary>
    public static IServiceCollection AddGovernmentUIServices(this IServiceCollection services, string baseUrl)
    {
        services.AddHttpContextAccessor();

        services.AddHttpClient<ICitizensService, CitizensService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        })
        .AddHttpMessageHandler<CookieForwardingHandler>();

        services.AddHttpClient<IApplicationsService, ApplicationsService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        })
        .AddHttpMessageHandler<CookieForwardingHandler>();

        services.AddHttpClient<IPassportsService, PassportsService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        })
        .AddHttpMessageHandler<CookieForwardingHandler>();

        services.AddHttpClient<ITravelIdentitiesService, TravelIdentitiesService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        })
        .AddHttpMessageHandler<CookieForwardingHandler>();

        services.AddHttpClient<ICertificatesService, CertificatesService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        })
        .AddHttpMessageHandler<CookieForwardingHandler>();

        // Register the cookie forwarding handler
        services.AddTransient<CookieForwardingHandler>();

        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}

/// <summary>
/// Handler that sets the base address from the current HTTP context.
/// </summary>
public class BaseAddressHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BaseAddressHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null && request.RequestUri != null && !request.RequestUri.IsAbsoluteUri)
        {
            var baseUri = new Uri($"{context.Request.Scheme}://{context.Request.Host}");
            request.RequestUri = new Uri(baseUri, request.RequestUri);
        }
        
        // Forward authentication cookie
        if (context?.Request.Cookies.TryGetValue(".AspNetCore.Cookies", out var cookie) == true)
        {
            request.Headers.Add("Cookie", $".AspNetCore.Cookies={cookie}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

/// <summary>
/// Handler that forwards all cookies from the current HTTP context to the HttpClient request.
/// </summary>
public class CookieForwardingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieForwardingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        
        // Forward all cookies from the current request
        if (context?.Request.Headers.TryGetValue("Cookie", out var cookieHeader) == true)
        {
            request.Headers.Add("Cookie", cookieHeader.ToString());
        }
        else if (context?.Request.Cookies.Count > 0)
        {
            // Build cookie header from cookie collection
            var cookieStrings = context.Request.Cookies
                .Select(c => $"{c.Key}={c.Value}")
                .ToArray();
            
            if (cookieStrings.Length > 0)
            {
                request.Headers.Add("Cookie", string.Join("; ", cookieStrings));
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
