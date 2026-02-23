using System.Text.RegularExpressions;

namespace Mamey.Portal.Shared.Storage.DocumentNaming;

public sealed class DefaultDocumentNamingService : IDocumentNamingService
{
    private static readonly Regex InvalidPathChars = new(@"[^a-zA-Z0-9\-\._/]+", RegexOptions.Compiled);

    public string GenerateObjectKey(DocumentNamingPattern pattern, DocumentNamingContext ctx)
    {
        var template = ResolveTemplate(pattern, ctx.Kind);

        var unique = Guid.NewGuid().ToString("N");
        var safeFile = SafeFileName(ctx.OriginalFileName);

        var resolved = template
            .Replace("{TenantId}", ctx.TenantId, StringComparison.OrdinalIgnoreCase)
            .Replace("{ApplicationNumber}", ctx.ApplicationNumber, StringComparison.OrdinalIgnoreCase)
            .Replace("{ApplicationId}", ctx.ApplicationId.ToString("N"), StringComparison.OrdinalIgnoreCase)
            .Replace("{Kind}", ctx.Kind, StringComparison.OrdinalIgnoreCase)
            .Replace("{OriginalFileName}", safeFile, StringComparison.OrdinalIgnoreCase)
            .Replace("{Unique}", unique, StringComparison.OrdinalIgnoreCase)
            .Replace("{Date:yyyyMMdd}", ctx.NowUtc.ToString("yyyyMMdd"), StringComparison.OrdinalIgnoreCase);

        resolved = resolved.Replace('\\', '/').TrimStart('/');
        resolved = InvalidPathChars.Replace(resolved, "-");

        // Avoid accidental path traversal / weird segments.
        while (resolved.Contains("..", StringComparison.Ordinal))
        {
            resolved = resolved.Replace("..", ".", StringComparison.Ordinal);
        }

        return resolved;
    }

    private static string ResolveTemplate(DocumentNamingPattern pattern, string kind)
    {
        if (kind.StartsWith("IdCard", StringComparison.OrdinalIgnoreCase))
        {
            return pattern.IdCardDocumentPattern;
        }

        if (kind.StartsWith("VehicleTag", StringComparison.OrdinalIgnoreCase))
        {
            return pattern.VehicleTagDocumentPattern;
        }

        return kind switch
        {
            "PersonalDocument" => pattern.PersonalDocumentPattern,
            "PassportPhoto" => pattern.PassportPhotoPattern,
            "SignatureImage" => pattern.SignatureImagePattern,
            "CitizenshipCertificate" => pattern.CitizenshipCertificatePattern,
            "BirthCertificate" => pattern.BirthCertificatePattern,
            "MarriageCertificate" => pattern.MarriageCertificatePattern,
            "NameChangeCertificate" => pattern.NameChangeCertificatePattern,
            "Passport" => pattern.PassportDocumentPattern,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown document kind."),
        };
    }

    private static string SafeFileName(string fileName)
    {
        fileName = string.IsNullOrWhiteSpace(fileName) ? "upload.bin" : Path.GetFileName(fileName);
        fileName = InvalidPathChars.Replace(fileName, "-");
        return fileName.Trim('-');
    }
}

