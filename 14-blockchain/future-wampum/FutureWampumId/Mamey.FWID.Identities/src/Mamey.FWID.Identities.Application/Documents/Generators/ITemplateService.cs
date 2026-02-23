namespace Mamey.FWID.Identities.Application.Documents.Generators;

/// <summary>
/// Service for loading HTML templates for document generation.
/// </summary>
internal interface ITemplateService
{
    /// <summary>
    /// Loads a template by name.
    /// </summary>
    Task<string> LoadTemplateAsync(string templateName, CancellationToken cancellationToken = default);
}
