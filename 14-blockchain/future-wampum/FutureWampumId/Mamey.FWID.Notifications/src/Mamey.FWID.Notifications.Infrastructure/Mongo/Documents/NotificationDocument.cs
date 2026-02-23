using Mamey.FWID.Notifications.Domain.Entities;
using Mamey.Microservice.Infrastructure.Mongo;

namespace Mamey.FWID.Notifications.Infrastructure.Mongo.Documents;

/// <summary>
/// MongoDB document for Notification read model.
/// </summary>
internal class NotificationDocument : IDocument
{
    public string Id { get; set; } = string.Empty;
    public string IdentityId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsRead { get; set; }
    public string? RelatedEntityType { get; set; }
    public string? RelatedEntityId { get; set; }

    public Notification ToEntity()
        => new(
            new NotificationId(Guid.Parse(Id), new Mamey.FWID.Identities.Domain.Entities.IdentityId(Guid.Parse(IdentityId))),
            new Mamey.FWID.Identities.Domain.Entities.IdentityId(Guid.Parse(IdentityId)),
            Title,
            Description,
            Message,
            Enum.Parse<NotificationType>(Type),
            Enum.Parse<NotificationStatus>(Status),
            CreatedAt,
            SentAt,
            ReadAt,
            IsRead,
            RelatedEntityType,
            !string.IsNullOrEmpty(RelatedEntityId) ? Guid.Parse(RelatedEntityId) : null);
}







