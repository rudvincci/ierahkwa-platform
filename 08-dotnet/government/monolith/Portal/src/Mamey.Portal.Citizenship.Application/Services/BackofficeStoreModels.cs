namespace Mamey.Portal.Citizenship.Application.Services;

public sealed record ApplicationCertificateSnapshot(
    Guid Id,
    string ApplicationNumber,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    DateTimeOffset CreatedAt);

public sealed record ApplicationPassportSnapshot(
    Guid Id,
    string ApplicationNumber,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string? Email);

public sealed record ApplicationIdCardSnapshot(
    Guid Id,
    string ApplicationNumber,
    string FirstName,
    string LastName,
    string? MiddleName,
    DateOnly DateOfBirth,
    string? AddressLine1,
    string? City,
    string? Region,
    string? PostalCode,
    string? Height,
    string? Sex,
    string? EyeColor,
    string? HairColor);

public sealed record ApplicationVehicleTagSnapshot(
    Guid Id,
    string ApplicationNumber,
    string FirstName,
    string LastName);

public sealed record ApplicationTravelIdSnapshot(
    Guid Id,
    string ApplicationNumber,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string? AddressLine1,
    string? City,
    string? Region,
    string? PostalCode);

public sealed record ApplicationDecisionSnapshot(
    Guid Id,
    string Status,
    string? Email);

public sealed record ApplicationRenewalSnapshot(
    Guid Id,
    string? Email);

public sealed record ApplicationReissueSnapshot(
    Guid Id,
    string? Email,
    string Status,
    DateTimeOffset CreatedAt);

public sealed record IssuedDocumentCreate(
    Guid ApplicationId,
    string Kind,
    string? DocumentNumber,
    DateTimeOffset? ExpiresAt,
    string FileName,
    string ContentType,
    long Size,
    string StorageBucket,
    string StorageKey,
    DateTimeOffset CreatedAt);
