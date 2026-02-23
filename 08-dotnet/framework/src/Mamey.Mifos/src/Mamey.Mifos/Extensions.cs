using Mamey.Mifos.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Mifos
{
    public static class Extensions
    {
        public static string SectionName = "mifos";
        public static IServiceCollection AddMifos(this IServiceCollection services, string? sectionName = null)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = SectionName;
            }

            var mifosOptions = services.GetOptions<MifosOptions>(sectionName);
            services.AddSingleton(mifosOptions);

            services.AddTransient<IMifosClientService, MifosClientService>();
            services.AddTransient<IMifosLoansService, MifosLoansService>();
            services.AddTransient<IMifosOfficesService, MifosOfficesService>();
            services.AddTransient<IMifosSavingsAccountService, MifosSavingsAccountService>();
            services.AddSingleton<IMifosApiClient, MifosApiClient>();


            return services;
        }
    }
}

