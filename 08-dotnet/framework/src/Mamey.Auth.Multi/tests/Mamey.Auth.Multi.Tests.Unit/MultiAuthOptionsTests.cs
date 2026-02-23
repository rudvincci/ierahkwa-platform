using FluentAssertions;
using Mamey.Auth.Multi;
using Xunit;

namespace Mamey.Auth.Multi.Tests.Unit;

public class MultiAuthOptionsTests
{
    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var options = new MultiAuthOptions();

        // Assert
        options.EnableJwt.Should().BeFalse();
        options.EnableDid.Should().BeFalse();
        options.EnableAzure.Should().BeFalse();
        options.EnableIdentity.Should().BeFalse();
        options.EnableDistributed.Should().BeFalse();
        options.EnableCertificate.Should().BeFalse();
        options.Policy.Should().Be(AuthenticationPolicy.EitherOr);
        options.JwtScheme.Should().Be("Bearer");
        options.DidScheme.Should().Be("DidBearer");
        options.AzureScheme.Should().Be("AzureAD");
        options.IdentityScheme.Should().Be("Identity");
        options.DistributedScheme.Should().Be("Distributed");
        options.CertificateScheme.Should().Be("Certificate");
        options.JwtSectionName.Should().Be("jwt");
        options.DidSectionName.Should().Be("didAuth");
        options.AzureSectionName.Should().Be("azure");
        options.IdentitySectionName.Should().Be("auth");
        options.DistributedSectionName.Should().Be("distributedAuth");
        options.CertificateSectionName.Should().Be("certificateAuth");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EnableFlags_WhenSet_ShouldReturnCorrectValues(bool value)
    {
        // Arrange
        var options = new MultiAuthOptions
        {
            EnableJwt = value,
            EnableDid = value,
            EnableAzure = value,
            EnableIdentity = value,
            EnableDistributed = value,
            EnableCertificate = value
        };

        // Act & Assert
        options.EnableJwt.Should().Be(value);
        options.EnableDid.Should().Be(value);
        options.EnableAzure.Should().Be(value);
        options.EnableIdentity.Should().Be(value);
        options.EnableDistributed.Should().Be(value);
        options.EnableCertificate.Should().Be(value);
    }

    [Theory]
    [InlineData(AuthenticationPolicy.JwtOnly)]
    [InlineData(AuthenticationPolicy.DidOnly)]
    [InlineData(AuthenticationPolicy.AzureOnly)]
    [InlineData(AuthenticationPolicy.IdentityOnly)]
    [InlineData(AuthenticationPolicy.DistributedOnly)]
    [InlineData(AuthenticationPolicy.CertificateOnly)]
    [InlineData(AuthenticationPolicy.EitherOr)]
    [InlineData(AuthenticationPolicy.PriorityOrder)]
    [InlineData(AuthenticationPolicy.AllRequired)]
    public void Policy_WhenSet_ShouldReturnCorrectValue(AuthenticationPolicy policy)
    {
        // Arrange
        var options = new MultiAuthOptions { Policy = policy };

        // Act & Assert
        options.Policy.Should().Be(policy);
    }

    [Fact]
    public void SchemeNames_WhenSet_ShouldReturnCorrectValues()
    {
        // Arrange
        var options = new MultiAuthOptions
        {
            JwtScheme = "CustomJwt",
            DidScheme = "CustomDid",
            AzureScheme = "CustomAzure",
            IdentityScheme = "CustomIdentity",
            DistributedScheme = "CustomDistributed",
            CertificateScheme = "CustomCertificate"
        };

        // Act & Assert
        options.JwtScheme.Should().Be("CustomJwt");
        options.DidScheme.Should().Be("CustomDid");
        options.AzureScheme.Should().Be("CustomAzure");
        options.IdentityScheme.Should().Be("CustomIdentity");
        options.DistributedScheme.Should().Be("CustomDistributed");
        options.CertificateScheme.Should().Be("CustomCertificate");
    }

    [Fact]
    public void SectionNames_WhenSet_ShouldReturnCorrectValues()
    {
        // Arrange
        var options = new MultiAuthOptions
        {
            JwtSectionName = "custom:jwt",
            DidSectionName = "custom:did",
            AzureSectionName = "custom:azure",
            IdentitySectionName = "custom:identity",
            DistributedSectionName = "custom:distributed",
            CertificateSectionName = "custom:certificate"
        };

        // Act & Assert
        options.JwtSectionName.Should().Be("custom:jwt");
        options.DidSectionName.Should().Be("custom:did");
        options.AzureSectionName.Should().Be("custom:azure");
        options.IdentitySectionName.Should().Be("custom:identity");
        options.DistributedSectionName.Should().Be("custom:distributed");
        options.CertificateSectionName.Should().Be("custom:certificate");
    }
}


