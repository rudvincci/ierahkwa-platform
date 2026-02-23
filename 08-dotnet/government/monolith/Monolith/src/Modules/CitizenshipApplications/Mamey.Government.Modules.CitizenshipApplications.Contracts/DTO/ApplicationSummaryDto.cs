namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record ApplicationSummaryDto(
    Guid Id,
    string ApplicationNumber,
    string ApplicantName,
    string Status,
    string Step,
    DateTime CreatedAt,
    DateTime? SubmittedAt,
    DateTime? UpdatedAt,
    string? Email);