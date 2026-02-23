using Mamey.Government.Modules.Citizens.Core.Domain.Entities;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Tenant.Core.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Government.Tests.Unit.Modules.Citizens;

public class CitizenTests
{
    [Fact]
    public void Citizen_ShouldBeCreated_WithValidData()
    {
        // Arrange
        var citizenId = new CitizenId(Guid.NewGuid());
        var tenantId = new TenantId(Guid.NewGuid());
        var firstName = new Name("John");
        var lastName = new Name("Doe");
        var dateOfBirth = new DateTime(1990, 1, 15);

        // Act
        var citizen = new Citizen(
            citizenId,
            tenantId,
            firstName,
            lastName,
            dateOfBirth,
            CitizenshipStatus.Probationary);

        // Assert
        citizen.Should().NotBeNull();
        citizen.Id.Should().Be(citizenId);
        citizen.TenantId.Should().Be(tenantId);
        citizen.FirstName.Should().Be(firstName);
        citizen.LastName.Should().Be(lastName);
        citizen.DateOfBirth.Should().Be(dateOfBirth);
        citizen.Status.Should().Be(CitizenshipStatus.Probationary);
    }

    [Fact]
    public void Citizen_ShouldProgressStatus_FromProbationaryToResident()
    {
        // Arrange
        var citizen = CreateTestCitizen(CitizenshipStatus.Probationary);

        // Act
        citizen.ProgressStatus(CitizenshipStatus.Resident);

        // Assert
        citizen.Status.Should().Be(CitizenshipStatus.Resident);
    }

    [Fact]
    public void Citizen_ShouldProgressStatus_FromResidentToCitizen()
    {
        // Arrange
        var citizen = CreateTestCitizen(CitizenshipStatus.Resident);

        // Act
        citizen.ProgressStatus(CitizenshipStatus.Citizen);

        // Assert
        citizen.Status.Should().Be(CitizenshipStatus.Citizen);
    }

    [Theory]
    [InlineData(CitizenshipStatus.Probationary)]
    [InlineData(CitizenshipStatus.Resident)]
    [InlineData(CitizenshipStatus.Citizen)]
    public void Citizen_ShouldRetainStatus_WhenDeactivated(CitizenshipStatus status)
    {
        // Arrange
        var citizen = CreateTestCitizen(status);

        // Act
        citizen.Deactivate();

        // Assert
        citizen.IsActive.Should().BeFalse();
        citizen.Status.Should().Be(status);
    }

    private static Citizen CreateTestCitizen(CitizenshipStatus status)
    {
        return new Citizen(
            new CitizenId(Guid.NewGuid()),
            new TenantId(Guid.NewGuid()),
            new Name("Test"),
            new Name("User"),
            new DateTime(1990, 1, 1),
            status);
    }
}
