using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Lessons.Infrastructure.EF;

internal sealed class LessonInitializer
{
    private readonly LessonDbContext _dbContext;
    private readonly ILogger<LessonInitializer> _logger;

    public LessonInitializer(LessonDbContext dbContext, ILogger<LessonInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.Lessons.AnyAsync())
        { }

        await AddLessonsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddLessonsAsync()
    {
        throw new NotImplementedException();
    }
}