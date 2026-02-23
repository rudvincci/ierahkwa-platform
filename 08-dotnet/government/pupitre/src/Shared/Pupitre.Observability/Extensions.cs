using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Pupitre.Observability;

public static class Extensions
{
    /// <summary>
    /// Adds Pupitre observability with OpenTelemetry tracing and metrics.
    /// </summary>
    public static IServiceCollection AddPupitreObservability(
        this IServiceCollection services,
        string serviceName,
        string serviceVersion = "1.0.0",
        string? jaegerEndpoint = null)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: serviceName,
                    serviceVersion: serviceVersion,
                    serviceInstanceId: Environment.MachineName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.Filter = httpContext =>
                            !httpContext.Request.Path.StartsWithSegments("/health");
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })
                    .AddSource(serviceName);

                if (!string.IsNullOrEmpty(jaegerEndpoint))
                {
                    tracing.AddJaegerExporter(options =>
                    {
                        options.AgentHost = jaegerEndpoint;
                        options.AgentPort = 6831;
                    });
                }
                else
                {
                    tracing.AddJaegerExporter();
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMeter(serviceName)
                    .AddPrometheusExporter();
            });

        return services;
    }

    /// <summary>
    /// Adds Pupitre observability with custom configuration.
    /// </summary>
    public static IServiceCollection AddPupitreObservability(
        this IServiceCollection services,
        Action<ObservabilityOptions> configure)
    {
        var options = new ObservabilityOptions();
        configure(options);

        return services.AddPupitreObservability(
            options.ServiceName,
            options.ServiceVersion,
            options.JaegerEndpoint);
    }
}

public class ObservabilityOptions
{
    public string ServiceName { get; set; } = "pupitre-service";
    public string ServiceVersion { get; set; } = "1.0.0";
    public string? JaegerEndpoint { get; set; }
    public bool EnablePrometheus { get; set; } = true;
    public bool EnableJaeger { get; set; } = true;
}
