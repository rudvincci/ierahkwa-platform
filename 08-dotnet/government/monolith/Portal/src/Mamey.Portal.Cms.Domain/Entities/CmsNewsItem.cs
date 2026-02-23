using Mamey.Portal.Cms.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Portal.Cms.Domain.Entities;

public sealed class CmsNewsItem : AggregateRoot<Guid>
{
    public string TenantId { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Summary { get; private set; } = string.Empty;
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

    private CmsNewsItem() { }

    public CmsNewsItem(
        Guid id,
        string tenantId,
        string title,
        string summary,
        string bodyHtml,
        CmsContentStatus status,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
        : base(id)
    {
        TenantId = tenantId;
        Title = title;
        Summary = summary;
        BodyHtml = bodyHtml;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static CmsNewsItem Rehydrate(
        Guid id,
        string tenantId,
        string title,
        string summary,
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
        var item = new CmsNewsItem(
            id,
            tenantId,
            title,
            summary,
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

        return item;
    }
}
