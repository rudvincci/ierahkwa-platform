namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record ResidencyDto(
    string CountryCode,
    DateTime EffectiveDate,
    string Status);