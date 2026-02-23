namespace Mamey.Portal.Shared.Storage.Templates;

public interface IDocumentTemplateStore
{
    Task<string?> GetTemplateAsync(string tenantId, string kind, CancellationToken ct = default);

    Task<IReadOnlyList<DocumentTemplateSummary>> ListTemplatesAsync(string tenantId, int take = 200, CancellationToken ct = default);

    Task UpsertTemplateAsync(string tenantId, string kind, string templateHtml, CancellationToken ct = default);

    Task DeleteTemplateAsync(string tenantId, string kind, CancellationToken ct = default);
}


