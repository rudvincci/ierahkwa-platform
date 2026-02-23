using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Crypto;
using Mamey.Auth.DecentralizedIdentifiers.Resolution;
using Mamey.Auth.DecentralizedIdentifiers.Services;
using Mamey.Auth.DecentralizedIdentifiers.Validation;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.DecentralizedIdentifiers.VC;

/// <summary>
/// Comprehensive service for creating and validating Verifiable Presentations with selective disclosure capabilities.
/// </summary>
public class VPService : IVPService
{
    private readonly IDidResolver _didResolver;
    private readonly IKeyProvider _keyProvider;
    private readonly IProofService _proofService;
    private readonly IVerifiableCredentialValidator _vcValidator;
    private readonly ICredentialStatusService _credentialStatusService;
    private readonly IJsonLdProcessor _jsonLdProcessor;
    private readonly ILogger<VPService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public VPService(
        IDidResolver didResolver,
        IKeyProvider keyProvider,
        IProofService proofService,
        IVerifiableCredentialValidator vcValidator,
        ICredentialStatusService credentialStatusService,
        IJsonLdProcessor jsonLdProcessor,
        ILogger<VPService> logger,
        IServiceProvider serviceProvider)
    {
        _didResolver = didResolver ?? throw new ArgumentNullException(nameof(didResolver));
        _keyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
        _proofService = proofService ?? throw new ArgumentNullException(nameof(proofService));
        _vcValidator = vcValidator ?? throw new ArgumentNullException(nameof(vcValidator));
        _credentialStatusService = credentialStatusService ?? throw new ArgumentNullException(nameof(credentialStatusService));
        _jsonLdProcessor = jsonLdProcessor ?? throw new ArgumentNullException(nameof(jsonLdProcessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task<VerifiablePresentation> CreatePresentationAsync(PresentationCreateRequest request)
    {
        try
        {
            _logger.LogInformation("Creating Verifiable Presentation for holder {HolderDid}", request.HolderDid);

            // Resolve holder DID document
            var holderDidDocument = await _didResolver.ResolveAsync(request.HolderDid);
            if (holderDidDocument == null)
            {
                throw new InvalidOperationException($"Could not resolve holder DID: {request.HolderDid}");
            }

            // Validate credentials
            var validatedCredentials = new List<VerifiableCredential>();
            foreach (var credential in request.Credentials)
            {
                var validationResult = await _vcValidator.ValidateAsync(credential);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Invalid credential {CredentialId}: {Errors}", 
                        credential.Id, string.Join(", ", validationResult.Errors));
                    continue;
                }
                validatedCredentials.Add(credential);
            }

            if (!validatedCredentials.Any())
            {
                throw new InvalidOperationException("No valid credentials provided");
            }

            // Create presentation
            var presentation = new VerifiablePresentation
            {
                Context = new List<object> { "https://www.w3.org/2018/credentials/v1" },
                Type = new List<string> { "VerifiablePresentation" },
                Holder = request.HolderDid,
                VerifiableCredential = validatedCredentials.ToArray()
            };

            // Add additional contexts
            foreach (var context in request.AdditionalContexts)
            {
                presentation.Context.Add(context);
            }

            // Add additional types
            foreach (var type in request.AdditionalTypes)
            {
                presentation.Type.Add(type);
            }

            // Set optional properties
            if (!string.IsNullOrEmpty(request.PresentationId))
            {
                presentation.Id = request.PresentationId;
            }

            if (!string.IsNullOrEmpty(request.Audience))
            {
                presentation.Audience = request.Audience;
            }

            if (!string.IsNullOrEmpty(request.Challenge))
            {
                presentation.Challenge = request.Challenge;
            }

            if (!string.IsNullOrEmpty(request.Domain))
            {
                presentation.Domain = request.Domain;
            }

            // Create proof
            var jsonLd = JsonSerializer.Serialize(presentation);
            var verificationMethod = holderDidDocument.DidDocument.VerificationMethods?.FirstOrDefault();
            if (verificationMethod == null)
                throw new InvalidOperationException("No verification method found in holder document");

            var privateKey = await _keyProvider.GetPrivateKeyAsync(request.HolderDid);
            var proof = await _proofService.CreateProofAsync(
                jsonLd,
                verificationMethod.Id,
                privateKey,
                request.ProofPurpose ?? "authentication",
                "Ed25519Signature2020",
                DateTimeOffset.UtcNow.ToString("o"));

            presentation.Proof = proof as Proof;

            _logger.LogInformation("Successfully created Verifiable Presentation for {HolderDid}", request.HolderDid);

            return presentation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Verifiable Presentation for {HolderDid}", request.HolderDid);
            throw;
        }
    }

    public async Task<VerifiablePresentation> CreateSelectiveDisclosurePresentationAsync(SelectiveDisclosureRequest request)
    {
        try
        {
            _logger.LogInformation("Creating selective disclosure presentation for holder {HolderDid}", request.HolderDid);

            // Create derived credentials with selective disclosure
            var disclosedCredentials = new List<VerifiableCredential>();

            foreach (var credential in request.Credentials)
            {
                var credentialId = credential.Id ?? Guid.NewGuid().ToString();
                
                if (request.FieldsToDisclose.TryGetValue(credentialId, out var fieldsToDisclose))
                {
                    // Create derived credential with only disclosed fields
                    var derivedCredential = await CreateDerivedCredentialAsync(credential, fieldsToDisclose);
                    disclosedCredentials.Add(derivedCredential);
                }
                else
                {
                    // Include full credential if no selective disclosure specified
                    disclosedCredentials.Add(credential);
                }
            }

            // Create base presentation request
            var baseRequest = new PresentationCreateRequest
            {
                HolderDid = request.HolderDid,
                Credentials = disclosedCredentials,
                Challenge = request.Challenge,
                Domain = request.Domain,
                ProofPurpose = request.ProofPurpose,
                PresentationId = request.PresentationId,
                Audience = request.Audience,
                AdditionalContexts = request.AdditionalContexts,
                AdditionalTypes = request.AdditionalTypes,
                IncludeCredentialStatus = request.IncludeCredentialStatus,
                Metadata = request.Metadata
            };

            // Add ZKP context if using zero-knowledge proofs
            if (request.UseZeroKnowledgeProofs)
            {
                baseRequest.AdditionalContexts.Add("https://w3id.org/security/bbs/v1");
                baseRequest.AdditionalTypes.Add("BbsBlsSignature2020");
            }

            return await CreatePresentationAsync(baseRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create selective disclosure presentation for {HolderDid}", request.HolderDid);
            throw;
        }
    }

    public async Task<PresentationValidationResult> ValidatePresentationAsync(PresentationValidationRequest request)
    {
        try
        {
            _logger.LogInformation("Validating Verifiable Presentation {PresentationId}", request.Presentation.Id);

            var result = new PresentationValidationResult
            {
                HolderDid = request.Presentation.Holder,
                Metadata = new Dictionary<string, object>
                {
                    ["validatedAt"] = DateTime.UtcNow,
                    ["presentationId"] = request.Presentation.Id ?? "unknown"
                }
            };

            // Validate presentation structure
            if (string.IsNullOrEmpty(request.Presentation.Holder))
            {
                result.Errors.Add("Presentation holder is required");
            }

            if (request.Presentation.VerifiableCredential == null)
            {
                result.Errors.Add("Presentation must contain at least one verifiable credential");
            }

            // Validate challenge if provided
            if (!string.IsNullOrEmpty(request.ExpectedChallenge))
            {
                if (request.Presentation.Challenge != request.ExpectedChallenge)
                {
                    result.Errors.Add("Challenge mismatch");
                }
            }

            // Validate domain if provided
            if (!string.IsNullOrEmpty(request.ExpectedDomain))
            {
                if (request.Presentation.Domain != request.ExpectedDomain)
                {
                    result.Errors.Add("Domain mismatch");
                }
            }

            // Validate presentation proof
            if (request.Presentation.Proof == null)
            {
                result.Errors.Add("Presentation proof is required");
            }
            else
            {
                try
                {
                    // Resolve holder DID
                    var holderDidDocument = await _didResolver.ResolveAsync(request.Presentation.Holder);
                    if (holderDidDocument == null)
                    {
                        result.Errors.Add($"Could not resolve holder DID: {request.Presentation.Holder}");
                    }
                    else
                    {
                        // Verify presentation proof
                        var jsonLd = JsonSerializer.Serialize(request.Presentation);
                        var verificationMethod = holderDidDocument.DidDocument.VerificationMethods?.FirstOrDefault();
                        if (verificationMethod == null)
                        {
                            result.Errors.Add("No verification method found in holder document");
                            return result;
                        }
                        
                        var proofObj = request.Presentation.Proof;
                        var proofType = proofObj is Proof proof ? proof.Type : 
                                       proofObj is Dictionary<string, object> dict ? dict.GetValueOrDefault("type")?.ToString() :
                                       "Ed25519Signature2020";
                        var proofPurpose = proofObj is Proof proof2 ? proof2.ProofPurpose :
                                          proofObj is Dictionary<string, object> dict2 ? dict2.GetValueOrDefault("proofPurpose")?.ToString() :
                                          "authentication";
                        
                        var proofValid = await _proofService.VerifyProofAsync(
                            jsonLd,
                            proofObj,
                            new byte[0], // Public key would be extracted from verification method
                            proofType ?? "Ed25519Signature2020",
                            proofPurpose ?? "authentication");

                        if (!proofValid)
                        {
                            result.Errors.Add("Presentation proof verification failed");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to verify presentation proof");
                    result.Errors.Add("Presentation proof verification failed");
                }
            }

            // Validate each credential
            if (request.Presentation.VerifiableCredential != null)
            {
                var credentials = GetCredentialsFromPresentation(request.Presentation);
                
                foreach (var credential in credentials)
                {
                    try
                    {
                        var credentialJson = JsonSerializer.Serialize(credential);
                        var credentialValidation = await _vcValidator.ValidateAsync(credentialJson);
                        
                        if (!credentialValidation.IsValid)
                        {
                            result.Errors.AddRange(credentialValidation.Errors);
                        }
                        else
                        {
                            result.ValidatedCredentials.Add(credential);
                        }

                        // Check revocation status if requested
                        if (request.CheckRevocationStatus && credential.CredentialStatus != null)
                        {
                            var isRevoked = await _credentialStatusService.IsRevokedAsync(credential.Id, 0);
                            if (isRevoked)
                            {
                                result.Errors.Add($"Credential {credential.Id} is revoked");
                            }
                        }

                        // Check issuer trust if requested
                        if (request.CheckIssuerTrust && request.TrustedIssuers.Any())
                        {
                            if (!request.TrustedIssuers.Contains(credential.Issuer))
                            {
                                result.Warnings.Add($"Issuer {credential.Issuer} is not in trusted list");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to validate credential {CredentialId}", credential.Id);
                        result.Errors.Add($"Failed to validate credential {credential.Id}");
                    }
                }
            }

            // Extract claims
            result.ExtractedClaims = await ExtractClaimsAsync(request.Presentation);

            // Determine overall validity
            result.IsValid = !result.Errors.Any();

            _logger.LogInformation("Presentation validation completed. Valid: {IsValid}, Errors: {ErrorCount}", 
                result.IsValid, result.Errors.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate Verifiable Presentation");
            throw;
        }
    }

    public async Task<BatchPresentationValidationResult> ValidatePresentationsBatchAsync(BatchPresentationValidationRequest request)
    {
        try
        {
            _logger.LogInformation("Starting batch validation of {Count} presentations", request.Presentations.Count);

            var startTime = DateTime.UtcNow;
            var results = new List<PresentationValidationResult>();

            foreach (var presentation in request.Presentations)
            {
                var presentationId = presentation.Id ?? Guid.NewGuid().ToString();
                
                // Get specific options for this presentation or use common options
                var options = request.IndividualOptions.TryGetValue(presentationId, out var specificOptions) 
                    ? specificOptions 
                    : request.CommonOptions;

                options.Presentation = presentation;

                var result = await ValidatePresentationAsync(options);
                results.Add(result);
            }

            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            var summary = new BatchValidationSummary
            {
                TotalPresentations = results.Count,
                ValidPresentations = results.Count(r => r.IsValid),
                InvalidPresentations = results.Count(r => !r.IsValid),
                TotalErrors = results.Sum(r => r.Errors.Count),
                TotalWarnings = results.Sum(r => r.Warnings.Count),
                ValidationDuration = duration
            };

            var batchResult = new BatchPresentationValidationResult
            {
                Results = results,
                Summary = summary,
                IsValid = results.All(r => r.IsValid)
            };

            _logger.LogInformation("Batch validation completed. Valid: {ValidCount}/{TotalCount}, Duration: {Duration}ms",
                summary.ValidPresentations, summary.TotalPresentations, duration.TotalMilliseconds);

            return batchResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to perform batch presentation validation");
            throw;
        }
    }

    public async Task<VerifiablePresentation> CreateZkpPresentationAsync(ZkpPresentationRequest request)
    {
        try
        {
            _logger.LogInformation("Creating ZKP Verifiable Presentation for holder {HolderDid}", request.HolderDid);

            // This is a placeholder implementation
            // In a real implementation, you would integrate with a ZKP library like BBS+
            // For now, we'll create a standard presentation with ZKP metadata

            var baseRequest = new PresentationCreateRequest
            {
                HolderDid = request.HolderDid,
                Credentials = request.Credentials,
                Challenge = request.Challenge,
                Domain = request.Domain,
                ProofPurpose = request.ProofPurpose,
                PresentationId = request.PresentationId,
                Audience = request.Audience,
                AdditionalContexts = new List<string> { "https://w3id.org/security/bbs/v1" },
                AdditionalTypes = new List<string> { "BbsBlsSignature2020" },
                IncludeCredentialStatus = request.IncludeCredentialStatus,
                Metadata = request.Metadata
            };

            var presentation = await CreatePresentationAsync(baseRequest);

            // Add ZKP-specific metadata
            presentation.Metadata = presentation.Metadata ?? new Dictionary<string, object>();
            presentation.Metadata["zkp"] = new Dictionary<string, object>
            {
                ["proofType"] = request.ZkpOptions.ProofType,
                ["hiddenFields"] = request.HiddenFields,
                ["revealedFields"] = request.RevealedFields,
                ["parameters"] = request.ZkpOptions.Parameters
            };

            _logger.LogInformation("Successfully created ZKP Verifiable Presentation for {HolderDid}", request.HolderDid);

            return presentation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create ZKP Verifiable Presentation for {HolderDid}", request.HolderDid);
            throw;
        }
    }

    public async Task<Dictionary<string, object>> ExtractClaimsAsync(VerifiablePresentation presentation)
    {
        try
        {
            var claims = new Dictionary<string, object>();

            // Add presentation-level claims
            claims["presentationId"] = presentation.Id ?? "unknown";
            claims["holder"] = presentation.Holder;
            claims["audience"] = presentation.Audience;
            claims["challenge"] = presentation.Challenge;
            claims["domain"] = presentation.Domain;

            // Extract claims from credentials
            if (presentation.VerifiableCredential != null)
            {
                var credentials = GetCredentialsFromPresentation(presentation);
                
                foreach (var credential in credentials)
                {
                    if (credential.CredentialSubject is Dictionary<string, object> credentialSubject)
                    {
                        foreach (var (key, value) in credentialSubject)
                        {
                            if (key != "id") // Skip the subject ID
                            {
                                claims[$"credential.{key}"] = value;
                            }
                        }
                    }

                    // Add credential metadata
                    claims[$"credential.{credential.Id}.issuer"] = credential.Issuer;
                    claims[$"credential.{credential.Id}.type"] = credential.Type;
                    claims[$"credential.{credential.Id}.issuedAt"] = credential.IssuanceDate;
                    claims[$"credential.{credential.Id}.expiresAt"] = credential.ExpirationDate;
                }
            }

            return claims;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract claims from Verifiable Presentation");
            throw;
        }
    }

    public async Task<VerifiablePresentation> CreateChallengeResponsePresentationAsync(ChallengeResponseRequest request)
    {
        try
        {
            _logger.LogInformation("Creating challenge-response presentation for holder {HolderDid}", request.HolderDid);

            // Validate challenge expiration
            if (request.ChallengeExpiration.HasValue && request.ChallengeExpiration.Value < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Challenge has expired");
            }

            var baseRequest = new PresentationCreateRequest
            {
                HolderDid = request.HolderDid,
                Credentials = request.Credentials,
                Challenge = request.Challenge,
                Domain = request.Domain,
                ProofPurpose = "authentication",
                PresentationId = request.PresentationId,
                Audience = request.Audience,
                AdditionalContexts = request.AdditionalContexts,
                AdditionalTypes = request.AdditionalTypes,
                IncludeCredentialStatus = request.IncludeCredentialStatus,
                Metadata = request.Metadata
            };

            // Add nonce to metadata if provided
            if (!string.IsNullOrEmpty(request.Nonce))
            {
                baseRequest.Metadata["nonce"] = request.Nonce;
            }

            var presentation = await CreatePresentationAsync(baseRequest);

            _logger.LogInformation("Successfully created challenge-response presentation for {HolderDid}", request.HolderDid);

            return presentation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create challenge-response presentation for {HolderDid}", request.HolderDid);
            throw;
        }
    }

    public async Task<bool> ValidateChallengeAsync(VerifiablePresentation presentation, string expectedChallenge)
    {
        try
        {
            if (string.IsNullOrEmpty(expectedChallenge))
            {
                return true; // No challenge expected
            }

            return presentation.Challenge == expectedChallenge;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate challenge");
            return false;
        }
    }

    public async Task<VerifiablePresentation> CreateDomainBoundPresentationAsync(DomainBindingRequest request)
    {
        try
        {
            _logger.LogInformation("Creating domain-bound presentation for holder {HolderDid} and domain {Domain}", 
                request.HolderDid, request.Domain);

            // Validate domain ownership if requested
            if (request.ValidateDomainOwnership)
            {
                var isValidDomain = await ValidateDomainOwnershipAsync(request.HolderDid, request.Domain);
                if (!isValidDomain)
                {
                    throw new InvalidOperationException($"Domain {request.Domain} is not owned by {request.HolderDid}");
                }
            }

            var baseRequest = new PresentationCreateRequest
            {
                HolderDid = request.HolderDid,
                Credentials = request.Credentials,
                Challenge = request.Challenge,
                Domain = request.Domain,
                ProofPurpose = request.ProofPurpose,
                PresentationId = request.PresentationId,
                Audience = request.Audience,
                AdditionalContexts = request.AdditionalContexts,
                AdditionalTypes = request.AdditionalTypes,
                IncludeCredentialStatus = request.IncludeCredentialStatus,
                Metadata = request.Metadata
            };

            var presentation = await CreatePresentationAsync(baseRequest);

            _logger.LogInformation("Successfully created domain-bound presentation for {HolderDid} and domain {Domain}", 
                request.HolderDid, request.Domain);

            return presentation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create domain-bound presentation for {HolderDid} and domain {Domain}", 
                request.HolderDid, request.Domain);
            throw;
        }
    }

    public async Task<bool> ValidateDomainBindingAsync(VerifiablePresentation presentation, string expectedDomain)
    {
        try
        {
            if (string.IsNullOrEmpty(expectedDomain))
            {
                return true; // No domain expected
            }

            return presentation.Domain == expectedDomain;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate domain binding");
            return false;
        }
    }

    private async Task<VerifiableCredential> CreateDerivedCredentialAsync(VerifiableCredential originalCredential, List<string> fieldsToDisclose)
    {
        try
        {
            var derivedCredential = new VerifiableCredential
            {
                Context = originalCredential.Context,
                Type = originalCredential.Type,
                Issuer = originalCredential.Issuer,
                IssuanceDate = originalCredential.IssuanceDate,
                ExpirationDate = originalCredential.ExpirationDate,
                Id = originalCredential.Id,
                CredentialSubject = new Dictionary<string, object>()
            };

            // Always include the subject ID
            if (originalCredential.CredentialSubject is Dictionary<string, object> originalSubject && 
                originalSubject.ContainsKey("id"))
            {
                if (derivedCredential.CredentialSubject is Dictionary<string, object> derivedSubject)
                {
                    derivedSubject["id"] = originalSubject["id"];
                }
            }

            // Include only the specified fields
            if (originalCredential.CredentialSubject is Dictionary<string, object> originalSubjectDict &&
                derivedCredential.CredentialSubject is Dictionary<string, object> derivedSubjectDict)
            {
                foreach (var field in fieldsToDisclose)
                {
                    if (originalSubjectDict.ContainsKey(field))
                    {
                        derivedSubjectDict[field] = originalSubjectDict[field];
                    }
                }
            }

            // Copy other properties
            derivedCredential.CredentialStatus = originalCredential.CredentialStatus;
            derivedCredential.Evidence = originalCredential.Evidence;
            derivedCredential.TermsOfUse = originalCredential.TermsOfUse;

            return derivedCredential;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create derived credential");
            throw;
        }
    }

    private List<VerifiableCredential> GetCredentialsFromPresentation(VerifiablePresentation presentation)
    {
        var credentials = new List<VerifiableCredential>();

        if (presentation.VerifiableCredential != null)
        {
            if (presentation.VerifiableCredential is VerifiableCredential singleCredential)
            {
                credentials.Add(singleCredential);
            }
            else if (presentation.VerifiableCredential is VerifiableCredential[] credentialArray)
            {
                credentials.AddRange(credentialArray);
            }
            else if (presentation.VerifiableCredential is List<VerifiableCredential> credentialList)
            {
                credentials.AddRange(credentialList);
            }
        }

        return credentials;
    }

    private async Task<bool> ValidateDomainOwnershipAsync(string holderDid, string domain)
    {
        try
        {
            // This is a placeholder implementation
            // In a real implementation, you would check DNS records, domain verification, etc.
            
            // For did:web, check if the domain matches the DID
            if (holderDid.StartsWith("did:web:"))
            {
                var didDomain = holderDid.Substring("did:web:".Length);
                return didDomain.Equals(domain, StringComparison.OrdinalIgnoreCase);
            }

            // For other DID methods, you might check domain verification records
            // stored in the DID document or other verification mechanisms
            
            return true; // Placeholder - always return true for now
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate domain ownership for {HolderDid} and {Domain}", holderDid, domain);
            return false;
        }
    }
}



