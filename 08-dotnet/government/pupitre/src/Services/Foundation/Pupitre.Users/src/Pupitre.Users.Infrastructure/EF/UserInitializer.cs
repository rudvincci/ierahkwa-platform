using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Users.Infrastructure.EF;

internal sealed class UserInitializer
{
    private readonly UserDbContext _dbContext;
    private readonly ILogger<UserInitializer> _logger;

    public UserInitializer(UserDbContext dbContext, ILogger<UserInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.Users.AnyAsync())
        { }

        await AddUsersAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddUsersAsync()
    {
        throw new NotImplementedException();
    }
}