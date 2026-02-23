using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mamey.ServiceName.Infrastructure.EF;

internal sealed class EntityNameInitializer
{
    private readonly EntityNameDbContext _dbContext;
    private readonly ILogger<EntityNameInitializer> _logger;

    public EntityNameInitializer(EntityNameDbContext dbContext, ILogger<EntityNameInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.EntityNames.AnyAsync())
        { }

        await AddEntityNamesAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddEntityNamesAsync()
    {
        throw new NotImplementedException();
    }
}