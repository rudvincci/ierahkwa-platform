using Mamey.MicroMonolith.Abstractions.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.MicroMonolith.Infrastructure.Contexts;

public static class Extensions
{
    public static IServiceCollection AddContext(this IServiceCollection services)
    {
        services.AddSingleton<ContextAccessor>();
        services.AddTransient<IContext>(sp => sp.GetRequiredService<ContextAccessor>().Context);
            
        return services;
    }

    public static IApplicationBuilder UseContext(this IApplicationBuilder app)
    {
        app.Use((ctx, next) =>
        {
            ctx.RequestServices.GetRequiredService<ContextAccessor>().Context = new Context(ctx);;
                
            return next();
        });
            
        return app;
    }
}