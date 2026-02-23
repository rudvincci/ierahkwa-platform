namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record ImmigrationHistoryDto(
    bool PreviouslyDeportedFromCountry,
    string? PreviouslyDeportedFromCountryCode,
    DateTime? PreviouslyDeportedFromCountryAt,
    string? PreviouslyDeportedFromCountryDetails);