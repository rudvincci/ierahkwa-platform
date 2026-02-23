namespace Mamey.Templates;

public interface ITemplateRepository
{
    Task<TemplateBlob> GetAsync(TemplateId templateId, int version, CancellationToken ct);
    Task<TemplateBlob> GetAsync(string templateName, int version, CancellationToken ct);
    Task<int> GetLatestVersionAsync(TemplateId templateId, CancellationToken ct);
    Task<int> GetLatestVersionAsync(string templateName, CancellationToken ct);
    
}