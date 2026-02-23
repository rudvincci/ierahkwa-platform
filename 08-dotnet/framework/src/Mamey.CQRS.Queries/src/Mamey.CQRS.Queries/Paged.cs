// using System;
// using System.Collections.Generic;
// using System.Linq;
//
// namespace Mamey.CQRS.Queries;
//
// public class Paged<T> : PagedQueryBase
// {
//     public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
//     
//     public bool Empty => Items is null || !Items.Any();
//
//     public Paged()
//     {
//         Page = 1;
//         Results = 10;
//     }
//
//     public int TotalPages => Items?.Count() / Results ?? 0;
//     public Paged(IEnumerable<T> items,
//         int currentPage, int resultsPerPage,
//         int totalPages, long totalResults) :
//         base(currentPage, resultsPerPage, totalPages, totalResults)
//     {
//         Items = items;
//     }
//
//     public static Paged<T> Create(IEnumerable<T> items,
//         int currentPage, int resultsPerPage,
//         int totalPages, long totalResults)
//         => new(items, currentPage, resultsPerPage, totalPages, totalResults);
//
//     public static Paged<T> From(PagedBase result, IEnumerable<T> items)
//         => new(items, result.CurrentPage, result.ResultsPerPage,
//             result.TotalPages, result.TotalResults);
//
//     public static Paged<T> AsEmpty => new();
//
//     public Paged<TResult> Map<TResult>(Func<T, TResult> map)
//         => Paged<TResult>.From(this, Items.Select(map).ToList());
// }