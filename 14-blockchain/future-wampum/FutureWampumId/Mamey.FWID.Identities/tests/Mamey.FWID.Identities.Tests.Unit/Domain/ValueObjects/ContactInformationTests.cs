using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Domain.ValueObjects;

public class ContactInformationTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateContactInformation()
    {
        // Arrange
        var email = new Email("john.doe@example.com");
        var address = new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null);
        var phoneNumbers = new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) };

        // Act
        var contactInfo = new ContactInformation(email, address, phoneNumbers);

        // Assert
        contactInfo.ShouldNotBeNull();
        contactInfo.Email.ShouldBe(email);
        contactInfo.Address.ShouldBe(address);
        contactInfo.PhoneNumbers.Count.ShouldBe(1);
    }

    [Fact]
    public void Constructor_WithEmailString_ShouldCreateContactInformation()
    {
        // Arrange
        var email = "john.doe@example.com";

        // Act
        var contactInfo = new ContactInformation(email);

        // Assert
        contactInfo.ShouldNotBeNull();
        contactInfo.Email!.Value.ShouldBe(email);
    }

    [Fact]
    public void UpdateEmail_WithValidEmail_ShouldUpdateEmail()
    {
        // Arrange
        var contactInfo = new ContactInformation("old.email@example.com");
        var newEmail = new Email("new.email@example.com");

        // Act
        contactInfo.UpdateEmail(newEmail);

        // Assert
        contactInfo.Email.ShouldBe(newEmail);
    }

    [Fact]
    public void UpdateEmail_WithEmailString_ShouldUpdateEmail()
    {
        // Arrange
        var contactInfo = new ContactInformation("old.email@example.com");
        var newEmail = "new.email@example.com";

        // Act
        contactInfo.UpdateEmail(newEmail);

        // Assert
        contactInfo.Email!.Value.ShouldBe(newEmail);
    }

    [Fact]
    public void UpdateAddress_WithValidAddress_ShouldUpdateAddress()
    {
        // Arrange
        var contactInfo = new ContactInformation("john.doe@example.com");
        var newAddress = new Address("", "456 New St", null, null, null, "Los Angeles", "CA", "90001", null, null, "US", null);

        // Act
        contactInfo.UpdateAddress(newAddress);

        // Assert
        contactInfo.Address.ShouldBe(newAddress);
    }

    [Fact]
    public void AddPhoneNumber_WithValidPhone_ShouldAddPhoneNumber()
    {
        // Arrange
        var contactInfo = new ContactInformation("john.doe@example.com");
        var phone = new Phone("1", "5551234567", null, Phone.PhoneType.Mobile);

        // Act
        contactInfo.AddPhoneNumber(phone);

        // Assert
        contactInfo.PhoneNumbers.Count.ShouldBe(1);
        contactInfo.PhoneNumbers.ShouldContain(phone);
    }

    [Fact]
    public void AddPhoneNumber_WithDuplicatePhone_ShouldNotAddDuplicate()
    {
        // Arrange
        var contactInfo = new ContactInformation("john.doe@example.com");
        var phone = new Phone("1", "5551234567", null, Phone.PhoneType.Mobile);
        contactInfo.AddPhoneNumber(phone);

        // Act
        contactInfo.AddPhoneNumber(phone);

        // Assert
        contactInfo.PhoneNumbers.Count.ShouldBe(1);
    }

    [Fact]
    public void AddPhoneNumber_WithNull_ShouldThrowException()
    {
        // Arrange
        var contactInfo = new ContactInformation("john.doe@example.com");

        // Act & Assert
        Should.Throw<ArgumentNullException>(
            () => contactInfo.AddPhoneNumber(null!));
    }

    [Fact]
    public void RemovePhoneNumber_WithValidPhone_ShouldRemovePhoneNumber()
    {
        // Arrange
        var contactInfo = new ContactInformation("john.doe@example.com");
        var phone = new Phone("1", "5551234567", null, Phone.PhoneType.Mobile);
        contactInfo.AddPhoneNumber(phone);

        // Act
        contactInfo.RemovePhoneNumber(phone);

        // Assert
        contactInfo.PhoneNumbers.Count.ShouldBe(0);
    }

    [Fact]
    public void ClearPhoneNumbers_ShouldRemoveAllPhoneNumbers()
    {
        // Arrange
        var contactInfo = new ContactInformation("john.doe@example.com");
        contactInfo.AddPhoneNumber(new Phone("1", "5551234567", null, Phone.PhoneType.Mobile));
        contactInfo.AddPhoneNumber(new Phone("1", "5559876543", null, Phone.PhoneType.Home));

        // Act
        contactInfo.ClearPhoneNumbers();

        // Assert
        contactInfo.PhoneNumbers.Count.ShouldBe(0);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var email = new Email("john.doe@example.com");
        var address = new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null);
        var phoneNumbers = new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) };

        var contactInfo1 = new ContactInformation(email, address, phoneNumbers);
        var contactInfo2 = new ContactInformation(email, address, phoneNumbers);

        // Act
        var result = contactInfo1.Equals(contactInfo2);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentEmail_ShouldReturnFalse()
    {
        // Arrange
        var address = new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null);
        var phoneNumbers = new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) };

        var contactInfo1 = new ContactInformation(new Email("john.doe@example.com"), address, phoneNumbers);
        var contactInfo2 = new ContactInformation(new Email("jane.smith@example.com"), address, phoneNumbers);

        // Act
        var result = contactInfo1.Equals(contactInfo2);

        // Assert
        result.ShouldBeFalse();
    }
}

