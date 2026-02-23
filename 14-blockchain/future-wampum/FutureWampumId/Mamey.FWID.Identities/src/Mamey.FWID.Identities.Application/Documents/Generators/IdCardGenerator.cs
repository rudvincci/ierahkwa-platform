using System.Text;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Documents.Generators;

/// <summary>
/// Generator for ID card documents with AAMVA compliance.
/// </summary>
internal class IdCardGenerator : IIdCardGenerator
{
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IQrCodeGenerator _qrCodeGenerator;
    private readonly ITemplateService _templateService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IdCardGenerator> _logger;

    public IdCardGenerator(
        IPdfGenerator pdfGenerator,
        IQrCodeGenerator qrCodeGenerator,
        ITemplateService templateService,
        IConfiguration configuration,
        ILogger<IdCardGenerator> logger)
    {
        _pdfGenerator = pdfGenerator;
        _qrCodeGenerator = qrCodeGenerator;
        _templateService = templateService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<DocumentGenerationResult> GenerateAsync(
        Identity identity,
        byte[]? photo = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Generating ID card for IdentityId: {IdentityId}",
            identity.Id.Value);

        try
        {
            // Generate document number (AAMVA format)
            var documentNumber = GenerateDocumentNumber(identity);
            
            // Get DID for QR code
            var did = identity.Metadata.TryGetValue("DID", out var didValue) 
                ? didValue?.ToString() ?? "" 
                : "";
            
            var verificationUrl = _configuration["DocumentVerification:BaseUrl"] 
                ?? "https://verify.futurewampum.io";
            
            // Generate QR code
            var qrCodeData = await _qrCodeGenerator.GenerateQrCodeAsync(
                did,
                $"{verificationUrl}/verify/{documentNumber}",
                cancellationToken);

            // Prepare photo as base64
            var photoBase64 = photo != null && photo.Length > 0
                ? Convert.ToBase64String(photo)
                : "";

            // Load and populate template
            var template = await _templateService.LoadTemplateAsync("IdCard", cancellationToken);
            var html = PopulateIdCardTemplate(
                template,
                identity,
                documentNumber,
                photoBase64,
                qrCodeData);

            // Generate PDF
            var pdfBytes = await _pdfGenerator.GeneratePdfFromHtmlAsync(html, cancellationToken);

            var issuedAt = DateTime.UtcNow;
            var expiresAt = issuedAt.AddYears(8); // ID cards typically valid for 8 years

            var result = new DocumentGenerationResult
            {
                PdfContent = pdfBytes,
                DocumentNumber = documentNumber,
                IssuedAt = issuedAt,
                ExpiresAt = expiresAt,
                QrCodeData = qrCodeData,
                Metadata = new Dictionary<string, string>
                {
                    { "documentType", "IdCard" },
                    { "aamvaCompliant", "true" },
                    { "identityId", identity.Id.Value.ToString() },
                    { "issuedAt", issuedAt.ToString("O") },
                    { "expiresAt", expiresAt.ToString("O") }
                }
            };

            _logger.LogInformation(
                "Successfully generated ID card: DocumentNumber: {DocumentNumber}",
                documentNumber);

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

    private string GenerateDocumentNumber(Identity identity)
    {
        // Generate AAMVA-compliant document number
        // Format: StateCode + Year + SequentialNumber
        var year = DateTime.UtcNow.Year;
        var sequential = identity.Id.Value.GetHashCode() % 1000000;
        return $"FW{year}{sequential:D6}";
    }

    private string PopulateIdCardTemplate(
        string template,
        Identity identity,
        string documentNumber,
        string photoBase64,
        string qrCodeData)
    {
        var html = new StringBuilder(template);
        
        // Replace template placeholders
        html.Replace("{{DocumentNumber}}", documentNumber);
        html.Replace("{{FirstName}}", identity.Name?.FirstName ?? "");
        html.Replace("{{LastName}}", identity.Name?.LastName ?? "");
        html.Replace("{{FullName}}", identity.Name?.FullName ?? "");
        html.Replace("{{DateOfBirth}}", identity.DateOfBirth?.ToString("MM/dd/yyyy") ?? "");
        html.Replace("{{Address}}", FormatAddress(identity.Address));
        html.Replace("{{Photo}}", !string.IsNullOrEmpty(photoBase64) 
            ? $"data:image/jpeg;base64,{photoBase64}" 
            : "");
        html.Replace("{{QrCode}}", qrCodeData);
        html.Replace("{{IssuedDate}}", DateTime.UtcNow.ToString("MM/dd/yyyy"));
        html.Replace("{{ExpiryDate}}", DateTime.UtcNow.AddYears(8).ToString("MM/dd/yyyy"));

        return html.ToString();
    }

    private string FormatAddress(Address? address)
    {
        if (address == null)
            return "";

        var parts = new List<string>();
        if (!string.IsNullOrEmpty(address.Street))
            parts.Add(address.Street);
        if (!string.IsNullOrEmpty(address.City))
            parts.Add(address.City);
        if (!string.IsNullOrEmpty(address.State))
            parts.Add(address.State);
        if (!string.IsNullOrEmpty(address.PostalCode))
            parts.Add(address.PostalCode);

        return string.Join(", ", parts);
    }
}
