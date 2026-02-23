using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Exceptions;
using System.Text.Json;

namespace Mamey.Auth.Decentralized.Validation;

/// <summary>
/// Validates compliance with W3C DID specification.
/// </summary>
public class W3cComplianceValidator
{
    private static readonly string[] RequiredContexts = { "https://www.w3.org/ns/did/v1" };
    private static readonly string[] SupportedVerificationMethodTypes = 
    {
        "Ed25519VerificationKey2020",
        "RsaVerificationKey2018",
        "Secp256k1VerificationKey2018",
        "JsonWebKey2020"
    };

    private static readonly string[] SupportedServiceTypes = 
    {
        "DIDCommMessaging",
        "LinkedDomains",
        "DIDCommV2",
        "DIDCommMessagingV1"
    };

    /// <summary>
    /// Validates W3C compliance of a DID document.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    /// <returns>A compliance validation result.</returns>
    public static W3cComplianceResult Validate(DidDocument didDocument)
    {
        var result = new W3cComplianceResult();

        try
        {
            // Validate required contexts
            ValidateRequiredContexts(didDocument, result);

            // Validate verification method compliance
            ValidateVerificationMethodCompliance(didDocument, result);

            // Validate service endpoint compliance
            ValidateServiceEndpointCompliance(didDocument, result);

            // Validate additional properties compliance
            ValidateAdditionalPropertiesCompliance(didDocument, result);

            // Validate overall structure compliance
            ValidateStructureCompliance(didDocument, result);

            result.IsCompliant = result.Errors.Count == 0;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Compliance validation error: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Validates that required contexts are present.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    /// <param name="result">The compliance result to update.</param>
    private static void ValidateRequiredContexts(DidDocument didDocument, W3cComplianceResult result)
    {
        if (didDocument.Context == null || !didDocument.Context.Any())
        {
            result.Errors.Add("DID document must have at least one context");
            return;
        }

        // Check for required W3C context
        var hasW3cContext = didDocument.Context.Any(c => 
            c.Equals("https://www.w3.org/ns/did/v1", StringComparison.OrdinalIgnoreCase));

        if (!hasW3cContext)
        {
            result.Errors.Add("DID document must include the W3C DID context: https://www.w3.org/ns/did/v1");
        }

        // Validate all contexts are valid URIs
        foreach (var context in didDocument.Context)
        {
            if (!IsValidContextUri(context))
            {
                result.Errors.Add($"Invalid context URI: {context}");
            }
        }
    }

    /// <summary>
    /// Validates verification method compliance.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    /// <param name="result">The compliance result to update.</param>
    private static void ValidateVerificationMethodCompliance(DidDocument didDocument, W3cComplianceResult result)
    {
        if (didDocument.VerificationMethod == null)
            return;

        foreach (var vm in didDocument.VerificationMethod)
        {
            // Validate verification method type
            if (!string.IsNullOrWhiteSpace(vm.Type) && 
                !SupportedVerificationMethodTypes.Contains(vm.Type))
            {
                result.Warnings.Add($"Unsupported verification method type: {vm.Type}");
            }

            // Validate public key material
            if ((vm.PublicKeyJwk == null || vm.PublicKeyJwk.Count == 0) && 
                string.IsNullOrWhiteSpace(vm.PublicKeyMultibase))
            {
                result.Errors.Add($"Verification method {vm.Id} must have public key material");
            }

            // Validate JWK compliance
            if (vm.PublicKeyJwk != null && vm.PublicKeyJwk.Count > 0)
            {
                ValidateJwkCompliance(vm.PublicKeyJwk, vm.Id, result);
            }

            // Validate multibase compliance
            if (!string.IsNullOrWhiteSpace(vm.PublicKeyMultibase))
            {
                ValidateMultibaseCompliance(vm.PublicKeyMultibase, vm.Id, result);
            }

            // Validate controller compliance
            if (!string.IsNullOrWhiteSpace(vm.Controller) && 
                !IsValidDid(vm.Controller))
            {
                result.Errors.Add($"Invalid controller DID: {vm.Controller}");
            }
        }
    }

    /// <summary>
    /// Validates service endpoint compliance.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    /// <param name="result">The compliance result to update.</param>
    private static void ValidateServiceEndpointCompliance(DidDocument didDocument, W3cComplianceResult result)
    {
        if (didDocument.Service == null)
            return;

        foreach (var service in didDocument.Service)
        {
            // Validate service type
            if (!string.IsNullOrWhiteSpace(service.Type) && 
                !SupportedServiceTypes.Contains(service.Type))
            {
                result.Warnings.Add($"Unsupported service type: {service.Type}");
            }

            // Validate service endpoint URL
            if (!string.IsNullOrWhiteSpace(service.ServiceEndpointUrl))
            {
                if (!IsValidServiceEndpointUrl(service.ServiceEndpointUrl))
                {
                    result.Errors.Add($"Invalid service endpoint URL: {service.ServiceEndpointUrl}");
                }
            }

            // Validate service properties
            if (service.Properties != null)
            {
                ValidateServiceProperties(service.Properties, service.Id, result);
            }
        }
    }

    /// <summary>
    /// Validates additional properties compliance.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    /// <param name="result">The compliance result to update.</param>
    private static void ValidateAdditionalPropertiesCompliance(DidDocument didDocument, W3cComplianceResult result)
    {
        if (didDocument.AdditionalProperties == null)
            return;

        foreach (var prop in didDocument.AdditionalProperties)
        {
            // Validate property key format
            if (!IsValidPropertyKey(prop.Key))
            {
                result.Warnings.Add($"Non-standard property key: {prop.Key}");
            }

            // Validate property value can be serialized
            try
            {
                JsonSerializer.Serialize(prop.Value);
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Property '{prop.Key}' has invalid value: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Validates overall structure compliance.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    /// <param name="result">The compliance result to update.</param>
    private static void ValidateStructureCompliance(DidDocument didDocument, W3cComplianceResult result)
    {
        // Validate that the document has at least one verification method or service
        if ((didDocument.VerificationMethod == null || !didDocument.VerificationMethod.Any()) &&
            (didDocument.Service == null || !didDocument.Service.Any()))
        {
            result.Warnings.Add("DID document should have at least one verification method or service");
        }

        // Validate document size (should be reasonable)
        var documentSize = EstimateDocumentSize(didDocument);
        if (documentSize > 1024 * 1024) // 1MB
        {
            result.Warnings.Add("DID document is unusually large");
        }
    }

    /// <summary>
    /// Validates JWK compliance.
    /// </summary>
    /// <param name="jwk">The JWK to validate.</param>
    /// <param name="verificationMethodId">The verification method ID.</param>
    /// <param name="result">The compliance result to update.</param>
    private static void ValidateJwkCompliance(Dictionary<string, object> jwk, string verificationMethodId, W3cComplianceResult result)
    {
        try
        {
            // Check required fields
            if (!jwk.ContainsKey("kty"))
            {
                result.Errors.Add($"JWK for {verificationMethodId} missing 'kty' field");
                return;
            }

            if (!jwk.ContainsKey("crv"))
            {
                result.Errors.Add($"JWK for {verificationMethodId} missing 'crv' field");
                return;
            }

            // Validate key type
            var keyType = jwk["kty"]?.ToString();
            if (string.IsNullOrWhiteSpace(keyType))
            {
                result.Errors.Add($"JWK for {verificationMethodId} has empty 'kty' field");
            }

            // Validate curve
            var curve = jwk["crv"]?.ToString();
            if (string.IsNullOrWhiteSpace(curve))
            {
                result.Errors.Add($"JWK for {verificationMethodId} has empty 'crv' field");
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Invalid JWK for {verificationMethodId}: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates multibase compliance.
    /// </summary>
    /// <param name="multibase">The multibase to validate.</param>
    /// <param name="verificationMethodId">The verification method ID.</param>
    /// <param name="result">The compliance result to update.</param>
    private static void ValidateMultibaseCompliance(string multibase, string verificationMethodId, W3cComplianceResult result)
    {
        if (string.IsNullOrWhiteSpace(multibase))
        {
            result.Errors.Add($"Empty multibase for {verificationMethodId}");
            return;
        }

        // Check multibase prefix
        var validPrefixes = new[] { 'z', 'm', 'u', 'b', 'B', 'f', 'F', 't', 'T', 'h', 'H', 'k', 'K' };
        if (!validPrefixes.Contains(multibase[0]))
        {
            result.Errors.Add($"Invalid multibase prefix for {verificationMethodId}");
        }

        // Check minimum length
        if (multibase.Length < 2)
        {
            result.Errors.Add($"Multibase too short for {verificationMethodId}");
        }
    }

    /// <summary>
    /// Validates service properties.
    /// </summary>
    /// <param name="properties">The properties to validate.</param>
    /// <param name="serviceId">The service ID.</param>
    /// <param name="result">The compliance result to update.</param>
    private static void ValidateServiceProperties(Dictionary<string, JsonElement> properties, string serviceId, W3cComplianceResult result)
    {
        foreach (var prop in properties)
        {
            if (string.IsNullOrWhiteSpace(prop.Key))
            {
                result.Errors.Add($"Service {serviceId} has property with empty key");
            }

            if (prop.Value.ValueKind == JsonValueKind.Null)
            {
                result.Warnings.Add($"Service {serviceId} has property '{prop.Key}' with null value");
            }
        }
    }

    /// <summary>
    /// Validates a context URI.
    /// </summary>
    /// <param name="context">The context to validate.</param>
    /// <returns>True if the context is valid; otherwise, false.</returns>
    private static bool IsValidContextUri(string context)
    {
        if (string.IsNullOrWhiteSpace(context))
            return false;

        return Uri.TryCreate(context, UriKind.Absolute, out var uri) && 
               (uri.Scheme == "http" || uri.Scheme == "https");
    }

    /// <summary>
    /// Validates a DID.
    /// </summary>
    /// <param name="did">The DID to validate.</param>
    /// <returns>True if the DID is valid; otherwise, false.</returns>
    private static bool IsValidDid(string did)
    {
        return DidValidator.IsValidDid(did);
    }

    /// <summary>
    /// Validates a service endpoint URL.
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <returns>True if the URL is valid; otherwise, false.</returns>
    private static bool IsValidServiceEndpointUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out var uri) && 
               (uri.Scheme == "http" || uri.Scheme == "https" || uri.Scheme == "did");
    }

    /// <summary>
    /// Validates a property key.
    /// </summary>
    /// <param name="key">The key to validate.</param>
    /// <returns>True if the key is valid; otherwise, false.</returns>
    private static bool IsValidPropertyKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        // Property keys should be valid JSON-LD property names
        return key.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ':');
    }

    /// <summary>
    /// Estimates the size of a DID document.
    /// </summary>
    /// <param name="didDocument">The DID document to estimate.</param>
    /// <returns>The estimated size in bytes.</returns>
    private static int EstimateDocumentSize(DidDocument didDocument)
    {
        try
        {
            var json = JsonSerializer.Serialize(didDocument);
            return json.Length;
        }
        catch
        {
            return 0;
        }
    }
}

/// <summary>
/// Result of W3C compliance validation.
/// </summary>
public class W3cComplianceResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the document is compliant.
    /// </summary>
    public bool IsCompliant { get; set; }

    /// <summary>
    /// Gets or sets the list of compliance errors.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of compliance warnings.
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Gets or sets the compliance score (0-100).
    /// </summary>
    public int ComplianceScore { get; set; }

    /// <summary>
    /// Gets or sets the list of missing required elements.
    /// </summary>
    public List<string> MissingElements { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of non-standard elements.
    /// </summary>
    public List<string> NonStandardElements { get; set; } = new();
}
