using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.AIContent.Infrastructure.EF;

internal sealed class ContentGenerationInitializer
{
    private readonly ContentGenerationDbContext _dbContext;
    private readonly ILogger<ContentGenerationInitializer> _logger;

    public ContentGenerationInitializer(ContentGenerationDbContext dbContext, ILogger<ContentGenerationInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.ContentGenerations.AnyAsync())
        { }

        await AddContentGenerationsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddContentGenerationsAsync()
    {
        throw new NotImplementedException();
    }
}