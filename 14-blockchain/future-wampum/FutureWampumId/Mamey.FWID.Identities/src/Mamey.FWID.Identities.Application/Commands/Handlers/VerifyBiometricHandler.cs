using Mamey.Contexts;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Mappers;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Microservice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

/// <summary>
/// Handler for verifying biometric data for an identity.
/// </summary>
internal sealed class VerifyBiometricHandler : ICommandHandler<VerifyBiometric>
{
    private readonly IIdentityRepository _repository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IBiometricStorageService _storageService;
    private readonly IBiometricEvidenceService _evidenceService;
    private readonly ILedgerTransactionClient _ledgerClient;
    private readonly IContext _context;
    private readonly ILogger<VerifyBiometricHandler> _logger;

    public VerifyBiometricHandler(
        IIdentityRepository repository,
        IEventProcessor eventProcessor,
        IBiometricStorageService storageService,
        IBiometricEvidenceService evidenceService,
        ILedgerTransactionClient ledgerClient,
        IContext context,
        ILogger<VerifyBiometricHandler> logger)
    {
        _repository = repository;
        _eventProcessor = eventProcessor;
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _evidenceService = evidenceService ?? throw new ArgumentNullException(nameof(evidenceService));
        _ledgerClient = ledgerClient;
        _context = context;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(VerifyBiometric command, CancellationToken cancellationToken = default)
    {
        var identity = await _repository.GetAsync(command.IdentityId, cancellationToken);
        if (identity == null)
            throw new IdentityNotFoundException(command.IdentityId);

        // Map DTO to domain value object
        var providedBiometric = command.ProvidedBiometric.ToDomain();
        
        // Check if we need to retrieve stored biometric data from MinIO
        // This would be used if the provided biometric only contains a reference
        // For now, we use the provided biometric data directly as it contains the encrypted template

        // Default threshold per Biometric Verification Microservice spec (§4): MATCH_MIN = 0.85
        var threshold = command.Threshold ?? 0.85;
        // Extract DID from identity metadata if available (for event emission)
        var did = identity.Metadata?.ContainsKey("Did") == true 
            ? identity.Metadata["Did"]?.ToString() 
            : null;
        
        // Calculate match score first to determine decision
        var matchScore = identity.BiometricData.Match(providedBiometric);
        var decision = matchScore >= threshold ? "PASS" : "FAIL";
        
        // Create evidence JWS if not already provided (per spec §2.4)
        var evidenceJws = providedBiometric.EvidenceJws;
        if (string.IsNullOrEmpty(evidenceJws))
        {
            evidenceJws = await _evidenceService.CreateVerificationEvidenceAsync(
                command.IdentityId,
                did,
                matchScore,
                providedBiometric,
                decision,
                null,
                cancellationToken);
            
            // Update provided biometric with evidence JWS
            var updatedBiometric = new BiometricData(
                providedBiometric.Type,
                providedBiometric.EncryptedTemplate,
                providedBiometric.Hash,
                providedBiometric.TemplateId,
                providedBiometric.AlgoVersion,
                providedBiometric.Format,
                providedBiometric.Quality,
                evidenceJws,
                providedBiometric.LivenessScore,
                providedBiometric.LivenessDecision);
            
            identity.VerifyBiometric(updatedBiometric, threshold, did);
        }
        else
        {
            identity.VerifyBiometric(providedBiometric, threshold, did);
        }

        // Handle optimistic locking with retry logic
        try
        {
            await _repository.UpdateAsync(identity, cancellationToken);
            await _eventProcessor.ProcessAsync(identity.Events);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Reload entity with fresh version and retry once
            var freshIdentity = await _repository.GetAsync(command.IdentityId, cancellationToken);
            if (freshIdentity == null)
                throw new IdentityNotFoundException(command.IdentityId);

            // Re-apply the domain operation
            // Re-calculate match score with fresh identity
            var retryMatchScore = freshIdentity.BiometricData.Match(providedBiometric);
            var retryDecision = retryMatchScore >= threshold ? "PASS" : "FAIL";
            var retryDid = freshIdentity.Metadata?.ContainsKey("Did") == true 
                ? freshIdentity.Metadata["Did"]?.ToString() 
                : null;
            
            // Re-create evidence JWS if needed
            var retryEvidenceJws = providedBiometric.EvidenceJws;
            if (string.IsNullOrEmpty(retryEvidenceJws))
            {
                retryEvidenceJws = await _evidenceService.CreateVerificationEvidenceAsync(
                    command.IdentityId,
                    retryDid,
                    retryMatchScore,
                    providedBiometric,
                    retryDecision,
                    null,
                    cancellationToken);
                
                var retryUpdatedBiometric = new BiometricData(
                    providedBiometric.Type,
                    providedBiometric.EncryptedTemplate,
                    providedBiometric.Hash,
                    providedBiometric.TemplateId,
                    providedBiometric.AlgoVersion,
                    providedBiometric.Format,
                    providedBiometric.Quality,
                    retryEvidenceJws,
                    providedBiometric.LivenessScore,
                    providedBiometric.LivenessDecision);
                
                freshIdentity.VerifyBiometric(retryUpdatedBiometric, threshold, retryDid);
            }
            else
            {
                freshIdentity.VerifyBiometric(providedBiometric, threshold, retryDid);
            }

            await _repository.UpdateAsync(freshIdentity, cancellationToken);
            await _eventProcessor.ProcessAsync(freshIdentity.Events);
        }

        // Log transaction to FutureWampumLedger.Transaction
        try
        {
            var transactionRequest = new TransactionLogRequest
            {
                TransactionType = "IdentityVerified",
                EntityType = "Identity",
                EntityId = identity.Id.Value,
                Description = $"Biometric verification: {decision} (Score: {matchScore:F2})",
                Metadata = new Dictionary<string, object>
                {
                    { "MatchScore", matchScore },
                    { "Decision", decision },
                    { "Threshold", threshold },
                    { "BiometricType", providedBiometric.Type.ToString() }
                },
                Timestamp = DateTime.UtcNow,
                CorrelationId = _context.CorrelationId.ToString()
            };

            await _ledgerClient.LogTransactionAsync(transactionRequest, cancellationToken);
            _logger.LogInformation("Logged IdentityVerified transaction to ledger for IdentityId: {IdentityId}, Decision: {Decision}", identity.Id.Value, decision);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the command - ledger logging is best effort
            _logger.LogWarning(ex, "Failed to log transaction to ledger for IdentityId: {IdentityId}", identity.Id.Value);
        }
    }
}

