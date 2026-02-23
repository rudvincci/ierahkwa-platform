using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.MessageBrokers;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.EventHandlers;

/// <summary>
/// Handles identity updated events and publishes integration events for credential sync.
/// </summary>
public class IdentityUpdatedEventHandler :
    IEventHandler<IdentityNameUpdated>,
    IEventHandler<IdentityAddressUpdated>,
    IEventHandler<IdentityPhotoUpdated>
{
    private readonly ILogger<IdentityUpdatedEventHandler> _logger;
    private readonly IBusPublisher _publisher;
    
    public IdentityUpdatedEventHandler(
        ILogger<IdentityUpdatedEventHandler> logger,
        IBusPublisher publisher)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }
    
    /// <summary>
    /// Handles identity name updates - triggers credential re-issue.
    /// </summary>
    public async Task HandleAsync(IdentityNameUpdated @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Identity name updated for {IdentityId}: {OldName} -> {NewName}",
            @event.IdentityId, @event.OldName, @event.NewName);
        
        // Publish integration event for credential sync
        await _publisher.PublishAsync(new IdentityAttributeChangedIntegrationEvent
        {
            IdentityId = @event.IdentityId.Value,
            AttributeType = "Name",
            OldValue = @event.OldName?.FullName,
            NewValue = @event.NewName?.FullName,
            RequiresCredentialReissue = true,
            ChangedAt = @event.UpdatedAt
        });
    }
    
    /// <summary>
    /// Handles identity address updates - triggers credential re-issue.
    /// </summary>
    public async Task HandleAsync(IdentityAddressUpdated @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Identity address updated for {IdentityId}", @event.IdentityId);
        
        await _publisher.PublishAsync(new IdentityAttributeChangedIntegrationEvent
        {
            IdentityId = @event.IdentityId.Value,
            AttributeType = "Address",
            OldValue = @event.OldAddress?.ToString(),
            NewValue = @event.NewAddress?.ToString(),
            RequiresCredentialReissue = true,
            ChangedAt = @event.UpdatedAt
        });
    }
    
    /// <summary>
    /// Handles identity photo updates - triggers credential re-issue with new photo.
    /// </summary>
    public async Task HandleAsync(IdentityPhotoUpdated @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Identity photo updated for {IdentityId}", @event.IdentityId);
        
        await _publisher.PublishAsync(new IdentityAttributeChangedIntegrationEvent
        {
            IdentityId = @event.IdentityId.Value,
            AttributeType = "Photo",
            OldValue = @event.OldPhotoUrl,
            NewValue = @event.NewPhotoUrl,
            RequiresCredentialReissue = true,
            ChangedAt = @event.UpdatedAt
        });
    }
}

/// <summary>
/// Integration event for identity attribute changes.
/// </summary>
public record IdentityAttributeChangedIntegrationEvent
{
    public Guid IdentityId { get; init; }
    public string AttributeType { get; init; } = null!;
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
    public bool RequiresCredentialReissue { get; init; }
    public DateTime ChangedAt { get; init; }
}

/// <summary>
/// Domain event for identity name updates.
/// </summary>
public record IdentityNameUpdated(
    Mamey.FWID.Identities.Domain.Entities.IdentityId IdentityId,
    Mamey.Types.Name? OldName,
    Mamey.Types.Name? NewName,
    DateTime UpdatedAt) : Mamey.CQRS.Events.IEvent;

/// <summary>
/// Domain event for identity address updates.
/// </summary>
public record IdentityAddressUpdated(
    Mamey.FWID.Identities.Domain.Entities.IdentityId IdentityId,
    Mamey.Types.Address? OldAddress,
    Mamey.Types.Address? NewAddress,
    DateTime UpdatedAt) : Mamey.CQRS.Events.IEvent;

/// <summary>
/// Domain event for identity photo updates.
/// </summary>
public record IdentityPhotoUpdated(
    Mamey.FWID.Identities.Domain.Entities.IdentityId IdentityId,
    string? OldPhotoUrl,
    string? NewPhotoUrl,
    DateTime UpdatedAt) : Mamey.CQRS.Events.IEvent;
