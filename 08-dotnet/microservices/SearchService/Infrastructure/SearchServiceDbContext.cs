using Microsoft.EntityFrameworkCore;
using Ierahkwa.SearchService.Domain;

namespace Ierahkwa.SearchService.Infrastructure;

public class SearchServiceDbContext : DbContext
{
    public SearchServiceDbContext(DbContextOptions<SearchServiceDbContext> options) : base(options) { }

    public DbSet<SearchIndex> SearchIndexs => Set<SearchIndex>();
    public DbSet<SearchQuery> SearchQuerys => Set<SearchQuery>();
    public DbSet<SearchResult> SearchResults => Set<SearchResult>();
    public DbSet<Facet> Facets => Set<Facet>();
    public DbSet<Suggestion> Suggestions => Set<Suggestion>();}
