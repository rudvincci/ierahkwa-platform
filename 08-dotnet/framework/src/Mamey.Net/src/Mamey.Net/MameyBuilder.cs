//using System.Collections.Concurrent;
//using Mamey;
//using Mamey.Types;

//namespace Mamey.Net;

//public sealed class MameyBuilder : IMameyBuilder
//{
//    private readonly ConcurrentDictionary<string, bool> _registry = new();
//    private readonly List<Action<IServiceProvider>> _buildActions;
//    private readonly IServiceCollection _services;
//    IServiceCollection IMameyBuilder.Services => _services;

//    public IConfiguration Configuration { get; }

//    private MameyBuilder(IServiceCollection services, IConfiguration configuration)
//    {
//        _buildActions = new List<Action<IServiceProvider>>();
//        _services = services;
//        _services.AddSingleton<IStartupInitializer>(new StartupInitializer());
//        Configuration = configuration;
//    }

//    public static IMameyBuilder Create(IServiceCollection services, IConfiguration configuration = null)
//        => new MameyBuilder(services, configuration);

//    public bool TryRegister(string name) => _registry.TryAdd(name, true);

//    public void AddBuildAction(Action<IServiceProvider> execute)
//        => _buildActions.Add(execute);

//    public void AddInitializer(IInitializer initializer)
//        => AddBuildAction(sp =>
//        {
//            var startupInitializer = sp.GetRequiredService<IStartupInitializer>();
//            startupInitializer.AddInitializer(initializer);
//        });

//    public void AddInitializer<TInitializer>() where TInitializer : IInitializer
//        => AddBuildAction(sp =>
//        {
//            var initializer = sp.GetRequiredService<TInitializer>();
//            var startupInitializer = sp.GetRequiredService<IStartupInitializer>();
//            startupInitializer.AddInitializer(initializer);
//        });

//    public IServiceProvider Build()
//    {
//        var serviceProvider = _services.BuildServiceProvider();
//        _buildActions.ForEach(a => a(serviceProvider));
//        return serviceProvider;
//    }
//}

