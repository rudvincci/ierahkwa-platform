using Mamey.Portal.Shared.Auth;
using Mamey.Portal.Web.Tenancy;

namespace Mamey.Portal.Shared.Tests;

public sealed class NormalizationTests
{
    [Theory]
    [InlineData("http://localhost:9100/", "http://localhost:9100")]
    [InlineData("http://localhost:9100", "http://localhost:9100")]
    [InlineData("  http://localhost:9100/application/o/mamey-portal/  ", "http://localhost:9100/application/o/mamey-portal")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void OidcIssuerNormalizer_Normalizes(string? input, string expected)
        => Assert.Equal(expected, OidcIssuerNormalizer.Normalize(input));

    [Theory]
    [InlineData("Default", "default")]
    [InlineData(" Ierahkwa ne Kanienke ", "ierahkwa-ne-kanienke")]
    [InlineData("a--b", "a--b")]
    [InlineData("a__b", "ab")]
    [InlineData("----", "")]
    public void TenantId_Normalization_MatchesExpectations(string input, string expected)
        => Assert.Equal(expected, ClaimsTenantContext.NormalizeTenantId(input));
}
