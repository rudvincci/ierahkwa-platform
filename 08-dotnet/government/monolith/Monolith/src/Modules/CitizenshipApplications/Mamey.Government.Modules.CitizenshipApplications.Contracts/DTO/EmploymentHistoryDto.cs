namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record EmploymentHistoryDto(
    string Employer,
    string Occupation,
    DateTime EmployedFrom,
    DateTime? EmployedTo,
    AddressDto? EmployerAddress,
    string? EmployerPhone);