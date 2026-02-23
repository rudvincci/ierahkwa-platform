using Mamey.Portal.Cms.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Portal.Cms.Domain.Entities;

public sealed class CmsPage : AggregateRoot<Guid>
{
    public string TenantId { get; private set; } = string.Empty;
    public CmsSlug Slug { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string BodyHtml { get; private set; } = string.Empty;
    public CmsContentStatus Status { get; private set; }

    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    public DateTimeOffset? SubmittedAt { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }
    public DateTimeOffset? RejectedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private CmsPage() { }

    public CmsPage(
        Guid id,
        string tenantId,
        CmsSlug slug,
        string title,
        string bodyHtml,
        CmsContentStatus status,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
        : base(id)
    {
        TenantId = tenantId;
        Slug = slug;
        Title = title;
        BodyHtml = bodyHtml;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static CmsPage Rehydrate(
        Guid id,
        string tenantId,
        CmsSlug slug,
        string title,
        string bodyHtml,
        CmsContentStatus status,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt,
        string? createdBy,
        string? updatedBy,
        DateTimeOffset? submittedAt,
        DateTimeOffset? approvedAt,
        DateTimeOffset? publishedAt,
        DateTimeOffset? rejectedAt,
        string? rejectionReason)
    {
        var page = new CmsPage(
            id,
            tenantId,
            slug,
            title,
            bodyHtml,
            status,
            createdAt,
            updatedAt)
        {
            CreatedBy = createdBy,
            UpdatedBy = updatedBy,
            SubmittedAt = submittedAt,
            ApprovedAt = approvedAt,
            PublishedAt = publishedAt,
            RejectedAt = rejectedAt,
            RejectionReason = rejectionReason
        };

        return page;
    }
}
