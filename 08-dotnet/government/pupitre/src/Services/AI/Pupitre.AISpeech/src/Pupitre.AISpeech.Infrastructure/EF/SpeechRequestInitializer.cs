using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.AISpeech.Infrastructure.EF;

internal sealed class SpeechRequestInitializer
{
    private readonly SpeechRequestDbContext _dbContext;
    private readonly ILogger<SpeechRequestInitializer> _logger;

    public SpeechRequestInitializer(SpeechRequestDbContext dbContext, ILogger<SpeechRequestInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.SpeechRequests.AnyAsync())
        { }

        await AddSpeechRequestsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddSpeechRequestsAsync()
    {
        throw new NotImplementedException();
    }
}