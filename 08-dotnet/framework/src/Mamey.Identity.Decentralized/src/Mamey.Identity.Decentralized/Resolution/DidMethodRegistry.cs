using System.Collections.Concurrent;
using Mamey.Identity.Decentralized.Abstractions;

namespace Mamey.Identity.Decentralized.Resolution;

/// <summary>
/// Thread-safe registry for dynamic discovery and management of DID methods.
/// </summary>
public class DidMethodRegistry : IDidMethodRegistry
{
    private readonly ConcurrentDictionary<string, IDidMethod> _methods = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Registers a DID method implementation by canonical method name.
    /// </summary>
    /// <param name="method">The DID method instance.</param>
    public void Register(IDidMethod method)
    {
        if (method == null) throw new ArgumentNullException(nameof(method));
        _methods[method.Name] = method;
    }

    /// <summary>
    /// Gets a DID method implementation by canonical name.
    /// </summary>
    /// <param name="methodName">The method name (e.g., "key", "web").</param>
    /// <returns>The method instance, or null if not registered.</returns>
    public IDidMethod Get(string methodName)
    {
        if (methodName == null) throw new ArgumentNullException(nameof(methodName));
        _methods.TryGetValue(methodName, out var method);
        return method;
    }

    /// <summary>
    /// Gets all registered DID methods.
    /// </summary>
    public IReadOnlyCollection<IDidMethod> GetAll() => _methods.Values.ToList().AsReadOnly();
}