using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

/// <summary>
/// Base class for command handlers with optimistic locking retry logic.
/// </summary>
internal abstract class BaseCommandHandler
{
    protected readonly IIdentityRepository Repository;
    protected readonly IEventProcessor EventProcessor;
    protected readonly ILogger Logger;

    protected BaseCommandHandler(
        IIdentityRepository repository,
        IEventProcessor eventProcessor,
        ILogger logger)
    {
        Repository = repository;
        EventProcessor = eventProcessor;
        Logger = logger;
    }

    /// <summary>
    /// Executes an update operation with optimistic locking retry logic.
    /// If a concurrency exception occurs, reloads the entity and retries once.
    /// </summary>
    protected async Task ExecuteWithRetryAsync<TIdentity>(
        TIdentity identity,
        Func<TIdentity, Task> updateAction,
        Func<TIdentity, Task<TIdentity>> reloadAction,
        int maxRetries = 1,
        CancellationToken cancellationToken = default)
        where TIdentity : class
    {
        var retryCount = 0;
        
        while (retryCount <= maxRetries)
        {
            try
            {
                await updateAction(identity);
                return;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (retryCount >= maxRetries)
                {
                    Logger.LogWarning(ex, 
                        "Optimistic locking conflict after {RetryCount} retries. Reloading entity and throwing exception.",
                        retryCount);
                    throw;
                }

                Logger.LogInformation(
                    "Optimistic locking conflict detected (attempt {Attempt}/{MaxAttempts}). Reloading entity and retrying.",
                    retryCount + 1, maxRetries + 1);

                // Reload entity with fresh version
                identity = await reloadAction(identity);
                retryCount++;
            }
        }
    }
}

