using System.Collections.Concurrent;
using Mamey.Types;

namespace Mamey;

public interface IMameyBuilder
{
    IServiceCollection Services { get; }
    IConfiguration Configuration { get; }
    bool TryRegister(string name);
    void AddBuildAction(Action<IServiceProvider> execute);
    void AddInitializer(IInitializer initializer);
    void AddInitializer<TInitializer>() where TInitializer : IInitializer;
    IServiceProvider Build();

}
public sealed class MameyBuilder : IMameyBuilder
{
    private readonly ConcurrentDictionary<string, bool> _registry = new();
    private readonly List<Action<IServiceProvider>> _buildActions;
    private readonly IServiceCollection _services;
    IServiceCollection IMameyBuilder.Services => _services;

    public IConfiguration Configuration { get; }

    private MameyBuilder(IServiceCollection services, IConfiguration? configuration = null)
    {
        _buildActions = new List<Action<IServiceProvider>>();
        _services = services;
        _services.AddSingleton<IStartupInitializer>(new StartupInitializer());

        Configuration = configuration is null ? BuildConfigurationRoot() : configuration;
    }

    public static IMameyBuilder Create(IServiceCollection services, IConfiguration? configuration = null)
        => new MameyBuilder(services, configuration);

    public bool TryRegister(string name) => _registry.TryAdd(name, true);

    public void AddBuildAction(Action<IServiceProvider> execute)
        => _buildActions.Add(execute);

    public void AddInitializer(IInitializer initializer)
        => AddBuildAction(sp =>
        {
            var startupInitializer = sp.GetRequiredService<IStartupInitializer>();
            startupInitializer.AddInitializer(initializer);
        });

    public void AddInitializer<TInitializer>() where TInitializer : IInitializer
        => AddBuildAction(sp =>
        {
            var initializer = sp.GetRequiredService<TInitializer>();
            var startupInitializer = sp.GetRequiredService<IStartupInitializer>();
            startupInitializer.AddInitializer(initializer);
        });

    public IServiceProvider Build()
    {
        var serviceProvider = _services.BuildServiceProvider();
    
        _buildActions.ForEach(a => a(serviceProvider));
        ;
        return serviceProvider;
    }

    private static IConfigurationRoot BuildConfigurationRoot()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower();

        return new ConfigurationBuilder()
                        .SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{environment}.json", optional: true)
                        .AddEnvironmentVariables()
                        .Build();
    }
}