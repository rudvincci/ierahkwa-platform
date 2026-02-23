using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Validation;
using Mamey.Auth.DecentralizedIdentifiers.VC;

namespace Mamey.Auth.DecentralizedIdentifiers.Services
{
    /// <summary>
    /// Production implementation of <see cref="IVerifiableCredentialValidator"/>.
    /// Handles both JWT and Linked Data Proof VCs and VPs.
    /// </summary>
    public class VerifiableCredentialValidator : IVerifiableCredentialValidator
    {
        private readonly IDidResolver _didResolver;
        private readonly ICredentialStatusService _statusService;
        private readonly ILogger<VerifiableCredentialValidator> _logger;
        private readonly List<string> _trustedIssuers;
        private readonly IProofService _proofService;

        /// <summary>
        /// Creates a new validator with all dependencies.
        /// </summary>
        public VerifiableCredentialValidator(
            IDidResolver didResolver,
            ICredentialStatusService statusService,
            ILogger<VerifiableCredentialValidator> logger,
            IProofService proofService,
            IEnumerable<string> trustedIssuers = null)
        {
            _didResolver = didResolver ?? throw new ArgumentNullException(nameof(didResolver));
            _statusService = statusService ?? throw new ArgumentNullException(nameof(statusService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _proofService = proofService ?? throw new ArgumentNullException(nameof(proofService));
            _trustedIssuers = trustedIssuers != null ? new List<string>(trustedIssuers) : new List<string>();
        }

        /// <inheritdoc />
        public async Task<VerifiableCredentialValidationResult> ValidateAsync(
            string vcOrVpToken,
            CancellationToken cancellationToken = default)
        {
            if (IsJwt(vcOrVpToken))
                return await ValidateJwtAsync(vcOrVpToken, null, cancellationToken);

            return await ValidateLinkedDataProofAsync(vcOrVpToken, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<VerifiableCredentialValidationResult> ValidateAsync(
            CredentialVerifyRequest request,
            CancellationToken cancellationToken = default)
        {
            if (IsJwt(request.CredentialJwt))
                return await ValidateJwtAsync(request.CredentialJwt, request, cancellationToken);

            return await ValidateLinkedDataProofAsync(request.CredentialJwt, request, cancellationToken);
        }

        public Task<VerifiableCredentialValidationResult> ValidateAsync(VerifiableCredential credential)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines if the input is a JWT token.
        /// </summary>
        private static bool IsJwt(string token) => token?.Split('.').Length == 3;

        private async Task<VerifiableCredentialValidationResult> ValidateJwtAsync(
            string jwtToken,
            CredentialVerifyRequest request,
            CancellationToken ct)
        {
            var result = new VerifiableCredentialValidationResult();
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jwt = handler.ReadJwtToken(jwtToken);
                string issuerDid = jwt.Issuer;

                // 1. Resolve DID Document for issuer
                var didResolution = await _didResolver.ResolveAsync(issuerDid, ct);
                var didDoc = didResolution.DidDocument ?? throw new InvalidOperationException("DID Document could not be resolved for issuer.");

                // 2. Lookup verification method for authentication or assertionMethod
                string kid = jwt.Header.Kid;
                var verificationMethod = FindVerificationMethod(didDoc, kid, "assertionMethod") 
                    ?? FindVerificationMethod(didDoc, kid, "authentication");
                if (verificationMethod == null)
                    throw new InvalidOperationException($"Verification method '{kid}' not found for assertion or authentication.");

                // 3. Build SecurityKey from verification method (implement per key type)
                var securityKey = verificationMethod.ToSecurityKey();

                // 4. Validate JWT signature
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuerDid,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = securityKey,
                };

                ClaimsPrincipal principal = handler.ValidateToken(jwtToken, parameters, out _);

                result.IsValid = true;
                result.CredentialId = jwt.Id;
                result.IssuerDid = issuerDid;
                result.SubjectDid = jwt.Subject;
                result.CredentialType = principal.FindFirst("vc:type")?.Value;
                result.Claims = new List<Claim>(principal.Claims);

                // 5. (Optional) Check status/revocation
                if (jwt.Payload.TryGetValue("credentialStatus", out var statusObj) && statusObj is JsonElement je)
                {
                    string statusId = je.GetProperty("id").GetString();
                    var statusResult = await _statusService.CheckStatusAsync(statusId, ct);
                    result.IsRevoked = statusResult.IsRevoked;
                    result.RevocationReason = statusResult.Reason;
                    if (result.IsRevoked) result.Errors.Add("Credential is revoked.");
                }

                // 6. (Optional) Challenge check
                if (request != null && !string.IsNullOrWhiteSpace(request.Challenge))
                {
                    var challengeClaim = principal.FindFirst("challenge")?.Value;
                    if (challengeClaim != request.Challenge)
                    {
                        result.IsValid = false;
                        result.Errors.Add("Challenge does not match.");
                    }
                }

                // 7. (Optional) Trust anchor check
                if (_trustedIssuers.Count > 0 && !_trustedIssuers.Contains(issuerDid))
                {
                    result.IsValid = false;
                    result.Errors.Add("Issuer is not trusted.");
                }
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "JWT verification failed.");
                result.IsValid = false;
                result.Errors.Add("Signature or claim validation failed: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception during JWT credential validation.");
                result.IsValid = false;
                result.Errors.Add("Exception: " + ex.Message);
            }

            return result;
        }

        private async Task<VerifiableCredentialValidationResult> ValidateLinkedDataProofAsync(
            string ldProofJson,
            CredentialVerifyRequest request,
            CancellationToken cancellationToken)
        {
            var result = new VerifiableCredentialValidationResult();

            try
            {
                using var doc = JsonDocument.Parse(ldProofJson);
                var root = doc.RootElement;

                var issuerDid = root.GetProperty("issuer").GetString();

                // 1. Resolve DID Document
                var didResolution = await _didResolver.ResolveAsync(issuerDid, cancellationToken);
                var didDoc = didResolution.DidDocument ?? throw new InvalidOperationException("DID Document could not be resolved for issuer.");

                // 2. Extract proof section
                var proof = root.GetProperty("proof");
                var verificationMethodId = proof.GetProperty("verificationMethod").GetString();

                // 3. Lookup verification method for assertionMethod
                var verificationMethod = FindVerificationMethod(didDoc, verificationMethodId, "assertionMethod")
                    ?? FindVerificationMethod(didDoc, verificationMethodId, "authentication");
                if (verificationMethod == null)
                    throw new InvalidOperationException($"Verification method '{verificationMethodId}' not found for assertion or authentication.");

                // 4. Verify Linked Data Proof (uses your canonicalization & IProofService)
                bool proofValid = await LinkedDataProofValidator.Verify(
                    root,
                    proof,
                    verificationMethodId,
                    _proofService,
                    _didResolver,
                    cancellationToken);
                if (!proofValid)
                {
                    result.IsValid = false;
                    result.Errors.Add("Linked Data Proof signature is invalid.");
                    return result;
                }

                result.IsValid = true;
                result.IssuerDid = issuerDid;
                result.SubjectDid = root.GetProperty("credentialSubject").GetProperty("id").GetString();
                result.CredentialType = root.GetProperty("type").ToString();
                // Parse and set claims as needed

                // 5. (Optional) Status/Revocation
                if (root.TryGetProperty("credentialStatus", out var statusObj))
                {
                    string statusId = statusObj.GetProperty("id").GetString();
                    var statusResult = await _statusService.CheckStatusAsync(statusId, cancellationToken);
                    result.IsRevoked = statusResult.IsRevoked;
                    result.RevocationReason = statusResult.Reason;
                    if (result.IsRevoked) result.Errors.Add("Credential is revoked.");
                }

                // 6. (Optional) Challenge
                if (request != null && !string.IsNullOrWhiteSpace(request.Challenge))
                {
                    var challenge = proof.TryGetProperty("challenge", out var ch) ? ch.GetString() : null;
                    if (challenge != request.Challenge)
                    {
                        result.IsValid = false;
                        result.Errors.Add("Challenge does not match.");
                    }
                }

                // 7. (Optional) Trust anchor check
                if (_trustedIssuers.Count > 0 && !_trustedIssuers.Contains(issuerDid))
                {
                    result.IsValid = false;
                    result.Errors.Add("Issuer is not trusted.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during LD-Proof credential validation.");
                result.IsValid = false;
                result.Errors.Add("Exception: " + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Finds a verification method for the given key ID and relationship in the DID Document.
        /// </summary>
        private static IDidVerificationMethod FindVerificationMethod(IDidDocument doc, string keyId, string relationship)
        {
            if (string.IsNullOrWhiteSpace(keyId)) return null;
            // Normalize: DID fragment or full DID#fragment allowed
            string searchId = keyId.Contains(':') ? keyId : $"{doc.Id}{(keyId.StartsWith("#") ? "" : "#")}{keyId}";

            IReadOnlyList<object> relationshipRefs = relationship switch
            {
                "authentication" => doc.Authentication,
                "assertionMethod" => doc.AssertionMethod,
                "keyAgreement" => doc.KeyAgreement,
                "capabilityDelegation" => doc.CapabilityDelegation,
                "capabilityInvocation" => doc.CapabilityInvocation,
                _ => null
            };

            if (relationshipRefs != null)
            {
                foreach (var entry in relationshipRefs)
                {
                    if (entry is IDidVerificationMethod m && m.Id == searchId)
                        return m;
                    if (entry is string refId && refId == searchId)
                        return doc.VerificationMethods?.FirstOrDefault(vm => vm.Id == searchId);
                }
            }
            // Fallback: search all methods
            return doc.VerificationMethods?.FirstOrDefault(vm => vm.Id == searchId);
        }
    }
}
