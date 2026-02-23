using Mamey.Government.Modules.Identity.Core.Domain.Entities;
using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Identity.Core.EF.Repositories;

internal class UserProfilePostgresRepository : IUserProfileRepository
{
    private readonly IdentityDbContext _context;
    private readonly ILogger<UserProfilePostgresRepository> _logger;

    public UserProfilePostgresRepository(
        IdentityDbContext context,
        ILogger<UserProfilePostgresRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserProfile?> GetAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.UserProfiles
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        return row?.AsEntity();
    }

    public async Task AddAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        var row = userProfile.AsRow();
        await _context.UserProfiles.AddAsync(row, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        var row = await _context.UserProfiles
            .FirstOrDefaultAsync(r => r.Id == userProfile.Id.Value, cancellationToken);
        
        if (row == null)
        {
            _logger.LogWarning("UserProfile not found for update: {UserId}", userProfile.Id.Value);
            return;
        }

        row.UpdateFromEntity(userProfile);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.UserProfiles
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        if (row != null)
        {
            _context.UserProfiles.Remove(row);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .AnyAsync(r => r.Id == id.Value, cancellationToken);
    }

    public async Task<IReadOnlyList<UserProfile>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _context.UserProfiles.ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<UserProfile?> GetByAuthenticatorAsync(string issuer, string subject, CancellationToken cancellationToken = default)
    {
        var row = await _context.UserProfiles
            .FirstOrDefaultAsync(r => r.AuthenticatorIssuer == issuer && r.AuthenticatorSubject == subject, cancellationToken);
        
        return row?.AsEntity();
    }

    public async Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var row = await _context.UserProfiles
            .FirstOrDefaultAsync(r => r.Email == email, cancellationToken);
        
        return row?.AsEntity();
    }
}

internal static class UserProfileRowExtensions
{
    public static UserProfile AsEntity(this UserProfileRow row)
    {
        var userId = new UserId(row.Id);
        var profile = new UserProfile(
            userId,
            row.AuthenticatorIssuer,
            row.AuthenticatorSubject,
            row.Email,
            row.DisplayName,
            row.TenantId,
            row.Version);
        
        // Use reflection to set private properties for read-only fields
        typeof(UserProfile).GetProperty("CreatedAt")?.SetValue(profile, row.CreatedAt);
        typeof(UserProfile).GetProperty("UpdatedAt")?.SetValue(profile, row.UpdatedAt);
        typeof(UserProfile).GetProperty("LastLoginAt")?.SetValue(profile, row.LastLoginAt);
        
        return profile;
    }

    public static UserProfileRow AsRow(this UserProfile entity)
    {
        return new UserProfileRow
        {
            Id = entity.Id.Value,
            AuthenticatorIssuer = entity.AuthenticatorIssuer,
            AuthenticatorSubject = entity.AuthenticatorSubject,
            Email = entity.Email,
            DisplayName = entity.DisplayName,
            TenantId = entity.TenantId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            LastLoginAt = entity.LastLoginAt,
            Version = entity.Version
        };
    }

    public static void UpdateFromEntity(this UserProfileRow row, UserProfile entity)
    {
        row.Email = entity.Email;
        row.DisplayName = entity.DisplayName;
        row.TenantId = entity.TenantId;
        row.UpdatedAt = entity.UpdatedAt;
        row.LastLoginAt = entity.LastLoginAt;
        row.Version = entity.Version;
    }
}
