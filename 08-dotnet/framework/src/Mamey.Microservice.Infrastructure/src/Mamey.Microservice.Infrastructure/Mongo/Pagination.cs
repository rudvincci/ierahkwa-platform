// using System.Linq.Expressions;
// using Mamey.CQRS.Queries;
// using MongoDB.Driver;
// using MongoDB.Driver.Linq;
//
// namespace Mamey.Microservice.Infrastructure.Mongo
// {
//     public static class Pagination
//     {
//         public static async Task<PagedResult<T>> PaginateAsync<T>(this IQueryable<T> collection, IPagedQuery query)
//         where T: class
//             => await collection.PaginateAsync<T>(query.Page, query.Results);
//
//         public static async Task<PagedResult<T>> PaginateAsync<T>(this IQueryable<T> collection,
//             int page = 1, int resultsPerPage = 10)
//         {
//             if (page <= 0)
//             {
//                 page = 1;
//             }
//             if (resultsPerPage <= 0)
//             {
//                 resultsPerPage = 10;
//             }
//             var isEmpty = await collection.AnyAsync() == false;
//             if (isEmpty)
//             {
//                 return PagedResult<T>.Empty;
//             }
//             var totalResults = await collection.CountAsync();
//             var totalPages = (int)Math.Ceiling((decimal)totalResults / resultsPerPage);
//             
//             var data = await collection
//                 .Limit(page, resultsPerPage)
//                 .ToListAsync();
//
//             return PagedResult<T>.Create(data, page, resultsPerPage, totalPages, totalResults);
//         }
//
//         public static PagedResult<T> Paginate<T>(this IEnumerable<T> list, IPagedQuery query)
//             => list.Paginate(query.Page, query.Results);
//
//         public static PagedResult<T> Paginate<T>(this IEnumerable<T> list,
//             int page = 1, int resultsPerPage = 10)
//         {
//             if (page <= 0)
//             {
//                 page = 1;
//             }
//             if (resultsPerPage <= 0)
//             {
//                 resultsPerPage = 10;
//             }
//             var isEmpty = list.Any() == false;
//             //var isEmpty = await collection.AnyAsync() == false;
//             if (isEmpty)
//             {
//                 return PagedResult<T>.Empty;
//             }
//             var totalResults = list.Count();
//             var totalPages = (int)Math.Ceiling((decimal)totalResults / resultsPerPage);
//
//             var data = list.Limit(page, resultsPerPage);
//
//             return PagedResult<T>.Create(data, page, resultsPerPage, totalPages, totalResults);
//         }
//
//         public static IMongoQueryable<T> Limit<T>(this IMongoQueryable<T> collection, IPagedQuery query)
//             => collection.Limit(query.Page, query.Results);
//
//         public static IMongoQueryable<T> Limit<T>(this IMongoQueryable<T> collection,
//             int page = 1, int resultsPerPage = 10)
//         {
//             if (page <= 0)
//             {
//                 page = 1;
//             }
//             if (resultsPerPage <= 0)
//             {
//                 resultsPerPage = 10;
//             }
//             var skip = (page - 1) * resultsPerPage;
//             var data = collection.Skip(skip)
//                 .Take(resultsPerPage);
//
//             return data;
//         }
//
//         public static IEnumerable<T> Limit<T>(this IEnumerable<T> list, IPagedQuery query)
//             => list.Limit(query.Page, query.Results);
//
//         public static IEnumerable<T> Limit<T>(this IEnumerable<T> list,
//             int page = 1, int resultsPerPage = 10)
//         {
//             if (page <= 0)
//             {
//                 page = 1;
//             }
//             if (resultsPerPage <= 0)
//             {
//                 resultsPerPage = 10;
//             }
//             var skip = (page - 1) * resultsPerPage;
//             var data = list.Skip(skip)
//                 .Take(resultsPerPage);
//
//             return data;
//         }
//
//         internal static Expression<Func<T, object>> ToLambda<T>(string propertyName)
//         {
//             var parameter = Expression.Parameter(typeof(T));
//             var property = Expression.Property(parameter, propertyName);
//             var propAsObject = Expression.Convert(property, typeof(object));
//
//             return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
//         }
//     }
//     
// }