namespace Mamey.Portal.Citizenship.Application.Models;

public sealed record CitizenProfileDto(
    string CitizenId,
    string FullName,
    DateOnly DateOfBirth,
    string Email);




