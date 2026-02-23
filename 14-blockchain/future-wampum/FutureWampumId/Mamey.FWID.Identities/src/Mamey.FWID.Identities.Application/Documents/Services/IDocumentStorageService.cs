using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Application.Documents.Services;

/// <summary>
/// Service for storing identity documents in MinIO.
/// </summary>
internal interface IDocumentStorageService
{
    /// <summary>
    /// Stores a document in MinIO.
    /// </summary>
    Task<string> StoreDocumentAsync(
        IdentityId identityId,
        DocumentType documentType,
        byte[] pdfContent,
        string documentNumber,
        Dictionary<string, string> metadata,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a document from MinIO.
    /// </summary>
    Task<byte[]> RetrieveDocumentAsync(
        IdentityId identityId,
        DocumentType documentType,
        string documentNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a presigned URL for document access.
    /// </summary>
    Task<string> GetPresignedUrlAsync(
        IdentityId identityId,
        DocumentType documentType,
        string documentNumber,
        int expirySeconds = 3600,
        CancellationToken cancellationToken = default);
}
