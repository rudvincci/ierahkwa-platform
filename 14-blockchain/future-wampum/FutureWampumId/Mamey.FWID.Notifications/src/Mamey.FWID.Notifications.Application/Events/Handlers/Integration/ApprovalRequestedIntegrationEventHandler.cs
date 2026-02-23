using Mamey.CQRS.Events;
using Mamey.FWID.Notifications.Application.Events.Integration.Approvals;
using Mamey.FWID.Notifications.Application.Services;
using Mamey.FWID.Notifications.Application.Templates;
using Mamey.FWID.Notifications.Application.Templates.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Application.Events.Handlers.Integration;

/// <summary>
/// Handles approval request notifications (clan approval, credential approval, etc.)
/// </summary>
internal sealed class ApprovalRequestedIntegrationEventHandler : IEventHandler<ApprovalRequestedIntegrationEvent>
{
    private readonly ILogger<ApprovalRequestedIntegrationEventHandler> _logger;
    private readonly INotificationService _notificationService;
    
    public ApprovalRequestedIntegrationEventHandler(
        ILogger<ApprovalRequestedIntegrationEventHandler> logger,
        INotificationService notificationService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }
    
    public async Task HandleAsync(ApprovalRequestedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing approval request notification for {ApprovalId}, Type: {ApprovalType}",
            @event.ApprovalId, @event.ApprovalType);
        
        // Send email notification to approver
        if (!string.IsNullOrEmpty(@event.ApproverEmail))
        {
            var model = new ApprovalRequestDto
            {
                ApprovalId = @event.ApprovalId.ToString(),
                ApprovalType = @event.ApprovalType,
                RequesterName = @event.RequesterName,
                RequestDescription = @event.Description,
                RequestedAt = @event.RequestedAt,
                ExpiresAt = @event.ExpiresAt,
                ApprovalUrl = $"https://portal.futurewampum.io/approvals/{@event.ApprovalId}"
            };
            
            await _notificationService.SendEmailUsingTemplate(
                @event.ApproverEmail,
                $"Approval Required: {@event.ApprovalType}",
                EmailTemplateType.ApprovalRequired,
                model);
        }
        
        // Send push notification to approver
        if (@event.ApproverIdentityId.HasValue)
        {
            await _notificationService.SendPushAsync(
                @event.ApproverIdentityId.Value,
                "Approval Required",
                $"You have a new {@event.ApprovalType} request to review from {@event.RequesterName}");
        }
        
        _logger.LogInformation("Approval request notification sent for {ApprovalId}", @event.ApprovalId);
    }
}

/// <summary>
/// Handles credential expiring notifications.
/// </summary>
internal sealed class CredentialExpiringIntegrationEventHandler : IEventHandler<CredentialExpiringIntegrationEvent>
{
    private readonly ILogger<CredentialExpiringIntegrationEventHandler> _logger;
    private readonly INotificationService _notificationService;
    
    public CredentialExpiringIntegrationEventHandler(
        ILogger<CredentialExpiringIntegrationEventHandler> logger,
        INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }
    
    public async Task HandleAsync(CredentialExpiringIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing credential expiring notification for {CredentialId}, Days remaining: {DaysRemaining}",
            @event.CredentialId, @event.DaysUntilExpiration);
        
        // Send email
        if (!string.IsNullOrEmpty(@event.HolderEmail))
        {
            var model = new CredentialExpiringDto
            {
                CredentialType = @event.CredentialType,
                ExpiresAt = @event.ExpiresAt,
                DaysRemaining = @event.DaysUntilExpiration,
                RenewalUrl = $"https://portal.futurewampum.io/credentials/{@event.CredentialId}/renew"
            };
            
            await _notificationService.SendEmailUsingTemplate(
                @event.HolderEmail,
                $"Your {@event.CredentialType} expires in {@event.DaysUntilExpiration} days",
                EmailTemplateType.CredentialExpiring,
                model);
        }
        
        // Send push notification
        await _notificationService.SendPushAsync(
            @event.HolderIdentityId,
            "Credential Expiring Soon",
            $"Your {@event.CredentialType} will expire in {@event.DaysUntilExpiration} days. Renew now to avoid interruption.");
        
        _logger.LogInformation("Credential expiring notification sent for {CredentialId}", @event.CredentialId);
    }
}

/// <summary>
/// Handles security alert notifications.
/// </summary>
internal sealed class SecurityAlertIntegrationEventHandler : IEventHandler<SecurityAlertIntegrationEvent>
{
    private readonly ILogger<SecurityAlertIntegrationEventHandler> _logger;
    private readonly INotificationService _notificationService;
    
    public SecurityAlertIntegrationEventHandler(
        ILogger<SecurityAlertIntegrationEventHandler> logger,
        INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }
    
    public async Task HandleAsync(SecurityAlertIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Processing SECURITY ALERT for {IdentityId}: {AlertType} - {AlertLevel}",
            @event.IdentityId, @event.AlertType, @event.AlertLevel);
        
        // Always send email for security alerts
        if (!string.IsNullOrEmpty(@event.Email))
        {
            var model = new SecurityAlertDto
            {
                AlertType = @event.AlertType,
                AlertLevel = @event.AlertLevel,
                Description = @event.Description,
                OccurredAt = @event.OccurredAt,
                IpAddress = @event.IpAddress,
                Location = @event.Location,
                ActionRequired = @event.ActionRequired
            };
            
            await _notificationService.SendEmailUsingTemplate(
                @event.Email,
                $"SECURITY ALERT: {@event.AlertType}",
                EmailTemplateType.SecurityAlert,
                model);
        }
        
        // Always send push notification
        await _notificationService.SendPushAsync(
            @event.IdentityId,
            $"Security Alert: {@event.AlertType}",
            @event.Description);
        
        // Send SMS for high/critical alerts
        if (@event.AlertLevel is "High" or "Critical" && !string.IsNullOrEmpty(@event.PhoneNumber))
        {
            await _notificationService.SendSmsAsync(
                @event.PhoneNumber,
                $"SECURITY ALERT: {@event.Description}. If this wasn't you, contact support immediately.");
        }
        
        _logger.LogWarning("Security alert notifications sent for {IdentityId}", @event.IdentityId);
    }
}
