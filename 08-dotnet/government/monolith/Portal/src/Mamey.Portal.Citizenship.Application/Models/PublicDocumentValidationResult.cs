namespace Mamey.Portal.Citizenship.Application.Models;

public sealed record PublicDocumentValidationResult(
    bool Found,
    bool IsValid,
    string? Status,
    string? Kind,
    string? DocumentNumberMasked,
    DateTimeOffset? IssuedAt,
    DateTimeOffset? ExpiresAt);




