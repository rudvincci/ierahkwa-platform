using Mamey.Portal.Cms.Application.Models;
using Mamey.Portal.Shared.Auth;
using Mamey.Portal.Shared.Tenancy;

namespace Mamey.Portal.Cms.Application.Services;

public sealed class InMemoryCmsContentService : ICmsContentService
{
    private static readonly Dictionary<string, List<NewsItem>> NewsByTenant = new(StringComparer.OrdinalIgnoreCase);
    private readonly ITenantContext _tenant;
    private readonly ICurrentUserContext _user;

    public InMemoryCmsContentService(ITenantContext tenant, ICurrentUserContext user)
    {
        _tenant = tenant;
        _user = user;

        // Seed once per tenant
        if (!NewsByTenant.ContainsKey(_tenant.TenantId))
        {
            NewsByTenant[_tenant.TenantId] =
            [
                new(
                    Id: Guid.NewGuid(),
                    TenantId: _tenant.TenantId,
                    Title: $"Welcome to the CMS (mock) — tenant '{_tenant.TenantId}'",
                    Summary: "This is an in-memory item. Later we’ll persist per-tenant content and add approval workflows.",
                    BodyHtml: "<p><strong>Welcome!</strong> This is a <em>WYSIWYG</em> HTML body.</p>",
                    CreatedAt: DateTimeOffset.UtcNow.AddDays(-3),
                    UpdatedAt: DateTimeOffset.UtcNow.AddDays(-3),
                    Status: CmsContentStatus.Published,
                    CreatedBy: "seed",
                    UpdatedBy: "seed",
                    SubmittedAt: DateTimeOffset.UtcNow.AddDays(-3),
                    ApprovedAt: DateTimeOffset.UtcNow.AddDays(-3),
                    PublishedAt: DateTimeOffset.UtcNow.AddDays(-3),
                    RejectedAt: null,
                    RejectionReason: null),
                new(
                    Id: Guid.NewGuid(),
                    TenantId: _tenant.TenantId,
                    Title: "Draft: Upcoming announcement",
                    Summary: "Draft item to demonstrate publish toggling.",
                    BodyHtml: "<p>Draft body content (HTML).</p>",
                    CreatedAt: DateTimeOffset.UtcNow.AddDays(-1),
                    UpdatedAt: DateTimeOffset.UtcNow.AddDays(-1),
                    Status: CmsContentStatus.Draft,
                    CreatedBy: "seed",
                    UpdatedBy: "seed",
                    SubmittedAt: null,
                    ApprovedAt: null,
                    PublishedAt: null,
                    RejectedAt: null,
                    RejectionReason: null),
            ];
        }
    }

    private List<NewsItem> News => NewsByTenant[_tenant.TenantId];

    public Task<IReadOnlyList<NewsItem>> GetNewsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NewsItem>>(News.OrderByDescending(x => x.CreatedAt).ToList());

    public Task<IReadOnlyList<NewsItem>> GetPublishedNewsAsync(int take = 10, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NewsItem>>(
            News.Where(x => x.Status == CmsContentStatus.Published)
                .OrderByDescending(x => x.PublishedAt ?? x.UpdatedAt)
                .Take(Math.Clamp(take, 1, 50))
                .ToList());

    public Task<NewsItem?> GetPublishedNewsItemAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(News.FirstOrDefault(x => x.Id == id && x.Status == CmsContentStatus.Published));

    public Task<NewsItem> CreateDraftAsync(string title, string summary, string bodyHtml, CancellationToken ct = default)
    {
        EnsureCanEdit();

        var item = new NewsItem(
            Id: Guid.NewGuid(),
            TenantId: _tenant.TenantId,
            Title: title.Trim(),
            Summary: summary.Trim(),
            BodyHtml: string.IsNullOrWhiteSpace(bodyHtml) ? "<p></p>" : bodyHtml.Trim(),
            CreatedAt: DateTimeOffset.UtcNow,
            UpdatedAt: DateTimeOffset.UtcNow,
            Status: CmsContentStatus.Draft,
            CreatedBy: _user.UserName,
            UpdatedBy: _user.UserName,
            SubmittedAt: null,
            ApprovedAt: null,
            PublishedAt: null,
            RejectedAt: null,
            RejectionReason: null);

        News.Insert(0, item);
        return Task.FromResult(item);
    }

    public Task<NewsItem> UpdateDraftAsync(Guid id, string title, string summary, string bodyHtml, CancellationToken ct = default)
    {
        EnsureCanEdit();
        var idx = News.FindIndex(x => x.Id == id);
        if (idx < 0) throw new InvalidOperationException("News item not found.");
        var current = News[idx];
        if (current.Status != CmsContentStatus.Draft && current.Status != CmsContentStatus.Rejected)
        {
            throw new InvalidOperationException("Only Draft/Rejected items can be edited.");
        }
        var next = current with
        {
            Title = title.Trim(),
            Summary = summary.Trim(),
            BodyHtml = string.IsNullOrWhiteSpace(bodyHtml) ? "<p></p>" : bodyHtml.Trim(),
            UpdatedAt = DateTimeOffset.UtcNow,
            UpdatedBy = _user.UserName,
            Status = CmsContentStatus.Draft,
            RejectedAt = null,
            RejectionReason = null,
        };
        News[idx] = next;
        return Task.FromResult(next);
    }

    public Task<NewsItem> SubmitForReviewAsync(Guid id, CancellationToken ct = default)
    {
        EnsureCanEdit();
        var idx = News.FindIndex(x => x.Id == id);
        if (idx < 0)
        {
            throw new InvalidOperationException("News item not found.");
        }

        var current = News[idx];
        if (current.Status != CmsContentStatus.Draft && current.Status != CmsContentStatus.Rejected)
        {
            throw new InvalidOperationException("Only Draft/Rejected items can be submitted for review.");
        }

        var next = current with
        {
            Status = CmsContentStatus.InReview,
            SubmittedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            UpdatedBy = _user.UserName,
        };
        News[idx] = next;
        return Task.FromResult(next);
    }

    public Task<NewsItem> ApproveAsync(Guid id, CancellationToken ct = default)
    {
        EnsureIsAdmin();
        var idx = News.FindIndex(x => x.Id == id);
        if (idx < 0) throw new InvalidOperationException("News item not found.");
        var current = News[idx];
        if (current.Status != CmsContentStatus.InReview) throw new InvalidOperationException("Only InReview items can be approved.");
        var next = current with
        {
            Status = CmsContentStatus.Approved,
            ApprovedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            UpdatedBy = _user.UserName,
        };
        News[idx] = next;
        return Task.FromResult(next);
    }

    public Task<NewsItem> RejectAsync(Guid id, string reason, CancellationToken ct = default)
    {
        EnsureIsAdmin();
        var idx = News.FindIndex(x => x.Id == id);
        if (idx < 0) throw new InvalidOperationException("News item not found.");
        var current = News[idx];
        if (current.Status != CmsContentStatus.InReview) throw new InvalidOperationException("Only InReview items can be rejected.");
        var next = current with
        {
            Status = CmsContentStatus.Rejected,
            RejectedAt = DateTimeOffset.UtcNow,
            RejectionReason = string.IsNullOrWhiteSpace(reason) ? "Rejected" : reason.Trim(),
            UpdatedAt = DateTimeOffset.UtcNow,
            UpdatedBy = _user.UserName,
        };
        News[idx] = next;
        return Task.FromResult(next);
    }

    public Task<NewsItem> PublishAsync(Guid id, CancellationToken ct = default)
    {
        EnsureIsAdmin();
        var idx = News.FindIndex(x => x.Id == id);
        if (idx < 0) throw new InvalidOperationException("News item not found.");
        var current = News[idx];
        if (current.Status != CmsContentStatus.Approved) throw new InvalidOperationException("Only Approved items can be published.");
        var next = current with
        {
            Status = CmsContentStatus.Published,
            PublishedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            UpdatedBy = _user.UserName,
        };
        News[idx] = next;
        return Task.FromResult(next);
    }

    public Task<NewsItem> UnpublishAsync(Guid id, CancellationToken ct = default)
    {
        EnsureIsAdmin();
        var idx = News.FindIndex(x => x.Id == id);
        if (idx < 0) throw new InvalidOperationException("News item not found.");
        var current = News[idx];
        if (current.Status != CmsContentStatus.Published) throw new InvalidOperationException("Only Published items can be unpublished.");
        var next = current with
        {
            Status = CmsContentStatus.Approved,
            PublishedAt = null,
            UpdatedAt = DateTimeOffset.UtcNow,
            UpdatedBy = _user.UserName,
        };
        News[idx] = next;
        return Task.FromResult(next);
    }

    private void EnsureCanEdit()
    {
        if (!_user.IsAuthenticated) throw new InvalidOperationException("Not authenticated.");
        if (!_user.IsInRole("ContentEditor") && !_user.IsInRole("Admin"))
        {
            throw new InvalidOperationException("Not authorized.");
        }
    }

    private void EnsureIsAdmin()
    {
        if (!_user.IsAuthenticated) throw new InvalidOperationException("Not authenticated.");
        if (!_user.IsInRole("Admin"))
        {
            throw new InvalidOperationException("Admin role required.");
        }
    }
}


