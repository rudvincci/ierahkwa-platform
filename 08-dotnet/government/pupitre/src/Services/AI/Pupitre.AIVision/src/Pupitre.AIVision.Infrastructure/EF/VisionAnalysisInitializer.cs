using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.AIVision.Infrastructure.EF;

internal sealed class VisionAnalysisInitializer
{
    private readonly VisionAnalysisDbContext _dbContext;
    private readonly ILogger<VisionAnalysisInitializer> _logger;

    public VisionAnalysisInitializer(VisionAnalysisDbContext dbContext, ILogger<VisionAnalysisInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.VisionAnalyses.AnyAsync())
        { }

        await AddVisionAnalysesAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddVisionAnalysesAsync()
    {
        throw new NotImplementedException();
    }
}