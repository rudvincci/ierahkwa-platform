using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Parents.Infrastructure.EF;

internal sealed class ParentInitializer
{
    private readonly ParentDbContext _dbContext;
    private readonly ILogger<ParentInitializer> _logger;

    public ParentInitializer(ParentDbContext dbContext, ILogger<ParentInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.Parents.AnyAsync())
        { }

        await AddParentsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddParentsAsync()
    {
        throw new NotImplementedException();
    }
}