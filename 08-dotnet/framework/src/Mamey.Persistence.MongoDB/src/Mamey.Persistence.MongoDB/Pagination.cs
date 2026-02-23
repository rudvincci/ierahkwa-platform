using System.Linq.Expressions;
using Mamey.CQRS.Queries;
using MongoDB.Driver.Linq;

namespace Mamey.Persistence.MongoDB;

public static class Pagination
{
    public static async Task<PagedResult<T>> PaginateAsync<T>(this IQueryable<T> collection, IPagedQuery query)
        => await collection.PaginateAsync(query.OrderBy, query.SortOrder, query.Page, query.ResultsPerPage);

    public static async Task<PagedResult<T>> PaginateAsync<T>(
        this IQueryable<T> collection,
        string orderBy,
        string sortOrder,
        int page = 1,
        int resultsPerPage = 10)
    {
        page = Math.Max(1, page);
        resultsPerPage = Math.Max(1, resultsPerPage);

        if (!await collection.AnyAsync())
        {
            return PagedResult<T>.Empty;
        }

        var totalResults = await collection.CountAsync();
        var totalPages = (int)Math.Ceiling((decimal)totalResults / resultsPerPage);

        List<T> data;

        if (string.IsNullOrWhiteSpace(orderBy))
        {
            data = await collection
                .Limit(page, resultsPerPage)
                .ToListAsync();

            return PagedResult<T>.Create(data, page, resultsPerPage, totalPages, totalResults);
        }

        var keySelector = ToLambda<T>(orderBy);

        data = sortOrder?.Trim().ToLowerInvariant() == "asc"
            ? await collection.OrderBy(keySelector).Limit(page, resultsPerPage).ToListAsync()
            : await collection.OrderByDescending(keySelector).Limit(page, resultsPerPage).ToListAsync();

        return PagedResult<T>.Create(data, page, resultsPerPage, totalPages, totalResults);
    }

    public static IQueryable<T> Limit<T>(this IQueryable<T> collection, IPagedQuery query)
        => collection.Limit(query.Page, query.ResultsPerPage);

    public static IQueryable<T> Limit<T>(this IQueryable<T> collection, int page = 1, int resultsPerPage = 10)
    {
        page = Math.Max(1, page);
        resultsPerPage = Math.Max(1, resultsPerPage);

        var skip = (page - 1) * resultsPerPage;

        return collection.Skip(skip).Take(resultsPerPage);
    }

    private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
    {
        var propertyInfo = typeof(T).GetProperty(propertyName);
        if (propertyInfo == null)
        {
            throw new ArgumentException(
                $"Property '{propertyName}' does not exist on type '{typeof(T).Name}'.", nameof(propertyName));
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyInfo);
        var converted = Expression.Convert(property, typeof(object));

        return Expression.Lambda<Func<T, object>>(converted, parameter);
    }
}
