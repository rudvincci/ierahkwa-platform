using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Models.Requests;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Documents.Services;

/// <summary>
/// Service for storing identity documents in MinIO.
/// </summary>
internal class DocumentStorageService : IDocumentStorageService
{
    private readonly IObjectService _objectService;
    private readonly IPresignedUrlService _presignedUrlService;
    private readonly ILogger<DocumentStorageService> _logger;
    private const string BucketName = "fwid-documents";

    public DocumentStorageService(
        IObjectService objectService,
        IPresignedUrlService presignedUrlService,
        ILogger<DocumentStorageService> logger)
    {
        _objectService = objectService;
        _presignedUrlService = presignedUrlService;
        _logger = logger;
    }

    public async Task<string> StoreDocumentAsync(
        IdentityId identityId,
        DocumentType documentType,
        byte[] pdfContent,
        string documentNumber,
        Dictionary<string, string> metadata,
        CancellationToken cancellationToken = default)
    {
        var objectName = GenerateObjectName(identityId, documentType, documentNumber);

        _logger.LogInformation(
            "Storing document: IdentityId={IdentityId}, Type={DocumentType}, DocumentNumber={DocumentNumber}, ObjectName={ObjectName}",
            identityId.Value,
            documentType,
            documentNumber,
            objectName);

        try
        {
            // Add standard metadata
            var fullMetadata = new Dictionary<string, string>(metadata)
            {
                { "identityId", identityId.Value.ToString() },
                { "documentType", documentType.ToString() },
                { "documentNumber", documentNumber },
                { "storedAt", DateTime.UtcNow.ToString("O") }
            };

            await _objectService.UploadBytesAsync(
                BucketName,
                objectName,
                pdfContent,
                "application/pdf",
                fullMetadata,
                cancellationToken);

            _logger.LogInformation(
                "Successfully stored document: ObjectName={ObjectName}, Size={Size} bytes",
                objectName,
                pdfContent.Length);

            return objectName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to store document: IdentityId={IdentityId}, DocumentNumber={DocumentNumber}",
                identityId.Value,
                documentNumber);
            throw;
        }
    }

    public async Task<byte[]> RetrieveDocumentAsync(
        IdentityId identityId,
        DocumentType documentType,
        string documentNumber,
        CancellationToken cancellationToken = default)
    {
        var objectName = GenerateObjectName(identityId, documentType, documentNumber);

        _logger.LogInformation(
            "Retrieving document: IdentityId={IdentityId}, Type={DocumentType}, DocumentNumber={DocumentNumber}",
            identityId.Value,
            documentType,
            documentNumber);

        try
        {
            var content = await _objectService.DownloadAsBytesAsync(
                BucketName,
                objectName,
                null,
                cancellationToken);

            _logger.LogInformation(
                "Successfully retrieved document: ObjectName={ObjectName}, Size={Size} bytes",
                objectName,
                content.Length);

            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to retrieve document: IdentityId={IdentityId}, DocumentNumber={DocumentNumber}",
                identityId.Value,
                documentNumber);
            throw;
        }
    }

    public async Task<string> GetPresignedUrlAsync(
        IdentityId identityId,
        DocumentType documentType,
        string documentNumber,
        int expirySeconds = 3600,
        CancellationToken cancellationToken = default)
    {
        var objectName = GenerateObjectName(identityId, documentType, documentNumber);

        _logger.LogInformation(
            "Generating presigned URL: IdentityId={IdentityId}, Type={DocumentType}, DocumentNumber={DocumentNumber}, Expiry={ExpirySeconds}s",
            identityId.Value,
            documentType,
            documentNumber,
            expirySeconds);

        try
        {
            var request = new PresignedUrlRequest
            {
                BucketName = BucketName,
                ObjectName = objectName,
                ExpiresInSeconds = expirySeconds
            };

            var result = await _presignedUrlService.PresignedGetObjectAsync(request, cancellationToken);

            _logger.LogInformation(
                "Successfully generated presigned URL: ObjectName={ObjectName}",
                objectName);

            return result.Url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to generate presigned URL: IdentityId={IdentityId}, DocumentNumber={DocumentNumber}",
                identityId.Value,
                documentNumber);
            throw;
        }
    }

    private static string GenerateObjectName(IdentityId identityId, DocumentType documentType, string documentNumber)
    {
        return $"{identityId.Value}/{documentType}/{documentNumber}.pdf";
    }
}
