namespace Mamey.Portal.Citizenship.Application.Models;

public sealed record BackofficeApplicationDetails(
    Guid Id,
    string ApplicationNumber,
    string Status,
    string TenantId,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string? Email,
    string? AddressLine1,
    string? City,
    string? Region,
    string? PostalCode,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<BackofficeUploadSummary> Uploads);




