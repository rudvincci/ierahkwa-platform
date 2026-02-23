namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

internal record Residency(string CountryCode, DateTime EffectiveDate, string Status);
