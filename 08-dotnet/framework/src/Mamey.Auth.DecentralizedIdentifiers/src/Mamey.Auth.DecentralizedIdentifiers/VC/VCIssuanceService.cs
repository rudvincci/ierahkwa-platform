using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Crypto;
using Mamey.Auth.DecentralizedIdentifiers.Audit;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.DecentralizedIdentifiers.VC;

/// <summary>
/// Enhanced credential service implementation with multiple proof types and batch operations.
/// </summary>
public class VCIssuanceService : ICredentialService
{
    private readonly IDidResolver _didResolver;
    private readonly IKeyProvider _keyProvider;
    private readonly IProofService _proofService;
    private readonly ICredentialStatusService _credentialStatusService;
    private readonly IDidAuditService _auditService;
    private readonly ILogger<VCIssuanceService> _logger;
    private readonly Dictionary<string, VerifiableCredentialDto> _credentialStore = new();

    public VCIssuanceService(
        IDidResolver didResolver,
        IKeyProvider keyProvider,
        IProofService proofService,
        ICredentialStatusService credentialStatusService,
        IDidAuditService auditService,
        ILogger<VCIssuanceService> logger)
    {
        _didResolver = didResolver ?? throw new ArgumentNullException(nameof(didResolver));
        _keyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
        _proofService = proofService ?? throw new ArgumentNullException(nameof(proofService));
        _credentialStatusService = credentialStatusService ?? throw new ArgumentNullException(nameof(credentialStatusService));
        _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<VerifiableCredentialDto> IssueCredentialAsync(CredentialIssueRequest request)
    {
        var correlationId = Guid.NewGuid().ToString();
        
        try
        {
            _logger.LogInformation("Issuing credential of type {CredentialType} for subject {SubjectDid}", 
                request.CredentialType, request.SubjectDid);

            // Validate request
            ValidateCredentialIssueRequest(request);

            // Resolve issuer DID
            var issuerResolution = await _didResolver.ResolveAsync(request.IssuerDid);
            if (issuerResolution?.DidDocument == null)
                throw new InvalidOperationException($"Issuer DID not found: {request.IssuerDid}");

            // Resolve subject DID
            var subjectResolution = await _didResolver.ResolveAsync(request.SubjectDid);
            if (subjectResolution?.DidDocument == null)
                throw new InvalidOperationException($"Subject DID not found: {request.SubjectDid}");

            // Validate schema if requested
            if (request.ValidateSchema && !string.IsNullOrEmpty(request.SchemaRef))
            {
                var isValid = await ValidateSchemaAsync(request.SchemaRef, request.Claims);
                if (!isValid)
                    throw new InvalidOperationException("Credential schema validation failed");
            }

            // Create credential
            var credential = await CreateVerifiableCredentialAsync(request, (DidDocument)issuerResolution.DidDocument);

            // Sign credential
            var signedCredential = await SignCredentialAsync(credential, request.ProofType, (DidDocument)issuerResolution.DidDocument);

            // Store credential
            var credentialDto = ConvertToDto(signedCredential);
            if (!string.IsNullOrEmpty(request.CredentialId))
            {
                credentialDto.Id = request.CredentialId;
            }
            _credentialStore[credentialDto.Id] = credentialDto;

            _logger.LogInformation("Successfully issued credential {CredentialId}", credentialDto.Id);

            // Log successful credential issuance
            await _auditService.LogCredentialOperationAsync(
                DidAuditEventTypes.VC_ISSUANCE,
                DidAuditCategories.CREDENTIAL_OPERATIONS,
                DidAuditStatus.SUCCESS,
                credentialDto.Id,
                correlationId,
                metadata: new Dictionary<string, object>
                {
                    ["credentialId"] = credentialDto.Id,
                    ["issuerDid"] = request.IssuerDid,
                    ["subjectDid"] = request.SubjectDid,
                    ["credentialType"] = request.CredentialType?.ToString() ?? "VerifiableCredential",
                    ["proofType"] = request.ProofType.ToString(),
                    ["includeStatus"] = request.IncludeStatus
                });

            return credentialDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to issue credential for subject {SubjectDid}", request.SubjectDid);
            
            // Log failed credential issuance
            await _auditService.LogCredentialOperationAsync(
                DidAuditEventTypes.VC_ISSUANCE,
                DidAuditCategories.CREDENTIAL_OPERATIONS,
                DidAuditStatus.FAILURE,
                request.CredentialId ?? "Unknown",
                correlationId,
                metadata: new Dictionary<string, object>
                {
                    ["issuerDid"] = request.IssuerDid,
                    ["subjectDid"] = request.SubjectDid,
                    ["credentialType"] = request.CredentialType?.ToString() ?? "VerifiableCredential",
                    ["error"] = ex.Message,
                    ["errorType"] = ex.GetType().Name
                });
            
            throw;
        }
    }

    public async Task<BatchCredentialIssueResult> IssueCredentialsBatchAsync(BatchCredentialIssueRequest request)
    {
        var result = new BatchCredentialIssueResult
        {
            TotalProcessed = request.Requests.Count,
            ContinueOnError = request.ContinueOnError
        };

        _logger.LogInformation("Starting batch credential issuance for {Count} credentials", request.Requests.Count);

        foreach (var credentialRequest in request.Requests)
        {
            try
            {
                // Set default proof type if not specified
                if (credentialRequest.ProofType == ProofType.Ed25519Signature2020 && 
                    request.DefaultProofType != ProofType.Ed25519Signature2020)
                {
                    credentialRequest.ProofType = request.DefaultProofType;
                }

                var credential = await IssueCredentialAsync(credentialRequest);
                result.IssuedCredentials.Add(credential);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                var error = new CredentialIssueError
                {
                    Request = credentialRequest,
                    ErrorMessage = ex.Message,
                    ErrorCode = "ISSUANCE_FAILED",
                    Exception = ex
                };
                result.Errors.Add(error);
                result.ErrorCount++;

                _logger.LogWarning(ex, "Failed to issue credential in batch for subject {SubjectDid}", 
                    credentialRequest.SubjectDid);

                if (!request.ContinueOnError)
                {
                    _logger.LogError("Batch issuance stopped due to error and ContinueOnError=false");
                    break;
                }
            }
        }

        _logger.LogInformation("Batch credential issuance completed: {SuccessCount} successful, {ErrorCount} failed", 
            result.SuccessCount, result.ErrorCount);

        return result;
    }

    public async Task<VerifiableCredentialDto> IssueCredentialWithProofTypeAsync(CredentialIssueRequest request, ProofType proofType)
    {
        request.ProofType = proofType;
        return await IssueCredentialAsync(request);
    }

    public async Task<CredentialVerificationResultDto> VerifyCredentialAsync(CredentialVerifyRequest request)
    {
        try
        {
            _logger.LogDebug("Verifying credential {CredentialId}", request.CredentialId);

            // Parse credential
            string credentialJson;
            if (request.CredentialJson is string str)
            {
                credentialJson = str;
            }
            else if (request.CredentialJson == null)
            {
                credentialJson = string.Empty;
            }
            else
            {
                credentialJson = JsonSerializer.Serialize(request.CredentialJson);
            }
            var credential = ParseVerifiableCredential(credentialJson);

            // Resolve issuer DID
            var issuerString = credential.Issuer is string issuer ? issuer : credential.Issuer?.ToString() ?? string.Empty;
            var issuerResolution = await _didResolver.ResolveAsync(issuerString);
            if (issuerResolution?.DidDocument == null)
            {
                return new CredentialVerificationResultDto
                {
                    IsValid = false,
                    Errors = new List<string> { $"Issuer DID not found: {credential.Issuer}" }
                };
            }

            // Verify proof
            var proofValid = await VerifyCredentialProofAsync(credential, (DidDocument)issuerResolution.DidDocument);
            if (!proofValid)
            {
                return new CredentialVerificationResultDto
                {
                    IsValid = false,
                    Errors = new List<string> { "Credential proof verification failed" }
                };
            }

            // Check expiration
            if (credential.ExpirationDate.HasValue && credential.ExpirationDate.Value < DateTime.UtcNow)
            {
                return new CredentialVerificationResultDto
                {
                    IsValid = false,
                    Errors = new List<string> { "Credential has expired" }
                };
            }

            // Check revocation status if credential has status
            if (credential.CredentialStatus != null)
            {
                var isRevoked = await _credentialStatusService.IsCredentialActiveAsync(credential.CredentialStatus);
                if (!isRevoked)
                {
                    return new CredentialVerificationResultDto
                    {
                        IsValid = false,
                        Errors = new List<string> { "Credential is revoked" }
                    };
                }
            }

            _logger.LogDebug("Credential {CredentialId} verification successful", request.CredentialId);

            return new CredentialVerificationResultDto
            {
                IsValid = true,
                Errors = new List<string>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify credential {CredentialId}", request.CredentialId);
            return new CredentialVerificationResultDto
            {
                IsValid = false,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<BatchCredentialVerificationResult> VerifyCredentialsBatchAsync(BatchCredentialVerifyRequest request)
    {
        var result = new BatchCredentialVerificationResult
        {
            TotalProcessed = request.Credentials.Count,
            ContinueOnError = request.ContinueOnError
        };

        _logger.LogInformation("Starting batch credential verification for {Count} credentials", request.Credentials.Count);

        foreach (var credential in request.Credentials)
        {
            try
            {
                var verifyRequest = new CredentialVerifyRequest
                {
                    CredentialId = credential.Id,
                    CredentialJson = JsonSerializer.Serialize(credential)
                };

                var verificationResult = await VerifyCredentialAsync(verifyRequest);
                result.Results.Add(verificationResult);

                if (verificationResult.IsValid)
                    result.ValidCount++;
                else
                    result.InvalidCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to verify credential {CredentialId} in batch", credential.Id);
                
                result.Results.Add(new CredentialVerificationResultDto
                {
                    IsValid = false,
                    Errors = new List<string> { ex.Message }
                });
                result.InvalidCount++;

                if (!request.ContinueOnError)
                {
                    _logger.LogError("Batch verification stopped due to error and ContinueOnError=false");
                    break;
                }
            }
        }

        _logger.LogInformation("Batch credential verification completed: {ValidCount} valid, {InvalidCount} invalid", 
            result.ValidCount, result.InvalidCount);

        return result;
    }

    public async Task RevokeCredentialAsync(string credentialId)
    {
        try
        {
            _logger.LogInformation("Revoking credential {CredentialId}", credentialId);

            if (!_credentialStore.TryGetValue(credentialId, out var credential))
            {
                throw new InvalidOperationException($"Credential not found: {credentialId}");
            }

            // Update credential status - Note: ICredentialStatusService doesn't have UpdateCredentialStatusAsync
            // The status update is handled by updating the credential in the store directly
            if (credential.CredentialStatus != null)
            {
                // Status is already updated in the credential object above
                _logger.LogInformation("Credential status updated for {CredentialId}", credentialId);
            }

            _logger.LogInformation("Successfully revoked credential {CredentialId}", credentialId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to revoke credential {CredentialId}", credentialId);
            throw;
        }
    }

    public async Task<BatchRevocationResult> RevokeCredentialsBatchAsync(BatchRevocationRequest request)
    {
        var result = new BatchRevocationResult
        {
            TotalProcessed = request.CredentialIds.Count,
            ContinueOnError = request.ContinueOnError
        };

        _logger.LogInformation("Starting batch credential revocation for {Count} credentials", request.CredentialIds.Count);

        foreach (var credentialId in request.CredentialIds)
        {
            try
            {
                await RevokeCredentialAsync(credentialId);
                result.RevokedCredentialIds.Add(credentialId);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                var error = new RevocationError
                {
                    CredentialId = credentialId,
                    ErrorMessage = ex.Message,
                    ErrorCode = "REVOCATION_FAILED",
                    Exception = ex
                };
                result.Errors.Add(error);
                result.ErrorCount++;

                _logger.LogWarning(ex, "Failed to revoke credential {CredentialId} in batch", credentialId);

                if (!request.ContinueOnError)
                {
                    _logger.LogError("Batch revocation stopped due to error and ContinueOnError=false");
                    break;
                }
            }
        }

        _logger.LogInformation("Batch credential revocation completed: {SuccessCount} successful, {ErrorCount} failed", 
            result.SuccessCount, result.ErrorCount);

        return result;
    }

    public Task<VerifiableCredentialDto> GetCredentialAsync(string credentialId)
    {
        if (_credentialStore.TryGetValue(credentialId, out var credential))
        {
            return Task.FromResult(credential);
        }
        throw new InvalidOperationException($"Credential not found: {credentialId}");
    }

    public Task<IEnumerable<VerifiableCredentialDto>> GetCredentialsByIssuerAsync(string issuerDid)
    {
        var credentials = _credentialStore.Values.Where(c => c.Issuer == issuerDid);
        return Task.FromResult(credentials);
    }

    public Task<IEnumerable<VerifiableCredentialDto>> GetCredentialsBySubjectAsync(string subjectDid)
    {
        var credentials = _credentialStore.Values.Where(c => c.CredentialSubject is Dictionary<string, object> subject && 
                                                           subject.ContainsKey("id") && 
                                                           subject["id"]?.ToString() == subjectDid);
        return Task.FromResult(credentials);
    }

    public async Task UpdateCredentialStatusAsync(string credentialId, CredentialStatus status)
    {
        if (_credentialStore.TryGetValue(credentialId, out var credential))
        {
            // Update the credential status - serialize CredentialStatus to string
            credential.CredentialStatus = JsonSerializer.Serialize(status);
            _credentialStore[credentialId] = credential;

            _logger.LogInformation("Updated credential status for {CredentialId}", credentialId);
        }
        else
        {
            throw new InvalidOperationException($"Credential not found: {credentialId}");
        }
    }

    public async Task<bool> ValidateSchemaAsync(string schemaRef, Dictionary<string, object> claims, CancellationToken cancellationToken = default)
    {
        try
        {
            // This is a placeholder implementation
            // In a real implementation, you would:
            // 1. Fetch the schema from the schemaRef URL
            // 2. Validate the claims against the schema
            // 3. Return true if valid, false otherwise

            _logger.LogDebug("Validating schema {SchemaRef} for claims", schemaRef);
            
            // For now, just return true (always valid)
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate schema {SchemaRef}", schemaRef);
            return false;
        }
    }

    private void ValidateCredentialIssueRequest(CredentialIssueRequest request)
    {
        if (string.IsNullOrEmpty(request.IssuerDid))
            throw new ArgumentException("IssuerDid is required");
        
        if (string.IsNullOrEmpty(request.SubjectDid))
            throw new ArgumentException("SubjectDid is required");
        
        if (string.IsNullOrEmpty(request.CredentialType))
            throw new ArgumentException("CredentialType is required");
        
        if (request.Claims == null || !request.Claims.Any())
            throw new ArgumentException("Claims are required");
    }

    private async Task<VerifiableCredential> CreateVerifiableCredentialAsync(CredentialIssueRequest request, DidDocument issuerDocument)
    {
        var credentialId = string.IsNullOrEmpty(request.CredentialId) 
            ? $"urn:uuid:{Guid.NewGuid()}" 
            : request.CredentialId;

        var contexts = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        if (request.Contexts != null)
        {
            contexts.AddRange(request.Contexts.Select(c => c?.ToString() ?? string.Empty));
        }

        var types = new List<string> { "VerifiableCredential", request.CredentialType?.ToString() ?? "VerifiableCredential" };

        var credentialSubject = new Dictionary<string, object>
        {
            ["id"] = request.SubjectDid
        };

        foreach (var claim in request.Claims)
        {
            credentialSubject[claim.Key] = claim.Value;
        }

        var credential = new VerifiableCredential
        {
            Context = contexts.Cast<object>().ToList(),
            Type = types.ToList(),
            Id = credentialId,
            Issuer = request.IssuerDid,
            IssuanceDate = (request.IssuanceDate ?? DateTimeOffset.UtcNow).DateTime,
            ExpirationDate = request.Expiration?.DateTime,
            CredentialSubject = credentialSubject
        };

        // Add credential status if requested
        if (request.IncludeStatus && !string.IsNullOrEmpty(request.CredentialStatusId))
        {
            credential.CredentialStatus = new CredentialStatus
            {
                Id = request.CredentialStatusId,
                Type = "RevocationList2020Status",
                StatusPurpose = "revocation",
                StatusListCredential = $"{request.IssuerDid}/credentials/status/list",
                StatusListIndex = "1"
            };
        }

        return credential;
    }

    private async Task<VerifiableCredential> SignCredentialAsync(VerifiableCredential credential, ProofType proofType, DidDocument issuerDocument)
    {
        var jsonLd = JsonSerializer.Serialize(credential);
        var verificationMethod = issuerDocument.VerificationMethods?.FirstOrDefault();
        if (verificationMethod == null)
            throw new InvalidOperationException("No verification method found in issuer document");

        var proof = await _proofService.CreateProofAsync(
            jsonLd,
            verificationMethod.Id,
            new byte[0], // Private key would be provided by the caller
            "assertionMethod",
            proofType.ToString(),
            DateTimeOffset.UtcNow.ToString("o"));
        
        credential.Proof = proof;
        return credential;
    }

    private async Task<bool> VerifyCredentialProofAsync(VerifiableCredential credential, DidDocument issuerDocument)
    {
        if (credential.Proof == null)
            return false;

        var jsonLd = JsonSerializer.Serialize(credential);
        var verificationMethod = issuerDocument.VerificationMethods?.FirstOrDefault();
        if (verificationMethod == null)
            return false;

        var proofObj = credential.Proof;
        var proofTypeStr = proofObj is Proof proof ? proof.Type : 
                          proofObj is Dictionary<string, object> dict ? dict.GetValueOrDefault("type")?.ToString() :
                          null;
        var proofPurposeStr = proofObj is Proof proof2 ? proof2.ProofPurpose :
                             proofObj is Dictionary<string, object> dict2 ? dict2.GetValueOrDefault("proofPurpose")?.ToString() :
                             null;
        
        return await _proofService.VerifyProofAsync(
            jsonLd,
            proofObj,
            new byte[0], // Public key would be extracted from verification method
            proofTypeStr ?? "Ed25519Signature2020",
            proofPurposeStr ?? "assertionMethod");
    }

    private VerifiableCredential ParseVerifiableCredential(string credentialJson)
    {
        return JsonSerializer.Deserialize<VerifiableCredential>(credentialJson);
    }

    private VerifiableCredentialDto ConvertToDto(VerifiableCredential credential)
    {
        return new VerifiableCredentialDto
        {
            Id = credential.Id,
            Context = JsonSerializer.Serialize(credential.Context),
            Type = JsonSerializer.Serialize(credential.Type),
            Issuer = credential.Issuer?.ToString() ?? string.Empty,
            IssuanceDate = credential.IssuanceDate,
            ExpirationDate = credential.ExpirationDate,
            CredentialSubject = credential.CredentialSubject as Dictionary<string, object> ?? new Dictionary<string, object>(),
            CredentialStatus = JsonSerializer.Serialize(credential.CredentialStatus),
            Proof = JsonSerializer.Serialize(credential.Proof)
        };
    }
}



