using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Application.Documents;

/// <summary>
/// Service for generating identity documents (ID cards, passports) as PDFs.
/// </summary>
public interface IDocumentGenerationService
{
    /// <summary>
    /// Generates an ID card PDF for an identity.
    /// </summary>
    Task<DocumentGenerationResult> GenerateIdCardAsync(
        Identity identity,
        byte[]? photo = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a passport PDF for an identity.
    /// </summary>
    Task<DocumentGenerationResult> GeneratePassportAsync(
        Identity identity,
        byte[]? photo = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a generated document in MinIO.
    /// </summary>
    Task<string> StoreDocumentAsync(
        DocumentGenerationResult document,
        IdentityId identityId,
        DocumentType documentType,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of document generation.
/// </summary>
public class DocumentGenerationResult
{
    public byte[] PdfContent { get; set; } = null!;
    public string DocumentNumber { get; set; } = null!;
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? QrCodeData { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Type of identity document.
/// </summary>
public enum DocumentType
{
    IdCard,
    Passport
}
