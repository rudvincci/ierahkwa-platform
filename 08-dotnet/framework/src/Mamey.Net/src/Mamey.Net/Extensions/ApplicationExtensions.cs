using Figgle.Fonts;
using Mamey.Types;
using Microsoft.AspNetCore.Builder;

namespace Mamey;

public static class ApplicationExtensions
{
    private const string SectionName = "app";

    public static IMameyBuilder AddMameyNet(this IMameyBuilder builder)
    {
        var options = builder.Services.GetOptions<AppOptions>("app");
        if (!options.DisplayBanner || string.IsNullOrWhiteSpace(options.Name))
        {
            return builder;
        }

        var version = options.DisplayVersion ? $" {options.Version}" : string.Empty;
        Console.WriteLine(FiggleFonts.Doom.Render($"{options.Name}{version}"));

        return builder;
    }

    public static IApplicationBuilder UseMamey(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IStartupInitializer>();
        Task.Run(() => initializer.InitializeAsync()).GetAwaiter().GetResult();

        return app;
    }


}

