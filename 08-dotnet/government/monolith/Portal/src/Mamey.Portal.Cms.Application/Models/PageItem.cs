namespace Mamey.Portal.Cms.Application.Models;

public sealed record PageItem(
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




