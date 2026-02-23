using System;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Linq;
using Mamey.CQRS.Queries;


namespace Mamey.MicroMonolith.Infrastructure.Mongo
{
    public static class Pagination
    {
        public static async Task<PagedResult<T>> PaginateAsync<T>(this IQueryable<T> collection, IPagedQuery query)
            => await collection.PaginateAsync(query.Page, query.ResultsPerPage);

        public static async Task<PagedResult<T>> PaginateAsync<T>(this IQueryable<T> collection,
            int page = 1, int resultsPerPage = 10)
        {
            if (page <= 0)
            {
                page = 1;
            }
            if (resultsPerPage <= 0)
            {
                resultsPerPage = 10;
            }
            var isEmpty = await collection.AnyAsync() == false;
            if (isEmpty)
            {
                return PagedResult<T>.Empty;
            }
            var totalResults = await collection.CountAsync();
            var totalPages = (int)Math.Ceiling((decimal)totalResults / resultsPerPage);
            
            var data = await collection.Limit(page, resultsPerPage).ToListAsync();

            return PagedResult<T>.Create(data, page, resultsPerPage, totalPages, totalResults);
        }

        public static PagedResult<T> Paginate<T>(this IEnumerable<T> list, IPagedQuery query)
            => list.Paginate(query.Page, query.ResultsPerPage);

        public static PagedResult<T> Paginate<T>(this IEnumerable<T> list,
            int page = 1, int resultsPerPage = 10)
        {
            if (page <= 0)
            {
                page = 1;
            }
            if (resultsPerPage <= 0)
            {
                resultsPerPage = 10;
            }
            var isEmpty = list.Any() == false;
            //var isEmpty = await collection.AnyAsync() == false;
            if (isEmpty)
            {
                return PagedResult<T>.Empty;
            }
            var totalResults = list.Count();
            var totalPages = (int)Math.Ceiling((decimal)totalResults / resultsPerPage);

            var data = list.Limit(page, resultsPerPage).ToList().AsReadOnly();

            return PagedResult<T>.Create(data, page, resultsPerPage, totalPages, totalResults);
        }

        public static IQueryable<T> Limit<T>(this IQueryable<T> collection, IPagedQuery query)
            => collection.Limit(query.Page, query.ResultsPerPage);

        public static IQueryable<T> Limit<T>(this IQueryable<T> collection,
            int page = 1, int resultsPerPage = 10)
        {
            if (page <= 0)
            {
                page = 1;
            }
            if (resultsPerPage <= 0)
            {
                resultsPerPage = 10;
            }
            var skip = (page - 1) * resultsPerPage;
            var data = collection.Skip(skip)
                .Take(resultsPerPage);

            return data;
        }

        public static IEnumerable<T> Limit<T>(this IEnumerable<T> list, IPagedQuery query)
            => list.Limit(query.Page, query.ResultsPerPage);

        public static IEnumerable<T> Limit<T>(this IEnumerable<T> list,
            int page = 1, int resultsPerPage = 10)
        {
            if (page <= 0)
            {
                page = 1;
            }
            if (resultsPerPage <= 0)
            {
                resultsPerPage = 10;
            }
            var skip = (page - 1) * resultsPerPage;
            var data = list.Skip(skip)
                .Take(resultsPerPage);

            return data;
        }
    }
}