namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record NameDto(
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Nickname);