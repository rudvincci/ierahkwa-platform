using Mamey.Portal.Citizenship.Application.Requests;
using Mamey.Portal.Shared.Storage;
using Mamey.Portal.Shared.Storage.DocumentNaming;
using Mamey.Portal.Shared.Tenancy;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class CitizenshipApplicationService : ICitizenshipApplicationService
{
    private const int MaxPersonalDocuments = 10;
    private const long MaxPersonalDocumentBytes = 10 * 1024 * 1024; // 10MB each
    private const long MaxPassportPhotoBytes = 5 * 1024 * 1024; // 5MB
    private const long MaxSignatureBytes = 2 * 1024 * 1024; // 2MB

    private readonly ITenantContext _tenant;
    private readonly IObjectStorage _storage;
    private readonly IDocumentNamingStore _namingStore;
    private readonly IDocumentNamingService _naming;
    private readonly IApplicationFormPdfGenerator _pdfGenerator;
    private readonly IApplicationSubmissionStore _store;

    public CitizenshipApplicationService(
        ITenantContext tenant,
        IObjectStorage storage,
        IDocumentNamingStore namingStore,
        IDocumentNamingService naming,
        IApplicationFormPdfGenerator pdfGenerator,
        IApplicationSubmissionStore store)
    {
        _tenant = tenant;
        _storage = storage;
        _namingStore = namingStore;
        _naming = naming;
        _pdfGenerator = pdfGenerator;
        _store = store;
    }

    public async Task<string> SubmitAsync(
        SubmitCitizenshipApplicationRequest request,
        IReadOnlyList<UploadFile> personalDocuments,
        UploadFile passportPhoto,
        UploadFile signatureImage,
        CancellationToken ct = default)
    {
        ValidateInputs(request, personalDocuments, passportPhoto, signatureImage);

        var tenantId = _tenant.TenantId;
        var bucket = ObjectStorageKeys.TenantBucket(tenantId);

        var appId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var applicationNumber = $"APP-{tenantId.ToUpperInvariant()}-{now:yyyyMMdd}-{appId.ToString("N")[..6].ToUpperInvariant()}";
        var namingPattern = await _namingStore.GetAsync(tenantId, ct);

        var submission = new ApplicationSubmission(
            appId,
            tenantId,
            applicationNumber,
            "Submitted",
            request.FirstName.Trim(),
            request.LastName.Trim(),
            request.DateOfBirth,
            request.Email.Trim(),
            string.IsNullOrWhiteSpace(request.MiddleName) ? null : request.MiddleName.Trim(),
            string.IsNullOrWhiteSpace(request.Height) ? null : request.Height.Trim(),
            string.IsNullOrWhiteSpace(request.EyeColor) ? null : request.EyeColor.Trim(),
            string.IsNullOrWhiteSpace(request.HairColor) ? null : request.HairColor.Trim(),
            string.IsNullOrWhiteSpace(request.Sex) ? null : request.Sex.Trim(),
            string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
            string.IsNullOrWhiteSpace(request.PlaceOfBirth) ? null : request.PlaceOfBirth.Trim(),
            string.IsNullOrWhiteSpace(request.CountryOfOrigin) ? null : request.CountryOfOrigin.Trim(),
            string.IsNullOrWhiteSpace(request.MaritalStatus) ? null : request.MaritalStatus.Trim(),
            string.IsNullOrWhiteSpace(request.PreviousNames) ? null : request.PreviousNames.Trim(),
            string.IsNullOrWhiteSpace(request.AddressLine1) ? null : request.AddressLine1.Trim(),
            string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim(),
            string.IsNullOrWhiteSpace(request.Region) ? null : request.Region.Trim(),
            string.IsNullOrWhiteSpace(request.PostalCode) ? null : request.PostalCode.Trim(),
            request.AcknowledgeTreaty,
            request.SwearAllegiance,
            request.AffidavitDate,
            request.HasBirthCertificate,
            request.HasPhotoId,
            request.HasProofOfResidence,
            request.HasBackgroundCheck,
            request.AuthorizeBiometricEnrollment,
            request.DeclareUnderstanding,
            request.ConsentToVerification,
            request.ConsentToDataProcessing,
            request.ExtendedDataJson,
            now,
            now);

        if (personalDocuments.Count > MaxPersonalDocuments)
        {
            throw new ArgumentException($"Too many personal documents (max {MaxPersonalDocuments}).");
        }

        var uploads = new List<ApplicationUploadRecord>();

        foreach (var doc in personalDocuments)
        {
            if (!IsAllowedDocContentType(doc.ContentType))
            {
                throw new ArgumentException($"Unsupported personal document type: {doc.ContentType}");
            }

            if (doc.Size <= 0 || doc.Size > MaxPersonalDocumentBytes)
            {
                throw new ArgumentException($"Personal document '{doc.FileName}' is too large (max {MaxPersonalDocumentBytes / (1024 * 1024)}MB).");
            }

            var ctx = new DocumentNamingContext(
                TenantId: tenantId,
                ApplicationNumber: applicationNumber,
                ApplicationId: appId,
                Kind: "PersonalDocument",
                OriginalFileName: doc.FileName,
                NowUtc: now);
            var key = _naming.GenerateObjectKey(namingPattern, ctx);
            var safeName = Path.GetFileName(key);

            var normalized = await NormalizeAndValidatePersonalDocumentAsync(doc, ct);
            await _storage.PutAsync(
                bucket,
                key,
                normalized.Content,
                normalized.Size,
                normalized.ContentType,
                new Dictionary<string, string>
                {
                    ["mamey-tenant"] = tenantId,
                    ["mamey-domain"] = "citizenship",
                    ["mamey-kind"] = "PersonalDocument",
                    ["mamey-application-number"] = applicationNumber,
                    ["mamey-application-id"] = appId.ToString("N"),
                    ["mamey-original-file-name"] = doc.FileName,
                },
                ct);

            uploads.Add(new ApplicationUploadRecord(
                Guid.NewGuid(),
                appId,
                "PersonalDocument",
                safeName,
                normalized.ContentType,
                normalized.Size,
                bucket,
                key,
                now));
        }

        uploads.Add(await StoreSingleValidatedImage(
            tenantId,
            appId,
            applicationNumber,
            namingPattern,
            now,
            bucket,
            "PassportPhoto",
            passportPhoto,
            ValidatePassportPhotoAsync,
            ct));

        uploads.Add(await StoreSingleValidatedImage(
            tenantId,
            appId,
            applicationNumber,
            namingPattern,
            now,
            bucket,
            "SignatureImage",
            signatureImage,
            ValidateSignatureImageAsync,
            ct));

        await _store.SaveAsync(submission, uploads, ct);

        try
        {
            var formData = new ApplicationFormData(
                ApplicationNumber: applicationNumber,
                FirstName: request.FirstName,
                LastName: request.LastName,
                MiddleName: request.MiddleName,
                DateOfBirth: request.DateOfBirth,
                PlaceOfBirth: request.PlaceOfBirth,
                CountryOfOrigin: request.CountryOfOrigin,
                Sex: request.Sex,
                Height: request.Height,
                EyeColor: request.EyeColor,
                HairColor: request.HairColor,
                MaritalStatus: request.MaritalStatus,
                PreviousNames: request.PreviousNames,
                Email: request.Email,
                PhoneNumber: request.PhoneNumber,
                AddressLine1: request.AddressLine1,
                City: request.City,
                Region: request.Region,
                PostalCode: request.PostalCode,
                AcknowledgeTreaty: request.AcknowledgeTreaty,
                SwearAllegiance: request.SwearAllegiance,
                AffidavitDate: request.AffidavitDate,
                HasBirthCertificate: request.HasBirthCertificate,
                HasPhotoId: request.HasPhotoId,
                HasProofOfResidence: request.HasProofOfResidence,
                HasBackgroundCheck: request.HasBackgroundCheck,
                AuthorizeBiometricEnrollment: request.AuthorizeBiometricEnrollment,
                DeclareUnderstanding: request.DeclareUnderstanding,
                ConsentToVerification: request.ConsentToVerification,
                ConsentToDataProcessing: request.ConsentToDataProcessing,
                SubmittedAt: now);

            var pdfs = await _pdfGenerator.GenerateAllFormsAsync(formData, ct);

            foreach (var (formName, pdfBytes) in pdfs)
            {
                var pdfKey = $"applications/{appId}/forms/{formName}.pdf";
                using var pdfStream = new MemoryStream(pdfBytes);
                await _storage.PutAsync(
                    bucket,
                    pdfKey,
                    pdfStream,
                    pdfBytes.Length,
                    "application/pdf",
                    new Dictionary<string, string>
                    {
                        ["mamey-tenant"] = tenantId,
                        ["mamey-domain"] = "citizenship",
                        ["mamey-kind"] = "ApplicationFormPdf",
                        ["mamey-form-name"] = formName,
                        ["mamey-application-number"] = applicationNumber,
                        ["mamey-application-id"] = appId.ToString("N"),
                    },
                    ct);
            }
        }
        catch (Exception)
        {
            // Best-effort PDF generation; submission succeeds even if this fails.
        }

        return applicationNumber;
    }

    private static void ValidateInputs(
        SubmitCitizenshipApplicationRequest request,
        IReadOnlyList<UploadFile> personalDocuments,
        UploadFile passportPhoto,
        UploadFile signatureImage)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            throw new ArgumentException("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            throw new ArgumentException("Last name is required.");
        }

        if (!request.AcknowledgeTreaty)
        {
            throw new ArgumentException("Treaty acknowledgment is required.");
        }

        if (!request.SwearAllegiance)
        {
            throw new ArgumentException("Affidavit of allegiance is required.");
        }

        if (!request.AuthorizeBiometricEnrollment)
        {
            throw new ArgumentException("Biometric enrollment authorization is required.");
        }

        if (!request.DeclareUnderstanding)
        {
            throw new ArgumentException("Declaration of understanding is required.");
        }

        if (!request.ConsentToVerification)
        {
            throw new ArgumentException("Consent to verification is required.");
        }

        if (!request.ConsentToDataProcessing)
        {
            throw new ArgumentException("Consent to data processing is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required for application notifications.");
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new ArgumentException("Email format is invalid.");
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var phoneDigits = System.Text.RegularExpressions.Regex.Replace(request.PhoneNumber, @"[^\d]", "");
            if (phoneDigits.Length < 10 || phoneDigits.Length > 15)
            {
                throw new ArgumentException("Phone number must be between 10 and 15 digits.");
            }
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.DateOfBirth > today)
        {
            throw new ArgumentException("Date of birth cannot be in the future.");
        }

        var minAge = today.AddYears(-120);
        if (request.DateOfBirth < minAge)
        {
            throw new ArgumentException("Date of birth is too far in the past.");
        }

        if (!string.IsNullOrWhiteSpace(request.Sex) && !new[] { "M", "F", "X", "O" }.Contains(request.Sex, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Sex must be M, F, X, or O.");
        }

        if (!string.IsNullOrWhiteSpace(request.Height))
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(request.Height, @"[\d]"))
            {
                throw new ArgumentException("Height must contain numbers.");
            }
        }

        if (!string.IsNullOrWhiteSpace(request.EyeColor))
        {
            var validEyeColors = new[] { "BLK", "BLU", "BRO", "GRN", "GRY", "HAZ", "MAR", "PNK", "DIC", "UNK" };
            if (!validEyeColors.Contains(request.EyeColor.ToUpperInvariant()))
            {
                throw new ArgumentException($"Eye color must be one of: {string.Join(", ", validEyeColors)}");
            }
        }

        if (!string.IsNullOrWhiteSpace(request.HairColor))
        {
            var validHairColors = new[] { "BAL", "BLK", "BLN", "BRO", "GRY", "RED", "SDY", "WHI", "UNK" };
            if (!validHairColors.Contains(request.HairColor.ToUpperInvariant()))
            {
                throw new ArgumentException($"Hair color must be one of: {string.Join(", ", validHairColors)}");
            }
        }

        if (!string.IsNullOrWhiteSpace(request.MaritalStatus))
        {
            var validMaritalStatuses = new[] { "Single", "Married", "Divorced", "Widowed", "Separated", "Other" };
            if (!validMaritalStatuses.Contains(request.MaritalStatus, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Marital status must be one of: {string.Join(", ", validMaritalStatuses)}");
            }
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName) && request.FirstName.Length > 35)
        {
            throw new ArgumentException("First name cannot exceed 35 characters (AAMVA limit).");
        }

        if (!string.IsNullOrWhiteSpace(request.LastName) && request.LastName.Length > 35)
        {
            throw new ArgumentException("Last name cannot exceed 35 characters (AAMVA limit).");
        }

        if (!string.IsNullOrWhiteSpace(request.MiddleName) && request.MiddleName.Length > 35)
        {
            throw new ArgumentException("Middle name cannot exceed 35 characters (AAMVA limit).");
        }

        if (!string.IsNullOrWhiteSpace(request.AddressLine1) && request.AddressLine1.Length > 35)
        {
            throw new ArgumentException("Address line 1 cannot exceed 35 characters (AAMVA limit).");
        }

        if (!string.IsNullOrWhiteSpace(request.City) && request.City.Length > 35)
        {
            throw new ArgumentException("City cannot exceed 35 characters (AAMVA limit).");
        }

        if (!string.IsNullOrWhiteSpace(request.PostalCode) && request.PostalCode.Length > 11)
        {
            throw new ArgumentException("Postal code cannot exceed 11 characters (AAMVA limit).");
        }

        if (personalDocuments is null || personalDocuments.Count == 0)
        {
            throw new ArgumentException("At least one personal document is required.");
        }

        if (passportPhoto is null || passportPhoto.Content is null || passportPhoto.Size <= 0)
        {
            throw new ArgumentException("Passport photo is required.");
        }

        if (signatureImage is null || signatureImage.Content is null || signatureImage.Size <= 0)
        {
            throw new ArgumentException("Signature image is required.");
        }
    }

    private static bool IsLikelyImage(string contentType)
        => !string.IsNullOrWhiteSpace(contentType) && contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

    private static bool IsAllowedDocContentType(string contentType)
        => contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase)
           || IsLikelyImage(contentType);

    private static async Task<UploadFile> ValidatePassportPhotoAsync(UploadFile file, CancellationToken ct)
    {
        if (!IsLikelyImage(file.ContentType))
        {
            throw new ArgumentException("Passport photo must be an image.");
        }

        if (file.Size > MaxPassportPhotoBytes)
        {
            throw new ArgumentException("Passport photo is too large (max 5MB).");
        }

        var ms = new MemoryStream((int)Math.Min(file.Size, 10 * 1024 * 1024));
        await file.Content.CopyToAsync(ms, ct);
        ms.Position = 0;

        var format = await Image.DetectFormatAsync(ms, ct);
        if (format is null || (!format.Name.Equals("JPEG", StringComparison.OrdinalIgnoreCase) && !format.Name.Equals("PNG", StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("Passport photo must be a PNG or JPEG image.");
        }

        ms.Position = 0;
        var info = await Image.IdentifyAsync(ms, ct);
        if (info is null)
        {
            throw new ArgumentException("Passport photo could not be read as an image.");
        }

        var w = info.Width;
        var h = info.Height;

        if (w < 600 || h < 600)
        {
            throw new ArgumentException("Passport photo must be at least 600x600 pixels.");
        }
        if (w > 4000 || h > 4000)
        {
            throw new ArgumentException("Passport photo is too large (max 4000x4000).");
        }

        var ratio = (double)w / h;
        if (ratio < 0.9 || ratio > 1.1)
        {
            throw new ArgumentException("Passport photo must be square (2x2).");
        }

        ms.Position = 0;
        return file with { Content = ms, Size = ms.Length, ContentType = format.DefaultMimeType ?? file.ContentType };
    }

    private static async Task<UploadFile> ValidateSignatureImageAsync(UploadFile file, CancellationToken ct)
    {
        if (!IsLikelyImage(file.ContentType))
        {
            throw new ArgumentException("Signature must be an image (PNG/JPG).");
        }

        if (file.Size > MaxSignatureBytes)
        {
            throw new ArgumentException("Signature image is too large (max 2MB).");
        }

        var ms = new MemoryStream((int)Math.Min(file.Size, 10 * 1024 * 1024));
        await file.Content.CopyToAsync(ms, ct);
        ms.Position = 0;

        var format = await Image.DetectFormatAsync(ms, ct);
        if (format is null || (!format.Name.Equals("JPEG", StringComparison.OrdinalIgnoreCase) && !format.Name.Equals("PNG", StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("Signature must be a PNG or JPEG image.");
        }

        ms.Position = 0;
        var info = await Image.IdentifyAsync(ms, ct);
        if (info is null)
        {
            throw new ArgumentException("Signature image could not be read as an image.");
        }

        var w = info.Width;
        var h = info.Height;
        if (w < 250 || h < 60)
        {
            throw new ArgumentException("Signature image is too small.");
        }
        if (w > 3000 || h > 1200)
        {
            throw new ArgumentException("Signature image is too large.");
        }

        var ratio = (double)w / h;
        if (ratio < 2.0 || ratio > 12.0)
        {
            throw new ArgumentException("Signature image must be a wide signature (width must be much larger than height).");
        }

        ms.Position = 0;
        return file with { Content = ms, Size = ms.Length, ContentType = format.DefaultMimeType ?? file.ContentType };
    }

    private static async Task<UploadFile> NormalizeAndValidatePersonalDocumentAsync(UploadFile file, CancellationToken ct)
    {
        if (file.Content is null)
        {
            throw new ArgumentException("Personal document content is missing.");
        }

        if (!IsAllowedDocContentType(file.ContentType))
        {
            throw new ArgumentException("Unsupported personal document content type.");
        }

        var ms = new MemoryStream((int)Math.Min(file.Size, MaxPersonalDocumentBytes));
        await file.Content.CopyToAsync(ms, ct);
        ms.Position = 0;

        if (file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
        {
            if (!LooksLikePdf(ms))
            {
                throw new ArgumentException("Personal document declared as PDF but does not look like a valid PDF.");
            }

            ms.Position = 0;
            return file with { Content = ms, Size = ms.Length, ContentType = "application/pdf" };
        }

        var format = await Image.DetectFormatAsync(ms, ct);
        if (format is null)
        {
            throw new ArgumentException("Personal document declared as image but is not a valid image.");
        }

        ms.Position = 0;
        var info = await Image.IdentifyAsync(ms, ct);
        if (info is null)
        {
            throw new ArgumentException("Personal document declared as image but could not be read.");
        }

        if (info.Width > 6000 || info.Height > 6000)
        {
            throw new ArgumentException("Personal document image is too large (max 6000x6000).");
        }

        ms.Position = 0;
        return file with { Content = ms, Size = ms.Length, ContentType = format.DefaultMimeType ?? file.ContentType };
    }

    private static bool LooksLikePdf(Stream stream)
    {
        if (!stream.CanSeek)
        {
            return false;
        }

        var originalPos = stream.Position;
        try
        {
            stream.Position = 0;
            Span<byte> header = stackalloc byte[5];
            if (stream.Read(header) != 5)
            {
                return false;
            }

            if (!(header[0] == (byte)'%' && header[1] == (byte)'P' && header[2] == (byte)'D' && header[3] == (byte)'F' && header[4] == (byte)'-'))
            {
                return false;
            }

            var len = stream.Length;
            var tailLen = (int)Math.Min(1024, len);
            stream.Position = Math.Max(0, len - tailLen);
            var tail = new byte[tailLen];
            var read = stream.Read(tail, 0, tailLen);
            var tailText = System.Text.Encoding.ASCII.GetString(tail, 0, read);
            return tailText.Contains("%%EOF", StringComparison.Ordinal);
        }
        finally
        {
            stream.Position = originalPos;
        }
    }

    private async Task<ApplicationUploadRecord> StoreSingleValidatedImage(
        string tenantId,
        Guid appId,
        string applicationNumber,
        DocumentNamingPattern namingPattern,
        DateTimeOffset now,
        string bucket,
        string kind,
        UploadFile file,
        Func<UploadFile, CancellationToken, Task<UploadFile>> validateAsync,
        CancellationToken ct)
    {
        if (file.Content is null)
        {
            throw new ArgumentException($"{kind} is missing.");
        }

        if (kind == "PersonalDocument" && !IsAllowedDocContentType(file.ContentType))
        {
            throw new ArgumentException("Unsupported document content type.");
        }

        var ctx = new DocumentNamingContext(
            TenantId: tenantId,
            ApplicationNumber: applicationNumber,
            ApplicationId: appId,
            Kind: kind,
            OriginalFileName: file.FileName,
            NowUtc: now);
        var key = _naming.GenerateObjectKey(namingPattern, ctx);
        var safeName = Path.GetFileName(key);
        var validated = await validateAsync(file, ct);

        await _storage.PutAsync(
            bucket,
            key,
            validated.Content,
            validated.Size,
            validated.ContentType,
            new Dictionary<string, string>
            {
                ["mamey-tenant"] = tenantId,
                ["mamey-domain"] = "citizenship",
                ["mamey-kind"] = kind,
                ["mamey-application-number"] = applicationNumber,
                ["mamey-application-id"] = appId.ToString("N"),
                ["mamey-original-file-name"] = file.FileName,
            },
            ct);

        return new ApplicationUploadRecord(
            Guid.NewGuid(),
            appId,
            kind,
            safeName,
            validated.ContentType,
            validated.Size,
            bucket,
            key,
            now);
    }
}
