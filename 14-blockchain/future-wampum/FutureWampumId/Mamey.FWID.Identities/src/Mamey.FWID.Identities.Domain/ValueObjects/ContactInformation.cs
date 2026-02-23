using System.Text.Json.Serialization;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.ValueObjects;

/// <summary>
/// Represents contact information for an identity.
/// </summary>
public class ContactInformation : IEquatable<ContactInformation>
{
    private readonly List<Phone> _phoneNumbers = new();

    /// <summary>
    /// Initializes a new instance for Entity Framework.
    /// </summary>
    private ContactInformation()
    {
        _phoneNumbers = new List<Phone>();
    }

    /// <summary>
    /// Initializes a new instance with an email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    public ContactInformation(string? email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            Email = new Email(email);
        }
        _phoneNumbers = new List<Phone>();
    }

    /// <summary>
    /// Initializes a new instance with the specified contact information.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="address">The address.</param>
    /// <param name="phoneNumbers">The phone numbers.</param>
    [JsonConstructor]
    public ContactInformation(Email? email, Address? address, List<Phone>? phoneNumbers)
    {
        Email = email;
        Address = address;
        _phoneNumbers = phoneNumbers?.ToList() ?? new List<Phone>();
    }

    /// <summary>
    /// The email address.
    /// </summary>
    public Email? Email { get; private set; }

    /// <summary>
    /// The address.
    /// </summary>
    public Address? Address { get; private set; }

    /// <summary>
    /// The phone numbers.
    /// </summary>
    public IReadOnlyList<Phone> PhoneNumbers => _phoneNumbers.AsReadOnly();

    /// <summary>
    /// Sets or updates the email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    public void UpdateEmail(Email? email)
    {
        Email = email;
    }

    /// <summary>
    /// Sets or updates the email address from a string.
    /// </summary>
    /// <param name="email">The email address as a string.</param>
    public void UpdateEmail(string? email)
    {
        Email = string.IsNullOrWhiteSpace(email) ? null : new Email(email);
    }

    /// <summary>
    /// Sets or updates the address.
    /// </summary>
    /// <param name="address">The address.</param>
    public void UpdateAddress(Address? address)
    {
        Address = address;
    }

    /// <summary>
    /// Adds a phone number to the contact information.
    /// </summary>
    /// <param name="phone">The phone number to add.</param>
    public void AddPhoneNumber(Phone phone)
    {
        if (phone is null)
            throw new ArgumentNullException(nameof(phone));

        if (!_phoneNumbers.Contains(phone))
        {
            _phoneNumbers.Add(phone);
        }
    }

    /// <summary>
    /// Removes a phone number from the contact information.
    /// </summary>
    /// <param name="phone">The phone number to remove.</param>
    public void RemovePhoneNumber(Phone phone)
    {
        if (phone is null)
            throw new ArgumentNullException(nameof(phone));

        _phoneNumbers.Remove(phone);
    }

    /// <summary>
    /// Clears all phone numbers.
    /// </summary>
    public void ClearPhoneNumbers()
    {
        _phoneNumbers.Clear();
    }

    public bool Equals(ContactInformation? other)
    {
        if (other is null)
            return false;
        
        if (ReferenceEquals(this, other))
            return true;
        
        return Equals(Email, other.Email) &&
               Equals(Address, other.Address) &&
               PhoneNumbers.SequenceEqual(other.PhoneNumbers);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ContactInformation);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Email, Address, PhoneNumbers);
    }

    public static bool operator ==(ContactInformation? left, ContactInformation? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ContactInformation? left, ContactInformation? right)
    {
        return !Equals(left, right);
    }
}

