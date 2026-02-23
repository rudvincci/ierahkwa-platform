using Mamey.FWID.Identities.Application.Mappers;
using Mamey.Contexts;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Microservice.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

/// <summary>
/// Handler for updating biometric data for an identity.
/// Follows the pattern: NO ILogger in handlers - delegate to services.
/// </summary>
internal sealed class UpdateBiometricHandler : ICommandHandler<UpdateBiometric>
{
    private readonly IIdentityService _identityService;
    private readonly IIdentityRepository _repository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IBiometricStorageService _storageService;
    private readonly IBiometricEvidenceService _evidenceService;
    private readonly IContext _context;

    public UpdateBiometricHandler(
        IIdentityService identityService,
        IIdentityRepository repository,
        IEventProcessor eventProcessor,
        IBiometricStorageService storageService,
        IBiometricEvidenceService evidenceService,
        IContext context)
    {
        _identityService = identityService;
        _repository = repository;
        _eventProcessor = eventProcessor;
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _evidenceService = evidenceService ?? throw new ArgumentNullException(nameof(evidenceService));
        _context = context;
    }

    public async Task HandleAsync(UpdateBiometric command, CancellationToken cancellationToken = default)
    {
        var identity = await _repository.GetAsync(command.IdentityId, cancellationToken);
        if (identity == null)
            throw new IdentityNotFoundException(command.IdentityId);

        // Map DTO to domain objects
        var newBiometric = command.NewBiometric.ToDomain();
        var verificationBiometric = command.VerificationBiometric?.ToDomain();

        // Upload biometric data to MinIO if it contains actual file data
        // The encrypted template in BiometricData is stored in the database
        // If we have raw biometric data to store as files, upload it here
        string? storageReference = null;
        if (newBiometric.EncryptedTemplate != null && newBiometric.EncryptedTemplate.Length > 0)
        {
            try
            {
                // Upload the biometric data to MinIO
                storageReference = await _storageService.UploadBiometricAsync(
                    command.IdentityId,
                    newBiometric.Type,
                    newBiometric.EncryptedTemplate,
                    cancellationToken);

                // Store the storage reference in metadata for future retrieval
                // Metadata is initialized in constructor, so it should never be null
                if (identity.Metadata != null)
                {
                    identity.Metadata[$"BiometricStorage_{newBiometric.Type}"] = storageReference;
                }
            }
            catch (Exception)
            {
                // Continue without MinIO storage - data will be stored in database only
                // Logging is handled by the storage service
            }
        }

        // Extract DID from identity metadata if available (for event emission)
        var did = identity.Metadata?.ContainsKey("Did") == true 
            ? identity.Metadata["Did"]?.ToString() 
            : null;
        // Determine if this is an enrollment (has template ID) or re-enrollment
        var isEnrollment = !string.IsNullOrEmpty(newBiometric.TemplateId) && 
                          string.IsNullOrEmpty(identity.BiometricData?.TemplateId);
        
        // Create evidence JWS if not already provided (per spec §2.4)
        var evidenceJws = newBiometric.EvidenceJws;
        if (string.IsNullOrEmpty(evidenceJws) && isEnrollment && !string.IsNullOrEmpty(newBiometric.TemplateId))
        {
            evidenceJws = await _evidenceService.CreateEnrollmentEvidenceAsync(
                command.IdentityId,
                did,
                newBiometric.TemplateId,
                newBiometric,
                null,
                cancellationToken);
            
            // Update biometric data with evidence JWS
            var updatedBiometric = new BiometricData(
                newBiometric.Type,
                newBiometric.EncryptedTemplate,
                newBiometric.Hash,
                newBiometric.TemplateId,
                newBiometric.AlgoVersion,
                newBiometric.Format,
                newBiometric.Quality,
                evidenceJws,
                newBiometric.LivenessScore,
                newBiometric.LivenessDecision);
            
            identity.UpdateBiometric(updatedBiometric, verificationBiometric, did, isEnrollment);
        }
        else
        {
            identity.UpdateBiometric(newBiometric, verificationBiometric, did, isEnrollment);
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
            // Note: For UpdateBiometric, we need to re-apply the entire operation including MinIO upload
            // This is a simplified retry - in production, you might want to check if MinIO upload is still needed
            var freshIdentity = await _repository.GetAsync(command.IdentityId, cancellationToken);
            if (freshIdentity == null)
                throw new IdentityNotFoundException(command.IdentityId);

            // Re-apply the domain operation
            // Note: We skip MinIO re-upload on retry to avoid duplicate uploads
            // The storage reference should already be in metadata if it was uploaded
            var retryDid = freshIdentity.Metadata?.ContainsKey("Did") == true 
                ? freshIdentity.Metadata["Did"]?.ToString() 
                : null;
            var retryIsEnrollment = !string.IsNullOrEmpty(newBiometric.TemplateId) && 
                                  string.IsNullOrEmpty(freshIdentity.BiometricData?.TemplateId);
            
            freshIdentity.UpdateBiometric(newBiometric, verificationBiometric, retryDid, retryIsEnrollment);

            await _repository.UpdateAsync(freshIdentity, cancellationToken);
            await _eventProcessor.ProcessAsync(freshIdentity.Events);
        }

        // Delegate to service for logging (service handles ledger transaction logging)
        await _identityService.LogBiometricUpdateAsync(
            command.IdentityId,
            newBiometric.Type.ToString(),
            isEnrollment,
            _context.CorrelationId.ToString(),
            cancellationToken);
    }
}

