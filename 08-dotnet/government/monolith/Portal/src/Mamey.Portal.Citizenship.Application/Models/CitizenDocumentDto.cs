namespace Mamey.Portal.Citizenship.Application.Models;

public sealed record CitizenDocumentDto(
    Guid IssuedDocumentId,
    string DocumentNumber,
    string Type,
    string Variant,
    DateTimeOffset IssuedAt,
    DateTimeOffset? ExpiresAt);


