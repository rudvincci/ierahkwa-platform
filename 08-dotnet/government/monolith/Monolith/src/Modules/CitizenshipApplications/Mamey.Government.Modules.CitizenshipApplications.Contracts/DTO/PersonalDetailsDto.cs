namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record PersonalDetailsDto(
    DateTime DateOfBirth,
    string PlaceOfBirth,
    string EyeColor,
    string HairColor,
    double HeightInInches,
    double WeightInPounds,
    string Gender,
    string MaritalStatus,
    NameDto? SpouseName,
    IReadOnlyList<string> CurrentNationalities,
    IReadOnlyList<string> Aliases);