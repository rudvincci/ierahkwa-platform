namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record DependentDto(
    NameDto Name,
    DateTime DateOfBirth,
    string Gender,
    int Age);