using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.AIBehavior.Infrastructure.EF;

internal sealed class BehaviorInitializer
{
    private readonly BehaviorDbContext _dbContext;
    private readonly ILogger<BehaviorInitializer> _logger;

    public BehaviorInitializer(BehaviorDbContext dbContext, ILogger<BehaviorInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.Behaviors.AnyAsync())
        { }

        await AddBehaviorsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddBehaviorsAsync()
    {
        throw new NotImplementedException();
    }
}