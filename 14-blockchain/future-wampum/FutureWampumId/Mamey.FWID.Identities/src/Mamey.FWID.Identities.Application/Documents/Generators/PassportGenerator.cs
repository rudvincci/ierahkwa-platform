using System.Text;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Documents.Generators;

/// <summary>
/// Generator for passport documents.
/// </summary>
internal class PassportGenerator : IPassportGenerator
{
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IQrCodeGenerator _qrCodeGenerator;
    private readonly ITemplateService _templateService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PassportGenerator> _logger;

    public PassportGenerator(
        IPdfGenerator pdfGenerator,
        IQrCodeGenerator qrCodeGenerator,
        ITemplateService templateService,
        IConfiguration configuration,
        ILogger<PassportGenerator> logger)
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
            "Generating passport for IdentityId: {IdentityId}",
            identity.Id.Value);

        try
        {
            // Generate passport number
            var passportNumber = GeneratePassportNumber(identity);
            
            // Get DID for QR code
            var did = identity.Metadata.TryGetValue("DID", out var didValue) 
                ? didValue?.ToString() ?? "" 
                : "";
            
            var verificationUrl = _configuration["DocumentVerification:BaseUrl"] 
                ?? "https://verify.futurewampum.io";
            
            // Generate QR code
            var qrCodeData = await _qrCodeGenerator.GenerateQrCodeAsync(
                did,
                $"{verificationUrl}/verify/{passportNumber}",
                cancellationToken);

            // Prepare photo as base64
            var photoBase64 = photo != null && photo.Length > 0
                ? Convert.ToBase64String(photo)
                : "";

            // Load and populate template
            var template = await _templateService.LoadTemplateAsync("Passport", cancellationToken);
            var html = PopulatePassportTemplate(
                template,
                identity,
                passportNumber,
                photoBase64,
                qrCodeData,
                did);

            // Generate PDF
            var pdfBytes = await _pdfGenerator.GeneratePdfFromHtmlAsync(html, cancellationToken);

            var issuedAt = DateTime.UtcNow;
            var expiresAt = issuedAt.AddYears(10); // Passports typically valid for 10 years

            var result = new DocumentGenerationResult
            {
                PdfContent = pdfBytes,
                DocumentNumber = passportNumber,
                IssuedAt = issuedAt,
                ExpiresAt = expiresAt,
                QrCodeData = qrCodeData,
                Metadata = new Dictionary<string, string>
                {
                    { "documentType", "Passport" },
                    { "identityId", identity.Id.Value.ToString() },
                    { "issuedAt", issuedAt.ToString("O") },
                    { "expiresAt", expiresAt.ToString("O") }
                }
            };

            _logger.LogInformation(
                "Successfully generated passport: PassportNumber: {PassportNumber}",
                passportNumber);

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

    private string GeneratePassportNumber(Identity identity)
    {
        // Generate passport number (format: FW + Year + Sequential)
        var year = DateTime.UtcNow.Year;
        var sequential = Math.Abs(identity.Id.Value.GetHashCode()) % 10000000;
        return $"FW{year}{sequential:D7}";
    }

    private string PopulatePassportTemplate(
        string template,
        Identity identity,
        string passportNumber,
        string photoBase64,
        string qrCodeData,
        string did)
    {
        var html = new StringBuilder(template);
        
        // Replace template placeholders
        html.Replace("{{PassportNumber}}", passportNumber);
        html.Replace("{{FirstName}}", identity.Name?.FirstName ?? "");
        html.Replace("{{LastName}}", identity.Name?.LastName ?? "");
        html.Replace("{{FullName}}", identity.Name?.FullName ?? "");
        html.Replace("{{DateOfBirth}}", identity.DateOfBirth?.ToString("dd MMM yyyy") ?? "");
        html.Replace("{{PlaceOfBirth}}", identity.Metadata.TryGetValue("PlaceOfBirth", out var pob) 
            ? pob?.ToString() ?? "" 
            : "");
        html.Replace("{{Nationality}}", identity.Metadata.TryGetValue("Nationality", out var nat) 
            ? nat?.ToString() ?? "FutureWampum" 
            : "FutureWampum");
        html.Replace("{{Photo}}", !string.IsNullOrEmpty(photoBase64) 
            ? $"data:image/jpeg;base64,{photoBase64}" 
            : "");
        html.Replace("{{QrCode}}", qrCodeData);
        html.Replace("{{DID}}", did);
        html.Replace("{{IssuedDate}}", DateTime.UtcNow.ToString("dd MMM yyyy"));
        html.Replace("{{ExpiryDate}}", DateTime.UtcNow.AddYears(10).ToString("dd MMM yyyy"));

        return html.ToString();
    }
}
