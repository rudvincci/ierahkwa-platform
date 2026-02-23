namespace Mamey.Portal.Cms.Application.Services;

public interface ICmsPageStore
{
    Task<IReadOnlyList<CmsPageSnapshot>> GetPagesAsync(string tenantId, CancellationToken ct = default);
    Task<CmsPageSnapshot?> GetPageAsync(string tenantId, Guid id, CancellationToken ct = default);
    Task<CmsPageSnapshot?> GetPublishedPageBySlugAsync(string tenantId, string slug, CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string tenantId, string slug, Guid? excludeId, CancellationToken ct = default);
    Task<CmsPageSnapshot> CreateDraftAsync(
        string tenantId,
        string? userName,
        string slug,
        string title,
        string bodyHtml,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<CmsPageSnapshot> UpdateDraftAsync(
        string tenantId,
        Guid id,
        string? userName,
        string slug,
        string title,
        string bodyHtml,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<CmsPageSnapshot> SubmitForReviewAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<CmsPageSnapshot> ApproveAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<CmsPageSnapshot> RejectAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        string reason,
        CancellationToken ct = default);
    Task<CmsPageSnapshot> PublishAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<CmsPageSnapshot> UnpublishAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default);
}
