using System.Text.Json.Serialization;

namespace Mamey.Auth.Decentralized.Core;

/// <summary>
/// Represents a DID Document as defined by W3C DID 1.1 specification.
/// A DID Document contains information about a DID subject, including verification methods,
/// authentication mechanisms, and service endpoints.
/// </summary>
public class DidDocument
{
    /// <summary>
    /// The JSON-LD context for the DID Document
    /// </summary>
    [JsonPropertyName("@context")]
    public List<string> Context { get; set; } = new() { "https://www.w3.org/ns/did/v1" };
    
    /// <summary>
    /// The DID that this document describes
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The DID controllers for this DID Document
    /// </summary>
    [JsonPropertyName("controller")]
    public List<string> Controller { get; set; } = new();
    
    /// <summary>
    /// Verification methods that can be used to verify proofs
    /// </summary>
    [JsonPropertyName("verificationMethod")]
    public List<VerificationMethod> VerificationMethod { get; set; } = new();
    
    /// <summary>
    /// Verification methods that can be used for authentication
    /// </summary>
    [JsonPropertyName("authentication")]
    public List<string> Authentication { get; set; } = new();
    
    /// <summary>
    /// Verification methods that can be used for assertion
    /// </summary>
    [JsonPropertyName("assertionMethod")]
    public List<string> AssertionMethod { get; set; } = new();
    
    /// <summary>
    /// Verification methods that can be used for key agreement
    /// </summary>
    [JsonPropertyName("keyAgreement")]
    public List<string> KeyAgreement { get; set; } = new();
    
    /// <summary>
    /// Verification methods that can be used for capability invocation
    /// </summary>
    [JsonPropertyName("capabilityInvocation")]
    public List<string> CapabilityInvocation { get; set; } = new();
    
    /// <summary>
    /// Verification methods that can be used for capability delegation
    /// </summary>
    [JsonPropertyName("capabilityDelegation")]
    public List<string> CapabilityDelegation { get; set; } = new();
    
    /// <summary>
    /// Service endpoints associated with this DID
    /// </summary>
    [JsonPropertyName("service")]
    public List<ServiceEndpoint> Service { get; set; } = new();
    
    /// <summary>
    /// Additional properties not defined in the W3C specification
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();
    
    /// <summary>
    /// Validates the DID Document according to W3C DID 1.1 specification
    /// </summary>
    /// <returns>True if the document is valid, false otherwise</returns>
    public bool ValidateW3cCompliance()
    {
        // Check required @context
        if (!Context.Contains("https://www.w3.org/ns/did/v1"))
            return false;
        
        // Check required id field
        if (string.IsNullOrEmpty(Id))
            return false;
        
        // Validate DID format
        if (!Did.TryParse(Id, out _))
            return false;
        
        // Validate verification methods
        foreach (var vm in VerificationMethod)
        {
            if (!vm.IsValid())
                return false;
        }
        
        // Validate service endpoints
        foreach (var service in Service)
        {
            if (!service.IsValid())
                return false;
        }
        
        // Validate verification relationships reference existing verification methods
        var vmIds = new HashSet<string>(VerificationMethod.Select(vm => vm.Id));
        
        foreach (var auth in Authentication)
        {
            if (!vmIds.Contains(auth))
                return false;
        }
        
        foreach (var assertion in AssertionMethod)
        {
            if (!vmIds.Contains(assertion))
                return false;
        }
        
        foreach (var keyAgreement in KeyAgreement)
        {
            if (!vmIds.Contains(keyAgreement))
                return false;
        }
        
        foreach (var capabilityInvocation in CapabilityInvocation)
        {
            if (!vmIds.Contains(capabilityInvocation))
                return false;
        }
        
        foreach (var capabilityDelegation in CapabilityDelegation)
        {
            if (!vmIds.Contains(capabilityDelegation))
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Gets a verification method by its ID
    /// </summary>
    /// <param name="id">The verification method ID</param>
    /// <returns>The verification method if found, null otherwise</returns>
    public VerificationMethod? GetVerificationMethod(string id)
    {
        return VerificationMethod.FirstOrDefault(vm => vm.Id == id);
    }
    
    /// <summary>
    /// Gets all verification methods for a specific purpose
    /// </summary>
    /// <param name="purpose">The verification purpose (authentication, assertionMethod, etc.)</param>
    /// <returns>List of verification methods for the specified purpose</returns>
    public List<VerificationMethod> GetVerificationMethods(string purpose)
    {
        var methodIds = purpose.ToLowerInvariant() switch
        {
            "authentication" => Authentication,
            "assertionmethod" => AssertionMethod,
            "keyagreement" => KeyAgreement,
            "capabilityinvocation" => CapabilityInvocation,
            "capabilitydelegation" => CapabilityDelegation,
            _ => new List<string>()
        };
        
        return methodIds
            .Select(id => GetVerificationMethod(id))
            .Where(vm => vm != null)
            .Cast<VerificationMethod>()
            .ToList();
    }
    
    /// <summary>
    /// Gets a service endpoint by its ID
    /// </summary>
    /// <param name="id">The service endpoint ID</param>
    /// <returns>The service endpoint if found, null otherwise</returns>
    public ServiceEndpoint? GetService(string id)
    {
        return Service.FirstOrDefault(s => s.Id == id);
    }
    
    /// <summary>
    /// Gets all service endpoints of a specific type
    /// </summary>
    /// <param name="type">The service type</param>
    /// <returns>List of service endpoints of the specified type</returns>
    public List<ServiceEndpoint> GetServices(string type)
    {
        return Service.Where(s => s.Type == type).ToList();
    }
    
    /// <summary>
    /// Adds a verification method to the document
    /// </summary>
    /// <param name="verificationMethod">The verification method to add</param>
    public void AddVerificationMethod(VerificationMethod verificationMethod)
    {
        if (verificationMethod == null)
            throw new ArgumentNullException(nameof(verificationMethod));
        
        if (!verificationMethod.IsValid())
            throw new ArgumentException("Invalid verification method", nameof(verificationMethod));
        
        VerificationMethod.Add(verificationMethod);
    }
    
    /// <summary>
    /// Adds a service endpoint to the document
    /// </summary>
    /// <param name="service">The service endpoint to add</param>
    public void AddService(ServiceEndpoint service)
    {
        if (service == null)
            throw new ArgumentNullException(nameof(service));
        
        if (!service.IsValid())
            throw new ArgumentException("Invalid service endpoint", nameof(service));
        
        Service.Add(service);
    }
    
    /// <summary>
    /// Removes a verification method from the document
    /// </summary>
    /// <param name="id">The ID of the verification method to remove</param>
    /// <returns>True if the method was removed, false if not found</returns>
    public bool RemoveVerificationMethod(string id)
    {
        var method = VerificationMethod.FirstOrDefault(vm => vm.Id == id);
        if (method == null)
            return false;
        
        // Remove from all verification relationships
        Authentication.RemoveAll(a => a == id);
        AssertionMethod.RemoveAll(a => a == id);
        KeyAgreement.RemoveAll(k => k == id);
        CapabilityInvocation.RemoveAll(c => c == id);
        CapabilityDelegation.RemoveAll(c => c == id);
        
        return VerificationMethod.Remove(method);
    }
    
    /// <summary>
    /// Removes a service endpoint from the document
    /// </summary>
    /// <param name="id">The ID of the service endpoint to remove</param>
    /// <returns>True if the service was removed, false if not found</returns>
    public bool RemoveService(string id)
    {
        var service = Service.FirstOrDefault(s => s.Id == id);
        return service != null && Service.Remove(service);
    }
}
