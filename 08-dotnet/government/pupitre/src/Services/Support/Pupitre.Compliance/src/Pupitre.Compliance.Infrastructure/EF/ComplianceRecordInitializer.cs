using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Compliance.Infrastructure.EF;

internal sealed class ComplianceRecordInitializer
{
    private readonly ComplianceRecordDbContext _dbContext;
    private readonly ILogger<ComplianceRecordInitializer> _logger;

    public ComplianceRecordInitializer(ComplianceRecordDbContext dbContext, ILogger<ComplianceRecordInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.ComplianceRecords.AnyAsync())
        { }

        await AddComplianceRecordsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddComplianceRecordsAsync()
    {
        throw new NotImplementedException();
    }
}