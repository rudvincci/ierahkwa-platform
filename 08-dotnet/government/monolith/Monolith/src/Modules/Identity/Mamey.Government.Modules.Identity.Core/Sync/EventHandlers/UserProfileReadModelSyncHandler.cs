using Mamey.CQRS;
using Mamey.Government.Modules.Identity.Core.Domain.Events;
using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Identity.Core.Sync.EventHandlers;

/// <summary>
/// Event handler for real-time synchronization of user profile changes from PostgreSQL to MongoDB.
/// Provides event-driven replication for CQRS consistency.
/// </summary>
internal sealed class UserProfileReadModelSyncHandler : 
    IDomainEventHandler<UserProfileCreated>,
    IDomainEventHandler<UserProfileModified>,
    IDomainEventHandler<UserProfileRemoved>
{
    private readonly IReadModelSyncService _syncService;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<UserProfileReadModelSyncHandler> _logger;

    public UserProfileReadModelSyncHandler(
        IReadModelSyncService syncService,
        IUserProfileRepository userProfileRepository,
        ILogger<UserProfileReadModelSyncHandler> logger)
    {
        _syncService = syncService;
        _userProfileRepository = userProfileRepository;
        _logger = logger;
    }

    public async Task HandleAsync(UserProfileCreated @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling UserProfileCreated event for read model sync: UserId={UserId}",
            @event.UserId.Value);

        try
        {
            // Fetch the full user profile from PostgreSQL
            var userProfile = await _userProfileRepository.GetAsync(@event.UserId, cancellationToken);
            if (userProfile == null)
            {
                _logger.LogWarning(
                    "UserProfile not found for read model sync: UserId={UserId}",
                    @event.UserId.Value);
                return;
            }

            // Sync to MongoDB read model
            await _syncService.SyncUserProfileAsync(userProfile, cancellationToken);
            
            _logger.LogInformation(
                "Successfully synced UserProfileCreated to MongoDB read model: UserId={UserId}",
                @event.UserId.Value);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the event handler - sync failures are logged but don't block domain events
            _logger.LogError(ex,
                "Error syncing UserProfileCreated to MongoDB read model: UserId={UserId}",
                @event.UserId.Value);
        }
    }

    public async Task HandleAsync(UserProfileModified @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling UserProfileModified event for read model sync: UserId={UserId}",
            @event.UserProfile.Id.Value);

        try
        {
            // The event contains the modified user profile
            await _syncService.SyncUserProfileAsync(@event.UserProfile, cancellationToken);
            
            _logger.LogInformation(
                "Successfully synced UserProfileModified to MongoDB read model: UserId={UserId}",
                @event.UserProfile.Id.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error syncing UserProfileModified to MongoDB read model: UserId={UserId}",
                @event.UserProfile.Id.Value);
        }
    }

    public async Task HandleAsync(UserProfileRemoved @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling UserProfileRemoved event for read model sync: UserId={UserId}",
            @event.UserProfile.Id.Value);

        try
        {
            // Remove from MongoDB read model
            await _syncService.RemoveUserProfileAsync(@event.UserProfile.Id, cancellationToken);
            
            _logger.LogInformation(
                "Successfully synced UserProfileRemoved to MongoDB read model: UserId={UserId}",
                @event.UserProfile.Id.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error syncing UserProfileRemoved to MongoDB read model: UserId={UserId}",
                @event.UserProfile.Id.Value);
        }
    }
}
