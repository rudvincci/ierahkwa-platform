using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Models;

public record StatusProgressionEligibilityDto(
    bool IsEligible,
    CitizenshipStatus CurrentStatus,
    CitizenshipStatus TargetStatus,
    int YearsCompleted,
    int YearsRequired,
    DateTimeOffset? StatusExpiresAt,
    string? ReasonNotEligible
);


