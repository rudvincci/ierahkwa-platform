using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mamey.Security;
using Mamey.Security.Internals;
using Mamey.Security.Tests.Shared.Helpers;
using Mamey.Security.Tests.Shared.Utilities;

namespace Mamey.Security.Tests.Shared.Fixtures;

/// <summary>
/// Test fixture for security-related tests.
/// </summary>
public class SecurityTestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; }
    public SecurityOptions SecurityOptions { get; }
    public IEncryptor Encryptor { get; }
    public IHasher Hasher { get; }
    public IRng Rng { get; }
    public ISigner Signer { get; }
    public IMd5 Md5 { get; }
    public ISecurityProvider SecurityProvider { get; }

    public SecurityTestFixture() : this(true)
    {
    }

    internal SecurityTestFixture(bool encryptionEnabled)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

        SecurityOptions = encryptionEnabled
            ? TestConfiguration.CreateSecurityOptionsWithEncryption()
            : TestConfiguration.CreateSecurityOptionsWithoutEncryption();

        services.AddSingleton(SecurityOptions);

        var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        var encryptorLogger = loggerFactory.CreateLogger<Encryptor>();

        Encryptor = new Encryptor(encryptorLogger);
        Hasher = new Hasher();
        Rng = new Rng();
        Signer = new Signer();
        Md5 = new Md5();

        services.AddSingleton<IEncryptor>(Encryptor);
        services.AddSingleton<IHasher>(Hasher);
        services.AddSingleton<IRng>(Rng);
        services.AddSingleton<ISigner>(Signer);
        services.AddSingleton<IMd5>(Md5);
        services.AddSingleton<ISecurityProvider>(sp =>
        {
            var encryptor = sp.GetRequiredService<IEncryptor>();
            var hasher = sp.GetRequiredService<IHasher>();
            var rng = sp.GetRequiredService<IRng>();
            var signer = sp.GetRequiredService<ISigner>();
            var md5 = sp.GetRequiredService<IMd5>();
            var options = sp.GetRequiredService<SecurityOptions>();
            return new SecurityProvider(encryptor, hasher, rng, signer, md5, options);
        });

        ServiceProvider = services.BuildServiceProvider();
        SecurityProvider = ServiceProvider.GetRequiredService<ISecurityProvider>();
    }

    public void Dispose()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}

