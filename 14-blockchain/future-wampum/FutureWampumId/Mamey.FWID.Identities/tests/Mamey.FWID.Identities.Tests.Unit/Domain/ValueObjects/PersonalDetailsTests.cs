using Mamey.FWID.Identities.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Domain.ValueObjects;

public class PersonalDetailsTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreatePersonalDetails()
    {
        // Arrange
        var dateOfBirth = new DateTime(1990, 1, 1);
        var placeOfBirth = "New York, NY";
        var gender = "Male";
        var clanAffiliation = "Wolf Clan";

        // Act
        var personalDetails = new PersonalDetails(dateOfBirth, placeOfBirth, gender, clanAffiliation);

        // Assert
        personalDetails.ShouldNotBeNull();
        personalDetails.DateOfBirth.ShouldBe(dateOfBirth);
        personalDetails.PlaceOfBirth.ShouldBe(placeOfBirth);
        personalDetails.Gender.ShouldBe(gender);
        personalDetails.ClanAffiliation.ShouldBe(clanAffiliation);
    }

    [Fact]
    public void Constructor_WithFutureDateOfBirth_ShouldThrowException()
    {
        // Arrange
        var dateOfBirth = DateTime.UtcNow.AddDays(1);
        var placeOfBirth = "New York, NY";
        var gender = "Male";
        var clanAffiliation = "Wolf Clan";

        // Act & Assert
        Should.Throw<ArgumentException>(
            () => new PersonalDetails(dateOfBirth, placeOfBirth, gender, clanAffiliation));
    }

    [Fact]
    public void Constructor_WithNullPlaceOfBirth_ShouldSetEmptyString()
    {
        // Arrange
        var dateOfBirth = new DateTime(1990, 1, 1);
        string? placeOfBirth = null;
        var gender = "Male";
        var clanAffiliation = "Wolf Clan";

        // Act
        var personalDetails = new PersonalDetails(dateOfBirth, placeOfBirth, gender, clanAffiliation);

        // Assert
        personalDetails.PlaceOfBirth.ShouldBe(string.Empty);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var dateOfBirth = new DateTime(1990, 1, 1);
        var placeOfBirth = "New York, NY";
        var gender = "Male";
        var clanAffiliation = "Wolf Clan";

        var personalDetails1 = new PersonalDetails(dateOfBirth, placeOfBirth, gender, clanAffiliation);
        var personalDetails2 = new PersonalDetails(dateOfBirth, placeOfBirth, gender, clanAffiliation);

        // Act
        var result = personalDetails1.Equals(personalDetails2);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentDateOfBirth_ShouldReturnFalse()
    {
        // Arrange
        var placeOfBirth = "New York, NY";
        var gender = "Male";
        var clanAffiliation = "Wolf Clan";

        var personalDetails1 = new PersonalDetails(new DateTime(1990, 1, 1), placeOfBirth, gender, clanAffiliation);
        var personalDetails2 = new PersonalDetails(new DateTime(1985, 1, 1), placeOfBirth, gender, clanAffiliation);

        // Act
        var result = personalDetails1.Equals(personalDetails2);

        // Assert
        result.ShouldBeFalse();
    }
}

