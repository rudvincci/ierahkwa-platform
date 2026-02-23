using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pupitre.Blockchain.Options;
using Pupitre.Blockchain.Services;
using Mamey.Blockchain.Government;
using Mamey.Blockchain.LedgerIntegration;
using Mamey.Blockchain.Node;

namespace Pupitre.Blockchain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPupitreBlockchain(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection("MameyNode");
        services.Configure<MameyNodeOptions>(section);

        services.AddMameyNodeClient(options =>
        {
            options.NodeUrl = section.GetValue<string>("NodeUrl") ?? "http://localhost:50051";
        });

        services.AddMameyLedgerClient(options =>
        {
            options.NodeUrl = section.GetValue<string>("NodeUrl") ?? "http://localhost:50051";
        });

        services.AddSingleton(provider =>
        {
            var nodeOptions = provider.GetRequiredService<IOptions<MameyNodeOptions>>().Value;
            var nodeUri = new Uri(nodeOptions.NodeUrl);
            var host = string.IsNullOrWhiteSpace(nodeOptions.GovernmentHost)
                ? nodeUri.Host
                : nodeOptions.GovernmentHost;
            var port = nodeOptions.GovernmentPort > 0 ? nodeOptions.GovernmentPort : nodeUri.Port;

            var governmentOptions = new GovernmentClientOptions
            {
                Host = host,
                Port = port
            };
            var logger = provider.GetService<ILogger<GovernmentClient>>();
            return new GovernmentClient(governmentOptions, logger);
        });

        services.AddSingleton<IEducationLedgerService, EducationLedgerService>();

        return services;
    }
}
