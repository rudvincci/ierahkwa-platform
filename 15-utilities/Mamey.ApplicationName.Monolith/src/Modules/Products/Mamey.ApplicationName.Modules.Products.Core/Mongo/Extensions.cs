// using System;
// using Mamey.ApplicationName.Modules.Products.Core.Mongo.Documents;
// using Mamey.ApplicationName.Modules.Products.Core.Mongo.Repositories;
// using Mamey.ApplicationName.Modules.Products.Core.Repositories;
// using Mamey.MicroMonolith.Infrastructure.Mongo;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace Mamey.ApplicationName.Modules.Products.Core.Mongo
// {
//     internal static class Extensions
//     {
//         private static readonly string Schema = "banking-products-module";
//
//         public static IServiceCollection AddMongo(this IServiceCollection services)
//         {
//             services.AddScoped<IBankingProductRepository, BankingProductMongoRepository>();
//             services.AddMongoRepository<BankingProductDocument, Guid>($"{Schema}.banking-products");
//             return services;
//         }
//     }
// }
