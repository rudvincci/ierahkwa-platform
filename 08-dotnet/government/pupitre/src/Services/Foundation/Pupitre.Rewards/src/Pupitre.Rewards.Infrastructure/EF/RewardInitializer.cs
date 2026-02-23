using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Rewards.Infrastructure.EF;

internal sealed class RewardInitializer
{
    private readonly RewardDbContext _dbContext;
    private readonly ILogger<RewardInitializer> _logger;

    public RewardInitializer(RewardDbContext dbContext, ILogger<RewardInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.Rewards.AnyAsync())
        { }

        await AddRewardsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddRewardsAsync()
    {
        throw new NotImplementedException();
    }
}