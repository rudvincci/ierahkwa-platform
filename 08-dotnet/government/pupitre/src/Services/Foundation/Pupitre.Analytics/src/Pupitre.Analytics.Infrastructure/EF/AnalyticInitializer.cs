using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Analytics.Infrastructure.EF;

internal sealed class AnalyticInitializer
{
    private readonly AnalyticDbContext _dbContext;
    private readonly ILogger<AnalyticInitializer> _logger;

    public AnalyticInitializer(AnalyticDbContext dbContext, ILogger<AnalyticInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.Analytics.AnyAsync())
        { }

        await AddAnalyticsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddAnalyticsAsync()
    {
        throw new NotImplementedException();
    }
}