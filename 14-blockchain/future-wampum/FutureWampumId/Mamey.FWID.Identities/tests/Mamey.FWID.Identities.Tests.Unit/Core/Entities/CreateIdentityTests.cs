using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Core.Entities;

public class CreateIdentityTests
{
    [Fact]
    public void given_valid_parameters_identity_should_be_created()
    {
        // Arrange
        var id = new IdentityId(Guid.NewGuid());
        var name = new Name("John", "Doe", "M.");
        var personalDetails = new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan");
        var contactInfo = new ContactInformation(
            new Email("john.doe@example.com"),
            new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
            new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
        );
        var biometricData = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant());

        // Act
        var identity = new Identity(id, name, personalDetails, contactInfo, biometricData, "zone-001", null);
        
        // Assert
        identity.ShouldNotBeNull();
        identity.Id.ShouldBe(id);
        identity.Name.ShouldBe(name);
        identity.Status.ShouldBe(IdentityStatus.Pending);
        identity.Events.Count().ShouldBe(1);

        var @event = identity.Events.Single();
        @event.ShouldBeOfType<IdentityCreated>();
    }
}