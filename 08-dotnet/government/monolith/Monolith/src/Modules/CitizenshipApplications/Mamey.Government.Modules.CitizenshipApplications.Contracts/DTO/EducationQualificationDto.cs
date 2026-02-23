namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record EducationQualificationDto(
    string Name,
    string AwardedBy,
    DateTime EffectiveDate);