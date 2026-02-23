namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

internal record ForeignCitizenshipApplication(
    string CountryCode,
    DateTime EffectiveDate,
    string Status,
    string? ForeignCitizenshipApplicationDetails);
