// using System;
// using Mamey.CQRS.Queries;
// using Mamey.Pagination;
// using Microsoft.EntityFrameworkCore;
//
// namespace Mamey.Persistence.PostgreSQL.Pagination;
//
// public static class Extensions
// {
//     public static Task<PagedResult<T>> PaginateAsync<T>(this IQueryable<T> data, IPagedQuery query,
//         CancellationToken cancellationToken = default)
//         => data.PaginateAsync(query.Page, query.Results, cancellationToken);
//
//     public static async Task<PagedResult<T>> PaginateAsync<T>(this IQueryable<T> data, int page, int results,
//         CancellationToken cancellationToken = default)
//     {
//         if (page <= 0)
//         {
//             page = 1;
//         }
//
//         results = results switch
//         {
//             <= 0 => 10,
//             > 100 => 100,
//             _ => results
//         };
//
//         var totalResults = await data.CountAsync();
//         var totalPages = totalResults <= results ? 1 : (int)Math.Floor((double)totalResults / results);
//         var result = await data.Skip((page - 1) * results).Take(results).ToListAsync(cancellationToken);
//         return PagedResult.Create(result.AsEnumerable<T>(), page, results, totalPages, totalResults);
//         //return new PagedResult<T>(result, page, results, totalPages, totalResults);
//     }
//
//     public static Task<List<T>> SkipAndTakeAsync<T>(this IQueryable<T> data, IPagedQuery query,
//         CancellationToken cancellationToken = default)
//         => data.SkipAndTakeAsync(query.Page, query.Results, cancellationToken);
//
//     public static async Task<List<T>> SkipAndTakeAsync<T>(this IQueryable<T> data, int page, int results,
//         CancellationToken cancellationToken = default)
//     {
//         if (page <= 0)
//         {
//             page = 1;
//         }
//
//         results = results switch
//         {
//             <= 0 => 10,
//             > 100 => 100,
//             _ => results
//         };
//
//         return await data.Skip((page - 1) * results).Take(results).ToListAsync(cancellationToken);
//     }
// }
//
