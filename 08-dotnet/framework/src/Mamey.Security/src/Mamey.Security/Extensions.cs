using Mamey.Security.Internals;
using Mamey.Security.PostQuantum;
using Mamey.Security.PostQuantum.Extensions;
using Mamey.Security.PostQuantum.Interfaces;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Mamey.Security;

public static class Extensions
{
    public static IMameyBuilder AddSecurity(this IMameyBuilder builder)
    {
        builder.Services.Configure();
        return builder;
    }

    private static IServiceCollection Configure(this IServiceCollection services)
    {
        var securityOptions = services.GetOptions<SecurityOptions>("security");
        using (var serviceProvider = services.BuildServiceProvider())
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ISecurityProvider>>();
            logger.LogInformation(securityOptions.Encryption.Enabled
                ? "AES-256 data encryption is enabled."
                : "Data encryption is disabled.");
        }

        if (securityOptions.Encryption.Enabled)
        {
            if (string.IsNullOrWhiteSpace(securityOptions.Encryption.Key))
            {
                throw new ArgumentException("Empty encryption key.", nameof(securityOptions.Encryption.Key));
            }

            var keyLength = securityOptions.Encryption.Key.Length;
            if (keyLength != 32)
            {
                throw new ArgumentException($"Invalid encryption key length: {keyLength} (required: 32 chars).",
                    nameof(securityOptions.Encryption.Key));
            }

            services
                .AddSingleton<IEncryptor, Encryptor>()
                //.AddSingleton<ICertificateProvider<IPrivateKey>, CertificateProvider<IPrivateKey>>()
                .AddCertificateGenerators()
                ;
        }
        else
        {
            // Register a dummy encryptor when encryption is disabled.
            // SecurityProvider requires IEncryptor but won't use it when encryption is disabled.
            using (var tempProvider = services.BuildServiceProvider())
            {
                var loggerFactory = tempProvider.GetRequiredService<ILoggerFactory>();
                var encryptorLogger = loggerFactory.CreateLogger<Encryptor>();
                services.AddSingleton<IEncryptor>(new Encryptor(encryptorLogger));
            }
        }

        // Register post-quantum security services (ML-DSA, ML-KEM, hybrid).
        services.AddPostQuantumSecurity();

        // Always register ISecurityProvider, even when encryption is disabled.
        services.AddSingleton<ISecurityProvider>(sp =>
        {
            var encryptor = sp.GetRequiredService<IEncryptor>();
            var hasher = sp.GetRequiredService<IHasher>();
            var rng = sp.GetRequiredService<IRng>();
            var signer = sp.GetRequiredService<ISigner>();
            var md5 = sp.GetRequiredService<IMd5>();
            var options = sp.GetRequiredService<SecurityOptions>();
            var pqSigner = sp.GetService<IPQSigner>();
            var pqKeyGenerator = sp.GetService<IPQKeyGenerator>();
            var pqEncryptor = sp.GetService<IPQEncryptor>();

            return new SecurityProvider(encryptor, hasher, rng, signer, md5, options, pqSigner, pqKeyGenerator, pqEncryptor);
        });
        services.AddScoped<IPrivateKeyService, PrivateKeyService>();
        return services
            .AddSingleton(securityOptions)
            .AddSingleton<IMd5, Md5>()
            .AddSingleton<IRng, Rng>()
            .AddSingleton<IHasher, Hasher>()
            .AddSingleton<ISigner, Signer>()
            .AddScoped<SecurityAttributeProcessor>()
            ;

    }

    /// <summary>
    /// Configures JsonSerializerOptions to automatically handle [EncryptedAttribute] and [HashedAttribute].
    /// </summary>
    /// <param name="options">The JsonSerializerOptions to configure.</param>
    /// <param name="serviceProvider">The service provider to resolve ISecurityProvider.</param>
    /// <returns>The JsonSerializerOptions instance for chaining.</returns>
    public static JsonSerializerOptions AddSecurityConverters(this JsonSerializerOptions options, IServiceProvider serviceProvider)
    {
        var securityProvider = serviceProvider.GetRequiredService<ISecurityProvider>();
        return JsonSerializerExtensions.AddSecurityConverters(options, securityProvider);
    }
    private static IServiceCollection AddCertificateGenerators(this IServiceCollection services)
    {
        //services.Scan(s =>
        //    s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
        //        .AddClasses(c =>
        //        {
        //c.AssignableTo(typeof(ICertificateProvider<>));
        //c.AssignableTo(typeof(IPrivateKeyGenerator<,>));
        //c.AssignableTo(typeof(ICertificateGenerator<,>));

        //        })
        //        .AsMatchingInterface()
        //        .WithSingletonLifetime()
        //    );
        services.Scan(s =>
            s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c =>
                {
                    c.AssignableTo(typeof(ICertificateProvider<>));
                    c.AssignableTo(typeof(IPrivateKeyGenerator<,>));
                    c.AssignableTo(typeof(ICertificateGenerator<,>));
                    c.WithoutAttribute(typeof(DecoratorAttribute));
                })

                .AsImplementedInterfaces()
                .WithSingletonLifetime()
            );
        return services;
    }
}