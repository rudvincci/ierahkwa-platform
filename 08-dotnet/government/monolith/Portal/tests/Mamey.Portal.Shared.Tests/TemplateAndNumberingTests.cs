using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Shared.Storage.Templates;

namespace Mamey.Portal.Shared.Tests;

public sealed class TemplateAndNumberingTests
{
    [Theory]
    [InlineData("Passport", new[] { "Passport" })]
    [InlineData("IdCard:MedicinalCannabis", new[] { "IdCard:MedicinalCannabis", "IdCard" })]
    [InlineData("VehicleTag:Veteran", new[] { "VehicleTag:Veteran", "VehicleTag" })]
    [InlineData("CitizenshipCertificate", new[] { "CitizenshipCertificate" })]
    public void TemplateKindFallback_ReturnsCandidates(string kind, string[] expected)
        => Assert.Equal(expected, DocumentTemplateKindFallback.GetCandidateKinds(kind));

    [Fact]
    public void IssuedDocumentNumbers_AreUniqueAcrossVariants()
    {
        var tenantId = "default";
        var appId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        var now = new DateTimeOffset(2026, 01, 05, 12, 0, 0, TimeSpan.Zero);

        var id1 = IssuedDocumentNumberGenerator.IdCard(tenantId, now, appId, "IdentificationCard");
        var id2 = IssuedDocumentNumberGenerator.IdCard(tenantId, now, appId, "MedicinalCannabis");
        Assert.NotEqual(id1, id2);
        Assert.Contains("IDENTIFICATIONCARD", id1);
        Assert.Contains("MEDICINALCANNABIS", id2);

        var tag1 = IssuedDocumentNumberGenerator.VehicleTag(tenantId, now, appId, "Standard");
        var tag2 = IssuedDocumentNumberGenerator.VehicleTag(tenantId, now, appId, "Veteran");
        Assert.NotEqual(tag1, tag2);
        Assert.Contains("STANDARD", tag1);
        Assert.Contains("VETERAN", tag2);

        var pass = IssuedDocumentNumberGenerator.Passport(tenantId, now, appId);
        Assert.StartsWith("P-", pass);
    }
}




