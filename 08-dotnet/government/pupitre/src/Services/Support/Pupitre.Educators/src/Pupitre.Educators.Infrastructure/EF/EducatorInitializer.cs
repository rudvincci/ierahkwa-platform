using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Educators.Infrastructure.EF;

internal sealed class EducatorInitializer
{
    private readonly EducatorDbContext _dbContext;
    private readonly ILogger<EducatorInitializer> _logger;

    public EducatorInitializer(EducatorDbContext dbContext, ILogger<EducatorInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.Educators.AnyAsync())
        { }

        await AddEducatorsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddEducatorsAsync()
    {
        throw new NotImplementedException();
    }
}