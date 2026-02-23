namespace Mamey.CQRS.Queries;

public interface IPagedQuery : IQuery
{
    int Page { get; set; }
    int ResultsPerPage { get; set; }
    string OrderBy { get; internal set; }
    string SortOrder { get; internal set;}
}