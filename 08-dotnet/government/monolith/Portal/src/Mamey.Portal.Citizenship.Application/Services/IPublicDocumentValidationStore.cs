namespace Mamey.Portal.Citizenship.Application.Services;

public interface IPublicDocumentValidationStore
{
    Task<IssuedDocumentSnapshot?> GetLatestIssuedDocumentAsync(string tenantId, string documentNumber, CancellationToken ct = default);
}

public sealed record IssuedDocumentSnapshot(
    string Kind,
    string? DocumentNumber,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt);
