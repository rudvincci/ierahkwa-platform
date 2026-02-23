namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record ReferenceDto(
    NameDto Name,
    AddressDto Address,
    string Phone,
    string Relationship);