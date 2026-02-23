using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.AIRecommendations.Infrastructure.EF;

internal sealed class AIRecommendationInitializer
{
    private readonly AIRecommendationDbContext _dbContext;
    private readonly ILogger<AIRecommendationInitializer> _logger;

    public AIRecommendationInitializer(AIRecommendationDbContext dbContext, ILogger<AIRecommendationInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.AIRecommendations.AnyAsync())
        { }

        await AddAIRecommendationsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddAIRecommendationsAsync()
    {
        throw new NotImplementedException();
    }
}