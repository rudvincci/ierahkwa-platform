namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record ForeignCitizenshipApplicationDto(
    string CountryCode,
    DateTime EffectiveDate,
    string Status,
    string? ForeignCitizenshipApplicationDetails);