using Mamey.Government.Modules.Documents.Core.Domain.Entities;
using Mamey.Government.Modules.Documents.Core.Domain.Repositories;
using Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Mamey.Government.Modules.Documents.Core.EF.Repositories;

internal class DocumentPostgresRepository : IDocumentRepository
{
    private readonly DocumentsDbContext _context;
    private readonly ILogger<DocumentPostgresRepository> _logger;

    public DocumentPostgresRepository(
        DocumentsDbContext context,
        ILogger<DocumentPostgresRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Document?> GetAsync(DocumentId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.Documents
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        return row?.AsEntity();
    }

    public async Task AddAsync(Document document, CancellationToken cancellationToken = default)
    {
        var row = document.AsRow();
        await _context.Documents.AddAsync(row, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Document document, CancellationToken cancellationToken = default)
    {
        var row = await _context.Documents
            .FirstOrDefaultAsync(r => r.Id == document.Id.Value, cancellationToken);
        
        if (row == null)
        {
            _logger.LogWarning("Document not found for update: {DocumentId}", document.Id.Value);
            return;
        }

        row.UpdateFromEntity(document);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DocumentId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.Documents
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        if (row != null)
        {
            _context.Documents.Remove(row);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(DocumentId id, CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .AnyAsync(r => r.Id == id.Value, cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _context.Documents.ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Document>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Documents
            .Where(r => r.TenantId == tenantId.Value)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Document>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Documents
            .Where(r => r.Category == category)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Document>> GetByStorageKeyAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Documents
            .Where(r => r.StorageKey == storageKey)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }
}

internal static class DocumentRowExtensions
{
    public static Document AsEntity(this DocumentRow row)
    {
        var documentId = new DocumentId(row.Id);
        var tenantId = new TenantId(row.TenantId);
        
        var document = new Document(
            documentId,
            tenantId,
            row.FileName,
            row.ContentType,
            row.FileSize,
            row.StorageBucket,
            row.StorageKey,
            row.Category,
            row.Version);
        
        typeof(Document).GetProperty("Description")?.SetValue(document, row.Description);
        typeof(Document).GetProperty("IsActive")?.SetValue(document, row.IsActive);
        typeof(Document).GetProperty("DeletedAt")?.SetValue(document, row.DeletedAt);
        typeof(Document).GetProperty("CreatedAt")?.SetValue(document, row.CreatedAt);
        typeof(Document).GetProperty("UpdatedAt")?.SetValue(document, row.UpdatedAt);
        
        if (!string.IsNullOrEmpty(row.MetadataJson))
        {
            var metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(row.MetadataJson);
            typeof(Document).GetProperty("Metadata")?.SetValue(document, metadata ?? new Dictionary<string, string>());
        }
        
        return document;
    }

    public static DocumentRow AsRow(this Document entity)
    {
        return new DocumentRow
        {
            Id = entity.Id.Value,
            TenantId = entity.TenantId.Value,
            FileName = entity.FileName,
            ContentType = entity.ContentType,
            FileSize = entity.FileSize,
            StorageBucket = entity.StorageBucket,
            StorageKey = entity.StorageKey,
            Category = entity.Category,
            Description = entity.Description,
            MetadataJson = entity.Metadata.Any() ? JsonSerializer.Serialize(entity.Metadata) : null,
            IsActive = entity.IsActive,
            DeletedAt = entity.DeletedAt,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Version = entity.Version
        };
    }

    public static void UpdateFromEntity(this DocumentRow row, Document entity)
    {
        row.Description = entity.Description;
        row.MetadataJson = entity.Metadata.Any() ? JsonSerializer.Serialize(entity.Metadata) : null;
        row.IsActive = entity.IsActive;
        row.DeletedAt = entity.DeletedAt;
        row.UpdatedAt = entity.UpdatedAt;
        row.Version = entity.Version;
    }
}
