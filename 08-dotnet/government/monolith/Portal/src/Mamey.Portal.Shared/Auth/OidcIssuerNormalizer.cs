namespace Mamey.Portal.Shared.Auth;

public static class OidcIssuerNormalizer
{
    public static string Normalize(string? issuer)
    {
        issuer = (issuer ?? string.Empty).Trim();
        if (issuer.Length == 0) return string.Empty;

        // Avoid mismatches due to trailing slashes in Authority vs token 'iss'
        while (issuer.EndsWith("/", StringComparison.Ordinal))
        {
            issuer = issuer[..^1];
        }

        return issuer;
    }
}




