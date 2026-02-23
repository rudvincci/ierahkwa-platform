namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record ContactInformationDto(
    string? Email,
    AddressDto? ResidentialAddress,
    AddressDto? PostalAddress,
    IReadOnlyList<string> PhoneNumbers,
    string EmergencyContactName,
    string EmergencyContactPhoneNumber,
    string EmergencyContactEmail,
    string EmergencyContactAddress,
    string EmergencyContactRelationship);