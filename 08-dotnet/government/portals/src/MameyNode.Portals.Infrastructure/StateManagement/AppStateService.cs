using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace MameyNode.Portals.Infrastructure.StateManagement;

/// <summary>
/// Application state management service
/// </summary>
public interface IAppStateService
{
    /// <summary>
    /// Get state value
    /// </summary>
    Task<T?> GetStateAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// Set state value
    /// </summary>
    Task SetStateAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// Remove state value
    /// </summary>
    Task RemoveStateAsync(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Clear all state
    /// </summary>
    Task ClearStateAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Subscribe to state changes
    /// </summary>
    void Subscribe<T>(string key, Action<T?> callback) where T : class;
    
    /// <summary>
    /// Unsubscribe from state changes
    /// </summary>
    void Unsubscribe<T>(string key, Action<T?> callback) where T : class;
}

/// <summary>
/// Application state management service implementation
/// </summary>
public class AppStateService : IAppStateService
{
    private readonly ConcurrentDictionary<string, object> _state = new();
    private readonly ConcurrentDictionary<string, List<Delegate>> _subscribers = new();
    private readonly ILogger<AppStateService> _logger;

    public AppStateService(ILogger<AppStateService> logger)
    {
        _logger = logger;
    }

    public Task<T?> GetStateAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (_state.TryGetValue(key, out var value) && value is T typedValue)
        {
            return Task.FromResult<T?>(typedValue);
        }
        return Task.FromResult<T?>(null);
    }

    public Task SetStateAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        _state.AddOrUpdate(key, value, (k, v) => value);
        
        // Notify subscribers
        if (_subscribers.TryGetValue(key, out var callbacks))
        {
            foreach (var callback in callbacks)
            {
                try
                {
                    if (callback is Action<T?> typedCallback)
                    {
                        typedCallback(value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error notifying subscriber for key {Key}", key);
                }
            }
        }
        
        _logger.LogDebug("State updated for key {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveStateAsync(string key, CancellationToken cancellationToken = default)
    {
        _state.TryRemove(key, out _);
        _logger.LogDebug("State removed for key {Key}", key);
        return Task.CompletedTask;
    }

    public Task ClearStateAsync(CancellationToken cancellationToken = default)
    {
        _state.Clear();
        _logger.LogDebug("All state cleared");
        return Task.CompletedTask;
    }

    public void Subscribe<T>(string key, Action<T?> callback) where T : class
    {
        _subscribers.AddOrUpdate(
            key,
            new List<Delegate> { callback },
            (k, list) =>
            {
                list.Add(callback);
                return list;
            });
    }

    public void Unsubscribe<T>(string key, Action<T?> callback) where T : class
    {
        if (_subscribers.TryGetValue(key, out var callbacks))
        {
            callbacks.Remove(callback);
            if (callbacks.Count == 0)
            {
                _subscribers.TryRemove(key, out _);
            }
        }
    }
}


