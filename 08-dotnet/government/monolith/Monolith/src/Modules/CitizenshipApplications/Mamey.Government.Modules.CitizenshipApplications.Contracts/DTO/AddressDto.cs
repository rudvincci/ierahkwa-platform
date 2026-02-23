namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record AddressDto(
    string? FirmName,
    string? Line,
    string? Line2,
    string? Line3,
    string? Urbanization,
    string? City,
    string? State,
    string? Zip5,
    string? Zip4,
    string? PostalCode,
    string? Country,
    string? Province,
    string Type,
    bool IsDefault);