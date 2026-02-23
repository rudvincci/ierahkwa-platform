using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Word;

public static class Extensions
{
    public static IServiceCollection AddWord(this IServiceCollection services)
    {
        services.AddScoped<IWordService, WordService>();
        return services;
    }
}