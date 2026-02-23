using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class PublicDocumentValidationService : IPublicDocumentValidationService
{
    private readonly IPublicDocumentValidationStore _store;

    public PublicDocumentValidationService(IPublicDocumentValidationStore store)
    {
        _store = store;
    }

    public async Task<PublicDocumentValidationResult> ValidateAsync(string tenantId, string documentNumber, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new ArgumentException("tenantId is required.", nameof(tenantId));
        }

        if (string.IsNullOrWhiteSpace(documentNumber))
        {
            throw new ArgumentException("documentNumber is required.", nameof(documentNumber));
        }

        tenantId = tenantId.Trim();
        documentNumber = documentNumber.Trim().ToUpperInvariant();

        var now = DateTimeOffset.UtcNow;
        var doc = await _store.GetLatestIssuedDocumentAsync(tenantId, documentNumber, ct);

        if (doc is null)
        {
            return new PublicDocumentValidationResult(
                Found: false,
                IsValid: false,
                Status: "NotFound",
                Kind: null,
                DocumentNumberMasked: MaskDocumentNumber(documentNumber),
                IssuedAt: null,
                ExpiresAt: null);
        }

        var expired = doc.ExpiresAt is not null && doc.ExpiresAt.Value < now;
        var valid = !expired;
        var status = valid ? "Valid" : "Expired";

        return new PublicDocumentValidationResult(
            Found: true,
            IsValid: valid,
            Status: status,
            Kind: doc.Kind,
            DocumentNumberMasked: MaskDocumentNumber(doc.DocumentNumber ?? documentNumber),
            IssuedAt: doc.CreatedAt,
            ExpiresAt: doc.ExpiresAt);
    }

    private static string MaskDocumentNumber(string? value)
    {
        value = (value ?? string.Empty).Trim();
        if (value.Length <= 8)
        {
            return value;
        }

        return $"{value[..4]}...{value[^4..]}";
    }
}
