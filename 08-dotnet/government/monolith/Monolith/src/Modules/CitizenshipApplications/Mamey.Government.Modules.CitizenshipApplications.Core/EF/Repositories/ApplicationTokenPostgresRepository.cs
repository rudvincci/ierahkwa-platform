using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.EF.Repositories;

internal class ApplicationTokenPostgresRepository : IApplicationTokenRepository
{
    private readonly CitizenshipApplicationsDbContext _context;
    private readonly ILogger<ApplicationTokenPostgresRepository> _logger;

    public ApplicationTokenPostgresRepository(
        CitizenshipApplicationsDbContext context,
        ILogger<ApplicationTokenPostgresRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApplicationToken?> GetByTokenHashAsync(string tokenHash, string email, CancellationToken cancellationToken = default)
    {
        return await _context.Set<ApplicationToken>()
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && t.Email == email, cancellationToken);
    }

    public async Task AddAsync(ApplicationToken token, CancellationToken cancellationToken = default)
    {
        await _context.Set<ApplicationToken>().AddAsync(token, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAsUsedAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        var token = await _context.Set<ApplicationToken>()
            .FirstOrDefaultAsync(t => t.Id == tokenId, cancellationToken);

        if (token == null)
        {
            _logger.LogWarning("Token not found for marking as used: {TokenId}", tokenId);
            return;
        }

        token.MarkAsUsed();
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var expiredTokens = await _context.Set<ApplicationToken>()
            .Where(t => t.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        if (expiredTokens.Any())
        {
            _context.Set<ApplicationToken>().RemoveRange(expiredTokens);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Deleted {Count} expired tokens", expiredTokens.Count);
        }
    }
}
