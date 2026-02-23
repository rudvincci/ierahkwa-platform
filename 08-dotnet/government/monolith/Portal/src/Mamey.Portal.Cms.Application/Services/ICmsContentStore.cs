using Mamey.Portal.Cms.Application.Models;

namespace Mamey.Portal.Cms.Application.Services;

public interface ICmsContentStore
{
    Task<IReadOnlyList<CmsNewsItemSnapshot>> GetNewsAsync(string tenantId, bool includeAll, CancellationToken ct = default);
    Task<IReadOnlyList<CmsNewsItemSnapshot>> GetPublishedNewsAsync(string tenantId, int take, CancellationToken ct = default);
    Task<CmsNewsItemSnapshot?> GetNewsItemAsync(string tenantId, Guid id, CancellationToken ct = default);
    Task<CmsNewsItemSnapshot?> GetPublishedNewsItemAsync(string tenantId, Guid id, CancellationToken ct = default);
    Task<CmsNewsItemSnapshot> CreateDraftAsync(
        string tenantId,
        string? userName,
        string title,
        string summary,
        string bodyHtml,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<CmsNewsItemSnapshot> UpdateDraftAsync(
        string tenantId,
        Guid id,
        string? userName,
        string title,
        string summary,
        string bodyHtml,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<CmsNewsItemSnapshot> SubmitForReviewAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<CmsNewsItemSnapshot> ApproveAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<CmsNewsItemSnapshot> RejectAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        string reason,
        CancellationToken ct = default);
    Task<CmsNewsItemSnapshot> PublishAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<CmsNewsItemSnapshot> UnpublishAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default);
}
