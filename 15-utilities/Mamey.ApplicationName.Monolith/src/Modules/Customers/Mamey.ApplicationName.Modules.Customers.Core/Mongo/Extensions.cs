// using System;

// using Mamey.MicroMonolith.Infrastructure.Mongo;
// using Microsoft.Extensions.DependencyInjection;

// namespace Mamey.ApplicationName.Modules.Customers.Core.Mongo
// {
//     internal static class Extensions
//     {
//         private static readonly string Schema = "customers-module";

//         public static IServiceCollection AddMongo(this IServiceCollection services)
//         {
//             services.AddScoped<ICustomersRepository, CustomersMongoRepository>();
//             services.AddMongoRepository<CustomerDocument, Guid>($"{Schema}.customers");
//             return services;
//         }
//     }
// }
