using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.AIAdaptive.Infrastructure.EF;

internal sealed class AdaptiveLearningInitializer
{
    private readonly AdaptiveLearningDbContext _dbContext;
    private readonly ILogger<AdaptiveLearningInitializer> _logger;

    public AdaptiveLearningInitializer(AdaptiveLearningDbContext dbContext, ILogger<AdaptiveLearningInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.AdaptiveLearnings.AnyAsync())
        { }

        await AddAdaptiveLearningsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddAdaptiveLearningsAsync()
    {
        throw new NotImplementedException();
    }
}