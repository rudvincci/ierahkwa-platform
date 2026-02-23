using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.Domain.Entities;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Tenant.Core.Domain.ValueObjects;

namespace Mamey.Government.Tests.Unit.Modules.Passports;

public class PassportTests
{
    [Fact]
    public void Passport_ShouldBeCreated_WithValidData()
    {
        // Arrange
        var passportId = new PassportId(Guid.NewGuid());
        var tenantId = new TenantId(Guid.NewGuid());
        var citizenId = new CitizenId(Guid.NewGuid());
        var passportNumber = new PassportNumber("P12345678");
        var issuedAt = DateTime.UtcNow;
        var expiresAt = issuedAt.AddYears(10);

        // Act
        var passport = new Passport(
            passportId,
            tenantId,
            citizenId,
            passportNumber,
            issuedAt,
            expiresAt);

        // Assert
        passport.Should().NotBeNull();
        passport.Id.Should().Be(passportId);
        passport.CitizenId.Should().Be(citizenId);
        passport.PassportNumber.Should().Be(passportNumber);
        passport.IssuedAt.Should().Be(issuedAt);
        passport.ExpiresAt.Should().Be(expiresAt);
        passport.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Passport_ShouldBeRevocable()
    {
        // Arrange
        var passport = CreateTestPassport();

        // Act
        passport.Revoke("Reported lost");

        // Assert
        passport.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Passport_ShouldBeExpired_WhenExpiryDatePassed()
    {
        // Arrange
        var passport = CreateTestPassport(expiresAt: DateTime.UtcNow.AddDays(-1));

        // Assert
        passport.IsExpired.Should().BeTrue();
    }

    [Fact]
    public void Passport_ShouldNotBeExpired_WhenExpiryDateInFuture()
    {
        // Arrange
        var passport = CreateTestPassport(expiresAt: DateTime.UtcNow.AddYears(5));

        // Assert
        passport.IsExpired.Should().BeFalse();
    }

    [Theory]
    [InlineData("P12345678")]
    [InlineData("P87654321")]
    [InlineData("P00000001")]
    public void PassportNumber_ShouldAcceptValidFormats(string number)
    {
        // Arrange & Act
        var passportNumber = new PassportNumber(number);

        // Assert
        passportNumber.Value.Should().Be(number);
    }

    private static Passport CreateTestPassport(DateTime? expiresAt = null)
    {
        var issuedAt = DateTime.UtcNow;
        return new Passport(
            new PassportId(Guid.NewGuid()),
            new TenantId(Guid.NewGuid()),
            new CitizenId(Guid.NewGuid()),
            new PassportNumber("P12345678"),
            issuedAt,
            expiresAt ?? issuedAt.AddYears(10));
    }
}
