using Mamey.AmvvaStandards;
using Mamey.Barcode;
using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class AamvaBarcodeService : IAamvaBarcodeService
{
    private readonly IBarcodeService _barcodeService;
    private readonly IStandardsComplianceValidator _validator;
    private const int AAMVA_SECURITY_LEVEL = 5; // Minimum Level 5 for AAMVA compliance
    private const int AAMVA_COLUMNS = 20; // Standard column count
    private const int AAMVA_SCALE = 2; // Standard scale
    private const int AAMVA_RATIO = 3; // Standard ratio

    public AamvaBarcodeService(
        IBarcodeService barcodeService,
        IStandardsComplianceValidator validator)
    {
        _barcodeService = barcodeService;
        _validator = validator;
    }

    public async Task<byte[]?> GenerateDriverLicenseBarcodeAsync(DriverLicenseCard card, CancellationToken cancellationToken = default)
    {
        // Sanitize and truncate fields before validation
        SanitizeAndTruncateCardFields(card);

        // Validate card fields
        if (!_validator.ValidateAamvaFields(card, out var errors))
        {
            var errorMessages = string.Join("; ", errors.Where(e => e.Severity == ValidationSeverity.Error).Select(e => e.Message));
            throw new InvalidOperationException($"Driver license card validation failed: {errorMessages}");
        }

        // Encode card data using AAMVA FieldEncodingHelper
        var encodedData = FieldEncodingHelper.EncodeDriverLicenseCard(card);

        // Generate PDF417 barcode image
        return await GenerateBarcodeFromEncodedDataAsync(encodedData, cancellationToken);
    }

    public async Task<byte[]?> GenerateIdentificationCardBarcodeAsync(IdentificationCard card, CancellationToken cancellationToken = default)
    {
        // Sanitize and truncate fields before validation
        SanitizeAndTruncateCardFields(card);

        // Validate card fields
        if (!_validator.ValidateAamvaFields(card, out var errors))
        {
            var errorMessages = string.Join("; ", errors.Where(e => e.Severity == ValidationSeverity.Error).Select(e => e.Message));
            throw new InvalidOperationException($"Identification card validation failed: {errorMessages}");
        }

        // Encode card data using AAMVA FieldEncodingHelper
        var encodedData = FieldEncodingHelper.EncodeIdentificationCard(card);

        // Generate PDF417 barcode image
        return await GenerateBarcodeFromEncodedDataAsync(encodedData, cancellationToken);
    }

    public async Task<byte[]?> GenerateBarcodeFromEncodedDataAsync(string aamvaEncodedData, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(aamvaEncodedData))
        {
            throw new ArgumentException("AAMVA encoded data cannot be null or empty.", nameof(aamvaEncodedData));
        }

        // Generate PDF417 barcode with AAMVA-compliant settings
        // Security level 5+ for AAMVA compliance
        var barcodeBytes = await _barcodeService.GeneratePDF417Async(
            data: aamvaEncodedData,
            columns: AAMVA_COLUMNS,
            securityLevel: AAMVA_SECURITY_LEVEL,
            scale: AAMVA_SCALE,
            ratio: AAMVA_RATIO,
            padding: 5);

        return barcodeBytes;
    }

    private void SanitizeAndTruncateCardFields(BaseAamvaCard card)
    {
        // Sanitize and truncate name fields
        if (!string.IsNullOrWhiteSpace(card.FamilyName))
        {
            card.FamilyName = _validator.TruncateAamvaField(
                _validator.SanitizeAamvaField(card.FamilyName),
                35,
                FieldType.Name);
        }
        if (!string.IsNullOrWhiteSpace(card.GivenName))
        {
            card.GivenName = _validator.TruncateAamvaField(
                _validator.SanitizeAamvaField(card.GivenName),
                35,
                FieldType.Name);
        }
        if (!string.IsNullOrWhiteSpace(card.MiddleNames))
        {
            card.MiddleNames = _validator.TruncateAamvaField(
                _validator.SanitizeAamvaField(card.MiddleNames),
                35,
                FieldType.Name);
        }

        // Sanitize and truncate address fields
        if (!string.IsNullOrWhiteSpace(card.StreetAddress))
        {
            card.StreetAddress = _validator.TruncateAamvaField(
                _validator.SanitizeAamvaField(card.StreetAddress),
                35,
                FieldType.Address);
        }
        if (!string.IsNullOrWhiteSpace(card.City))
        {
            card.City = _validator.TruncateAamvaField(
                _validator.SanitizeAamvaField(card.City),
                35,
                FieldType.Address);
        }
        if (!string.IsNullOrWhiteSpace(card.PostalCode))
        {
            card.PostalCode = _validator.TruncateAamvaField(
                _validator.SanitizeAamvaField(card.PostalCode),
                11,
                FieldType.PostalCode);
        }
    }
}

