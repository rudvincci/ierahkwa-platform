using Mamey.Portal.Shared.Storage.Templates;

namespace Mamey.Portal.Web.Storage.Templates;

public sealed class InMemoryDocumentTemplateStore : IDocumentTemplateStore
{
    private readonly object _gate = new();
    private readonly Dictionary<(string TenantId, string Kind), (string Html, DateTimeOffset UpdatedAt)> _templates = new();

    public Task<string?> GetTemplateAsync(string tenantId, string kind, CancellationToken ct = default)
    {
        tenantId = (tenantId ?? string.Empty).Trim();
        kind = (kind ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(kind))
        {
            return Task.FromResult<string?>(null);
        }

        lock (_gate)
        {
            return Task.FromResult(_templates.TryGetValue((tenantId, kind), out var tpl) ? tpl.Html : null);
        }
    }

    public Task<IReadOnlyList<DocumentTemplateSummary>> ListTemplatesAsync(string tenantId, int take = 200, CancellationToken ct = default)
    {
        tenantId = (tenantId ?? string.Empty).Trim();
        take = Math.Clamp(take, 1, 500);

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return Task.FromResult<IReadOnlyList<DocumentTemplateSummary>>(Array.Empty<DocumentTemplateSummary>());
        }

        lock (_gate)
        {
            var list = _templates
                .Where(kv => string.Equals(kv.Key.TenantId, tenantId, StringComparison.OrdinalIgnoreCase))
                .OrderBy(kv => kv.Key.Kind)
                .Select(kv => new DocumentTemplateSummary(kv.Key.Kind, kv.Value.UpdatedAt))
                .Take(take)
                .ToList();

            return Task.FromResult<IReadOnlyList<DocumentTemplateSummary>>(list);
        }
    }

    public Task UpsertTemplateAsync(string tenantId, string kind, string templateHtml, CancellationToken ct = default)
    {
        tenantId = (tenantId ?? string.Empty).Trim();
        kind = (kind ?? string.Empty).Trim();
        templateHtml = templateHtml ?? string.Empty;

        if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("tenantId is required.");
        if (string.IsNullOrWhiteSpace(kind)) throw new ArgumentException("kind is required.");

        lock (_gate)
        {
            _templates[(tenantId, kind)] = (templateHtml, DateTimeOffset.UtcNow);
        }

        return Task.CompletedTask;
    }

    public Task DeleteTemplateAsync(string tenantId, string kind, CancellationToken ct = default)
    {
        tenantId = (tenantId ?? string.Empty).Trim();
        kind = (kind ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(kind))
        {
            return Task.CompletedTask;
        }

        lock (_gate)
        {
            _templates.Remove((tenantId, kind));
        }

        return Task.CompletedTask;
    }
}


