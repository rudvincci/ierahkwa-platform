using Mamey.Portal.Cms.Application.Models;

namespace Mamey.Portal.Cms.Application.Services;

public interface ICmsPageService
{
    Task<IReadOnlyList<PageItem>> GetPagesAsync(CancellationToken ct = default);
    Task<PageItem?> GetPublishedPageBySlugAsync(string slug, CancellationToken ct = default);

    Task<PageItem> CreateDraftAsync(string slug, string title, string bodyHtml, CancellationToken ct = default);
    Task<PageItem> UpdateDraftAsync(Guid id, string slug, string title, string bodyHtml, CancellationToken ct = default);

    Task<PageItem> SubmitForReviewAsync(Guid id, CancellationToken ct = default);
    Task<PageItem> ApproveAsync(Guid id, CancellationToken ct = default);
    Task<PageItem> RejectAsync(Guid id, string reason, CancellationToken ct = default);

    Task<PageItem> PublishAsync(Guid id, CancellationToken ct = default);
    Task<PageItem> UnpublishAsync(Guid id, CancellationToken ct = default);
}




