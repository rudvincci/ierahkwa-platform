using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Curricula.Infrastructure.EF;

internal sealed class CurriculumInitializer
{
    private readonly CurriculumDbContext _dbContext;
    private readonly ILogger<CurriculumInitializer> _logger;

    public CurriculumInitializer(CurriculumDbContext dbContext, ILogger<CurriculumInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.Curriculums.AnyAsync())
        { }

        await AddCurriculumsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddCurriculumsAsync()
    {
        throw new NotImplementedException();
    }
}