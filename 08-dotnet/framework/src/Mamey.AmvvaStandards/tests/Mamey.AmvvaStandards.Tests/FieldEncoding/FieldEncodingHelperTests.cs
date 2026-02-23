namespace Mamey.AmvvaStandards.Tests.FieldEncoding;

public class FieldEncodingHelperTests
{
    [Fact]
    public void EncodeDriverLicenseCard_ShouldContainSubfileDesignatorDL()
    {
        // Arrange
        var card = new DriverLicenseCard
        {
            LicenseOrIdNumber = "LIC12345",
            FamilyName = "Doe",
            GivenName = "John",
            DateOfBirth = new DateTime(1990, 1, 1),
            IssueDate = new DateTime(2020, 1, 1),
            ExpirationDate = new DateTime(2025, 1, 1),
            Sex = Sex.Male,
            EyeColor = EyeColor.BLU,
            HairColor = HairColor.BRO,
            HeightInches = 70,
            LicenseClass = "C",
            Restrictions = "None",
            Endorsements = "None",
            DocumentDiscriminator = "DOC999",
            Revision = CardDesignRevision.AAMVA2013
        };

        // Act
        var encoded = FieldEncodingHelper.EncodeDriverLicenseCard(card);

        // Assert
        // Expect it to contain "DL" + data element codes (e.g. "DAQ", "DCS", "DAC", etc.)
        Assert.Contains("DL", encoded);
        Assert.Contains("DAQ" + card.LicenseOrIdNumber, encoded);
        Assert.Contains("DCS" + card.FamilyName, encoded);
        Assert.Contains("DAC" + card.GivenName, encoded);
        Assert.Contains("DBC" + (int)card.Sex, encoded); // Sex = Male => 1
    }

    [Fact]
    public void EncodeIdentificationCard_ShouldContainSubfileDesignatorID()
    {
        // Arrange
        var card = new IdentificationCard
        {
            LicenseOrIdNumber = "ID12345",
            FamilyName = "Smith",
            GivenName = "Jane",
            DateOfBirth = new DateTime(1995, 5, 5),
            IssueDate = new DateTime(2021, 5, 5),
            ExpirationDate = new DateTime(2026, 5, 5),
            Sex = Sex.Female,
            EyeColor = EyeColor.HAZ,
            HairColor = HairColor.BLN,
            HeightInches = 65,
            DocumentDiscriminator = "ID-DOC888",
            Suffix = "3RD",
            Revision = CardDesignRevision.AAMVA2010
        };

        // Act
        var encoded = FieldEncodingHelper.EncodeIdentificationCard(card);

        // Assert
        Assert.Contains("ID", encoded);
        Assert.Contains(AamvaFieldDefinitions.LicenseNumber + card.LicenseOrIdNumber, encoded);
        Assert.Contains(AamvaFieldDefinitions.FamilyName + card.FamilyName, encoded);
        Assert.Contains(AamvaFieldDefinitions.GivenName + card.GivenName, encoded);
        Assert.Contains(AamvaFieldDefinitions.NameSuffix + card.Suffix, encoded); // check suffix? Only if it’s set
        Assert.Contains("DBC" + (int)card.Sex, encoded); // Sex = Female => 2
    }
}