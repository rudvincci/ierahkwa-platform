namespace Mamey.TravelIdentityStandards.MachineReadableZone;

public interface IMrzData
{
    string DocumentNumber { get; set; }
    string IssuingCountry { get; set; }  // ISO 3166-1 Alpha-3
    string Nationality { get; set; } // ISO 3166-1 Alpha-3
    DateTime DateOfBirth { get; set; }
    DateTime ExpiryDate { get; set; }
    string Surname { get; set; }
    string GivenNames { get; set; }
    string Gender { get; set; } // M/F/X
    string OptionalData { get; set; }
}