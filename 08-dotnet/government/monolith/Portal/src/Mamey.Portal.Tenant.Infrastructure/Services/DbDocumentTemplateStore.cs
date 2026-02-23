using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Shared.Storage.Templates;
using Mamey.Portal.Tenant.Infrastructure.Persistence;

namespace Mamey.Portal.Tenant.Infrastructure.Services;

public sealed class DbDocumentTemplateStore : IDocumentTemplateStore
{
    private readonly TenantDbContext _db;

    public DbDocumentTemplateStore(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<string?> GetTemplateAsync(string tenantId, string kind, CancellationToken ct = default)
    {
        tenantId = (tenantId ?? string.Empty).Trim();
        kind = (kind ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(kind))
        {
            return null;
        }

        return await _db.DocumentTemplates.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Kind == kind)
            .Select(x => x.TemplateHtml)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<DocumentTemplateSummary>> ListTemplatesAsync(string tenantId, int take = 200, CancellationToken ct = default)
    {
        tenantId = (tenantId ?? string.Empty).Trim();
        take = Math.Clamp(take, 1, 500);

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return Array.Empty<DocumentTemplateSummary>();
        }

        return await _db.DocumentTemplates.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Kind)
            .Select(x => new DocumentTemplateSummary(x.Kind, x.UpdatedAt))
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task UpsertTemplateAsync(string tenantId, string kind, string templateHtml, CancellationToken ct = default)
    {
        tenantId = (tenantId ?? string.Empty).Trim();
        kind = (kind ?? string.Empty).Trim();
        templateHtml = templateHtml ?? string.Empty;

        if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("tenantId is required.");
        if (string.IsNullOrWhiteSpace(kind)) throw new ArgumentException("kind is required.");

        var now = DateTimeOffset.UtcNow;

        var row = await _db.DocumentTemplates
            .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Kind == kind, ct);

        if (row is null)
        {
            row = new TenantDocumentTemplateRow
            {
                TenantId = tenantId,
                Kind = kind,
                TemplateHtml = templateHtml,
                UpdatedAt = now,
            };
            _db.DocumentTemplates.Add(row);
        }
        else
        {
            row.TemplateHtml = templateHtml;
            row.UpdatedAt = now;
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteTemplateAsync(string tenantId, string kind, CancellationToken ct = default)
    {
        tenantId = (tenantId ?? string.Empty).Trim();
        kind = (kind ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(kind))
        {
            return;
        }

        var row = await _db.DocumentTemplates
            .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Kind == kind, ct);

        if (row is null) return;

        _db.DocumentTemplates.Remove(row);
        await _db.SaveChangesAsync(ct);
    }
}


