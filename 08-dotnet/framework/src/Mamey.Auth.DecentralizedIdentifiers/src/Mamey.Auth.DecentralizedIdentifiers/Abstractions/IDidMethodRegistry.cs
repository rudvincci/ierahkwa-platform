namespace Mamey.Auth.DecentralizedIdentifiers.Abstractions;

/// <summary>
/// Registry of supported DID methods, allowing dynamic discovery and loading of method implementations.
/// </summary>
public interface IDidMethodRegistry
{
    /// <summary>
    /// Registers a DID method implementation.
    /// </summary>
    /// <param name="method">The DID method instance.</param>
    void Register(IDidMethod method);

    /// <summary>
    /// Gets a DID method implementation by canonical name.
    /// </summary>
    /// <param name="methodName">The method name (e.g., "key").</param>
    /// <returns>The method instance, or null if not registered.</returns>
    IDidMethod Get(string methodName);

    /// <summary>
    /// Gets all registered DID methods.
    /// </summary>
    IReadOnlyCollection<IDidMethod> GetAll();
}