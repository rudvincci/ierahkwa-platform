using Mamey.Portal.Cms.Application.Models;

namespace Mamey.Portal.Cms.Application.Services;

public interface ICmsContentService
{
    Task<IReadOnlyList<NewsItem>> GetNewsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<NewsItem>> GetPublishedNewsAsync(int take = 10, CancellationToken ct = default);
    Task<NewsItem?> GetPublishedNewsItemAsync(Guid id, CancellationToken ct = default);

    Task<NewsItem> CreateDraftAsync(string title, string summary, string bodyHtml, CancellationToken ct = default);
    Task<NewsItem> UpdateDraftAsync(Guid id, string title, string summary, string bodyHtml, CancellationToken ct = default);

    Task<NewsItem> SubmitForReviewAsync(Guid id, CancellationToken ct = default);
    Task<NewsItem> ApproveAsync(Guid id, CancellationToken ct = default);
    Task<NewsItem> RejectAsync(Guid id, string reason, CancellationToken ct = default);

    Task<NewsItem> PublishAsync(Guid id, CancellationToken ct = default);
    Task<NewsItem> UnpublishAsync(Guid id, CancellationToken ct = default);
}


