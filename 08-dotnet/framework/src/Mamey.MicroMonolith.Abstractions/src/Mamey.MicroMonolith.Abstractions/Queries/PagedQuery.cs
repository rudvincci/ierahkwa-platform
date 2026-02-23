// using Mamey.Pagination;
//
// namespace Mamey.MicroMonolith.Abstractions.Queries;
//
// public abstract class PagedQuery : IPagedQuery
// {
//     public int Page { get; set; }
//     public int Results { get; set; }
//     public string OrderBy { get; set; } = "id";
//     public string SortOrder { get; set; } = "desc";
// }
//     
// public abstract class PagedQuery<T> : PagedQuery, IPagedQuery<Paged<T>>
// {
// }