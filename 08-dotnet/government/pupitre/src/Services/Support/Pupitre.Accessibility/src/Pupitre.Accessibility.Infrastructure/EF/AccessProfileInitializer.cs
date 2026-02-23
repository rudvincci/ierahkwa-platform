using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Accessibility.Infrastructure.EF;

internal sealed class AccessProfileInitializer
{
    private readonly AccessProfileDbContext _dbContext;
    private readonly ILogger<AccessProfileInitializer> _logger;

    public AccessProfileInitializer(AccessProfileDbContext dbContext, ILogger<AccessProfileInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.AccessProfiles.AnyAsync())
        { }

        await AddAccessProfilesAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddAccessProfilesAsync()
    {
        throw new NotImplementedException();
    }
}