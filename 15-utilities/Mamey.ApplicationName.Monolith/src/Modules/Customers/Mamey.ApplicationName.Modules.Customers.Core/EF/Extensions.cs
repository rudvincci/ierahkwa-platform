using Mamey.ApplicationName.Modules.Customers.Core.Domain.Repositories;
using Mamey.Bank.Modules.Customers.Infrastructure.EF.Repositories;
using Mamey.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Bank.Modules.Customers.Infrastructure.EF
{
    public static class Extensions
    {
        public static IServiceCollection AddPostgres(this IServiceCollection services)
        {
            services
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddPostgres<CustomersDbContext>()
                .AddUnitOfWork<CustomersUnitOfWork>();

            return services;
        }
    }
}