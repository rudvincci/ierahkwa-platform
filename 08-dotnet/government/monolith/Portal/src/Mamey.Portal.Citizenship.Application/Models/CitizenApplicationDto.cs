namespace Mamey.Portal.Citizenship.Application.Models;

public sealed record CitizenApplicationDto(
    string ApplicationNumber,
    string Status,
    DateTimeOffset CreatedAt);




