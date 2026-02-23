using Mamey.TravelIdentityStandards.Exceptions;

namespace Mamey.TravelIdentityStandards;

public class PersonData
{
    public const int MinimumAge = 15;
    public const int MaximumAge = 99;
    public PersonData(
        
        string givenNames,
        string surname,
        string gender,
        DateTime dateOfBirth,
        string nationality,
        List<string> biometricData = null)
    {
        Nationality = nationality ?? throw new ArgumentException(nameof(nationality));
        int age = Mamey.DateTimeExtensions.CalculateAge(dateOfBirth);

        if (dateOfBirth.Date > DateTime.UtcNow.Date)
        {
            throw new AgeRangeException($"Date of {dateOfBirth.Date}, does not meet age requirements.");
        }
        if (age < MinimumAge || age > MaximumAge || dateOfBirth.Year > DateTime.Now.Year)
        {
            throw new AgeRangeException(age);
        }
        
        Surname = (string.IsNullOrEmpty(surname?.Trim())) ? throw new ArgumentException("Surname cannot be null or empty.", nameof(surname)) : surname?.Trim();
        GivenNames = (string.IsNullOrEmpty(givenNames?.Trim())) ? throw new ArgumentException("Given Names cannot be null or empty.", nameof(givenNames)) : givenNames?.Trim();;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        BiometricData = biometricData ?? new List<string>();
    }

    /// <summary>Given names (secondary identifiers).</summary>
    public string GivenNames { get; set; }

    /// <summary>Surname (primary identifier).</summary>
    public string Surname { get; set; }

    /// <summary>Date of birth of the document holder.</summary>
    public DateTime DateOfBirth { get; set; }
    
    /// <summary>Gender ('M', 'F', or '<').</summary>
    public string Gender { get; set; }

    /// <summary>3-character ISO code for the holder's nationality.</summary>
    public string Nationality { get; set; }
    
    /// <summary>List of biometric data included in the document.</summary>
    public List<string> BiometricData { get; set; }
}