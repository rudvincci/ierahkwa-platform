using Mamey.Government.Modules.CMS.Core.Domain.Entities;
using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;
using GovTenantId = Mamey.Types.TenantId;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Mamey.Government.Modules.CMS.Core.EF.Repositories;

internal class ContentPostgresRepository : IContentRepository
{
    private readonly CMSDbContext _context;
    private readonly ILogger<ContentPostgresRepository> _logger;

    public ContentPostgresRepository(
        CMSDbContext context,
        ILogger<ContentPostgresRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Content?> GetAsync(ContentId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.Contents
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        return row?.AsEntity();
    }

    public async Task AddAsync(Content content, CancellationToken cancellationToken = default)
    {
        var row = content.AsRow();
        await _context.Contents.AddAsync(row, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Content content, CancellationToken cancellationToken = default)
    {
        var row = await _context.Contents
            .FirstOrDefaultAsync(r => r.Id == content.Id.Value, cancellationToken);
        
        if (row == null)
        {
            _logger.LogWarning("Content not found for update: {ContentId}", content.Id.Value);
            return;
        }

        row.UpdateFromEntity(content);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ContentId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.Contents
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        if (row != null)
        {
            _context.Contents.Remove(row);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(ContentId id, CancellationToken cancellationToken = default)
    {
        return await _context.Contents
            .AnyAsync(r => r.Id == id.Value, cancellationToken);
    }

    public async Task<IReadOnlyList<Content>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _context.Contents.ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<Content?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var row = await _context.Contents
            .FirstOrDefaultAsync(r => r.Slug == slug, cancellationToken);
        
        return row?.AsEntity();
    }

    public async Task<IReadOnlyList<Content>> GetByTenantAsync(GovTenantId tenantId, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Contents
            .Where(r => r.TenantId == tenantId.Value)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Content>> GetByStatusAsync(ContentStatus status, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Contents
            .Where(r => r.Status == status)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Content>> GetByContentTypeAsync(string contentType, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Contents
            .Where(r => r.ContentType == contentType)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }
}

internal static class ContentRowExtensions
{
    public static Content AsEntity(this ContentRow row)
    {
        var contentId = new ContentId(row.Id);
        var tenantId = new GovTenantId(row.TenantId);
        
        var content = new Content(
            contentId,
            tenantId,
            row.Title,
            row.Slug,
            row.ContentType,
            row.Status,
            row.Version);
        
        typeof(Content).GetProperty("Body")?.SetValue(content, row.Body);
        typeof(Content).GetProperty("Excerpt")?.SetValue(content, row.Excerpt);
        typeof(Content).GetProperty("PublishedAt")?.SetValue(content, row.PublishedAt);
        typeof(Content).GetProperty("CreatedAt")?.SetValue(content, row.CreatedAt);
        typeof(Content).GetProperty("UpdatedAt")?.SetValue(content, row.UpdatedAt);
        
        if (!string.IsNullOrEmpty(row.MetadataJson))
        {
            var metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(row.MetadataJson);
            typeof(Content).GetProperty("Metadata")?.SetValue(content, metadata ?? new Dictionary<string, string>());
        }
        
        return content;
    }

    public static ContentRow AsRow(this Content entity)
    {
        return new ContentRow
        {
            Id = entity.Id.Value,
            TenantId = entity.TenantId.Value,
            Title = entity.Title,
            Slug = entity.Slug,
            ContentType = entity.ContentType,
            Body = entity.Body,
            Excerpt = entity.Excerpt,
            Status = entity.Status,
            PublishedAt = entity.PublishedAt,
            MetadataJson = entity.Metadata.Any() ? JsonSerializer.Serialize(entity.Metadata) : null,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Version = entity.Version
        };
    }

    public static void UpdateFromEntity(this ContentRow row, Content entity)
    {
        row.Title = entity.Title;
        row.Slug = entity.Slug;
        row.Body = entity.Body;
        row.Excerpt = entity.Excerpt;
        row.Status = entity.Status;
        row.PublishedAt = entity.PublishedAt;
        row.MetadataJson = entity.Metadata.Any() ? JsonSerializer.Serialize(entity.Metadata) : null;
        row.UpdatedAt = entity.UpdatedAt;
        row.Version = entity.Version;
    }
}
