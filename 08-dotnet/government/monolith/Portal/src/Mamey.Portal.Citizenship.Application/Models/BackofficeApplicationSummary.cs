namespace Mamey.Portal.Citizenship.Application.Models;

public sealed record BackofficeApplicationSummary(
    Guid Id,
    string ApplicationNumber,
    string Status,
    string ApplicantName,
    DateOnly DateOfBirth,
    int UploadCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);




