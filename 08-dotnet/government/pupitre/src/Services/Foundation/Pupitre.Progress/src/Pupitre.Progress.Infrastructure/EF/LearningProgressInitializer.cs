using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Progress.Infrastructure.EF;

internal sealed class LearningProgressInitializer
{
    private readonly LearningProgressDbContext _dbContext;
    private readonly ILogger<LearningProgressInitializer> _logger;

    public LearningProgressInitializer(LearningProgressDbContext dbContext, ILogger<LearningProgressInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.LearningProgresss.AnyAsync())
        { }

        await AddLearningProgresssAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddLearningProgresssAsync()
    {
        throw new NotImplementedException();
    }
}