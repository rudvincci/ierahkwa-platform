namespace Mamey.Portal.Citizenship.Application.Services;

public sealed record CitizenPortalProfileSnapshot(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string? Email);

public sealed record CitizenPortalDocumentSnapshot(
    Guid Id,
    string Kind,
    string? DocumentNumber,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt);
