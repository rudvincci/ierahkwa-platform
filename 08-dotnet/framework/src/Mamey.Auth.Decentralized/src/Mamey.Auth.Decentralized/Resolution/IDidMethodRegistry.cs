using Mamey.Auth.Decentralized.Methods;

namespace Mamey.Auth.Decentralized.Resolution;

/// <summary>
/// Interface for DID method registry
/// </summary>
public interface IDidMethodRegistry
{
    /// <summary>
    /// Registers a DID method
    /// </summary>
    /// <param name="method">The DID method to register</param>
    void RegisterMethod(IDidMethod method);
    
    /// <summary>
    /// Unregisters a DID method
    /// </summary>
    /// <param name="methodName">The name of the method to unregister</param>
    /// <returns>True if the method was unregistered</returns>
    bool UnregisterMethod(string methodName);
    
    /// <summary>
    /// Gets a DID method by name
    /// </summary>
    /// <param name="methodName">The name of the method</param>
    /// <returns>The DID method if found, null otherwise</returns>
    IDidMethod? GetMethod(string methodName);
    
    /// <summary>
    /// Gets all registered DID methods
    /// </summary>
    /// <returns>List of all registered methods</returns>
    IEnumerable<IDidMethod> GetAllMethods();
    
    /// <summary>
    /// Gets the names of all registered methods
    /// </summary>
    /// <returns>List of method names</returns>
    IEnumerable<string> GetMethodNames();
    
    /// <summary>
    /// Checks if a method is registered
    /// </summary>
    /// <param name="methodName">The name of the method</param>
    /// <returns>True if the method is registered</returns>
    bool IsMethodRegistered(string methodName);
}
