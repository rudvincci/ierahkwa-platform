namespace Mamey.Portal.Cms.Infrastructure.Persistence;

public sealed class CmsPageRow
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    public DateTimeOffset? SubmittedAt { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public DateTimeOffset? RejectedAt { get; set; }

    public string? RejectionReason { get; set; }
}




