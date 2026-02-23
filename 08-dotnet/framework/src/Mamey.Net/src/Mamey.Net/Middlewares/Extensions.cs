using Microsoft.AspNetCore.Builder;

namespace Mamey.Net.Middlewares;

public static class Extensions
{
    public static IMameyBuilder AddCorrelationContextLogging(this IMameyBuilder builder)
    {
        builder.Services.AddTransient<CorrelationContextLoggingMiddleware>();

        return builder;
    }

    public static IApplicationBuilder UserCorrelationContextLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationContextLoggingMiddleware>();

        return app;
    }
}