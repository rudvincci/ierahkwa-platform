using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.AIAssessments.Infrastructure.EF;

internal sealed class AIAssessmentInitializer
{
    private readonly AIAssessmentDbContext _dbContext;
    private readonly ILogger<AIAssessmentInitializer> _logger;

    public AIAssessmentInitializer(AIAssessmentDbContext dbContext, ILogger<AIAssessmentInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.AIAssessments.AnyAsync())
        { }

        await AddAIAssessmentsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddAIAssessmentsAsync()
    {
        throw new NotImplementedException();
    }
}