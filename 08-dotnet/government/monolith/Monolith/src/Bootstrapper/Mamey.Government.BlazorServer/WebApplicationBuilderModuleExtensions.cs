namespace Mamey.Government.BlazorServer;

public static class WebApplicationBuilderModuleExtensions
{
    public static WebApplicationBuilder AddModuleSettings(this WebApplicationBuilder builder)
    {
        var env = builder.Environment;
        var root = env.ContentRootPath;

        foreach (var file in Directory.EnumerateFiles(root, "module.*.json", SearchOption.AllDirectories))
            builder.Configuration.AddJsonFile(file, optional: true, reloadOnChange: true);

        foreach (var file in Directory.EnumerateFiles(root, $"module.*.{env.EnvironmentName}.json", SearchOption.AllDirectories))
            builder.Configuration.AddJsonFile(file, optional: true, reloadOnChange: true);

        return builder;
    }
}