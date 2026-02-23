using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Application.DTO;
using Mamey.FWID.Notifications.Application.Queries;
using Mamey.FWID.Notifications.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Application.Queries.Handlers;

/// <summary>
/// Handler for GetNotifications query.
/// </summary>
internal sealed class GetNotificationsHandler : IQueryHandler<GetNotifications, IEnumerable<NotificationDto>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<GetNotificationsHandler> _logger;

    public GetNotificationsHandler(
        INotificationRepository notificationRepository,
        ILogger<GetNotificationsHandler> logger)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<NotificationDto>> HandleAsync(GetNotifications query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetNotifications query: IdentityId={IdentityId}", query.IdentityId);

        try
        {
            var identityId = new IdentityId(query.IdentityId);
            var notifications = await _notificationRepository.GetByIdentityIdAsync(identityId, cancellationToken);

            var dtos = notifications.Select(n => new NotificationDto
            {
                Id = n.Id.Value,
                IdentityId = n.IdentityId.Value,
                Title = n.Title,
                Description = n.Description,
                Message = n.Message,
                Type = n.Type.ToString(),
                Status = n.Status.ToString(),
                CreatedAt = n.CreatedAt,
                SentAt = n.SentAt,
                ReadAt = n.ReadAt,
                IsRead = n.IsRead,
                RelatedEntityType = n.RelatedEntityType,
                RelatedEntityId = n.RelatedEntityId
            });

            _logger.LogInformation("Retrieved {Count} notifications for IdentityId: {IdentityId}",
                dtos.Count(), query.IdentityId);

            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling GetNotifications query: IdentityId={IdentityId}", query.IdentityId);
            throw;
        }
    }
}







