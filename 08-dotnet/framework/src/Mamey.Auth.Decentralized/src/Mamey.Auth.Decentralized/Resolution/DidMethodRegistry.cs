using Mamey.Auth.Decentralized.Methods;
using Mamey.Auth.Decentralized.Exceptions;

namespace Mamey.Auth.Decentralized.Resolution;

/// <summary>
/// Registry for DID methods
/// </summary>
public class DidMethodRegistry : IDidMethodRegistry
{
    private readonly Dictionary<string, IDidMethod> _methods = new();
    
    /// <summary>
    /// Registers a DID method
    /// </summary>
    /// <param name="method">The DID method to register</param>
    public void RegisterMethod(IDidMethod method)
    {
        if (method == null)
            throw new ArgumentNullException(nameof(method));
        
        if (string.IsNullOrEmpty(method.MethodName))
            throw new ArgumentException("Method name cannot be null or empty", nameof(method));
        
        _methods[method.MethodName] = method;
    }
    
    /// <summary>
    /// Unregisters a DID method
    /// </summary>
    /// <param name="methodName">The name of the method to unregister</param>
    /// <returns>True if the method was unregistered</returns>
    public bool UnregisterMethod(string methodName)
    {
        if (string.IsNullOrEmpty(methodName))
            throw new ArgumentException("Method name cannot be null or empty", nameof(methodName));
        
        return _methods.Remove(methodName);
    }
    
    /// <summary>
    /// Gets a DID method by name
    /// </summary>
    /// <param name="methodName">The name of the method</param>
    /// <returns>The DID method if found, null otherwise</returns>
    public IDidMethod? GetMethod(string methodName)
    {
        if (string.IsNullOrEmpty(methodName))
            throw new ArgumentException("Method name cannot be null or empty", nameof(methodName));
        
        return _methods.TryGetValue(methodName, out var method) ? method : null;
    }
    
    /// <summary>
    /// Gets all registered DID methods
    /// </summary>
    /// <returns>List of all registered methods</returns>
    public IEnumerable<IDidMethod> GetAllMethods()
    {
        return _methods.Values;
    }
    
    /// <summary>
    /// Gets the names of all registered methods
    /// </summary>
    /// <returns>List of method names</returns>
    public IEnumerable<string> GetMethodNames()
    {
        return _methods.Keys;
    }
    
    /// <summary>
    /// Checks if a method is registered
    /// </summary>
    /// <param name="methodName">The name of the method</param>
    /// <returns>True if the method is registered</returns>
    public bool IsMethodRegistered(string methodName)
    {
        if (string.IsNullOrEmpty(methodName))
            return false;
        
        return _methods.ContainsKey(methodName);
    }
}
