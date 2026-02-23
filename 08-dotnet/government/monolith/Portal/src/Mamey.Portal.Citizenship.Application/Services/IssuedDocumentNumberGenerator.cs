namespace Mamey.Portal.Citizenship.Application.Services;

public static class IssuedDocumentNumberGenerator
{
    public static string Passport(string tenantId, DateTimeOffset issuedAtUtc, Guid applicationId)
    {
        var t = NormalizeTenantForNumber(tenantId);
        var suffix = ApplicationSuffix(applicationId);
        return $"P-{t}-{issuedAtUtc:yyMMdd}-{suffix}".ToUpperInvariant();
    }

    public static string IdCard(string tenantId, DateTimeOffset issuedAtUtc, Guid applicationId, string variant)
    {
        var t = NormalizeTenantForNumber(tenantId);
        var suffix = ApplicationSuffix(applicationId);
        var v = NormalizeVariant(variant, "IdentificationCard");
        return $"ID-{t}-{issuedAtUtc:yyMMdd}-{suffix}-{v}".ToUpperInvariant();
    }

    public static string VehicleTag(string tenantId, DateTimeOffset issuedAtUtc, Guid applicationId, string variant)
    {
        var t = NormalizeTenantForNumber(tenantId);
        var suffix = ApplicationSuffix(applicationId);
        var v = NormalizeVariant(variant, "Standard");
        return $"TAG-{t}-{issuedAtUtc:yyMMdd}-{suffix}-{v}".ToUpperInvariant();
    }

    private static string NormalizeTenantForNumber(string tenantId)
        => (tenantId ?? string.Empty).Trim().ToUpperInvariant();

    private static string NormalizeVariant(string? variant, string fallback)
    {
        variant = (variant ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(variant))
        {
            variant = fallback;
        }
        return variant.ToUpperInvariant();
    }

    private static string ApplicationSuffix(Guid applicationId)
        => applicationId.ToString("N")[..6].ToUpperInvariant();
}




