using System.Text.Json.Serialization;
using Mamey.Types;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

internal class ContactInformation
{
    public ContactInformation()
    {
        PhoneNumbers = new();
    }

    public ContactInformation(string? email)
    {
        Email = new Email(email);
    }

    [JsonConstructor]
    public ContactInformation(Email? email, Address? residentialAddress,
        Address? postalAddress, List<Phone> phoneNumbers)
    {
        Email = email;
        ResidentialAddress = residentialAddress;
        PostalAddress = postalAddress;
        PhoneNumbers = phoneNumbers;
    }

    public Email? Email { get; private set; } = null;
    public Address? ResidentialAddress { get; private set; }
    public Address? PostalAddress { get; private set; }
    public List<Phone> PhoneNumbers { get; private set; } = new List<Phone>();
    
    public string EmergencyContactName { get; init; } = string.Empty;
    public string EmergencyContactPhoneNumber { get; init; } = string.Empty;
    public string EmergencyContactEmail { get; init; } = string.Empty;
    public string EmergencyContactAddress { get; init; } = string.Empty;
    public string EmergencyContactRelationship { get; set; } = string.Empty;

    public ContactInformation SetResidentialAddress(Address address)
    {
        ResidentialAddress = address ?? throw new ArgumentNullException(nameof(address));
        return this;
    }

    public ContactInformation SetPostalAddress(Address address)
    {
        PostalAddress = address ?? throw new ArgumentNullException(nameof(address));
        return this;
    }

    public ContactInformation AddPhoneNumber(Phone phone)
    {
        if (phone is null)
        {
            throw new ArgumentNullException(nameof(phone));
        }

        PhoneNumbers.Add(phone);
        return this;
    }

    public ContactInformation RemovePhoneNumber(Phone phone)
    {
        if (phone is null)
        {
            throw new ArgumentNullException(nameof(phone));
        }
        PhoneNumbers.Remove(phone);
        return this;
    }
}
