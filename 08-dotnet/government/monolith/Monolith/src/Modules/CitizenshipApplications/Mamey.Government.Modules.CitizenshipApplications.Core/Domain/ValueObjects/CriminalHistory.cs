namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

internal record CriminalHistory(
    string Charge,
    DateTime EffectiveDate,
    string CriminalChargeJurisdiction,
    bool Convicted,
    string CriminalRecordDetails);
