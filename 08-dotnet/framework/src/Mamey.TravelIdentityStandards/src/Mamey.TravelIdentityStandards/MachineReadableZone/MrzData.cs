namespace Mamey.TravelIdentityStandards.MachineReadableZone;

public class MrzData : IMrzData
{
    public MrzData(string documentNumber, string issuingCountry, string nationality, DateTime dateOfBirth,
        DateTime expiryDate, string surname, string givenNames, string gender, string optionalData)
    {
        DocumentNumber = documentNumber ?? throw new ArgumentNullException(nameof(documentNumber));
        IssuingCountry = issuingCountry ?? throw new ArgumentNullException(nameof(issuingCountry));
        Nationality = nationality ?? throw new ArgumentNullException(nameof(nationality));
        DateOfBirth = dateOfBirth;
        ExpiryDate = expiryDate;
        Surname = surname ?? throw new ArgumentNullException(nameof(surname));
        GivenNames = givenNames ?? throw new ArgumentNullException(nameof(givenNames));
        if (gender != "M" && gender != "F" && gender != "<")
        {
            throw new ArgumentException(nameof(surname), "Must be 'M', 'F', or '<'.");
        }
        Gender = gender;
        OptionalData = optionalData;
    }

    public string DocumentNumber { get; set; }
    public string IssuingCountry { get; set; } // ISO 3166-1 Alpha-3
    public string Nationality { get; set; } // ISO 3166-1 Alpha-3
    public DateTime DateOfBirth { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Surname { get; set; } 
    public string GivenNames { get; set; } 
    public string Gender { get; set; }  // M/F/X
    public string OptionalData { get; set; } 
}