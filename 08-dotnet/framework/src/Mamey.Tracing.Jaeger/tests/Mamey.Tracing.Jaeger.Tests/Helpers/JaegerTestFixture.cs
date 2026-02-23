using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;
using Mamey.Tracing.Jaeger;
using Mamey.OpenTracingContrib;
using Mamey;

namespace Mamey.Tracing.Jaeger.Tests.Helpers;

public class JaegerTestFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly HttpClient _jaegerApiClient;
    private readonly ITracer _tracer;

    public JaegerTestFixture()
    {
        // Setup services
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        
            // Create MameyBuilder and configure Jaeger
            var mameyBuilder = MameyBuilder.Create(services);
            mameyBuilder.AddJaeger(options =>
            {
                return options.Enable(true)
                              .WithServiceName("test-service")
                              .WithSampler("const")
                              .WithSamplingRate(1.0);
            });
            services.AddOpenTracing();

        _serviceProvider = (ServiceProvider)mameyBuilder.Build();
        _tracer = _serviceProvider.GetRequiredService<ITracer>();
        _jaegerApiClient = new HttpClient { BaseAddress = new Uri("http://localhost:16686") };
    }

    public ITracer Tracer => _tracer;
    public HttpClient JaegerApiClient => _jaegerApiClient;

    public async Task<bool> WaitForTraceAsync(string serviceName, TimeSpan timeout)
    {
        var startTime = DateTime.UtcNow;
        while (DateTime.UtcNow - startTime < timeout)
        {
            try
            {
                var response = await _jaegerApiClient.GetFromJsonAsync<JaegerTracesResponse>(
                    $"/api/traces?service={serviceName}&limit=20");
                
                if (response?.Data?.Any() == true)
                {
                    return true;
                }
            }
            catch (HttpRequestException)
            {
                // Jaeger might not be ready yet
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        return false;
    }

    public async Task<List<JaegerTrace>?> GetTracesAsync(string serviceName)
    {
        try
        {
            var response = await _jaegerApiClient.GetFromJsonAsync<JaegerTracesResponse>(
                $"/api/traces?service={serviceName}&limit=20");
            
            return response?.Data;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<JaegerTrace?> GetTraceAsync(string serviceName)
    {
        var traces = await GetTracesAsync(serviceName);
        return traces?.FirstOrDefault();
    }

    public async Task<List<string>> GetServicesAsync()
    {
        try
        {
            var response = await _jaegerApiClient.GetFromJsonAsync<JaegerServicesResponse>("/api/services");
            return response?.Data ?? new List<string>();
        }
        catch (HttpRequestException)
        {
            return new List<string>();
        }
    }

    public async Task CleanupTracesAsync(string serviceName)
    {
        try
        {
            // Note: Jaeger doesn't have a direct API to delete traces
            // They will expire based on the retention policy
            await Task.CompletedTask;
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    public async Task FlushTracerAsync()
    {
        try
        {
            // Force flush the tracer by disposing and recreating it
            // This ensures all pending spans are sent to Jaeger
            if (_tracer is IDisposable disposableTracer)
            {
                disposableTracer.Dispose();
            }
            
            // Wait a bit for the flush to complete
            await Task.Delay(2000);
        }
        catch
        {
            // Ignore flush errors
        }
    }

    public void Dispose()
    {
        _jaegerApiClient?.Dispose();
        _serviceProvider?.Dispose();
    }
}
