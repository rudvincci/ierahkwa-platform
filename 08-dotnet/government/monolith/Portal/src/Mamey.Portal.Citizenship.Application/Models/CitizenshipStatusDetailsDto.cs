using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Models;

public record CitizenshipStatusDetailsDto(
    CitizenshipStatus Status,
    int YearsCompleted,
    DateTimeOffset StatusGrantedAt,
    DateTimeOffset? StatusExpiresAt
);


