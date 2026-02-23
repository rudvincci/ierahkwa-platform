using Mamey.Portal.Cms.Application.Models;
using Mamey.Portal.Shared.Auth;
using Mamey.Portal.Shared.Tenancy;

namespace Mamey.Portal.Cms.Application.Services;

public sealed class CmsPageService : ICmsPageService
{
    private readonly ICmsPageStore _store;
    private readonly ITenantContext _tenant;
    private readonly ICurrentUserContext _user;
    private readonly ICmsHtmlSanitizer _html;

    public CmsPageService(ICmsPageStore store, ITenantContext tenant, ICurrentUserContext user, ICmsHtmlSanitizer html)
    {
        _store = store;
        _tenant = tenant;
        _user = user;
        _html = html;
    }

    public async Task<IReadOnlyList<PageItem>> GetPagesAsync(CancellationToken ct = default)
    {
        EnsureCanEdit();
        var tenantId = _tenant.TenantId;

        var rows = await _store.GetPagesAsync(tenantId, ct);
        return rows.Select(x => Map(x, sanitizeBody: false)).ToList();
    }

    public async Task<PageItem?> GetPublishedPageBySlugAsync(string slug, CancellationToken ct = default)
    {
        slug = NormalizeSlug(slug);
        if (string.IsNullOrWhiteSpace(slug)) return null;

        var tenantId = _tenant.TenantId;
        var row = await _store.GetPublishedPageBySlugAsync(tenantId, slug, ct);
        return row is null ? null : Map(row, sanitizeBody: true);
    }

    public async Task<PageItem> CreateDraftAsync(string slug, string title, string bodyHtml, CancellationToken ct = default)
    {
        EnsureCanEdit();
        slug = NormalizeSlug(slug);
        title = (title ?? string.Empty).Trim();
        bodyHtml ??= string.Empty;

        if (string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Slug and title are required.");
        }

        var tenantId = _tenant.TenantId;
        var exists = await _store.SlugExistsAsync(tenantId, slug, excludeId: null, ct);
        if (exists)
        {
            throw new InvalidOperationException($"Page slug '{slug}' already exists.");
        }

        var row = await _store.CreateDraftAsync(
            tenantId,
            _user.UserName,
            slug,
            title,
            string.IsNullOrWhiteSpace(bodyHtml) ? "<p></p>" : bodyHtml.Trim(),
            DateTimeOffset.UtcNow,
            ct);

        return Map(row, sanitizeBody: false);
    }

    public async Task<PageItem> UpdateDraftAsync(Guid id, string slug, string title, string bodyHtml, CancellationToken ct = default)
    {
        EnsureCanEdit();
        slug = NormalizeSlug(slug);
        title = (title ?? string.Empty).Trim();
        bodyHtml ??= string.Empty;

        if (string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Slug and title are required.");
        }

        var tenantId = _tenant.TenantId;
        var existing = await _store.GetPageAsync(tenantId, id, ct)
                      ?? throw new InvalidOperationException("Page not found.");

        if (existing.Status != CmsContentStatus.Draft && existing.Status != CmsContentStatus.Rejected && existing.Status != CmsContentStatus.Approved)
        {
            throw new InvalidOperationException("Only Draft/Rejected/Approved pages can be edited.");
        }

        if (!string.Equals(existing.Slug, slug, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _store.SlugExistsAsync(tenantId, slug, excludeId: id, ct);
            if (exists)
            {
                throw new InvalidOperationException($"Page slug '{slug}' already exists.");
            }
        }

        var row = await _store.UpdateDraftAsync(
            tenantId,
            id,
            _user.UserName,
            slug,
            title,
            string.IsNullOrWhiteSpace(bodyHtml) ? "<p></p>" : bodyHtml.Trim(),
            DateTimeOffset.UtcNow,
            ct);

        return Map(row, sanitizeBody: false);
    }

    public async Task<PageItem> SubmitForReviewAsync(Guid id, CancellationToken ct = default)
    {
        EnsureCanEdit();
        var tenantId = _tenant.TenantId;
        var existing = await _store.GetPageAsync(tenantId, id, ct)
                      ?? throw new InvalidOperationException("Page not found.");

        if (existing.Status != CmsContentStatus.Draft && existing.Status != CmsContentStatus.Rejected)
        {
            throw new InvalidOperationException("Only Draft/Rejected pages can be submitted for review.");
        }

        var row = await _store.SubmitForReviewAsync(tenantId, id, _user.UserName, DateTimeOffset.UtcNow, ct);
        return Map(row, sanitizeBody: false);
    }

    public async Task<PageItem> ApproveAsync(Guid id, CancellationToken ct = default)
    {
        EnsureIsAdmin();
        var tenantId = _tenant.TenantId;
        var existing = await _store.GetPageAsync(tenantId, id, ct)
                      ?? throw new InvalidOperationException("Page not found.");

        if (existing.Status != CmsContentStatus.InReview)
        {
            throw new InvalidOperationException("Only InReview pages can be approved.");
        }

        var row = await _store.ApproveAsync(tenantId, id, _user.UserName, DateTimeOffset.UtcNow, ct);
        return Map(row, sanitizeBody: false);
    }

    public async Task<PageItem> RejectAsync(Guid id, string reason, CancellationToken ct = default)
    {
        EnsureIsAdmin();
        var tenantId = _tenant.TenantId;
        var existing = await _store.GetPageAsync(tenantId, id, ct)
                      ?? throw new InvalidOperationException("Page not found.");

        if (existing.Status != CmsContentStatus.InReview)
        {
            throw new InvalidOperationException("Only InReview pages can be rejected.");
        }

        var row = await _store.RejectAsync(
            tenantId,
            id,
            _user.UserName,
            DateTimeOffset.UtcNow,
            string.IsNullOrWhiteSpace(reason) ? "Rejected" : reason.Trim(),
            ct);
        return Map(row, sanitizeBody: false);
    }

    public async Task<PageItem> PublishAsync(Guid id, CancellationToken ct = default)
    {
        EnsureIsAdmin();
        var tenantId = _tenant.TenantId;
        var existing = await _store.GetPageAsync(tenantId, id, ct)
                      ?? throw new InvalidOperationException("Page not found.");

        if (existing.Status != CmsContentStatus.Approved)
        {
            throw new InvalidOperationException("Only Approved pages can be published.");
        }

        var row = await _store.PublishAsync(tenantId, id, _user.UserName, DateTimeOffset.UtcNow, ct);
        return Map(row, sanitizeBody: false);
    }

    public async Task<PageItem> UnpublishAsync(Guid id, CancellationToken ct = default)
    {
        EnsureIsAdmin();
        var tenantId = _tenant.TenantId;
        var existing = await _store.GetPageAsync(tenantId, id, ct)
                      ?? throw new InvalidOperationException("Page not found.");

        if (existing.Status != CmsContentStatus.Published)
        {
            throw new InvalidOperationException("Only Published pages can be unpublished.");
        }

        var row = await _store.UnpublishAsync(tenantId, id, _user.UserName, DateTimeOffset.UtcNow, ct);
        return Map(row, sanitizeBody: false);
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

    private static string NormalizeSlug(string slug)
        => (slug ?? string.Empty).Trim().Trim('/').ToLowerInvariant();

    private PageItem Map(CmsPageSnapshot x, bool sanitizeBody) => new(
        x.Id,
        x.TenantId,
        x.Slug,
        x.Title,
        sanitizeBody ? _html.SanitizePageBody(x.BodyHtml) : x.BodyHtml,
        x.Status,
        x.CreatedAt,
        x.UpdatedAt,
        x.CreatedBy,
        x.UpdatedBy,
        x.SubmittedAt,
        x.ApprovedAt,
        x.PublishedAt,
        x.RejectedAt,
        x.RejectionReason);
}
