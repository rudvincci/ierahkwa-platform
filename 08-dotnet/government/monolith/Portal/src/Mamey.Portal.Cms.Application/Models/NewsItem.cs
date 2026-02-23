namespace Mamey.Portal.Cms.Application.Models;

public static class CmsContentStatus
{
    public const string Draft = "Draft";
    public const string InReview = "InReview";
    public const string Approved = "Approved";
    public const string Published = "Published";
    public const string Rejected = "Rejected";
}

public sealed record NewsItem(
    Guid Id,
    string TenantId,
    string Title,
    string Summary,
    string BodyHtml,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    DateTimeOffset? SubmittedAt,
    DateTimeOffset? ApprovedAt,
    DateTimeOffset? PublishedAt,
    DateTimeOffset? RejectedAt,
    string? RejectionReason);


