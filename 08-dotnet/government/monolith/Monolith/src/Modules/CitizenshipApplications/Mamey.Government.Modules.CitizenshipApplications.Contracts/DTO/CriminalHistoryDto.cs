namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record CriminalHistoryDto(
    string Charge,
    DateTime EffectiveDate,
    string CriminalChargeJurisdiction,
    bool Convicted,
    string CriminalRecordDetails);