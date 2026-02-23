namespace Mamey.Portal.Cms.Application.Services;

public sealed record CmsNewsItemSnapshot(
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

public sealed record CmsPageSnapshot(
    Guid Id,
    string TenantId,
    string Slug,
    string Title,
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
