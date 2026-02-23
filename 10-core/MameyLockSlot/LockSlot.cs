using System.Collections.Concurrent;

namespace MameyLockSlot;

/// <summary>
/// Distributed lock slot - prevents race conditions across services
/// </summary>
public interface ILockSlot
{
    Task<ILockHandle?> AcquireAsync(string resource, TimeSpan expiry, CancellationToken ct = default);
    Task<bool> ReleaseAsync(string resource, string lockId);
    Task<bool> ExtendAsync(string resource, string lockId, TimeSpan extension);
    Task<bool> IsLockedAsync(string resource);
    Task<LockInfo?> GetLockInfoAsync(string resource);
}

public interface ILockHandle : IAsyncDisposable
{
    string Resource { get; }
    string LockId { get; }
    DateTime ExpiresAt { get; }
    bool IsValid { get; }
    Task<bool> ExtendAsync(TimeSpan extension);
}

public class LockInfo
{
    public string Resource { get; set; } = string.Empty;
    public string LockId { get; set; } = string.Empty;
    public string? OwnerId { get; set; }
    public DateTime AcquiredAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}

/// <summary>
/// In-memory lock implementation (for single instance or testing)
/// </summary>
public class InMemoryLockSlot : ILockSlot
{
    private readonly ConcurrentDictionary<string, LockEntry> _locks = new();
    private readonly Timer _cleanupTimer;

    public InMemoryLockSlot()
    {
        _cleanupTimer = new Timer(CleanupExpired, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
    }

    public Task<ILockHandle?> AcquireAsync(string resource, TimeSpan expiry, CancellationToken ct = default)
    {
        var lockId = Guid.NewGuid().ToString("N");
        var entry = new LockEntry
        {
            LockId = lockId,
            Resource = resource,
            AcquiredAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(expiry)
        };

        if (_locks.TryAdd(resource, entry))
        {
            return Task.FromResult<ILockHandle?>(new LockHandle(this, resource, lockId, entry.ExpiresAt));
        }

        // Check if existing lock is expired
        if (_locks.TryGetValue(resource, out var existing) && existing.IsExpired)
        {
            if (_locks.TryUpdate(resource, entry, existing))
            {
                return Task.FromResult<ILockHandle?>(new LockHandle(this, resource, lockId, entry.ExpiresAt));
            }
        }

        return Task.FromResult<ILockHandle?>(null);
    }

    public Task<bool> ReleaseAsync(string resource, string lockId)
    {
        if (_locks.TryGetValue(resource, out var entry) && entry.LockId == lockId)
        {
            return Task.FromResult(_locks.TryRemove(resource, out _));
        }
        return Task.FromResult(false);
    }

    public Task<bool> ExtendAsync(string resource, string lockId, TimeSpan extension)
    {
        if (_locks.TryGetValue(resource, out var entry) && entry.LockId == lockId && !entry.IsExpired)
        {
            entry.ExpiresAt = DateTime.UtcNow.Add(extension);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> IsLockedAsync(string resource)
    {
        if (_locks.TryGetValue(resource, out var entry))
        {
            return Task.FromResult(!entry.IsExpired);
        }
        return Task.FromResult(false);
    }

    public Task<LockInfo?> GetLockInfoAsync(string resource)
    {
        if (_locks.TryGetValue(resource, out var entry) && !entry.IsExpired)
        {
            return Task.FromResult<LockInfo?>(new LockInfo
            {
                Resource = entry.Resource,
                LockId = entry.LockId,
                OwnerId = entry.OwnerId,
                AcquiredAt = entry.AcquiredAt,
                ExpiresAt = entry.ExpiresAt
            });
        }
        return Task.FromResult<LockInfo?>(null);
    }

    private void CleanupExpired(object? state)
    {
        var expired = _locks.Where(kv => kv.Value.IsExpired).Select(kv => kv.Key).ToList();
        foreach (var key in expired)
        {
            _locks.TryRemove(key, out _);
        }
    }

    private class LockEntry
    {
        public string LockId { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public string? OwnerId { get; set; }
        public DateTime AcquiredAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    }

    private class LockHandle : ILockHandle
    {
        private readonly InMemoryLockSlot _lockSlot;
        private bool _released;

        public string Resource { get; }
        public string LockId { get; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsValid => !_released && DateTime.UtcNow < ExpiresAt;

        public LockHandle(InMemoryLockSlot lockSlot, string resource, string lockId, DateTime expiresAt)
        {
            _lockSlot = lockSlot;
            Resource = resource;
            LockId = lockId;
            ExpiresAt = expiresAt;
        }

        public async Task<bool> ExtendAsync(TimeSpan extension)
        {
            if (await _lockSlot.ExtendAsync(Resource, LockId, extension))
            {
                ExpiresAt = DateTime.UtcNow.Add(extension);
                return true;
            }
            return false;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_released)
            {
                _released = true;
                await _lockSlot.ReleaseAsync(Resource, LockId);
            }
        }
    }
}

/// <summary>
/// Lock slot extensions for common patterns
/// </summary>
public static class LockSlotExtensions
{
    public static async Task<T> WithLockAsync<T>(this ILockSlot lockSlot, string resource, TimeSpan expiry, Func<Task<T>> action)
    {
        await using var handle = await lockSlot.AcquireAsync(resource, expiry);
        if (handle == null)
            throw new LockAcquisitionException($"Failed to acquire lock for resource: {resource}");

        return await action();
    }

    public static async Task WithLockAsync(this ILockSlot lockSlot, string resource, TimeSpan expiry, Func<Task> action)
    {
        await using var handle = await lockSlot.AcquireAsync(resource, expiry);
        if (handle == null)
            throw new LockAcquisitionException($"Failed to acquire lock for resource: {resource}");

        await action();
    }

    public static async Task<ILockHandle?> TryAcquireWithRetryAsync(this ILockSlot lockSlot, string resource, TimeSpan expiry, int maxRetries = 3, TimeSpan? retryDelay = null)
    {
        var delay = retryDelay ?? TimeSpan.FromMilliseconds(100);
        
        for (int i = 0; i < maxRetries; i++)
        {
            var handle = await lockSlot.AcquireAsync(resource, expiry);
            if (handle != null)
                return handle;

            await Task.Delay(delay);
        }
        
        return null;
    }
}

public class LockAcquisitionException : Exception
{
    public LockAcquisitionException(string message) : base(message) { }
}
