using System.Text.Json.Serialization;

namespace Mamey.FWID.Identities.Domain.ValueObjects;

/// <summary>
/// Represents personal details of an identity.
/// </summary>
public class PersonalDetails : IEquatable<PersonalDetails>
{
    /// <summary>
    /// Initializes a new instance for Entity Framework.
    /// </summary>
    private PersonalDetails()
    {
        PlaceOfBirth = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance with the specified personal details.
    /// </summary>
    /// <param name="dateOfBirth">The date of birth.</param>
    /// <param name="placeOfBirth">The place of birth.</param>
    /// <param name="gender">The gender.</param>
    /// <param name="clanAffiliation">The clan affiliation.</param>
    [JsonConstructor]
    public PersonalDetails(DateTime? dateOfBirth, string? placeOfBirth, string? gender, string? clanAffiliation)
    {
        if (dateOfBirth.HasValue && dateOfBirth.Value > DateTime.UtcNow.Date)
            throw new ArgumentException("Date of birth cannot be in the future.", nameof(dateOfBirth));

        DateOfBirth = dateOfBirth;
        PlaceOfBirth = placeOfBirth ?? string.Empty;
        Gender = gender;
        ClanAffiliation = clanAffiliation;
    }

    /// <summary>
    /// The date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; private set; }

    /// <summary>
    /// The place of birth.
    /// </summary>
    public string PlaceOfBirth { get; private set; }

    /// <summary>
    /// The gender.
    /// </summary>
    public string? Gender { get; private set; }

    /// <summary>
    /// The clan affiliation.
    /// </summary>
    public string? ClanAffiliation { get; private set; }

    public bool Equals(PersonalDetails? other)
    {
        if (other is null)
            return false;
        
        if (ReferenceEquals(this, other))
            return true;
        
        return DateOfBirth == other.DateOfBirth &&
               PlaceOfBirth == other.PlaceOfBirth &&
               Gender == other.Gender &&
               ClanAffiliation == other.ClanAffiliation;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as PersonalDetails);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DateOfBirth, PlaceOfBirth, Gender, ClanAffiliation);
    }

    public static bool operator ==(PersonalDetails? left, PersonalDetails? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(PersonalDetails? left, PersonalDetails? right)
    {
        return !Equals(left, right);
    }
}

