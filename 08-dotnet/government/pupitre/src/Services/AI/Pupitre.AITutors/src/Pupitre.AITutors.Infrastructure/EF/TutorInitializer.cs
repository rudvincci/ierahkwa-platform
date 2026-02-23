using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.AITutors.Infrastructure.EF;

internal sealed class TutorInitializer
{
    private readonly TutorDbContext _dbContext;
    private readonly ILogger<TutorInitializer> _logger;

    public TutorInitializer(TutorDbContext dbContext, ILogger<TutorInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.Tutors.AnyAsync())
        { }

        await AddTutorsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddTutorsAsync()
    {
        throw new NotImplementedException();
    }
}