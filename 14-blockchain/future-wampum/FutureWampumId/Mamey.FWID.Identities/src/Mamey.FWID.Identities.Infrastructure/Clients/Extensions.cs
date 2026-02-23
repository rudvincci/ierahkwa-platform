using System;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.FWID.Identities.Infrastructure.Clients
{
    internal static class Extensions
    {
        public static IServiceCollection AddServiceClients(this IServiceCollection services)
        {
            services.AddScoped<ISamplesServiceClient, SamplesServiceClient>();
            services.AddScoped<IDIDsServiceClient, DIDsServiceClient>();
            services.AddScoped<ICredentialsServiceClient, CredentialsServiceClient>();
            services.AddScoped<IZKPsServiceClient, ZKPsServiceClient>();
            services.AddScoped<IAccessControlsServiceClient, AccessControlsServiceClient>();
            
            // Register Biometric Client for integration with external Biometric Verification Microservice
            // Note: This is a placeholder implementation - actual gRPC/HTTP client would be implemented here
            services.AddScoped<IBiometricClient, BiometricClient>();
            services.AddOptions<BiometricClientOptions>()
                .Configure<IConfiguration>((options, configuration) =>
                {
                    configuration.GetSection("BiometricClient").Bind(options);
                });
            
            // Register Ledger Transaction Client for FutureWampumLedger.Transaction integration
            services.AddScoped<ILedgerTransactionClient, LedgerTransactionClient>();
            
            // Register MameyNode Banking Client for blockchain integration
            services.AddScoped<IMameyNodeBankingClient, MameyNodeBankingClient>();
            
            return services;
        }
    }
}

