using Grpc.Core;
using Mamey.FWID.Notifications.Api.Protos;

namespace Mamey.FWID.Notifications.GrpcClient.Services;

/// <summary>
/// Client wrapper for NotificationService gRPC calls.
/// </summary>
public class NotificationServiceClient
{
    private readonly NotificationService.NotificationServiceClient _client;
    private readonly ILogger<NotificationServiceClient> _logger;

    public NotificationServiceClient(
        NotificationService.NotificationServiceClient client,
        ILogger<NotificationServiceClient> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets notifications for an identity.
    /// </summary>
    public async Task<GetNotificationsResponse> GetNotificationsAsync(
        GetNotificationsRequest request,
        CallOptions? callOptions = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting notifications for identity: {IdentityId}", request.IdentityId);
        
        try
        {
            var response = await _client.GetNotificationsAsync(
                request,
                callOptions ?? new CallOptions(),
                cancellationToken: cancellationToken);
            
            _logger.LogInformation(
                "Retrieved {Count} notifications for identity: {IdentityId}",
                response.Notifications.Count,
                request.IdentityId);
            
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting notifications for identity: {IdentityId}", request.IdentityId);
            throw;
        }
    }

    /// <summary>
    /// Marks a notification as read.
    /// </summary>
    public async Task<MarkAsReadResponse> MarkAsReadAsync(
        MarkAsReadRequest request,
        CallOptions? callOptions = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Marking notification as read: NotificationId={NotificationId}, IdentityId={IdentityId}",
            request.NotificationId, request.IdentityId);
        
        try
        {
            var response = await _client.MarkAsReadAsync(
                request,
                callOptions ?? new CallOptions(),
                cancellationToken: cancellationToken);
            
            _logger.LogInformation(
                "Marked notification as read. Success: {Success}, Message: {Message}",
                response.Success,
                response.Message);
            
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error marking notification as read: NotificationId={NotificationId}", request.NotificationId);
            throw;
        }
    }

    /// <summary>
    /// Sends a notification.
    /// </summary>
    public async Task<SendNotificationResponse> SendNotificationAsync(
        SendNotificationRequest request,
        CallOptions? callOptions = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending notification for identity: {IdentityId}, Title: {Title}",
            request.IdentityId, request.Title);
        
        try
        {
            var response = await _client.SendNotificationAsync(
                request,
                callOptions ?? new CallOptions(),
                cancellationToken: cancellationToken);
            
            _logger.LogInformation(
                "Notification sent. NotificationId: {NotificationId}, Success: {Success}, Message: {Message}",
                response.NotificationId,
                response.Success,
                response.Message);
            
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error sending notification for identity: {IdentityId}", request.IdentityId);
            throw;
        }
    }
}







