#nullable enable
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Domain.ValueObjects;

/// <summary>
/// Boundary value and edge case tests for value objects.
/// Tests maximum/minimum values, empty strings, very large data, special characters, unicode.
/// </summary>
public class BoundaryValueTests
{
    #region Name Boundary Tests (Mamey.Types.Name)

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Name_WithEmptyOrWhitespaceFirstName_ShouldThrowException(string? firstName)
    {
        // Arrange - Business Rule: Mamey.Types.Name requires FirstName and LastName (cannot be null/whitespace)
        var lastName = "Doe";

        // Act & Assert
        Should.Throw<ArgumentException>(
            () => new Name(firstName!, lastName));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Name_WithEmptyOrWhitespaceLastName_ShouldThrowException(string? lastName)
    {
        // Arrange - Business Rule: Mamey.Types.Name requires FirstName and LastName (cannot be null/whitespace)
        var firstName = "John";

        // Act & Assert
        Should.Throw<ArgumentException>(
            () => new Name(firstName, lastName!));
    }

    [Theory]
    [InlineData("John", "Doe", null, null)]
    [InlineData("John", "Doe", "M.", null)]
    [InlineData("John", "Doe", null, "Johnny")]
    [InlineData("John", "Doe", "M.", "Johnny")]
    public void Name_WithValidParameters_ShouldCreateName(string firstName, string lastName, string? middleName, string? nickname)
    {
        // Arrange & Act
        var name = new Name(firstName, lastName, middleName, nickname);

        // Assert
        name.FirstName.ShouldBe(firstName);
        name.LastName.ShouldBe(lastName);
        name.MiddleName.ShouldBe(middleName);
        name.Nickname.ShouldBe(nickname);
    }

    [Theory]
    [InlineData("John", "O'Brien")]
    [InlineData("José", "García")]
    [InlineData("李", "王")]
    [InlineData("Александр", "Петров")]
    [InlineData("محمد", "أحمد")]
    public void Name_WithSpecialCharactersAndUnicode_ShouldHandleCorrectly(string firstName, string lastName)
    {
        // Arrange & Act - Business Rule: Mamey.Types.Name accepts unicode characters
        var name = new Name(firstName, lastName);

        // Assert
        name.FirstName.ShouldBe(firstName);
        name.LastName.ShouldBe(lastName);
    }

    #endregion

    #region Email Boundary Tests (Mamey.Types.Email)

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("test+tag@example.com")]
    [InlineData("test.tag@example.co.uk")]
    [InlineData("user123@subdomain.example.com")]
    [InlineData("test_email@example-domain.com")]
    [InlineData("test.email@example.com")]
    [InlineData("user_name@example.com")]
    public void Email_WithValidFormats_ShouldCreateEmail(string emailAddress)
    {
        // Arrange & Act - Business Rule: Mamey.Types.Email validates with regex and converts to lowercase
        var email = new Email(emailAddress);

        // Assert
        email.Value.ShouldBe(emailAddress.ToLowerInvariant()); // Email converts to lowercase
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test@.com")]
    // Note: "test..test@example.com" is valid according to the current Email regex pattern
    [InlineData("test@example")]
    [InlineData("test@example.")]
    public void Email_WithInvalidFormats_ShouldThrowException(string emailAddress)
    {
        // Arrange & Act & Assert - Business Rule: Mamey.Types.Email throws InvalidEmailException for invalid formats
        Should.Throw<Mamey.Exceptions.InvalidEmailException>(
            () => new Email(emailAddress));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Email_WithEmptyOrNull_ShouldThrowException(string? emailAddress)
    {
        // Arrange & Act & Assert - Business Rule: Mamey.Types.Email requires non-null, non-whitespace value
        Should.Throw<Mamey.Exceptions.InvalidEmailException>(
            () => new Email(emailAddress!));
    }

    [Fact]
    public void Email_WithMaximumLength_ShouldCreateEmail()
    {
        // Arrange - Business Rule: Mamey.Types.Email has MaxEmailLength = 100
        // Create email with exactly 100 characters: 88 'a's + "@example.com" (11 chars) = 99 total
        // Note: MaxEmailLength is 100, so we need <= 100 characters. Using 99 to be safe.
        var emailAddress = new string('a', 88) + "@example.com"; // 88 + 11 = 99 characters

        // Act
        var email = new Email(emailAddress);

        // Assert
        email.Value.ShouldBe(emailAddress.ToLowerInvariant());
    }

    [Fact]
    public void Email_ExceedingMaximumLength_ShouldThrowException()
    {
        // Arrange - Business Rule: Mamey.Types.Email has MaxEmailLength = 100
        var emailAddress = "a".PadRight(100, 'a') + "@example.com"; // 101+ characters

        // Act & Assert
        Should.Throw<Mamey.Exceptions.InvalidEmailException>(
            () => new Email(emailAddress));
    }

    [Theory]
    [InlineData("test+special@example.com")]
    [InlineData("test_tag@example.com")]
    [InlineData("test.tag@example.com")]
    [InlineData("user@example-domain.com")]
    [InlineData("user.name+tag@example-domain.com")]
    public void Email_WithSpecialCharacters_ShouldHandleCorrectly(string emailAddress)
    {
        // Arrange & Act - Business Rule: Mamey.Types.Email accepts special characters per regex
        var email = new Email(emailAddress);

        // Assert
        email.Value.ShouldBe(emailAddress.ToLowerInvariant());
    }

    #endregion

    #region BiometricData Boundary Tests

    [Theory]
    [InlineData(1)]
    [InlineData(1024)]
    [InlineData(1024 * 1024)] // 1MB
    [InlineData(10 * 1024 * 1024)] // 10MB
    public void BiometricData_WithVariousSizes_ShouldHandleCorrectly(int size)
    {
        // Arrange
        var biometricBytes = new byte[size];
        Array.Fill(biometricBytes, (byte)1);
        var hash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();

        // Act
        var biometricData = new BiometricData(BiometricType.Fingerprint, biometricBytes, hash);

        // Assert
        biometricData.EncryptedTemplate.Length.ShouldBe(size);
        biometricData.Hash.ShouldBe(hash);
    }

    [Fact]
    public void BiometricData_WithEmptyTemplate_ShouldThrowException()
    {
        // Arrange
        var biometricBytes = Array.Empty<byte>(); // Empty template should throw exception
        var hash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant();

        // Act & Assert
        Should.Throw<ArgumentException>(
            () => new BiometricData(BiometricType.Fingerprint, biometricBytes, hash));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void BiometricData_WithInvalidHash_ShouldThrowException(string? hash)
    {
        // Arrange
        var biometricBytes = new byte[] { 1, 2, 3 };

        // Act & Assert
        if (hash == null)
        {
            Should.Throw<ArgumentNullException>(
                () => new BiometricData(BiometricType.Fingerprint, biometricBytes, hash!));
        }
        else
        {
            Should.Throw<ArgumentException>(
                () => new BiometricData(BiometricType.Fingerprint, biometricBytes, hash));
        }
    }

    #endregion

    #region PersonalDetails Boundary Tests

    [Theory]
    [InlineData("1900-01-01")]
    [InlineData("2000-01-01")]
    [InlineData("2020-01-01")]
    public void PersonalDetails_WithValidDates_ShouldCreatePersonalDetails(string dateString)
    {
        // Arrange
        var dateOfBirth = DateTime.Parse(dateString);
        var placeOfBirth = "New York, NY";
        var gender = "Male";
        var clanAffiliation = "Wolf Clan";

        // Act
        var personalDetails = new PersonalDetails(dateOfBirth, placeOfBirth, gender, clanAffiliation);

        // Assert
        personalDetails.DateOfBirth.ShouldBe(dateOfBirth);
    }

    [Fact]
    public void PersonalDetails_WithFutureDate_ShouldThrowException()
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

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void PersonalDetails_WithEmptyPlaceOfBirth_ShouldSetEmptyString(string? placeOfBirth)
    {
        // Arrange
        var dateOfBirth = new DateTime(1990, 1, 1);
        var gender = "Male";
        var clanAffiliation = "Wolf Clan";

        // Act
        var personalDetails = new PersonalDetails(dateOfBirth, placeOfBirth, gender, clanAffiliation);

        // Assert
        personalDetails.PlaceOfBirth.ShouldBe(string.Empty);
    }
    
    [Fact]
    public void PersonalDetails_WithWhitespacePlaceOfBirth_ShouldPreserveWhitespace()
    {
        // Arrange
        var dateOfBirth = new DateTime(1990, 1, 1);
        var placeOfBirth = " ";
        var gender = "Male";
        var clanAffiliation = "Wolf Clan";

        // Act
        var personalDetails = new PersonalDetails(dateOfBirth, placeOfBirth, gender, clanAffiliation);

        // Assert
        // Note: PersonalDetails constructor doesn't trim whitespace, so whitespace is preserved
        personalDetails.PlaceOfBirth.ShouldBe(placeOfBirth);
    }

    [Theory]
    [InlineData("New York, NY")]
    [InlineData("São Paulo, SP")]
    [InlineData("北京, 中国")]
    [InlineData("Москва, Россия")]
    [InlineData("القاهرة, مصر")]
    public void PersonalDetails_WithUnicodePlaceOfBirth_ShouldHandleCorrectly(string placeOfBirth)
    {
        // Arrange
        var dateOfBirth = new DateTime(1990, 1, 1);
        var gender = "Male";
        var clanAffiliation = "Wolf Clan";

        // Act
        var personalDetails = new PersonalDetails(dateOfBirth, placeOfBirth, gender, clanAffiliation);

        // Assert
        personalDetails.PlaceOfBirth.ShouldBe(placeOfBirth);
    }

    #endregion

    #region Address Boundary Tests (Mamey.Types.Address)

    [Theory]
    [InlineData("123 Main St", "New York", "NY", "10001", "US")]
    [InlineData("123 Main St", "Los Angeles", "CA", "90001", "US")]
    public void Address_WithValidUSAddress_ShouldCreateAddress(string line, string city, string state, string zip5, string country)
    {
        // Arrange & Act - Business Rule: Mamey.Types.Address requires Country, Line, City for US addresses
        // US addresses also require State and Zip5 (5-digit number)
        var address = new Address("", line, null, null, null, city, state, zip5, null, null, country, null);

        // Assert
        address.Line.ShouldBe(line);
        address.City.ShouldBe(city);
        address.State.ShouldBe(state);
        address.Zip5.ShouldBe(zip5);
        address.Country.ShouldBe(country);
        address.IsUSAddress.ShouldBeTrue();
    }

    [Theory]
    [InlineData("Calle 123 #45-67", "Bogotá", "Cundinamarca", "110111", "CO")]
    [InlineData("ул. Ленина, д. 10", "Москва", "Московская область", "101000", "RU")]
    [InlineData("123 Main St", "Toronto", "Ontario", "M5H 2N2", "CA")]
    public void Address_WithNonUSAddress_ShouldCreateAddress(string line, string city, string province, string postalCode, string country)
    {
        // Arrange & Act - Business Rule: Mamey.Types.Address requires Country, Line, City, PostalCode, Province for non-US addresses
        var address = new Address("", line, null, null, null, city, "", "", null, postalCode, country, province);

        // Assert
        address.Line.ShouldBe(line);
        address.City.ShouldBe(city);
        address.Province.ShouldBe(province);
        address.PostalCode.ShouldBe(postalCode);
        address.Country.ShouldBe(country);
        address.IsUSAddress.ShouldBeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Address_WithEmptyLine_ShouldThrowException(string? line)
    {
        // Arrange & Act & Assert - Business Rule: Mamey.Types.Address requires Line (cannot be null/whitespace)
        Should.Throw<ArgumentException>(
            () => new Address("", line!, null, null, null, "New York", "NY", "10001", null, null, "US", null));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Address_WithEmptyCity_ShouldThrowException(string? city)
    {
        // Arrange & Act & Assert - Business Rule: Mamey.Types.Address requires City (cannot be null/whitespace)
        Should.Throw<ArgumentException>(
            () => new Address("", "123 Main St", null, null, null, city!, "NY", "10001", null, null, "US", null));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Address_WithEmptyCountry_ShouldThrowException(string? country)
    {
        // Arrange & Act & Assert - Business Rule: Mamey.Types.Address requires Country (cannot be null/whitespace)
        Should.Throw<ArgumentException>(
            () => new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, country!, null));
    }

    [Theory]
    [InlineData("1234")] // Too short
    [InlineData("123456")] // Too long
    [InlineData("abcde")] // Not numeric
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Address_WithInvalidUSZip5_ShouldThrowException(string? zip5)
    {
        // Arrange & Act & Assert - Business Rule: Mamey.Types.Address requires Zip5 to be exactly 5 digits for US addresses
        Should.Throw<ArgumentException>(
            () => new Address("", "123 Main St", null, null, null, "New York", "NY", zip5!, null, null, "US", null));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Address_WithEmptyUSState_ShouldThrowException(string? state)
    {
        // Arrange & Act & Assert - Business Rule: Mamey.Types.Address requires State for US addresses
        Should.Throw<ArgumentException>(
            () => new Address("", "123 Main St", null, null, null, "New York", state!, "10001", null, null, "US", null));
    }

    [Theory]
    [InlineData("123 Main St, Apt 4B", "New York", "NY", "10001", "US")]
    [InlineData("Calle 123 #45-67", "Bogotá", "Cundinamarca", "110111", "CO")]
    [InlineData("ул. Ленина, д. 10", "Москва", "Московская область", "101000", "RU")]
    public void Address_WithSpecialCharactersAndUnicode_ShouldHandleCorrectly(string line, string city, string stateOrProvince, string zipOrPostalCode, string country)
    {
        // Arrange & Act - Business Rule: Mamey.Types.Address accepts unicode characters
        Address address;
        if (country == "US")
        {
            address = new Address("", line, null, null, null, city, stateOrProvince, zipOrPostalCode, null, null, country, null);
        }
        else
        {
            address = new Address("", line, null, null, null, city, "", "", null, zipOrPostalCode, country, stateOrProvince);
        }

        // Assert
        address.Line.ShouldBe(line);
        address.City.ShouldBe(city);
        address.Country.ShouldBe(country);
    }

    #endregion

    #region Phone Boundary Tests (Mamey.Types.Phone)

    [Theory]
    [InlineData("1", "5551234567")]
    [InlineData("1", "555-123-4567")]
    [InlineData("1", "(555) 123-4567")]
    [InlineData("44", "20 1234 5678")]
    [InlineData("33", "1 23 45 67 89")]
    [InlineData("1", "5551234567", "123")]
    [InlineData("1", "5551234567", null, Phone.PhoneType.Mobile)]
    [InlineData("1", "5551234567", null, Phone.PhoneType.Home)]
    [InlineData("1", "5551234567", null, Phone.PhoneType.Fax)]
    public void Phone_WithValidParameters_ShouldCreatePhone(string countryCode, string number, string? extension = null, Phone.PhoneType type = Phone.PhoneType.Main)
    {
        // Arrange & Act - Business Rule: Mamey.Types.Phone requires CountryCode and Number (cannot be null/whitespace)
        var phone = new Phone(countryCode, number, extension, type);

        // Assert
        phone.CountryCode.ShouldBe(countryCode);
        phone.Number.ShouldBe(number);
        phone.Extension.ShouldBe(extension);
        phone.Type.ShouldBe(type);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Phone_WithEmptyCountryCode_ShouldThrowException(string? countryCode)
    {
        // Arrange & Act & Assert - Business Rule: Mamey.Types.Phone requires CountryCode (cannot be null/whitespace)
        Should.Throw<ArgumentException>(
            () => new Phone(countryCode!, "5551234567", null, Phone.PhoneType.Mobile));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Phone_WithEmptyNumber_ShouldThrowException(string? number)
    {
        // Arrange & Act & Assert - Business Rule: Mamey.Types.Phone requires Number (cannot be null/whitespace)
        Should.Throw<ArgumentException>(
            () => new Phone("1", number!, null, Phone.PhoneType.Mobile));
    }

    [Theory]
    [InlineData(Phone.PhoneType.Main)]
    [InlineData(Phone.PhoneType.Home)]
    [InlineData(Phone.PhoneType.Mobile)]
    [InlineData(Phone.PhoneType.Fax)]
    [InlineData(Phone.PhoneType.Other)]
    public void Phone_WithAllPhoneTypes_ShouldCreatePhone(Phone.PhoneType type)
    {
        // Arrange & Act
        var phone = new Phone("1", "5551234567", null, type);

        // Assert
        phone.Type.ShouldBe(type);
    }

    #endregion

    #region Threshold Boundary Tests

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.01)]
    [InlineData(0.5)]
    [InlineData(0.85)]
    [InlineData(0.99)]
    [InlineData(1.0)]
    public void VerifyBiometric_WithVariousThresholds_ShouldHandleCorrectly(double threshold)
    {
        // Arrange
        var identity = new Identity(
            new IdentityId(Guid.NewGuid()),
            new Name("John", "Doe"),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);

        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);

        // Act
        if (threshold < 0.85)
        {
            // Low thresholds might allow verification
            identity.VerifyBiometric(providedBiometric, threshold);
            identity.Status.ShouldBeOneOf(IdentityStatus.Verified, IdentityStatus.Pending);
        }
        else
        {
            // High thresholds require perfect match
            identity.VerifyBiometric(providedBiometric, threshold);
            identity.Status.ShouldBe(IdentityStatus.Verified);
        }
    }

    #endregion
}

