using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Documents.Services;

/// <summary>
/// Service for generating identity documents (ID cards, passports) as PDFs.
/// </summary>
internal class DocumentGenerationService : IDocumentGenerationService
{
    private readonly IIdCardGenerator _idCardGenerator;
    private readonly IPassportGenerator _passportGenerator;
    private readonly IDocumentStorageService _storageService;
    private readonly ILogger<DocumentGenerationService> _logger;

    public DocumentGenerationService(
        IIdCardGenerator idCardGenerator,
        IPassportGenerator passportGenerator,
        IDocumentStorageService storageService,
        ILogger<DocumentGenerationService> logger)
    {
        _idCardGenerator = idCardGenerator;
        _passportGenerator = passportGenerator;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<DocumentGenerationResult> GenerateIdCardAsync(
        Identity identity,
        byte[]? photo = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Generating ID card for IdentityId: {IdentityId}",
            identity.Id.Value);

        try
        {
            var result = await _idCardGenerator.GenerateAsync(identity, photo, cancellationToken);
            
            _logger.LogInformation(
                "Successfully generated ID card for IdentityId: {IdentityId}, DocumentNumber: {DocumentNumber}",
                identity.Id.Value,
                result.DocumentNumber);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to generate ID card for IdentityId: {IdentityId}",
                identity.Id.Value);
            throw;
        }
    }

    public async Task<DocumentGenerationResult> GeneratePassportAsync(
        Identity identity,
        byte[]? photo = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Generating passport for IdentityId: {IdentityId}",
            identity.Id.Value);

        try
        {
            var result = await _passportGenerator.GenerateAsync(identity, photo, cancellationToken);
            
            _logger.LogInformation(
                "Successfully generated passport for IdentityId: {IdentityId}, DocumentNumber: {DocumentNumber}",
                identity.Id.Value,
                result.DocumentNumber);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to generate passport for IdentityId: {IdentityId}",
                identity.Id.Value);
            throw;
        }
    }

    public async Task<string> StoreDocumentAsync(
        DocumentGenerationResult document,
        IdentityId identityId,
        DocumentType documentType,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Storing document for IdentityId: {IdentityId}, Type: {DocumentType}, DocumentNumber: {DocumentNumber}",
            identityId.Value,
            documentType,
            document.DocumentNumber);

        try
        {
            var objectName = await _storageService.StoreDocumentAsync(
                identityId,
                documentType,
                document.PdfContent,
                document.DocumentNumber,
                document.Metadata,
                cancellationToken);

            _logger.LogInformation(
                "Successfully stored document for IdentityId: {IdentityId}, ObjectName: {ObjectName}",
                identityId.Value,
                objectName);

            return objectName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to store document for IdentityId: {IdentityId}",
                identityId.Value);
            throw;
        }
    }
}
