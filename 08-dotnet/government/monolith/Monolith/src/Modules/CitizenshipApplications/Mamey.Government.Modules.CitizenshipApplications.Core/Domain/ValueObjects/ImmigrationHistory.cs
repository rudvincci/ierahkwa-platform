namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

internal record ImmigrationHistory(
    bool PreviouslyDeportedFromCountry,
    string? PreviouslyDeportedFromCountryCode,
    DateTime? PreviouslyDeportedFromCountryAt,
    string? PreviouslyDeportedFromCountryDetails);
