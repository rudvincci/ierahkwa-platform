namespace Mamey.Portal.Citizenship.Application.Models;

public record StatusProgressionApplicationDto(
    Guid Id,
    string ApplicationNumber,
    string TargetStatus,
    string Status,
    int YearsCompletedAtApplication,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);


