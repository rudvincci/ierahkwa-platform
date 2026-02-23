using System;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Infrastructure.Services.Cleanup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Mamey.FWID.Identities.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddSingleton<IEventMapper, EventMapper>();
        
        // Register IdentityService (handlers delegate to this service for business logic and logging)
        services.AddScoped<IIdentityService, IdentityService>();
        
        // Register blockchain account service with caching
        services.AddScoped<IIdentityBlockchainAccountService, IdentityBlockchainAccountService>();
        
        // Configure cleanup options
        services.AddOptions<CleanupOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection("cleanup").Bind(options);
            });
        
        // Register cleanup background services
        services.AddHostedService<SessionCleanupService>();
        services.AddHostedService<EmailConfirmationCleanupService>();
        services.AddHostedService<SmsConfirmationCleanupService>();
        
        return services;
    }
}

