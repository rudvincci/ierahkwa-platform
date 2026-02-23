using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Exceptions;
using System.Text.Json;

namespace Mamey.Auth.Decentralized.Validation;

/// <summary>
/// Validates DID documents according to W3C DID specification.
/// </summary>
public class DidDocumentValidator
{
    /// <summary>
    /// Validates a DID document.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    /// <returns>A validation result.</returns>
    public static DidDocumentValidationResult Validate(DidDocument didDocument)
    {
        var result = new DidDocumentValidationResult();

        try
        {
            // Validate basic structure
            ValidateBasicStructure(didDocument, result);

            // Validate verification methods
            ValidateVerificationMethods(didDocument, result);

            // Validate service endpoints
            ValidateServiceEndpoints(didDocument, result);

            // Validate additional properties
            ValidateAdditionalProperties(didDocument, result);

            result.IsValid = result.Errors.Count == 0;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Validation error: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Validates the basic structure of a DID document.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    /// <param name="result">The validation result to update.</param>
    private static void ValidateBasicStructure(DidDocument didDocument, DidDocumentValidationResult result)
    {
        // Validate ID
        if (string.IsNullOrWhiteSpace(didDocument.Id))
        {
            result.Errors.Add("DID document must have an ID");
        }
        else if (!DidValidator.IsValidDid(didDocument.Id))
        {
            result.Errors.Add($"Invalid DID format: {didDocument.Id}");
        }

        // Validate context
        if (didDocument.Context == null || !didDocument.Context.Any())
        {
            result.Errors.Add("DID document must have at least one context");
        }
        else
        {
            foreach (var context in didDocument.Context)
            {
                if (string.IsNullOrWhiteSpace(context))
                {
                    result.Errors.Add("Context cannot be null or empty");
                }
                else if (!IsValidContext(context))
                {
                    result.Errors.Add($"Invalid context format: {context}");
                }
            }
        }
    }

    /// <summary>
    /// Validates verification methods in a DID document.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    /// <param name="result">The validation result to update.</param>
    private static void ValidateVerificationMethods(DidDocument didDocument, DidDocumentValidationResult result)
    {
        if (didDocument.VerificationMethod == null)
            return;

        if (didDocument.VerificationMethod.Count > 100) // Configurable limit
        {
            result.Errors.Add("Too many verification methods (maximum 100)");
            return;
        }

        var verificationMethodIds = new HashSet<string>();
        var controllerIds = new HashSet<string>();

        foreach (var vm in didDocument.VerificationMethod)
        {
            // Validate ID
            if (string.IsNullOrWhiteSpace(vm.Id))
            {
                result.Errors.Add("Verification method must have an ID");
                continue;
            }

            if (!DidValidator.IsValidDidUrl(vm.Id))
            {
                result.Errors.Add($"Invalid verification method ID format: {vm.Id}");
                continue;
            }

            // Check for duplicate IDs
            if (!verificationMethodIds.Add(vm.Id))
            {
                result.Errors.Add($"Duplicate verification method ID: {vm.Id}");
            }

            // Validate type
            if (string.IsNullOrWhiteSpace(vm.Type))
            {
                result.Errors.Add($"Verification method {vm.Id} must have a type");
            }

            // Validate controller
            if (string.IsNullOrWhiteSpace(vm.Controller))
            {
                result.Errors.Add($"Verification method {vm.Id} must have a controller");
            }
            else if (!DidValidator.IsValidDid(vm.Controller))
            {
                result.Errors.Add($"Invalid controller DID format: {vm.Controller}");
            }
            else
            {
                controllerIds.Add(vm.Controller);
            }

            // Validate public key material
            if ((vm.PublicKeyJwk == null || vm.PublicKeyJwk.Count == 0) && string.IsNullOrWhiteSpace(vm.PublicKeyMultibase))
            {
                result.Errors.Add($"Verification method {vm.Id} must have either publicKeyJwk or publicKeyMultibase");
            }

            // Validate public key JWK if present
            if (vm.PublicKeyJwk != null && vm.PublicKeyJwk.Count > 0)
            {
                if (!IsValidJwk(vm.PublicKeyJwk))
                {
                    result.Errors.Add($"Invalid JWK format for verification method {vm.Id}");
                }
            }

            // Validate public key multibase if present
            if (!string.IsNullOrWhiteSpace(vm.PublicKeyMultibase))
            {
                if (!IsValidMultibase(vm.PublicKeyMultibase))
                {
                    result.Errors.Add($"Invalid multibase format for verification method {vm.Id}");
                }
            }
        }

        // Validate that all controllers are valid DIDs
        foreach (var controllerId in controllerIds)
        {
            if (!DidValidator.IsValidDid(controllerId))
            {
                result.Errors.Add($"Invalid controller DID: {controllerId}");
            }
        }
    }

    /// <summary>
    /// Validates service endpoints in a DID document.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    /// <param name="result">The validation result to update.</param>
    private static void ValidateServiceEndpoints(DidDocument didDocument, DidDocumentValidationResult result)
    {
        if (didDocument.Service == null)
            return;

        if (didDocument.Service.Count > 100) // Configurable limit
        {
            result.Errors.Add("Too many service endpoints (maximum 100)");
            return;
        }

        var serviceIds = new HashSet<string>();

        foreach (var service in didDocument.Service)
        {
            // Validate ID
            if (string.IsNullOrWhiteSpace(service.Id))
            {
                result.Errors.Add("Service endpoint must have an ID");
                continue;
            }

            if (!DidValidator.IsValidDidUrl(service.Id))
            {
                result.Errors.Add($"Invalid service endpoint ID format: {service.Id}");
                continue;
            }

            // Check for duplicate IDs
            if (!serviceIds.Add(service.Id))
            {
                result.Errors.Add($"Duplicate service endpoint ID: {service.Id}");
            }

            // Validate type
            if (string.IsNullOrWhiteSpace(service.Type))
            {
                result.Errors.Add($"Service endpoint {service.Id} must have a type");
            }

            // Validate service endpoint URL
            if (string.IsNullOrWhiteSpace(service.ServiceEndpointUrl))
            {
                result.Errors.Add($"Service endpoint {service.Id} must have a service endpoint URL");
            }
            else if (!IsValidUrl(service.ServiceEndpointUrl))
            {
                result.Errors.Add($"Invalid service endpoint URL: {service.ServiceEndpointUrl}");
            }

            // Validate properties
            if (service.Properties != null)
            {
                foreach (var prop in service.Properties)
                {
                    if (string.IsNullOrWhiteSpace(prop.Key))
                    {
                        result.Errors.Add($"Service endpoint {service.Id} has a property with empty key");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Validates additional properties in a DID document.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    /// <param name="result">The validation result to update.</param>
    private static void ValidateAdditionalProperties(DidDocument didDocument, DidDocumentValidationResult result)
    {
        if (didDocument.AdditionalProperties == null)
            return;

        foreach (var prop in didDocument.AdditionalProperties)
        {
            if (string.IsNullOrWhiteSpace(prop.Key))
            {
                result.Errors.Add("Additional property has empty key");
            }

            // Validate that the value can be serialized
            try
            {
                JsonSerializer.Serialize(prop.Value);
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Additional property '{prop.Key}' has invalid value: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Validates a context string.
    /// </summary>
    /// <param name="context">The context to validate.</param>
    /// <returns>True if the context is valid; otherwise, false.</returns>
    private static bool IsValidContext(string context)
    {
        if (string.IsNullOrWhiteSpace(context))
            return false;

        // Must be a valid URI
        return Uri.TryCreate(context, UriKind.Absolute, out _);
    }

    /// <summary>
    /// Validates a JWK dictionary.
    /// </summary>
    /// <param name="jwk">The JWK to validate.</param>
    /// <returns>True if the JWK is valid; otherwise, false.</returns>
    private static bool IsValidJwk(Dictionary<string, object> jwk)
    {
        if (jwk == null || jwk.Count == 0)
            return false;

        try
        {
            return jwk.ContainsKey("kty") && jwk.ContainsKey("crv");
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates a multibase string.
    /// </summary>
    /// <param name="multibase">The multibase to validate.</param>
    /// <returns>True if the multibase is valid; otherwise, false.</returns>
    private static bool IsValidMultibase(string multibase)
    {
        if (string.IsNullOrWhiteSpace(multibase))
            return false;

        // Basic multibase validation - should start with a valid encoding character
        var validEncodings = new[] { 'z', 'm', 'u', 'b', 'B', 'f', 'F', 't', 'T', 'h', 'H', 'k', 'K' };
        return multibase.Length > 1 && validEncodings.Contains(multibase[0]);
    }

    /// <summary>
    /// Validates a URL string.
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <returns>True if the URL is valid; otherwise, false.</returns>
    private static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out var uri) && 
               (uri.Scheme == "http" || uri.Scheme == "https" || uri.Scheme == "did");
    }
}

/// <summary>
/// Result of DID document validation.
/// </summary>
public class DidDocumentValidationResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the validation was successful.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the list of validation errors.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of validation warnings.
    /// </summary>
    public List<string> Warnings { get; set; } = new();
}
