using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Aftercare.Infrastructure.EF;

internal sealed class AftercarePlanInitializer
{
    private readonly AftercarePlanDbContext _dbContext;
    private readonly ILogger<AftercarePlanInitializer> _logger;

    public AftercarePlanInitializer(AftercarePlanDbContext dbContext, ILogger<AftercarePlanInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.AftercarePlans.AnyAsync())
        { }

        await AddAftercarePlansAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddAftercarePlansAsync()
    {
        throw new NotImplementedException();
    }
}