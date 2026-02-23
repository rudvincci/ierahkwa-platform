namespace Mamey.Templates.Registries;

public interface ITemplateRegistry
{
    Task<DocumentTemplate?> FindAsync(TemplateId id, int version, CancellationToken ct = default);
    Task<int> LatestApprovedVersionAsync(TemplateId id, CancellationToken ct = default);
}