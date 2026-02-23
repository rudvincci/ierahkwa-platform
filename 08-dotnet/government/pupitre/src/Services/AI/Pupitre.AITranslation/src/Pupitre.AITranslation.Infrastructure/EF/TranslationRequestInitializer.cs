using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.AITranslation.Infrastructure.EF;

internal sealed class TranslationRequestInitializer
{
    private readonly TranslationRequestDbContext _dbContext;
    private readonly ILogger<TranslationRequestInitializer> _logger;

    public TranslationRequestInitializer(TranslationRequestDbContext dbContext, ILogger<TranslationRequestInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.TranslationRequests.AnyAsync())
        { }

        await AddTranslationRequestsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddTranslationRequestsAsync()
    {
        throw new NotImplementedException();
    }
}