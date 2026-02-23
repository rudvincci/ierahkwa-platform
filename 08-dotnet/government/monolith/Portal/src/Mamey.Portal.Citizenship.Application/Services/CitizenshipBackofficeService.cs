using Microsoft.Extensions.Logging;
using Mamey.AmvvaStandards;
using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Citizenship.Application.Requests;
using Mamey.Portal.Shared.Storage;
using Mamey.Portal.Shared.Storage.DocumentNaming;
using Mamey.Portal.Shared.Storage.Templates;
using Mamey.Portal.Shared.Tenancy;
using Mamey.Word;
using System.Text;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class CitizenshipBackofficeService : ICitizenshipBackofficeService
{
    private readonly ICitizenshipBackofficeStore _store;
    private readonly ITenantContext _tenant;
    private readonly ICitizenshipApplicationService _citizenSubmit;
    private readonly IObjectStorage _storage;
    private readonly IDocumentNamingStore _namingStore;
    private readonly IDocumentNamingService _naming;
    private readonly IDocumentTemplateStore _templates;
    private readonly ICitizenshipRealtimeNotifier _realtime;
    private readonly IAamvaBarcodeService _aamvaBarcode;
    private readonly IMrzGenerator _mrzGenerator;
    private readonly IDocumentNumberGenerator _documentNumberGen;
    private readonly IDocumentValidityCalculator _validityCalculator;
    private readonly ICitizenshipStatusService _statusService;
    private readonly IWordService _word;
    private readonly ILogger<CitizenshipBackofficeService> _logger;

    public CitizenshipBackofficeService(
        ICitizenshipBackofficeStore store,
        ITenantContext tenant,
        ICitizenshipApplicationService citizenSubmit,
        IObjectStorage storage,
        IDocumentNamingStore namingStore,
        IDocumentNamingService naming,
        IDocumentTemplateStore templates,
        ICitizenshipRealtimeNotifier realtime,
        IAamvaBarcodeService aamvaBarcode,
        IMrzGenerator mrzGenerator,
        IDocumentNumberGenerator documentNumberGen,
        IDocumentValidityCalculator validityCalculator,
        ICitizenshipStatusService statusService,
        IWordService word,
        ILogger<CitizenshipBackofficeService> logger)
    {
        _store = store;
        _tenant = tenant;
        _citizenSubmit = citizenSubmit;
        _storage = storage;
        _namingStore = namingStore;
        _naming = naming;
        _templates = templates;
        _realtime = realtime;
        _aamvaBarcode = aamvaBarcode;
        _mrzGenerator = mrzGenerator;
        _documentNumberGen = documentNumberGen;
        _validityCalculator = validityCalculator;
        _statusService = statusService;
        _word = word;
        _logger = logger;
    }

    public Task<IReadOnlyList<BackofficeApplicationSummary>> GetRecentApplicationsAsync(int take = 50, CancellationToken ct = default)
        => _store.GetRecentApplicationsAsync(_tenant.TenantId, take, ct);

    public Task<BackofficeApplicationDetails?> GetApplicationAsync(Guid id, CancellationToken ct = default)
        => _store.GetApplicationAsync(_tenant.TenantId, id, ct);

    public async Task<string> CreateManualApplicationAsync(
        SubmitCitizenshipApplicationRequest request,
        IReadOnlyList<UploadFile> personalDocuments,
        UploadFile passportPhoto,
        UploadFile signatureImage,
        CancellationToken ct = default)
    {
        // Minimal Phase 4.1.2 implementation:
        // Reuse the citizen submission pipeline (validation + storage + persistence),
        // then mark as SubmittedByAgent for backoffice visibility.
        var appNo = await _citizenSubmit.SubmitAsync(request, personalDocuments, passportPhoto, signatureImage, ct);

        await _store.UpdateApplicationStatusByNumberAsync(
            _tenant.TenantId,
            appNo,
            "SubmittedByAgent",
            DateTimeOffset.UtcNow,
            ct);

        return appNo;
    }

    public Task<IReadOnlyList<IssuedDocumentSummary>> GetIssuedDocumentsAsync(Guid applicationId, CancellationToken ct = default)
        => _store.GetIssuedDocumentsAsync(_tenant.TenantId, applicationId, ct);

    public async Task<IssuedDocumentSummary> GenerateCitizenshipCertificateAsync(Guid applicationId, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var app = await _store.GetApplicationForCertificateAsync(tenantId, applicationId, ct);

        if (app is null)
        {
            throw new InvalidOperationException("Application not found (or not in this tenant).");
        }

        var now = DateTimeOffset.UtcNow;
        var fullName = $"{app.FirstName} {app.LastName}";
        var docxValues = new Dictionary<string, object>
        {
            ["TenantId"] = tenantId,
            ["ApplicationNumber"] = app.ApplicationNumber,
            ["FullName"] = fullName,
            ["DateOfBirth"] = app.DateOfBirth.ToString("yyyy-MM-dd"),
            ["IssuedAt"] = now.ToString("yyyy-MM-dd")
        };

        var docx = await TryGenerateDocxAsync(
            "CitizenshipCertificate",
            $"citizenship-certificate-{app.ApplicationNumber}",
            docxValues,
            ct);

        byte[] bytes;
        string fileName;
        string contentType;

        if (docx is not null)
        {
            bytes = docx.Bytes;
            fileName = docx.FileName;
            contentType = docx.ContentType;
        }
        else
        {
            var template = await _templates.GetTemplateAsync(tenantId, "CitizenshipCertificate", ct);
            var html = string.IsNullOrWhiteSpace(template)
                ? RenderCitizenshipCertificateHtml(
                    tenantId,
                    app.ApplicationNumber,
                    fullName,
                    app.DateOfBirth,
                    now)
                : RenderFromTemplate(
                    template,
                    tenantId,
                    app.ApplicationNumber,
                    fullName,
                    app.DateOfBirth,
                    now);

            bytes = Encoding.UTF8.GetBytes(html);
            fileName = $"citizenship-certificate-{app.ApplicationNumber}.html";
            contentType = "text/html; charset=utf-8";
        }

        var bucket = ObjectStorageKeys.TenantBucket(tenantId);
        var naming = await _namingStore.GetAsync(tenantId, ct);
        var key = _naming.GenerateObjectKey(
            naming,
            new DocumentNamingContext(
                TenantId: tenantId,
                ApplicationNumber: app.ApplicationNumber,
                ApplicationId: app.Id,
                Kind: "CitizenshipCertificate",
                OriginalFileName: fileName,
                NowUtc: now));

        await _storage.PutAsync(
            bucket,
            key,
            new MemoryStream(bytes),
            bytes.Length,
            contentType,
            new Dictionary<string, string>
            {
                ["mamey-tenant"] = tenantId,
                ["mamey-domain"] = "citizenship",
                ["mamey-kind"] = "CitizenshipCertificate",
                ["mamey-application-number"] = app.ApplicationNumber,
                ["mamey-application-id"] = app.Id.ToString("N"),
            },
            ct);

        var created = await _store.InsertIssuedDocumentAsync(
            tenantId,
            new IssuedDocumentCreate(
                app.Id,
                "CitizenshipCertificate",
                DocumentNumber: null,
                ExpiresAt: null,
                FileName: fileName,
                ContentType: contentType,
                Size: bytes.Length,
                StorageBucket: bucket,
                StorageKey: key,
                CreatedAt: now),
            ct);

        // Touch application timestamp so citizen portal / backoffice can reflect change consistently.
        await _store.UpdateApplicationUpdatedAtAsync(tenantId, applicationId, now, ct);

        return created;
    }

    private const string DocxContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
    private const string DocxTemplatePrefix = "Mamey.Portal.Citizenship.Application.Templates";
    private static readonly string TemplateAssemblyName = typeof(CitizenshipBackofficeService).Assembly.GetName().Name ?? "Mamey.Portal.Citizenship.Application";
    private static readonly string[] EmbeddedResources = typeof(CitizenshipBackofficeService).Assembly.GetManifestResourceNames();

    private sealed record GeneratedDocument(byte[] Bytes, string FileName, string ContentType);

    private async Task<GeneratedDocument?> TryGenerateDocxAsync(
        string kind,
        string fileNameStem,
        IDictionary<string, object> values,
        CancellationToken ct)
    {
        var resource = GetDocxResource(kind);
        if (!ResourceExists(resource))
        {
            return null;
        }

        try
        {
            var outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.docx");
            var props = new WordDocumentProperties
            {
                Title = kind,
                Author = "Mamey.Portal",
                Company = _tenant.TenantId,
                Subject = kind,
                Category = "Citizenship",
            };

            await _word.GenerateWordToPathAsync(resource, TemplateAssemblyName, values, props, null, outputPath);

            var bytes = await File.ReadAllBytesAsync(outputPath, ct);
            TryDeleteTempFile(outputPath);

            return new GeneratedDocument(bytes, $"{fileNameStem}.docx", DocxContentType);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "DOCX template generation failed for {Kind}. Falling back to HTML.", kind);
            return null;
        }
    }

    private static string GetDocxResource(string kind)
    {
        var baseKind = GetBaseKind(kind);
        return $"{DocxTemplatePrefix}.{baseKind}.docx";
    }

    private static string GetBaseKind(string kind)
    {
        kind = (kind ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(kind))
        {
            return string.Empty;
        }

        var idx = kind.IndexOf(':');
        return idx > 0 ? kind[..idx].Trim() : kind;
    }

    private static bool ResourceExists(string resource)
        => Array.Exists(EmbeddedResources, name => string.Equals(name, resource, StringComparison.Ordinal));

    private static void TryDeleteTempFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
        }
    }

    private static string RenderFromTemplate(
        string templateHtml,
        string tenantId,
        string applicationNumber,
        string fullName,
        DateOnly dateOfBirth,
        DateTimeOffset issuedAt)
    {
        return TemplateTokenRenderer.Apply(templateHtml, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["TenantId"] = tenantId,
            ["ApplicationNumber"] = applicationNumber,
            ["FullName"] = fullName,
            ["DateOfBirth"] = dateOfBirth.ToString("yyyy-MM-dd"),
            ["IssuedAt"] = issuedAt.ToString("yyyy-MM-dd"),
        });
    }

    private static string ApplyTokens(string templateHtml, IReadOnlyDictionary<string, string> tokens)
        => TemplateTokenRenderer.Apply(templateHtml, tokens);

    private async Task<string?> GetFirstTemplateAsync(string tenantId, IReadOnlyList<string> candidateKinds, CancellationToken ct)
    {
        foreach (var kind in candidateKinds)
        {
            var html = await _templates.GetTemplateAsync(tenantId, kind, ct);
            if (!string.IsNullOrWhiteSpace(html))
            {
                return html;
            }
        }

        return null;
    }

    public async Task<IssuedDocumentSummary> IssuePassportAsync(Guid applicationId, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;

        // If already issued, return the latest.
        var existing = await _store.GetIssuedDocumentAsync(tenantId, applicationId, "Passport", ct);
        if (existing is not null)
        {
            var appCheck = await _store.GetApplicationForPassportAsync(tenantId, applicationId, ct);
            if (appCheck is null)
            {
                throw new InvalidOperationException("Application not found (or not in this tenant).");
            }

            return existing;
        }

        var app = await _store.GetApplicationForPassportAsync(tenantId, applicationId, ct);
        if (app is null)
        {
            throw new InvalidOperationException("Application not found (or not in this tenant).");
        }

        var now = DateTimeOffset.UtcNow;
        var nowDate = now.DateTime;
        var expiresAt = now.AddYears(5);
        var expiresAtDate = expiresAt.DateTime;

        // Generate AAMVA/MRZ-compliant passport number
        var passportNumber = _documentNumberGen.GeneratePassportNumber(app.LastName, app.DateOfBirth.ToDateTime(TimeOnly.MinValue));

        // Generate MRZ for passport (passports use TD2 format, but for ID cards we use TD1)
        // For now, we'll generate TD1 format MRZ for passports as well
        string mrzLine1 = string.Empty;
        string mrzLine2 = string.Empty;
        string mrzLine3 = string.Empty;
        try
        {
            mrzLine1 = _mrzGenerator.GenerateTD1Line1(passportNumber);
            mrzLine2 = _mrzGenerator.GenerateTD1Line2(
                app.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                expiresAtDate,
                string.Empty); // Optional data
            mrzLine3 = _mrzGenerator.GenerateTD1Line3(app.LastName, app.FirstName);
        }
        catch (Exception ex)
        {
            // Log error but don't fail passport issuance if MRZ generation fails
            _logger.LogWarning(ex, "MRZ generation failed for passport application {ApplicationId}, continuing without MRZ", applicationId);
        }

        var fullName = $"{app.FirstName} {app.LastName}";
        var docxValues = new Dictionary<string, object>
        {
            ["TenantId"] = tenantId,
            ["ApplicationNumber"] = app.ApplicationNumber,
            ["PassportNumber"] = passportNumber,
            ["FullName"] = fullName,
            ["DateOfBirth"] = app.DateOfBirth.ToString("yyyy-MM-dd"),
            ["IssuedAt"] = now.ToString("yyyy-MM-dd"),
            ["ExpiresAt"] = expiresAt.ToString("yyyy-MM-dd"),
            ["MrzLine1"] = mrzLine1,
            ["MrzLine2"] = mrzLine2,
            ["MrzLine3"] = mrzLine3,
        };

        var docx = await TryGenerateDocxAsync("Passport", $"passport-{passportNumber}", docxValues, ct);

        byte[] bytes;
        string fileName;
        string contentType;

        if (docx is not null)
        {
            bytes = docx.Bytes;
            fileName = docx.FileName;
            contentType = docx.ContentType;
        }
        else
        {
            var template = await _templates.GetTemplateAsync(tenantId, "Passport", ct);
            var html = string.IsNullOrWhiteSpace(template)
                ? RenderPassportHtml(
                    tenantId,
                    passportNumber,
                    app.ApplicationNumber,
                    fullName,
                    app.DateOfBirth,
                    now,
                    expiresAt,
                    mrzLine1,
                    mrzLine2,
                    mrzLine3)
                : ApplyTokens(template, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["TenantId"] = tenantId,
                    ["Kind"] = "Passport",
                    ["Variant"] = "",
                    ["ApplicationNumber"] = app.ApplicationNumber,
                    ["FullName"] = fullName,
                    ["DateOfBirth"] = app.DateOfBirth.ToString("yyyy-MM-dd"),
                    ["IssuedAt"] = now.ToString("yyyy-MM-dd"),
                    ["DocumentNumber"] = passportNumber,
                    ["ExpiresAt"] = expiresAt.ToString("yyyy-MM-dd"),
                    ["MrzLine1"] = mrzLine1,
                    ["MrzLine2"] = mrzLine2,
                    ["MrzLine3"] = mrzLine3,
                });

            bytes = Encoding.UTF8.GetBytes(html);
            fileName = $"passport-{passportNumber}.html";
            contentType = "text/html; charset=utf-8";
        }

        var bucket = ObjectStorageKeys.TenantBucket(tenantId);
        var naming = await _namingStore.GetAsync(tenantId, ct);
        var key = _naming.GenerateObjectKey(
            naming,
            new DocumentNamingContext(
                TenantId: tenantId,
                ApplicationNumber: app.ApplicationNumber,
                ApplicationId: app.Id,
                Kind: "Passport",
                OriginalFileName: fileName,
                NowUtc: now));

        await _storage.PutAsync(
            bucket,
            key,
            new MemoryStream(bytes),
            bytes.Length,
            contentType,
            new Dictionary<string, string>
            {
                ["mamey-tenant"] = tenantId,
                ["mamey-domain"] = "citizenship",
                ["mamey-kind"] = "Passport",
                ["mamey-application-number"] = app.ApplicationNumber,
                ["mamey-application-id"] = app.Id.ToString("N"),
                ["mamey-document-number"] = passportNumber,
            },
            ct);

        var created = await _store.InsertIssuedDocumentAsync(
            tenantId,
            new IssuedDocumentCreate(
                app.Id,
                "Passport",
                passportNumber,
                expiresAt,
                fileName,
                contentType,
                bytes.Length,
                bucket,
                key,
                now),
            ct);

        // Minimal workflow wiring: mark status to reflect mandatory passport issuance.
        await _store.UpdateApplicationStatusAsync(tenantId, applicationId, "PassportIssued", now, null, ct);

        // Create or update citizenship status (Probationary) when passport is issued
        if (!string.IsNullOrWhiteSpace(app.Email))
        {
            await _statusService.CreateOrUpdateStatusAsync(app.Id, app.Email, ct);
        }

        // Realtime notify citizen (best-effort; email is our current identifier in the mock auth story).
        if (!string.IsNullOrWhiteSpace(app.Email))
        {
            await _realtime.NotifyIssuedDocumentCreatedAsync(tenantId, app.Email, app.Id, created.Id, ct);
            await _realtime.NotifyApplicationUpdatedAsync(tenantId, app.Email, app.Id, ct);
        }

        return created;
    }

    public async Task<IssuedDocumentSummary> IssueIdCardAsync(Guid applicationId, string variant, CancellationToken ct = default)
    {
        variant = string.IsNullOrWhiteSpace(variant) ? "IdentificationCard" : variant.Trim();
        var kind = $"IdCard:{variant}";
        var tenantId = _tenant.TenantId;

        var existing = await _store.GetIssuedDocumentAsync(tenantId, applicationId, kind, ct);
        if (existing is not null)
        {
            var appCheck = await _store.GetApplicationForIdCardAsync(tenantId, applicationId, ct);
            if (appCheck is null)
            {
                throw new InvalidOperationException("Application not found (or not in this tenant).");
            }

            // Backfill numbers for old rows (created before we started persisting DocumentNumber/ExpiresAt).
            if (string.IsNullOrWhiteSpace(existing.DocumentNumber))
            {
                var documentNumber = IssuedDocumentNumberGenerator.IdCard(tenantId, existing.CreatedAt, existing.ApplicationId, variant);
                var backfillExpiresAt = existing.ExpiresAt ?? existing.CreatedAt.AddYears(5);
                await _store.UpdateIssuedDocumentAsync(existing.Id, documentNumber, backfillExpiresAt, ct);
                existing = existing with { DocumentNumber = documentNumber, ExpiresAt = backfillExpiresAt };
            }

            return existing;
        }

        var app = await _store.GetApplicationForIdCardAsync(tenantId, applicationId, ct);
        if (app is null)
        {
            throw new InvalidOperationException("Application not found (or not in this tenant).");
        }

        var now = DateTimeOffset.UtcNow;
        var nowDate = now.DateTime;

        // Get citizenship status to determine document validity period
        var citizenshipStatus = await _statusService.GetStatusByApplicationIdAsync(applicationId, ct)
            ?? Application.Models.CitizenshipStatus.Probationary; // Default to Probationary for new approvals

        // Calculate expiration date based on citizenship status and document type
        var expiresAt = _validityCalculator.CalculateExpirationDate(citizenshipStatus, kind, now);
        var expiresAtDate = expiresAt.DateTime;

        // Generate AAMVA-compliant document number for MRZ
        // Use the existing card number generator for display, but generate MRZ-compatible number
        var cardNumber = IssuedDocumentNumberGenerator.IdCard(tenantId, now, app.Id, variant);

        // For MRZ, we need a 9-digit numeric document number
        // Generate a passport-style number that can be used for MRZ
        var mrzDocumentNumber = _documentNumberGen.GeneratePassportNumber(app.LastName, app.DateOfBirth.ToDateTime(TimeOnly.MinValue));

        // Create AAMVA card model - use DriverLicenseCard for driver license variants, IdentificationCard otherwise
        BaseAamvaCard aamvaCard;
        var isDriverLicense = variant.Equals("DriversLicense", StringComparison.OrdinalIgnoreCase) ||
                              variant.Equals("DriverLicense", StringComparison.OrdinalIgnoreCase);

        // Parse AAMVA fields from application data
        var heightInches = ParseHeightToInches(app.Height);
        var sex = ParseSex(app.Sex);
        var eyeColor = ParseEyeColor(app.EyeColor);
        var hairColor = ParseHairColor(app.HairColor);
        var middleNames = app.MiddleName ?? string.Empty;

        if (isDriverLicense)
        {
            aamvaCard = new DriverLicenseCard
            {
                FamilyName = app.LastName,
                GivenName = app.FirstName,
                MiddleNames = middleNames,
                DateOfBirth = app.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                IssueDate = nowDate,
                ExpirationDate = expiresAtDate,
                StreetAddress = app.AddressLine1 ?? string.Empty,
                City = app.City ?? string.Empty,
                PostalCode = app.PostalCode ?? string.Empty,
                Jurisdiction = ParseJurisdictionCode(app.Region),
                Country = IssuingCountry.USA,
                Revision = CardDesignRevision.AAMVA2013,
                DocumentDiscriminator = Guid.NewGuid().ToString("N")[..20], // Required for AAMVA 2010+
                HeightInches = heightInches,
                Sex = sex,
                EyeColor = eyeColor,
                HairColor = hairColor,
                LicenseClass = string.Empty, // Not available in current schema
                Restrictions = string.Empty,
                Endorsements = string.Empty,
                IsCommercial = false
            };
            aamvaCard.AssignCardNumber(cardNumber);
        }
        else
        {
            aamvaCard = new IdentificationCard
            {
                FamilyName = app.LastName,
                GivenName = app.FirstName,
                MiddleNames = middleNames,
                DateOfBirth = app.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                IssueDate = nowDate,
                ExpirationDate = expiresAtDate,
                StreetAddress = app.AddressLine1 ?? string.Empty,
                City = app.City ?? string.Empty,
                PostalCode = app.PostalCode ?? string.Empty,
                Jurisdiction = ParseJurisdictionCode(app.Region),
                Country = IssuingCountry.USA,
                Revision = CardDesignRevision.AAMVA2013,
                DocumentDiscriminator = Guid.NewGuid().ToString("N")[..20], // Required for AAMVA 2010+
                HeightInches = heightInches,
                Sex = sex,
                EyeColor = eyeColor,
                HairColor = hairColor,
                IsRealIdCompliant = true
            };
            aamvaCard.AssignCardNumber(cardNumber);
        }

        // Generate PDF417 barcode
        byte[]? barcodeImageBytes = null;
        string? barcodeBase64 = null;
        try
        {
            if (isDriverLicense)
            {
                barcodeImageBytes = await _aamvaBarcode.GenerateDriverLicenseBarcodeAsync((DriverLicenseCard)aamvaCard, ct);
            }
            else
            {
                barcodeImageBytes = await _aamvaBarcode.GenerateIdentificationCardBarcodeAsync((IdentificationCard)aamvaCard, ct);
            }

            if (barcodeImageBytes != null)
            {
                barcodeBase64 = Convert.ToBase64String(barcodeImageBytes);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail card issuance if barcode generation fails
            _logger.LogWarning(ex, "PDF417 barcode generation failed for application {ApplicationId}, continuing without barcode", applicationId);
        }

        // Generate MRZ
        string mrzLine1 = string.Empty;
        string mrzLine2 = string.Empty;
        string mrzLine3 = string.Empty;
        try
        {
            mrzLine1 = _mrzGenerator.GenerateTD1Line1(mrzDocumentNumber);
            mrzLine2 = _mrzGenerator.GenerateTD1Line2(
                app.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                expiresAtDate,
                string.Empty); // Optional data
            mrzLine3 = _mrzGenerator.GenerateTD1Line3(app.LastName, app.FirstName);
        }
        catch (Exception ex)
        {
            // Log error but don't fail card issuance if MRZ generation fails
            _logger.LogWarning(ex, "MRZ generation failed for application {ApplicationId}, continuing without MRZ", applicationId);
        }

        var fullName = $"{app.FirstName} {app.LastName}";
        var docxValues = new Dictionary<string, object>
        {
            ["TenantId"] = tenantId,
            ["ApplicationNumber"] = app.ApplicationNumber,
            ["CardNumber"] = cardNumber,
            ["Variant"] = variant,
            ["FullName"] = fullName,
            ["DateOfBirth"] = app.DateOfBirth.ToString("yyyy-MM-dd"),
            ["Height"] = app.Height ?? string.Empty,
            ["Sex"] = app.Sex ?? string.Empty,
            ["EyeColor"] = app.EyeColor ?? string.Empty,
            ["HairColor"] = app.HairColor ?? string.Empty,
            ["AddressLine1"] = app.AddressLine1 ?? string.Empty,
            ["City"] = app.City ?? string.Empty,
            ["Region"] = app.Region ?? string.Empty,
            ["PostalCode"] = app.PostalCode ?? string.Empty,
            ["IssuedAt"] = now.ToString("yyyy-MM-dd"),
            ["ExpiresAt"] = expiresAt.ToString("yyyy-MM-dd"),
            ["MrzLine1"] = mrzLine1,
            ["MrzLine2"] = mrzLine2,
            ["MrzLine3"] = mrzLine3,
        };

        var docx = await TryGenerateDocxAsync(kind, $"id-card-{variant}-{cardNumber}", docxValues, ct);

        byte[] bytes;
        string fileName;
        string contentType;

        if (docx is not null)
        {
            bytes = docx.Bytes;
            fileName = docx.FileName;
            contentType = docx.ContentType;
        }
        else
        {
            var template = await GetFirstTemplateAsync(tenantId, DocumentTemplateKindFallback.GetCandidateKinds(kind), ct);
            var html = string.IsNullOrWhiteSpace(template)
                ? RenderIdCardHtml(
                    tenantId,
                    kind,
                    cardNumber,
                    app.ApplicationNumber,
                    fullName,
                    app.DateOfBirth,
                    now,
                    expiresAt,
                    barcodeBase64,
                    mrzLine1,
                    mrzLine2,
                    mrzLine3)
                : ApplyTokens(template, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["TenantId"] = tenantId,
                    ["Kind"] = kind,
                    ["Variant"] = variant,
                    ["ApplicationNumber"] = app.ApplicationNumber,
                    ["FullName"] = fullName,
                    ["DateOfBirth"] = app.DateOfBirth.ToString("yyyy-MM-dd"),
                    ["IssuedAt"] = now.ToString("yyyy-MM-dd"),
                    ["DocumentNumber"] = cardNumber,
                    ["ExpiresAt"] = expiresAt.ToString("yyyy-MM-dd"),
                    ["BarcodeBase64"] = barcodeBase64 ?? string.Empty,
                    ["MrzLine1"] = mrzLine1,
                    ["MrzLine2"] = mrzLine2,
                    ["MrzLine3"] = mrzLine3,
                });

            bytes = Encoding.UTF8.GetBytes(html);
            fileName = $"id-card-{variant}-{cardNumber}.html";
            contentType = "text/html; charset=utf-8";
        }

        var bucket = ObjectStorageKeys.TenantBucket(tenantId);
        var naming = await _namingStore.GetAsync(tenantId, ct);
        var key = _naming.GenerateObjectKey(
            naming,
            new DocumentNamingContext(
                TenantId: tenantId,
                ApplicationNumber: app.ApplicationNumber,
                ApplicationId: app.Id,
                Kind: kind,
                OriginalFileName: fileName,
                NowUtc: now));

        await _storage.PutAsync(
            bucket,
            key,
            new MemoryStream(bytes),
            bytes.Length,
            contentType,
            new Dictionary<string, string>
            {
                ["mamey-tenant"] = tenantId,
                ["mamey-domain"] = "citizenship",
                ["mamey-kind"] = kind,
                ["mamey-application-number"] = app.ApplicationNumber,
                ["mamey-application-id"] = app.Id.ToString("N"),
                ["mamey-document-number"] = cardNumber,
            },
            ct);

        var created = await _store.InsertIssuedDocumentAsync(
            tenantId,
            new IssuedDocumentCreate(
                app.Id,
                kind,
                cardNumber,
                expiresAt,
                fileName,
                contentType,
                bytes.Length,
                bucket,
                key,
                now),
            ct);

        // Touch application timestamp so citizen portal / backoffice can reflect change consistently.
        await _store.UpdateApplicationUpdatedAtAsync(tenantId, applicationId, now, ct);

        var email = await _store.GetApplicationEmailAsync(tenantId, applicationId, ct);
        if (!string.IsNullOrWhiteSpace(email))
        {
            await _realtime.NotifyIssuedDocumentCreatedAsync(tenantId, email!, created.ApplicationId, created.Id, ct);
            await _realtime.NotifyApplicationUpdatedAsync(tenantId, email!, created.ApplicationId, ct);
        }

        return created;
    }

    public async Task<IssuedDocumentSummary> IssueVehicleTagAsync(Guid applicationId, string variant, CancellationToken ct = default)
    {
        variant = string.IsNullOrWhiteSpace(variant) ? "Standard" : variant.Trim();
        var kind = $"VehicleTag:{variant}";
        var tenantId = _tenant.TenantId;

        var existing = await _store.GetIssuedDocumentAsync(tenantId, applicationId, kind, ct);
        if (existing is not null)
        {
            var appCheck = await _store.GetApplicationForVehicleTagAsync(tenantId, applicationId, ct);
            if (appCheck is null)
            {
                throw new InvalidOperationException("Application not found (or not in this tenant).");
            }

            if (string.IsNullOrWhiteSpace(existing.DocumentNumber))
            {
                var documentNumber = IssuedDocumentNumberGenerator.VehicleTag(tenantId, existing.CreatedAt, existing.ApplicationId, variant);
                await _store.UpdateIssuedDocumentAsync(existing.Id, documentNumber, null, ct);
                existing = existing with { DocumentNumber = documentNumber, ExpiresAt = null };
            }

            return existing;
        }

        var app = await _store.GetApplicationForVehicleTagAsync(tenantId, applicationId, ct);
        if (app is null)
        {
            throw new InvalidOperationException("Application not found (or not in this tenant).");
        }

        var now = DateTimeOffset.UtcNow;

        // Get citizenship status to determine document validity period
        var citizenshipStatus = await _statusService.GetStatusByApplicationIdAsync(applicationId, ct)
            ?? Application.Models.CitizenshipStatus.Probationary; // Default to Probationary for new approvals

        // Calculate expiration date based on citizenship status (VehicleTag: 1yr for Citizen, 2-3yrs for others)
        var expiresAt = _validityCalculator.CalculateExpirationDate(citizenshipStatus, kind, now);

        // Include variant to avoid collisions across multiple tag variants for the same citizen/app.
        var tagNumber = IssuedDocumentNumberGenerator.VehicleTag(tenantId, now, app.Id, variant);

        var fullName = $"{app.FirstName} {app.LastName}";
        var docxValues = new Dictionary<string, object>
        {
            ["TenantId"] = tenantId,
            ["ApplicationNumber"] = app.ApplicationNumber,
            ["TagNumber"] = tagNumber,
            ["Variant"] = variant,
            ["FullName"] = fullName,
            ["IssuedAt"] = now.ToString("yyyy-MM-dd"),
            ["ExpiresAt"] = expiresAt.ToString("yyyy-MM-dd"),
        };

        var docx = await TryGenerateDocxAsync(kind, $"vehicle-tag-{variant}-{tagNumber}", docxValues, ct);

        byte[] bytes;
        string fileName;
        string contentType;

        if (docx is not null)
        {
            bytes = docx.Bytes;
            fileName = docx.FileName;
            contentType = docx.ContentType;
        }
        else
        {
            var template = await GetFirstTemplateAsync(tenantId, DocumentTemplateKindFallback.GetCandidateKinds(kind), ct);
            var html = string.IsNullOrWhiteSpace(template)
                ? RenderVehicleTagHtml(
                    tenantId,
                    kind,
                    tagNumber,
                    app.ApplicationNumber,
                    fullName,
                    now)
                : ApplyTokens(template, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["TenantId"] = tenantId,
                    ["Kind"] = kind,
                    ["Variant"] = variant,
                    ["ApplicationNumber"] = app.ApplicationNumber,
                    ["FullName"] = fullName,
                    ["IssuedAt"] = now.ToString("yyyy-MM-dd"),
                    ["DocumentNumber"] = tagNumber,
                    ["ExpiresAt"] = expiresAt.ToString("yyyy-MM-dd"),
                });

            bytes = Encoding.UTF8.GetBytes(html);
            fileName = $"vehicle-tag-{variant}-{tagNumber}.html";
            contentType = "text/html; charset=utf-8";
        }

        var bucket = ObjectStorageKeys.TenantBucket(tenantId);
        var naming = await _namingStore.GetAsync(tenantId, ct);
        var key = _naming.GenerateObjectKey(
            naming,
            new DocumentNamingContext(
                TenantId: tenantId,
                ApplicationNumber: app.ApplicationNumber,
                ApplicationId: app.Id,
                Kind: kind,
                OriginalFileName: fileName,
                NowUtc: now));

        await _storage.PutAsync(
            bucket,
            key,
            new MemoryStream(bytes),
            bytes.Length,
            contentType,
            new Dictionary<string, string>
            {
                ["mamey-tenant"] = tenantId,
                ["mamey-domain"] = "citizenship",
                ["mamey-kind"] = kind,
                ["mamey-application-number"] = app.ApplicationNumber,
                ["mamey-application-id"] = app.Id.ToString("N"),
                ["mamey-document-number"] = tagNumber,
            },
            ct);

        var created = await _store.InsertIssuedDocumentAsync(
            tenantId,
            new IssuedDocumentCreate(
                app.Id,
                kind,
                tagNumber,
                expiresAt,
                fileName,
                contentType,
                bytes.Length,
                bucket,
                key,
                now),
            ct);

        var email = await _store.GetApplicationEmailAsync(tenantId, applicationId, ct);
        if (!string.IsNullOrWhiteSpace(email))
        {
            await _realtime.NotifyIssuedDocumentCreatedAsync(tenantId, email!, created.ApplicationId, created.Id, ct);
            await _realtime.NotifyApplicationUpdatedAsync(tenantId, email!, created.ApplicationId, ct);
        }

        return created;
    }

    private static string RenderCitizenshipCertificateHtml(
        string tenantId,
        string applicationNumber,
        string fullName,
        DateOnly dob,
        DateTimeOffset issuedAt)
    {
        // Phase 4.1.4 UI-first: HTML template for preview/download.
        // Later: per-tenant stored templates + PDF renderer.
        return $$"""
               <!doctype html>
               <html lang="en">
               <head>
                 <meta charset="utf-8" />
                 <meta name="viewport" content="width=device-width, initial-scale=1" />
                 <title>Certificate of Citizenship</title>
                 <style>
                   body { font-family: ui-serif, Georgia, Cambria, "Times New Roman", Times, serif; margin: 40px; }
                   .frame { border: 6px double #111; padding: 28px; }
                   .title { text-align:center; font-size: 28px; letter-spacing: 1px; margin: 0; }
                   .subtitle { text-align:center; margin-top: 6px; color:#333; }
                   .section { margin-top: 22px; font-size: 16px; line-height: 1.6; }
                   .row { display:flex; justify-content:space-between; gap:16px; margin-top: 18px; }
                   .muted { color:#444; font-size: 13px; }
                   .seal { margin-top: 26px; text-align:center; }
                 </style>
               </head>
               <body>
                 <div class="frame">
                   <h1 class="title">Certificate of Citizenship</h1>
                   <div class="subtitle">Tenant: {System.Net.WebUtility.HtmlEncode(tenantId)}</div>
                   <div class="section">
                     This certifies that <strong>{System.Net.WebUtility.HtmlEncode(fullName)}</strong>,
                     born on <strong>{dob:MMMM dd, yyyy}</strong>, is recognized as a citizen under the authority of this nation.
                   </div>
                   <div class="row">
                     <div class="muted">Application: {System.Net.WebUtility.HtmlEncode(applicationNumber)}</div>
                     <div class="muted">Issued: {issuedAt:yyyy-MM-dd HH:mm} UTC</div>
                   </div>
                   <div class="seal">
                     <div class="muted">Official Seal (placeholder)</div>
                   </div>
                 </div>
               </body>
               </html>
               """;
    }

    private static string RenderPassportHtml(
        string tenantId,
        string passportNumber,
        string applicationNumber,
        string fullName,
        DateOnly dob,
        DateTimeOffset issuedAt,
        DateTimeOffset expiresAt,
        string? mrzLine1 = null,
        string? mrzLine2 = null,
        string? mrzLine3 = null)
    {
        var mrzSection = !string.IsNullOrEmpty(mrzLine1) && !string.IsNullOrEmpty(mrzLine2) && !string.IsNullOrEmpty(mrzLine3)
            ? $@"
                   <div class=""mrz-section"">
                     <div class=""mrz-label"">Machine Readable Zone (MRZ)</div>
                     <div class=""mrz-line"">{System.Net.WebUtility.HtmlEncode(mrzLine1)}</div>
                     <div class=""mrz-line"">{System.Net.WebUtility.HtmlEncode(mrzLine2)}</div>
                     <div class=""mrz-line"">{System.Net.WebUtility.HtmlEncode(mrzLine3)}</div>
                   </div>"
            : "<div class=\"muted\">MRZ not available</div>";

        return $@"<!doctype html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
  <title>Passport</title>
  <style>
    body {{ font-family: ui-sans-serif, system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif; margin: 24px; }}
    .card {{ border: 2px solid #111; border-radius: 14px; padding: 18px; max-width: 860px; }}
    .row {{ display:flex; justify-content:space-between; gap:16px; }}
    .hdr {{ font-weight: 700; font-size: 20px; }}
    .muted {{ color:#444; font-size: 13px; }}
    .grid {{ display:grid; grid-template-columns: 1fr 1fr; gap: 12px 18px; margin-top: 14px; }}
    .kv .k {{ font-size: 12px; color:#555; text-transform: uppercase; letter-spacing: .06em; }}
    .kv .v {{ font-size: 16px; font-weight: 600; }}
    .photo {{ width: 140px; height: 180px; border: 1px solid #333; border-radius: 10px; background: repeating-linear-gradient(45deg,#eee,#eee 10px,#f7f7f7 10px,#f7f7f7 20px); }}
    .footer {{ margin-top: 16px; display:flex; justify-content:space-between; align-items:flex-end; }}
    .sig {{ width: 220px; height: 58px; border-bottom: 2px solid #111; }}
    .mrz-section {{ margin-top: 20px; padding: 12px; background: #f0f0f0; border-radius: 8px; }}
    .mrz-label {{ font-size: 11px; color: #666; text-transform: uppercase; letter-spacing: 0.1em; margin-bottom: 8px; }}
    .mrz-line {{ font-family: 'Courier New', monospace; font-size: 16px; letter-spacing: 2px; margin: 4px 0; color: #000; }}
  </style>
</head>
<body>
  <div class=""card"">
    <div class=""row"">
      <div>
        <div class=""hdr"">Passport</div>
        <div class=""muted"">Tenant: {System.Net.WebUtility.HtmlEncode(tenantId)}</div>
        <div class=""muted"">Application: {System.Net.WebUtility.HtmlEncode(applicationNumber)}</div>
      </div>
      <div style=""text-align:right"">
        <div class=""muted"">Passport No.</div>
        <div style=""font-weight:800; font-size:18px"">{System.Net.WebUtility.HtmlEncode(passportNumber)}</div>
      </div>
    </div>

    <div class=""row"" style=""margin-top:14px"">
      <div class=""photo""></div>
      <div style=""flex:1"">
        <div class=""grid"">
          <div class=""kv""><div class=""k"">Full name</div><div class=""v"">{System.Net.WebUtility.HtmlEncode(fullName)}</div></div>
          <div class=""kv""><div class=""k"">Date of birth</div><div class=""v"">{dob:yyyy-MM-dd}</div></div>
          <div class=""kv""><div class=""k"">Issued</div><div class=""v"">{issuedAt:yyyy-MM-dd}</div></div>
          <div class=""kv""><div class=""k"">Expires</div><div class=""v"">{expiresAt:yyyy-MM-dd}</div></div>
        </div>
      </div>
    </div>

    {mrzSection}

    <div class=""footer"">
      <div class=""muted"">Official Seal (placeholder)</div>
      <div style=""text-align:right"">
        <div class=""sig""></div>
        <div class=""muted"">Authorized Signature</div>
      </div>
    </div>
  </div>
</body>
</html>";
    }

    private static JurisdictionCode ParseJurisdictionCode(string? region)
    {
        if (string.IsNullOrWhiteSpace(region))
        {
            return JurisdictionCode.CA; // Default to California
        }

        // Try to parse as 2-letter code
        var code = region.Trim().ToUpperInvariant();
        if (code.Length == 2 && Enum.TryParse<JurisdictionCode>(code, out var parsed))
        {
            return parsed;
        }

        // Try common state name mappings
        return code switch
        {
            "CALIFORNIA" or "CA" => JurisdictionCode.CA,
            "NEW YORK" or "NY" => JurisdictionCode.NY,
            "TEXAS" or "TX" => JurisdictionCode.TX,
            "FLORIDA" or "FL" => JurisdictionCode.FL,
            _ => JurisdictionCode.CA // Default fallback
        };
    }

    private static int ParseHeightToInches(string? height)
    {
        if (string.IsNullOrWhiteSpace(height))
        {
            return 70; // Default 5'10"
        }

        // Try to parse as inches directly (e.g., "70")
        if (int.TryParse(height, out var inches))
        {
            return inches;
        }

        // Try to parse feet and inches format (e.g., "5'10" or "5'10\"")
        var feetInchesMatch = System.Text.RegularExpressions.Regex.Match(height, @"(\d+)['']?\s*(\d+)?");
        if (feetInchesMatch.Success)
        {
            var feet = int.Parse(feetInchesMatch.Groups[1].Value);
            var inchesPart = feetInchesMatch.Groups[2].Success ? int.Parse(feetInchesMatch.Groups[2].Value) : 0;
            return (feet * 12) + inchesPart;
        }

        // Try to parse as centimeters (e.g., "178cm" or "178")
        var cmMatch = System.Text.RegularExpressions.Regex.Match(height, @"(\d+)\s*cm", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (cmMatch.Success && int.TryParse(cmMatch.Groups[1].Value, out var cm))
        {
            return (int)(cm / 2.54); // Convert cm to inches
        }

        // Default fallback
        return 70;
    }

    private static Sex ParseSex(string? sex)
    {
        if (string.IsNullOrWhiteSpace(sex))
        {
            return Sex.NotSpecified;
        }

        // Try to parse as enum value directly
        if (Enum.TryParse<Sex>(sex, true, out var parsedSex))
        {
            return parsedSex;
        }

        // Map common string values
        return sex.ToUpperInvariant() switch
        {
            "M" or "MALE" or "1" => Sex.Male,
            "F" or "FEMALE" or "2" => Sex.Female,
            "X" or "NON-BINARY" or "OTHER" or "O" or "9" => Sex.NotSpecified,
            _ => Sex.NotSpecified
        };
    }

    private static EyeColor ParseEyeColor(string? eyeColor)
    {
        if (string.IsNullOrWhiteSpace(eyeColor))
        {
            return EyeColor.UNK;
        }

        // Try to parse as enum value directly (AAMVA codes are uppercase)
        if (Enum.TryParse<EyeColor>(eyeColor.ToUpperInvariant(), true, out var parsedColor))
        {
            return parsedColor;
        }

        // Map common string values to AAMVA codes
        return eyeColor.ToUpperInvariant() switch
        {
            "BLACK" or "BLK" => EyeColor.BLK,
            "BLUE" or "BLU" => EyeColor.BLU,
            "BROWN" or "BRO" => EyeColor.BRO,
            "GREEN" or "GRN" => EyeColor.GRN,
            "GRAY" or "GREY" or "GRY" => EyeColor.GRY,
            "HAZEL" or "HAZ" => EyeColor.HAZ,
            "MAROON" or "MAR" => EyeColor.MAR,
            "PINK" or "PNK" => EyeColor.PNK,
            "DICHROMATIC" or "DIC" => EyeColor.DIC,
            _ => EyeColor.UNK
        };
    }

    private static HairColor ParseHairColor(string? hairColor)
    {
        if (string.IsNullOrWhiteSpace(hairColor))
        {
            return HairColor.UNK;
        }

        // Try to parse as enum value directly (AAMVA codes are uppercase)
        if (Enum.TryParse<HairColor>(hairColor.ToUpperInvariant(), true, out var parsedColor))
        {
            return parsedColor;
        }

        // Map common string values to AAMVA codes
        return hairColor.ToUpperInvariant() switch
        {
            "BALD" or "BAL" => HairColor.BAL,
            "BLACK" or "BLK" => HairColor.BLK,
            "BLOND" or "BLONDE" or "BLN" => HairColor.BLN,
            "BROWN" or "BRO" => HairColor.BRO,
            "GRAY" or "GREY" or "GRY" => HairColor.GRY,
            "RED" => HairColor.RED,
            "SANDY" or "SDY" => HairColor.SDY,
            "WHITE" or "WHI" => HairColor.WHI,
            _ => HairColor.UNK
        };
    }

    private static string RenderIdCardHtml(
        string tenantId,
        string kind,
        string cardNumber,
        string applicationNumber,
        string fullName,
        DateOnly dob,
        DateTimeOffset issuedAt,
        DateTimeOffset expiresAt,
        string? barcodeBase64 = null,
        string? mrzLine1 = null,
        string? mrzLine2 = null,
        string? mrzLine3 = null)
    {
        var barcodeImg = !string.IsNullOrEmpty(barcodeBase64)
            ? $"<img src=\"data:image/png;base64,{barcodeBase64}\" alt=\"PDF417 Barcode\" style=\"max-width: 300px; height: auto;\" />"
            : "<div class=\"muted\">Barcode not available</div>";

        var mrzSection = !string.IsNullOrEmpty(mrzLine1) && !string.IsNullOrEmpty(mrzLine2) && !string.IsNullOrEmpty(mrzLine3)
            ? $@"
                   <div class=""mrz-section"">
                     <div class=""mrz-label"">Machine Readable Zone (MRZ)</div>
                     <div class=""mrz-line"">{System.Net.WebUtility.HtmlEncode(mrzLine1)}</div>
                     <div class=""mrz-line"">{System.Net.WebUtility.HtmlEncode(mrzLine2)}</div>
                     <div class=""mrz-line"">{System.Net.WebUtility.HtmlEncode(mrzLine3)}</div>
                   </div>"
            : "<div class=\"muted\">MRZ not available</div>";

        return $@"<!doctype html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
  <title>ID Card</title>
  <style>
    body {{ font-family: ui-sans-serif, system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif; margin: 24px; }}
    .card {{ border: 2px solid #111; border-radius: 14px; padding: 18px; max-width: 860px; }}
    .row {{ display:flex; justify-content:space-between; gap:16px; }}
    .hdr {{ font-weight: 800; font-size: 20px; }}
    .muted {{ color:#444; font-size: 13px; }}
    .grid {{ display:grid; grid-template-columns: 1fr 1fr; gap: 12px 18px; margin-top: 14px; }}
    .kv .k {{ font-size: 12px; color:#555; text-transform: uppercase; letter-spacing: .06em; }}
    .kv .v {{ font-size: 16px; font-weight: 700; }}
    .photo {{ width: 140px; height: 140px; border: 1px solid #333; border-radius: 10px; background: repeating-linear-gradient(45deg,#eee,#eee 10px,#f7f7f7 10px,#f7f7f7 20px); }}
    .barcode-section {{ margin-top: 20px; padding: 12px; background: #f9f9f9; border-radius: 8px; text-align: center; }}
    .mrz-section {{ margin-top: 20px; padding: 12px; background: #f0f0f0; border-radius: 8px; }}
    .mrz-label {{ font-size: 11px; color: #666; text-transform: uppercase; letter-spacing: 0.1em; margin-bottom: 8px; }}
    .mrz-line {{ font-family: 'Courier New', monospace; font-size: 16px; letter-spacing: 2px; margin: 4px 0; color: #000; }}
  </style>
</head>
<body>
  <div class=""card"">
    <div class=""row"">
      <div>
        <div class=""hdr"">ID Card</div>
        <div class=""muted"">Type: {System.Net.WebUtility.HtmlEncode(kind)}</div>
        <div class=""muted"">Tenant: {System.Net.WebUtility.HtmlEncode(tenantId)}</div>
        <div class=""muted"">Application: {System.Net.WebUtility.HtmlEncode(applicationNumber)}</div>
      </div>
      <div style=""text-align:right"">
        <div class=""muted"">Card No.</div>
        <div style=""font-weight:900; font-size:18px"">{System.Net.WebUtility.HtmlEncode(cardNumber)}</div>
      </div>
    </div>

    <div class=""row"" style=""margin-top:14px"">
      <div class=""photo""></div>
      <div style=""flex:1"">
        <div class=""grid"">
          <div class=""kv""><div class=""k"">Full name</div><div class=""v"">{System.Net.WebUtility.HtmlEncode(fullName)}</div></div>
          <div class=""kv""><div class=""k"">Date of birth</div><div class=""v"">{dob:yyyy-MM-dd}</div></div>
          <div class=""kv""><div class=""k"">Issued</div><div class=""v"">{issuedAt:yyyy-MM-dd}</div></div>
          <div class=""kv""><div class=""k"">Expires</div><div class=""v"">{expiresAt:yyyy-MM-dd}</div></div>
        </div>
      </div>
    </div>

    <div class=""barcode-section"">
      <div class=""muted"" style=""margin-bottom: 8px;"">AAMVA PDF417 Barcode</div>
      {barcodeImg}
    </div>

    {mrzSection}
  </div>
</body>
</html>";
    }

    private static string RenderVehicleTagHtml(
        string tenantId,
        string kind,
        string tagNumber,
        string applicationNumber,
        string fullName,
        DateTimeOffset issuedAt)
    {
        return $$"""
               <!doctype html>
               <html lang="en">
               <head>
                 <meta charset="utf-8" />
                 <meta name="viewport" content="width=device-width, initial-scale=1" />
                 <title>Vehicle Tag</title>
                 <style>
                   body { font-family: ui-sans-serif, system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif; margin: 24px; }
                   .tag { border: 3px solid #111; border-radius: 18px; padding: 18px; max-width: 860px; }
                   .row { display:flex; justify-content:space-between; gap:16px; align-items:baseline; }
                   .hdr { font-weight: 900; font-size: 24px; letter-spacing: .04em; }
                   .muted { color:#444; font-size: 13px; }
                   .num { font-weight: 1000; font-size: 34px; letter-spacing: .08em; }
                   .grid { display:grid; grid-template-columns: 1fr 1fr; gap: 10px 18px; margin-top: 14px; }
                 </style>
               </head>
               <body>
                 <div class="tag">
                   <div class="row">
                     <div>
                       <div class="hdr">Vehicle Tag</div>
                       <div class="muted">Type: {{System.Net.WebUtility.HtmlEncode(kind)}}</div>
                       <div class="muted">Tenant: {{System.Net.WebUtility.HtmlEncode(tenantId)}}</div>
                     </div>
                     <div class="num">{{System.Net.WebUtility.HtmlEncode(tagNumber)}}</div>
                   </div>
                   <div class="grid">
                     <div class="muted">Application: {{System.Net.WebUtility.HtmlEncode(applicationNumber)}}</div>
                     <div class="muted">Issued: {{issuedAt:yyyy-MM-dd}} UTC</div>
                     <div class="muted">Assigned to: {{System.Net.WebUtility.HtmlEncode(fullName)}}</div>
                     <div class="muted">Official Seal (placeholder)</div>
                   </div>
                 </div>
               </body>
               </html>
               """;
    }

    public async Task<IReadOnlyList<IssuedDocumentSummary>> ReissueDocumentsForStatusProgressionAsync(string email, CitizenshipStatus newStatus, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;

        // Find all approved/completed applications for this citizen
        var applications = await _store.GetApplicationsForReissueAsync(tenantId, email, ct);
        if (applications.Count == 0)
        {
            return Array.Empty<IssuedDocumentSummary>();
        }

        // Use the most recent application for re-issuance
        var application = applications.First();
        var reissuedDocuments = new List<IssuedDocumentSummary>();

        // Get all currently issued documents for this application (tracked for deletion)
        var existingDocuments = await _store.GetIssuedDocumentsForApplicationAsync(application.Id, ct);
        if (existingDocuments.Count == 0)
        {
            return Array.Empty<IssuedDocumentSummary>();
        }

        // Group documents by kind to avoid duplicate re-issuance
        var documentKinds = existingDocuments
            .Select(d => d.Kind.ToLowerInvariant())
            .Distinct()
            .ToList();

        // Re-issue each document type (the issue methods will create new documents with new validity periods)
        foreach (var kind in documentKinds)
        {
            try
            {
                IssuedDocumentSummary? reissued = null;

                switch (kind)
                {
                    case "passport":
                        // Delete old passport first to allow re-issuance
                        var oldPassport = existingDocuments.FirstOrDefault(d => d.Kind.Equals("Passport", StringComparison.OrdinalIgnoreCase));
                        if (oldPassport is not null)
                        {
                            await _store.RemoveIssuedDocumentAsync(oldPassport.Id, ct);
                        }
                        reissued = await IssuePassportAsync(application.Id, ct);
                        break;

                    case "idcard":
                    case "idcard:driverslicense":
                    case "idcard:medicinalcannabis":
                    case "idcard:medicinalmushroom":
                    case "idcard:concealedweapons":
                    case "idcard:identificationcard":
                        var variant = kind.Contains(":") ? kind.Split(':')[1] : "DriversLicense";
                        // Delete old ID card first
                        var oldIdCard = existingDocuments.FirstOrDefault(d => d.Kind.Equals(kind, StringComparison.OrdinalIgnoreCase));
                        if (oldIdCard is not null)
                        {
                            await _store.RemoveIssuedDocumentAsync(oldIdCard.Id, ct);
                        }
                        reissued = await IssueIdCardAsync(application.Id, variant, ct);
                        break;

                    case "vehicletag":
                    case "vehicletag:standard":
                    case "vehicletag:veteran":
                        var tagVariant = kind.Contains(":") ? kind.Split(':')[1] : "Standard";
                        // Delete old vehicle tag first
                        var oldTag = existingDocuments.FirstOrDefault(d => d.Kind.Equals(kind, StringComparison.OrdinalIgnoreCase));
                        if (oldTag is not null)
                        {
                            await _store.RemoveIssuedDocumentAsync(oldTag.Id, ct);
                        }
                        reissued = await IssueVehicleTagAsync(application.Id, tagVariant, ct);
                        break;

                    case "citizenshipcertificate":
                        // Certificates typically don't need re-issuance, but we can regenerate if needed
                        reissued = await GenerateCitizenshipCertificateAsync(application.Id, ct);
                        break;
                }

                if (reissued is not null)
                {
                    reissuedDocuments.Add(reissued);
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with other documents
                _logger.LogError(ex, "Failed to re-issue document {DocumentKind} for status progression", kind);
            }
        }

        // Notify via SignalR if any documents were re-issued
        if (reissuedDocuments.Count > 0)
        {
            // Notify for each re-issued document
            foreach (var doc in reissuedDocuments)
            {
                await _realtime.NotifyIssuedDocumentCreatedAsync(
                    tenantId,
                    application.Email ?? string.Empty,
                    application.Id,
                    doc.Id,
                    ct);
            }
        }

        return reissuedDocuments;
    }

    public async Task SubmitIntakeReviewAsync(SubmitIntakeReviewRequest request, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var now = DateTimeOffset.UtcNow;

        // Verify application exists and belongs to tenant
        if (!await _store.ApplicationExistsAsync(tenantId, request.ApplicationId, ct))
        {
            throw new ArgumentException("Application not found or does not belong to this tenant.");
        }

        await _store.UpsertIntakeReviewAsync(tenantId, request, now, ct);
    }

    public async Task ApproveApplicationAsync(Guid applicationId, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var now = DateTimeOffset.UtcNow;

        var application = await _store.GetApplicationForDecisionAsync(tenantId, applicationId, ct);
        if (application is null)
        {
            throw new ArgumentException("Application not found or does not belong to this tenant.");
        }

        if (application.Status == "Approved")
        {
            return; // Already approved
        }

        if (string.IsNullOrWhiteSpace(application.Email))
        {
            throw new InvalidOperationException("Application email is required for status creation.");
        }

        await _store.UpdateApplicationStatusAsync(tenantId, applicationId, "Approved", now, null, ct);

        // Create Probationary citizenship status
        await _statusService.CreateOrUpdateStatusAsync(applicationId, application.Email, ct);

        // Notify applicant
        await _realtime.NotifyApplicationUpdatedAsync(tenantId, application.Email, applicationId, ct);
    }

    public async Task RejectApplicationAsync(Guid applicationId, string reason, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var now = DateTimeOffset.UtcNow;

        var application = await _store.GetApplicationForDecisionAsync(tenantId, applicationId, ct);
        if (application is null)
        {
            throw new ArgumentException("Application not found or does not belong to this tenant.");
        }

        if (application.Status == "Rejected")
        {
            return; // Already rejected
        }

        await _store.UpdateApplicationStatusAsync(tenantId, applicationId, "Rejected", now, reason, ct);

        // Notify applicant
        if (!string.IsNullOrWhiteSpace(application.Email))
        {
            await _realtime.NotifyApplicationUpdatedAsync(
                tenantId,
                application.Email,
                applicationId,
                ct);
        }
    }

    public async Task<IssuedDocumentSummary> RenewDocumentAsync(Guid expiredDocumentId, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var now = DateTimeOffset.UtcNow;

        // Find the expired document and its application
        var expiredDoc = await _store.GetIssuedDocumentByIdAsync(expiredDocumentId, ct);
        if (expiredDoc is null)
        {
            throw new ArgumentException("Document not found.");
        }

        // Get the application
        var application = await _store.GetApplicationForRenewalAsync(tenantId, expiredDoc.ApplicationId, ct);
        if (application is null)
        {
            throw new ArgumentException("Application not found or does not belong to this tenant.");
        }

        // Verify the document is expired or expiring soon (within 30 days)
        if (expiredDoc.ExpiresAt.HasValue && expiredDoc.ExpiresAt.Value > now.AddDays(30))
        {
            throw new InvalidOperationException("Document is not expired or expiring soon. Renewal is only available for expired documents or documents expiring within 30 days.");
        }

        if (string.IsNullOrWhiteSpace(application.Email))
        {
            throw new InvalidOperationException("Application email is required.");
        }

        // Get current citizenship status to determine new expiration
        var status = await _statusService.GetStatusByEmailAsync(application.Email, ct);
        if (status is null)
        {
            throw new InvalidOperationException("Citizenship status not found. Cannot renew documents without an active citizenship status.");
        }

        // Determine document type and variant from the expired document
        var kind = expiredDoc.Kind;
        IssuedDocumentSummary renewedDoc;

        if (kind == "Passport")
        {
            renewedDoc = await IssuePassportAsync(expiredDoc.ApplicationId, ct);
        }
        else if (kind.StartsWith("IdCard:", StringComparison.Ordinal))
        {
            var variant = kind["IdCard:".Length..];
            renewedDoc = await IssueIdCardAsync(expiredDoc.ApplicationId, variant, ct);
        }
        else if (kind.StartsWith("VehicleTag:", StringComparison.Ordinal))
        {
            var variant = kind["VehicleTag:".Length..];
            renewedDoc = await IssueVehicleTagAsync(expiredDoc.ApplicationId, variant, ct);
        }
        else
        {
            throw new InvalidOperationException($"Document type '{kind}' cannot be renewed automatically. Please contact support.");
        }

        // Notify citizen of renewal
        await _realtime.NotifyIssuedDocumentCreatedAsync(
            tenantId,
            application.Email,
            expiredDoc.ApplicationId,
            renewedDoc.Id,
            ct);

        return renewedDoc;
    }

    public async Task<IssuedDocumentSummary> IssueTravelIdAsync(Guid applicationId, CancellationToken ct = default)
    {
        const string kind = "TravelId";
        var tenantId = _tenant.TenantId;

        var existing = await _store.GetIssuedDocumentAsync(tenantId, applicationId, kind, ct);
        if (existing is not null)
        {
            var appCheck = await _store.GetApplicationForTravelIdAsync(tenantId, applicationId, ct);
            if (appCheck is null)
            {
                throw new InvalidOperationException("Application not found (or not in this tenant).");
            }

            if (string.IsNullOrWhiteSpace(existing.DocumentNumber))
            {
                var backfillDocumentNumber = IssuedDocumentNumberGenerator.IdCard(tenantId, existing.CreatedAt, existing.ApplicationId, "TravelId");
                var backfillExpiresAt = existing.ExpiresAt ?? existing.CreatedAt.AddYears(2);
                await _store.UpdateIssuedDocumentAsync(existing.Id, backfillDocumentNumber, backfillExpiresAt, ct);
                existing = existing with { DocumentNumber = backfillDocumentNumber, ExpiresAt = backfillExpiresAt };
            }

            return existing;
        }

        var app = await _store.GetApplicationForTravelIdAsync(tenantId, applicationId, ct);
        if (app is null)
        {
            throw new InvalidOperationException("Application not found (or not in this tenant).");
        }

        var now = DateTimeOffset.UtcNow;
        var nowDate = now.DateTime;

        // Get citizenship status to determine document validity period
        var citizenshipStatus = await _statusService.GetStatusByApplicationIdAsync(applicationId, ct)
            ?? Application.Models.CitizenshipStatus.Probationary;

        // Calculate expiration date based on citizenship status
        var expiresAt = _validityCalculator.CalculateExpirationDate(citizenshipStatus, kind, now);
        var expiresAtDate = expiresAt.DateTime;

        // Generate document number
        var documentNumber = IssuedDocumentNumberGenerator.IdCard(tenantId, now, app.Id, "TravelId");

        var fullName = $"{app.FirstName} {app.LastName}";
        var docxValues = new Dictionary<string, object>
        {
            ["TenantId"] = tenantId,
            ["ApplicationNumber"] = app.ApplicationNumber,
            ["DocumentNumber"] = documentNumber,
            ["FullName"] = fullName,
            ["DateOfBirth"] = app.DateOfBirth.ToString("yyyy-MM-dd"),
            ["AddressLine1"] = app.AddressLine1 ?? string.Empty,
            ["City"] = app.City ?? string.Empty,
            ["Region"] = app.Region ?? string.Empty,
            ["PostalCode"] = app.PostalCode ?? string.Empty,
            ["IssuedAt"] = nowDate.ToString("yyyy-MM-dd"),
            ["ExpiresAt"] = expiresAtDate.ToString("yyyy-MM-dd"),
        };

        var docx = await TryGenerateDocxAsync(kind, $"travel-id-{documentNumber}", docxValues, ct);

        byte[] bytes;
        string fileName;
        string contentType;

        if (docx is not null)
        {
            bytes = docx.Bytes;
            fileName = docx.FileName;
            contentType = docx.ContentType;
        }
        else
        {
            var template = await GetFirstTemplateAsync(tenantId, DocumentTemplateKindFallback.GetCandidateKinds(kind), ct);
            var html = string.IsNullOrWhiteSpace(template)
                ? RenderTravelIdHtml(
                    tenantId,
                    documentNumber,
                    app.ApplicationNumber,
                    fullName,
                    app.DateOfBirth,
                    app.AddressLine1,
                    app.City,
                    app.Region,
                    app.PostalCode,
                    nowDate,
                    expiresAtDate)
                : ApplyTokens(template, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["TenantId"] = tenantId,
                    ["Kind"] = kind,
                    ["ApplicationNumber"] = app.ApplicationNumber,
                    ["FullName"] = fullName,
                    ["DocumentNumber"] = documentNumber,
                    ["DateOfBirth"] = app.DateOfBirth.ToString("yyyy-MM-dd"),
                    ["AddressLine1"] = app.AddressLine1 ?? string.Empty,
                    ["City"] = app.City ?? string.Empty,
                    ["Region"] = app.Region ?? string.Empty,
                    ["PostalCode"] = app.PostalCode ?? string.Empty,
                    ["IssuedAt"] = nowDate.ToString("yyyy-MM-dd"),
                    ["ExpiresAt"] = expiresAtDate.ToString("yyyy-MM-dd"),
                });

            bytes = Encoding.UTF8.GetBytes(html);
            fileName = $"travel-id-{documentNumber}.html";
            contentType = "text/html; charset=utf-8";
        }

        var bucket = ObjectStorageKeys.TenantBucket(tenantId);
        var naming = await _namingStore.GetAsync(tenantId, ct);
        var key = _naming.GenerateObjectKey(
            naming,
            new DocumentNamingContext(
                TenantId: tenantId,
                ApplicationNumber: app.ApplicationNumber,
                ApplicationId: app.Id,
                Kind: kind,
                OriginalFileName: fileName,
                NowUtc: now));

        await _storage.PutAsync(
            bucket,
            key,
            new MemoryStream(bytes),
            bytes.Length,
            contentType,
            new Dictionary<string, string>
            {
                ["mamey-tenant"] = tenantId,
                ["mamey-domain"] = "citizenship",
                ["mamey-kind"] = kind,
                ["mamey-application-number"] = app.ApplicationNumber,
                ["mamey-application-id"] = app.Id.ToString("N"),
            },
            ct);

        var created = await _store.InsertIssuedDocumentAsync(
            tenantId,
            new IssuedDocumentCreate(
                app.Id,
                kind,
                documentNumber,
                expiresAt,
                fileName,
                contentType,
                bytes.Length,
                bucket,
                key,
                now),
            ct);

        var email = await _store.GetApplicationEmailAsync(tenantId, applicationId, ct);
        if (!string.IsNullOrWhiteSpace(email))
        {
            await _realtime.NotifyIssuedDocumentCreatedAsync(tenantId, email!, created.ApplicationId, created.Id, ct);
        }

        return created;
    }

    private string RenderTravelIdHtml(
        string tenantId,
        string documentNumber,
        string applicationNumber,
        string fullName,
        DateOnly dateOfBirth,
        string? addressLine1,
        string? city,
        string? region,
        string? postalCode,
        DateTime issuedAt,
        DateTime expiresAt)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Travel ID - {fullName}</title>
    <style>
        body {{ font-family: Arial, sans-serif; padding: 20px; }}
        .travel-id {{ border: 2px solid #000; padding: 20px; max-width: 600px; margin: 0 auto; }}
        .header {{ text-align: center; border-bottom: 2px solid #000; padding-bottom: 10px; margin-bottom: 20px; }}
        .content {{ display: flex; justify-content: space-between; }}
        .left {{ flex: 1; }}
        .right {{ flex: 1; text-align: right; }}
        .field {{ margin-bottom: 10px; }}
        .label {{ font-weight: bold; }}
        .footer {{ margin-top: 20px; text-align: center; font-size: 12px; }}
    </style>
</head>
<body>
    <div class=""travel-id"">
        <div class=""header"">
            <h2>TRAVEL IDENTIFICATION CARD</h2>
            <p>Tenant: {tenantId}</p>
        </div>
        <div class=""content"">
            <div class=""left"">
                <div class=""field"">
                    <span class=""label"">Name:</span> {fullName}
                </div>
                <div class=""field"">
                    <span class=""label"">Date of Birth:</span> {dateOfBirth:yyyy-MM-dd}
                </div>
                <div class=""field"">
                    <span class=""label"">Address:</span> {addressLine1 ?? "N/A"}
                </div>
                <div class=""field"">
                    <span class=""label"">City:</span> {city ?? "N/A"}
                </div>
                <div class=""field"">
                    <span class=""label"">Region:</span> {region ?? "N/A"}
                </div>
                <div class=""field"">
                    <span class=""label"">Postal Code:</span> {postalCode ?? "N/A"}
                </div>
            </div>
            <div class=""right"">
                <div class=""field"">
                    <span class=""label"">Document Number:</span> {documentNumber}
                </div>
                <div class=""field"">
                    <span class=""label"">Application Number:</span> {applicationNumber}
                </div>
                <div class=""field"">
                    <span class=""label"">Issued:</span> {issuedAt:yyyy-MM-dd}
                </div>
                <div class=""field"">
                    <span class=""label"">Expires:</span> {expiresAt:yyyy-MM-dd}
                </div>
            </div>
        </div>
        <div class=""footer"">
            <p>This is an official travel identification document issued by {tenantId}</p>
        </div>
    </div>
</body>
</html>";
    }
}
