using Mamey.Stripe.Configuration;
using Mamey.Stripe.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Stripe;

public static class Extensions
{
    public static IServiceCollection AddStripe(this IServiceCollection services, StripeOptions options)
    {
        services.AddSingleton(options);
        services.AddScoped<IStripeService, StripeServiceBase>();
        return services;
    }
    //public static IApplicationBuilder UseStripe(this IApplicationBuilder app)
    //{
    //    return app
    //        .UseStripe();
    //}
}



