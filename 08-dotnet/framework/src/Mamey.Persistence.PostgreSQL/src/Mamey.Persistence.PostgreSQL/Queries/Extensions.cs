// using System.Reflection;
// using Mamey.CQRS.Queries;
// using Mamey.Persistence.SQL;
// using Mamey.Postgres;
// using Mamey.Postgres.Decorators;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace Mamey.Persistence.PostgresSQL.Queries;
//
// public static class Extensions
// {
//     public static IServiceCollection AddQueries(this IServiceCollection services, IEnumerable<Assembly> assemblies)
//     {
//         services.AddSingleton<IQueryDispatcher, QueryDispatcher>();
//         services.Scan(s => s.FromAssemblies(assemblies)
//             .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>))
//                 .WithoutAttribute<DecoratorAttribute>())
//             .AsImplementedInterfaces()
//             .WithScopedLifetime());
//
//         return services;
//     }
//
//     public static IServiceCollection AddPagedQueryDecorator(this IServiceCollection services)
//     {
//         services.TryDecorate(typeof(IQueryHandler<,>), typeof(PagedQueryHandlerDecorator<,>));
//
//         return services;
//     }
// }