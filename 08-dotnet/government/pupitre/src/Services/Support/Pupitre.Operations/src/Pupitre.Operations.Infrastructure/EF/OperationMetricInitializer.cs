using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Operations.Infrastructure.EF;

internal sealed class OperationMetricInitializer
{
    private readonly OperationMetricDbContext _dbContext;
    private readonly ILogger<OperationMetricInitializer> _logger;

    public OperationMetricInitializer(OperationMetricDbContext dbContext, ILogger<OperationMetricInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.OperationMetrics.AnyAsync())
        { }

        await AddOperationMetricsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddOperationMetricsAsync()
    {
        throw new NotImplementedException();
    }
}