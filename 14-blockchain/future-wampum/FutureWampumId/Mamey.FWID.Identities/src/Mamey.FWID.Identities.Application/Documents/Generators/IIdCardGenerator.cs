using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Documents.Generators;

/// <summary>
/// Generator for ID card documents.
/// </summary>
internal interface IIdCardGenerator
{
    /// <summary>
    /// Generates an ID card PDF.
    /// </summary>
    Task<DocumentGenerationResult> GenerateAsync(
        Identity identity,
        byte[]? photo = null,
        CancellationToken cancellationToken = default);
}
