using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.ApplicationName.Modules.Raffles.Api")]
namespace Mamey.ApplicationName.Modules.Raffles.Infrastructure;

internal static class Extensions
{
    public static IServiceCollection AddRafflesCore(this IServiceCollection services)
    {
        return services;
    }
}
