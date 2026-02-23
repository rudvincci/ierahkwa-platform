using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.AISafety.Infrastructure.EF;

internal sealed class SafetyCheckInitializer
{
    private readonly SafetyCheckDbContext _dbContext;
    private readonly ILogger<SafetyCheckInitializer> _logger;

    public SafetyCheckInitializer(SafetyCheckDbContext dbContext, ILogger<SafetyCheckInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.SafetyChecks.AnyAsync())
        { }

        await AddSafetyChecksAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddSafetyChecksAsync()
    {
        throw new NotImplementedException();
    }
}