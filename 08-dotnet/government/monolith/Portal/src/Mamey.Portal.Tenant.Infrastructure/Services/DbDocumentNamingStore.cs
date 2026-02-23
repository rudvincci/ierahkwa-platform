using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Shared.Storage.DocumentNaming;
using Mamey.Portal.Tenant.Infrastructure.Persistence;

namespace Mamey.Portal.Tenant.Infrastructure.Services;

public sealed class DbDocumentNamingStore : IDocumentNamingStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly TenantDbContext _db;

    public DbDocumentNamingStore(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<DocumentNamingPattern> GetAsync(string tenantId, CancellationToken ct = default)
    {
        tenantId = (tenantId ?? string.Empty).Trim();
        if (tenantId.Length == 0) tenantId = "default";

        var row = await _db.DocumentNaming.AsNoTracking()
            .SingleOrDefaultAsync(x => x.TenantId == tenantId, ct);

        if (row is null || string.IsNullOrWhiteSpace(row.PatternJson))
        {
            return DocumentNamingPattern.Default;
        }

        try
        {
            return JsonSerializer.Deserialize<DocumentNamingPattern>(row.PatternJson, JsonOptions) ?? DocumentNamingPattern.Default;
        }
        catch
        {
            return DocumentNamingPattern.Default;
        }
    }

    public async Task SetAsync(string tenantId, DocumentNamingPattern pattern, CancellationToken ct = default)
    {
        tenantId = (tenantId ?? string.Empty).Trim();
        if (tenantId.Length == 0) tenantId = "default";

        var json = JsonSerializer.Serialize(pattern ?? DocumentNamingPattern.Default, JsonOptions);

        var existing = await _db.DocumentNaming
            .SingleOrDefaultAsync(x => x.TenantId == tenantId, ct);

        if (existing is null)
        {
            _db.DocumentNaming.Add(new TenantDocumentNamingRow
            {
                TenantId = tenantId,
                PatternJson = json,
                UpdatedAt = DateTimeOffset.UtcNow,
            });
        }
        else
        {
            existing.PatternJson = json;
            existing.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
    }
}




