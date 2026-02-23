using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Documents.Generators;

/// <summary>
/// Generator for passport documents.
/// </summary>
internal interface IPassportGenerator
{
    /// <summary>
    /// Generates a passport PDF.
    /// </summary>
    Task<DocumentGenerationResult> GenerateAsync(
        Identity identity,
        byte[]? photo = null,
        CancellationToken cancellationToken = default);
}
