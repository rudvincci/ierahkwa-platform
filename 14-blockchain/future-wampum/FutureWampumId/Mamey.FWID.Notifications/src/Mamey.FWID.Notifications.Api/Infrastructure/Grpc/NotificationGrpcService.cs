using Grpc.Core;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Api.Protos;
using Mamey.FWID.Notifications.Application.Commands;
using Mamey.FWID.Notifications.Application.DTO;
using Mamey.FWID.Notifications.Application.Queries;
using Mamey.FWID.Notifications.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Repositories;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Api.Infrastructure.Grpc;

/// <summary>
/// gRPC service implementation for NotificationService.
/// </summary>
public class NotificationGrpcService : NotificationService.NotificationServiceBase
{
    private readonly INotificationRepository _repository;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ILogger<NotificationGrpcService> _logger;

    public NotificationGrpcService(
        INotificationRepository repository,
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILogger<NotificationGrpcService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
        _queryDispatcher = queryDispatcher ?? throw new ArgumentNullException(nameof(queryDispatcher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<GetNotificationsResponse> GetNotifications(
        GetNotificationsRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Received gRPC GetNotifications request for IdentityId: {IdentityId}", request.IdentityId);

        try
        {
            var identityId = new IdentityId(Guid.Parse(request.IdentityId));
            var query = new GetNotifications(identityId.Value);
            var notifications = await _queryDispatcher.QueryAsync<GetNotifications, IEnumerable<NotificationDto>>(query, context.CancellationToken);

            var response = new GetNotificationsResponse();
            response.Notifications.AddRange(notifications.Select(n => new NotificationMessage
            {
                Id = n.Id.ToString(),
                IdentityId = n.IdentityId.ToString(),
                Title = n.Title,
                Description = n.Description,
                Message = n.Message,
                Type = n.Type,
                Status = n.Status,
                CreatedAt = n.CreatedAt.ToUnixTimeSeconds(),
                SentAt = n.SentAt?.ToUnixTimeSeconds() ?? 0,
                ReadAt = n.ReadAt?.ToUnixTimeSeconds() ?? 0,
                IsRead = n.IsRead,
                RelatedEntityType = n.RelatedEntityType ?? string.Empty,
                RelatedEntityId = n.RelatedEntityId?.ToString() ?? string.Empty
            }));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications for IdentityId: {IdentityId}", request.IdentityId);
            throw new RpcException(new Status(StatusCode.Internal, $"Error getting notifications: {ex.Message}"));
        }
    }

    public override async Task<MarkAsReadResponse> MarkAsRead(
        MarkAsReadRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Received gRPC MarkAsRead request for NotificationId: {NotificationId}, IdentityId: {IdentityId}",
            request.NotificationId, request.IdentityId);

        try
        {
            var command = new MarkAsRead(
                Guid.Parse(request.NotificationId),
                Guid.Parse(request.IdentityId));

            await _commandDispatcher.SendAsync(command, context.CancellationToken);

            return new MarkAsReadResponse
            {
                Success = true,
                Message = "Notification marked as read successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read: NotificationId: {NotificationId}", request.NotificationId);
            return new MarkAsReadResponse
            {
                Success = false,
                Message = $"Error marking notification as read: {ex.Message}"
            };
        }
    }

    public override async Task<SendNotificationResponse> SendNotification(
        SendNotificationRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Received gRPC SendNotification request for IdentityId: {IdentityId}, Title: {Title}",
            request.IdentityId, request.Title);

        try
        {
            var command = new AddNotification(
                Guid.Parse(request.IdentityId),
                request.Title,
                request.Description,
                request.Message,
                request.NotificationType);

            await _commandDispatcher.SendAsync(command, context.CancellationToken);

            return new SendNotificationResponse
            {
                NotificationId = command.Id.ToString(),
                Success = true,
                Message = "Notification sent successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification for IdentityId: {IdentityId}", request.IdentityId);
            return new SendNotificationResponse
            {
                Success = false,
                Message = $"Error sending notification: {ex.Message}"
            };
        }
    }
}







