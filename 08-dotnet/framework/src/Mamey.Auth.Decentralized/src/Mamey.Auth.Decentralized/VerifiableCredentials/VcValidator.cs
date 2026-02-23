using Mamey.Auth.Decentralized.Exceptions;
using Mamey.Auth.Decentralized.Crypto;

namespace Mamey.Auth.Decentralized.VerifiableCredentials;

/// <summary>
/// Validator for Verifiable Credentials and Presentations
/// </summary>
public class VcValidator
{
    private readonly IKeyGenerator _keyGenerator;
    private readonly ILogger<VcValidator> _logger;

    /// <summary>
    /// Initializes a new instance of the VcValidator class
    /// </summary>
    /// <param name="keyGenerator">The key generator service</param>
    /// <param name="logger">The logger</param>
    public VcValidator(IKeyGenerator keyGenerator, ILogger<VcValidator> logger)
    {
        _keyGenerator = keyGenerator;
        _logger = logger;
    }

    /// <summary>
    /// Validates a Verifiable Credential
    /// </summary>
    /// <param name="credential">The verifiable credential to validate</param>
    /// <param name="publicKey">The public key for verification</param>
    /// <param name="algorithm">The verification algorithm</param>
    /// <returns>Validation result</returns>
    public VcValidationResult ValidateCredential(VerifiableCredential credential, byte[]? publicKey = null, string algorithm = "Ed25519")
    {
        var result = new VcValidationResult();

        try
        {
            // Basic structure validation
            if (!credential.IsValid())
            {
                result.AddError("Invalid credential structure");
                return result;
            }

            // Check expiration
            if (credential.IsExpired())
            {
                result.AddError("Credential has expired");
            }

            // Validate context
            if (!ValidateContext(credential.Context))
            {
                result.AddError("Invalid or missing context");
            }

            // Validate type
            if (!ValidateType(credential.Type))
            {
                result.AddError("Invalid or missing type");
            }

            // Validate credential subject
            if (!credential.CredentialSubject.IsValid())
            {
                result.AddError("Invalid credential subject");
            }

            // Validate proof if present and public key provided
            if (credential.Proof != null && publicKey != null)
            {
                if (!ValidateProof(credential, publicKey, algorithm))
                {
                    result.AddError("Invalid proof");
                }
            }

            result.IsValid = result.Errors.Count == 0;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating verifiable credential");
            result.AddError($"Validation error: {ex.Message}");
            return result;
        }
    }

    /// <summary>
    /// Validates a Verifiable Presentation
    /// </summary>
    /// <param name="presentation">The verifiable presentation to validate</param>
    /// <param name="publicKey">The public key for verification</param>
    /// <param name="algorithm">The verification algorithm</param>
    /// <returns>Validation result</returns>
    public VcValidationResult ValidatePresentation(VerifiablePresentation presentation, byte[]? publicKey = null, string algorithm = "Ed25519")
    {
        var result = new VcValidationResult();

        try
        {
            // Basic structure validation
            if (!presentation.IsValid())
            {
                result.AddError("Invalid presentation structure");
                return result;
            }

            // Validate context
            if (!ValidateContext(presentation.Context))
            {
                result.AddError("Invalid or missing context");
            }

            // Validate type
            if (!ValidatePresentationType(presentation.Type))
            {
                result.AddError("Invalid or missing type");
            }

            // Validate all credentials in the presentation
            if (presentation.VerifiableCredentials != null)
            {
                foreach (var credential in presentation.VerifiableCredentials)
                {
                    var credentialResult = ValidateCredential(credential, publicKey, algorithm);
                    if (!credentialResult.IsValid)
                    {
                        result.AddError($"Invalid credential in presentation: {string.Join(", ", credentialResult.Errors)}");
                    }
                }
            }

            // Validate proof if present and public key provided
            if (presentation.Proof != null && publicKey != null)
            {
                if (!ValidatePresentationProof(presentation, publicKey, algorithm))
                {
                    result.AddError("Invalid presentation proof");
                }
            }

            result.IsValid = result.Errors.Count == 0;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating verifiable presentation");
            result.AddError($"Validation error: {ex.Message}");
            return result;
        }
    }

    private bool ValidateContext(List<string> context)
    {
        if (context == null || !context.Any())
            return false;

        // Check for required VC context
        return context.Contains("https://www.w3.org/2018/credentials/v1");
    }

    private bool ValidateType(List<string> type)
    {
        if (type == null || !type.Any())
            return false;

        // Check for required VC type
        return type.Contains("VerifiableCredential");
    }

    private bool ValidatePresentationType(List<string> type)
    {
        if (type == null || !type.Any())
            return false;

        // Check for required VP type
        return type.Contains("VerifiablePresentation");
    }

    private bool ValidateProof(VerifiableCredential credential, byte[] publicKey, string algorithm)
    {
        try
        {
            if (credential.Proof == null)
                return false;

            if (!credential.Proof.IsValid())
                return false;

            var provider = _keyGenerator.GetProvider(algorithm);
            var signatureValue = credential.Proof.GetSignatureValue();
            
            if (string.IsNullOrEmpty(signatureValue))
                return false;

            var signature = Convert.FromBase64String(signatureValue);
            
            // Create canonical form for verification
            var canonicalForm = CreateCanonicalForm(credential, credential.Proof);
            
            return provider.Verify(Encoding.UTF8.GetBytes(canonicalForm), signature, publicKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating proof");
            return false;
        }
    }

    private bool ValidatePresentationProof(VerifiablePresentation presentation, byte[] publicKey, string algorithm)
    {
        try
        {
            if (presentation.Proof == null)
                return false;

            if (!presentation.Proof.IsValid())
                return false;

            var provider = _keyGenerator.GetProvider(algorithm);
            var signatureValue = presentation.Proof.GetSignatureValue();
            
            if (string.IsNullOrEmpty(signatureValue))
                return false;

            var signature = Convert.FromBase64String(signatureValue);
            
            // Create canonical form for verification
            var canonicalForm = CreateCanonicalForm(presentation, presentation.Proof);
            
            return provider.Verify(Encoding.UTF8.GetBytes(canonicalForm), signature, publicKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating presentation proof");
            return false;
        }
    }

    private string CreateCanonicalForm(object document, Proof proof)
    {
        // Create a copy of the document without the proof
        var documentJson = JsonSerializer.Serialize(document, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        var documentDict = JsonSerializer.Deserialize<Dictionary<string, object>>(documentJson) ?? new Dictionary<string, object>();

        // Remove the proof from the document
        documentDict.Remove("proof");

        // Create the canonical form
        var canonicalForm = JsonSerializer.Serialize(documentDict, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        return canonicalForm;
    }
}

/// <summary>
/// Result of Verifiable Credential validation
/// </summary>
public class VcValidationResult
{
    /// <summary>
    /// Gets or sets whether the validation was successful
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets the list of validation errors
    /// </summary>
    public List<string> Errors { get; } = new();

    /// <summary>
    /// Gets the list of validation warnings
    /// </summary>
    public List<string> Warnings { get; } = new();

    /// <summary>
    /// Adds an error to the validation result
    /// </summary>
    /// <param name="error">The error message</param>
    public void AddError(string error)
    {
        Errors.Add(error);
    }

    /// <summary>
    /// Adds a warning to the validation result
    /// </summary>
    /// <param name="warning">The warning message</param>
    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }

    /// <summary>
    /// Gets a summary of the validation result
    /// </summary>
    /// <returns>Validation summary</returns>
    public string GetSummary()
    {
        var summary = $"Validation {(IsValid ? "succeeded" : "failed")}";
        
        if (Errors.Any())
        {
            summary += $"\nErrors: {string.Join(", ", Errors)}";
        }
        
        if (Warnings.Any())
        {
            summary += $"\nWarnings: {string.Join(", ", Warnings)}";
        }
        
        return summary;
    }
}
