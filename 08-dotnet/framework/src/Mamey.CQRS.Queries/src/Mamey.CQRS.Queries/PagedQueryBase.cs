namespace Mamey.CQRS.Queries;

public abstract class PagedQueryBase : IPagedQuery
{
    protected PagedQueryBase() { }
    protected PagedQueryBase(int page, int resultsPerPage, string orderBy, string sortOrder)
    {
        Page = page;
        ResultsPerPage = resultsPerPage;
        OrderBy = orderBy;
        SortOrder = sortOrder;
    }
    
    public int Page { get; set; }
    public int ResultsPerPage { get; set; }
    public string OrderBy { get; set; }
    public string SortOrder { get; set; }
    public int TotalResults { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalResults / ResultsPerPage);
    
}