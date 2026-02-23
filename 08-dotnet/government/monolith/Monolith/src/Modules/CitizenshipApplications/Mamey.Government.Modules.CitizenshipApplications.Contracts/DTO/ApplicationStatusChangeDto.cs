namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record ApplicationStatusChangeDto(
    string FromStatus,
    string ToStatus,
    DateTime ChangedAt,
    string? ChangedBy,
    string? Notes);