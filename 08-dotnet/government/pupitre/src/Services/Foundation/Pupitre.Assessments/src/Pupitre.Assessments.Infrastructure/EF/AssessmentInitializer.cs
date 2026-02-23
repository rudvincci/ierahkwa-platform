using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Assessments.Infrastructure.EF;

internal sealed class AssessmentInitializer
{
    private readonly AssessmentDbContext _dbContext;
    private readonly ILogger<AssessmentInitializer> _logger;

    public AssessmentInitializer(AssessmentDbContext dbContext, ILogger<AssessmentInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.Assessments.AnyAsync())
        { }

        await AddAssessmentsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddAssessmentsAsync()
    {
        throw new NotImplementedException();
    }
}