using Chronicle;
using Mamey.FWID.Sagas.Core.Commands;
using Mamey.FWID.Sagas.Core.Data;
using Mamey.FWID.Sagas.Core.Handlers;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.DIDs.Domain.Events;
using Mamey.FWID.Credentials.Domain.Events;
using Mamey.FWID.AccessControls.Domain.Events;
using Mamey.MessageBrokers;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Sagas.Core.Sagas.IdentityRegistration;

/// <summary>
/// Saga for orchestrating Identity Registration process.
/// Complete workflow: StartRegistration → CreateIdentity → CaptureBiometrics → SubmitForClanApproval → 
///                   WaitForApproval → IssueDID → GrantZoneAccess → GenerateCredentials → Complete
/// Includes compensation (rollback) for each step.
/// </summary>
internal sealed class IdentityRegistrationSaga : Saga<IdentityRegistrationSagaData>,
    ISagaStartAction<StartIdentityRegistration>,
    ISagaAction<IdentityCreated>,
    ISagaAction<BiometricsCaptured>,
    ISagaAction<BiometricCaptureFailed>,
    ISagaAction<ClanApprovalGranted>,
    ISagaAction<ClanApprovalDenied>,
    ISagaAction<ClanApprovalTimedOut>,
    ISagaAction<DIDCreated>,
    ISagaAction<ZoneAccessGranted>,
    ISagaAction<CredentialIssued>,
    ISagaAction<RegistrationCancelled>
{
    private readonly ILogger<IdentityRegistrationSaga> _logger;
    private readonly IBusPublisher _publisher;
    private readonly ICorrelationContextAccessor _accessor;
    
    private static readonly TimeSpan ClanApprovalTimeout = TimeSpan.FromHours(48);

    public IdentityRegistrationSaga(
        ILogger<IdentityRegistrationSaga> logger,
        IBusPublisher publisher,
        ICorrelationContextAccessor accessor)
    {
        _logger = logger;
        _publisher = publisher;
        _accessor = accessor;
    }

    public override SagaId ResolveId(object message, ISagaContext context)
        => message switch
        {
            StartIdentityRegistration cmd => (SagaId)(cmd.IdentityId?.ToString() ?? Guid.NewGuid().ToString()),
            IdentityCreated e => (SagaId)e.IdentityId.Value.ToString(),
            BiometricsCaptured e => (SagaId)e.IdentityId.ToString(),
            BiometricCaptureFailed e => (SagaId)e.IdentityId.ToString(),
            ClanApprovalGranted e => (SagaId)e.IdentityId.ToString(),
            ClanApprovalDenied e => (SagaId)e.IdentityId.ToString(),
            ClanApprovalTimedOut e => (SagaId)e.IdentityId.ToString(),
            DIDCreated e => (SagaId)e.IdentityId.Value.ToString(),
            ZoneAccessGranted e => (SagaId)e.IdentityId.Value.ToString(),
            CredentialIssued e => (SagaId)e.IdentityId.Value.ToString(),
            RegistrationCancelled e => (SagaId)e.IdentityId.ToString(),
            _ => base.ResolveId(message, context)
        };

    #region Step 1: Start Registration
    
    /// <summary>
    /// Step 1: Starts the saga by initializing data and creating identity.
    /// </summary>
    public async Task HandleAsync(StartIdentityRegistration message, ISagaContext context)
    {
        Data.IdentityId = message.IdentityId ?? Guid.NewGuid();
        Data.FirstName = message.FirstName;
        Data.LastName = message.LastName;
        Data.Email = message.Email;
        Data.DateOfBirth = message.DateOfBirth;
        Data.Zone = message.Zone ?? "general";
        Data.Clan = message.Clan;
        Data.Status = "Processing";
        Data.StartedAt = DateTime.UtcNow;
        Data.CurrentStep = RegistrationStep.Started;

        Data.AddAuditEntry("Saga started", $"Name: {message.FirstName} {message.LastName}, Zone: {Data.Zone}");

        _logger.LogInformation("Identity Registration saga started for IdentityId: {IdentityId}, Name: {Name}",
            Data.IdentityId, $"{Data.FirstName} {Data.LastName}");

        // The API route handler should publish the CreateIdentity command
        // The saga will react to IdentityCreated event
        await Task.CompletedTask;
    }
    
    public Task CompensateAsync(StartIdentityRegistration message, ISagaContext context)
    {
        _logger.LogWarning("Compensating saga start for IdentityId: {IdentityId}", Data.IdentityId);
        Data.Status = "Cancelled";
        Data.CurrentStep = RegistrationStep.Cancelled;
        Data.AddAuditEntry("Saga start compensated");
        return Task.CompletedTask;
    }
    
    #endregion

    #region Step 2: Identity Created
    
    /// <summary>
    /// Step 2: Handles IdentityCreated event - proceeds to biometric capture.
    /// </summary>
    public Task HandleAsync(IdentityCreated e, ISagaContext context)
    {
        Data.IdentityId = e.IdentityId.Value;
        Data.IdentityCreated = true;
        Data.CurrentStep = RegistrationStep.BiometricCapture;
        
        if (e.Name?.Value != null)
        {
            var nameParts = e.Name.Value.Split(' ', 2);
            Data.FirstName = nameParts[0];
            Data.LastName = nameParts.Length > 1 ? nameParts[1] : "";
        }

        Data.AddAuditEntry("Identity created", $"IdentityId: {Data.IdentityId}");

        _logger.LogInformation("Identity created: {IdentityId}. Awaiting biometric capture.", Data.IdentityId);

        // The client should now submit biometrics via SubmitBiometrics command
        // Saga will react to BiometricsCaptured event
        return Task.CompletedTask;
    }

    public async Task CompensateAsync(IdentityCreated message, ISagaContext context)
    {
        _logger.LogWarning("Compensating Identity Creation for IdentityId: {IdentityId}", Data.IdentityId);
        Data.CurrentStep = RegistrationStep.CompensationInProgress;
        Data.AddAuditEntry("Identity creation compensation initiated");
        
        // Send command to deactivate/delete identity
        // await _publisher.PublishAsync(new DeactivateIdentity { IdentityId = Data.IdentityId });
        await Task.CompletedTask;
    }
    
    #endregion

    #region Step 3: Biometric Capture
    
    /// <summary>
    /// Step 3: Handles successful biometric capture - proceeds to clan approval.
    /// </summary>
    public async Task HandleAsync(BiometricsCaptured e, ISagaContext context)
    {
        Data.BiometricsCaptured = true;
        Data.BiometricsVerified = true;
        Data.BiometricTemplateId = e.BiometricTemplateId;
        Data.HasFingerprint = e.HasFingerprint;
        Data.HasFaceRecognition = e.HasFaceRecognition;
        Data.BiometricQualityScore = e.QualityScore;
        Data.BiometricsCapturedAt = e.CapturedAt;
        Data.CurrentStep = RegistrationStep.ClanApprovalPending;

        Data.AddAuditEntry("Biometrics captured",
            $"TemplateId: {e.BiometricTemplateId}, Fingerprint: {e.HasFingerprint}, Face: {e.HasFaceRecognition}, Score: {e.QualityScore:F2}");

        _logger.LogInformation("Biometrics captured for Identity: {IdentityId}. Submitting for clan approval.",
            Data.IdentityId);

        // Submit for clan approval
        Data.ClanApprovalId = Guid.NewGuid();
        Data.ClanApprovalSubmittedAt = DateTime.UtcNow;
        Data.ClanApprovalTimeout = DateTime.UtcNow.Add(ClanApprovalTimeout);
        Data.ClanApprovalSubmitted = true;
        Data.ClanApprovalStatus = ClanApprovalStatus.Pending;

        // Publish event to notify clan registrar service
        await _publisher.PublishAsync(new ClanApprovalRequested
        {
            IdentityId = Data.IdentityId,
            ApprovalId = Data.ClanApprovalId.Value,
            FirstName = Data.FirstName!,
            LastName = Data.LastName!,
            Zone = Data.Zone!,
            Clan = Data.Clan,
            BiometricTemplateId = e.BiometricTemplateId,
            RequestedAt = DateTime.UtcNow,
            TimeoutAt = Data.ClanApprovalTimeout.Value
        });

        Data.AddAuditEntry("Clan approval requested", $"ApprovalId: {Data.ClanApprovalId}, Timeout: {Data.ClanApprovalTimeout}");
    }

    /// <summary>
    /// Handles biometric capture failure - retry or fail saga.
    /// </summary>
    public Task HandleAsync(BiometricCaptureFailed e, ISagaContext context)
    {
        Data.RetryCount++;
        
        Data.AddAuditEntry("Biometric capture failed", $"Reason: {e.Reason}, RetryCount: {Data.RetryCount}");

        _logger.LogWarning("Biometric capture failed for Identity: {IdentityId}. Reason: {Reason}. RetryCount: {RetryCount}",
            Data.IdentityId, e.Reason, Data.RetryCount);

        if (!Data.CanProceed)
        {
            Data.Status = "Failed";
            Data.CurrentStep = RegistrationStep.Failed;
            Data.ErrorMessage = $"Biometric capture failed after {Data.RetryCount} attempts: {e.Reason}";
            Data.AddAuditEntry("Saga failed", Data.ErrorMessage);
            
            // Trigger compensation
            Reject();
        }

        return Task.CompletedTask;
    }

    public Task CompensateAsync(BiometricsCaptured message, ISagaContext context)
    {
        _logger.LogWarning("Compensating Biometric Capture for TemplateId: {TemplateId}", message.BiometricTemplateId);
        Data.AddAuditEntry("Biometric capture compensation initiated");
        // Delete biometric template
        return Task.CompletedTask;
    }

    public Task CompensateAsync(BiometricCaptureFailed message, ISagaContext context)
    {
        // No compensation needed for failure event
        return Task.CompletedTask;
    }
    
    #endregion

    #region Step 4: Clan Approval
    
    /// <summary>
    /// Step 4a: Handles clan approval granted - proceeds to DID issuance.
    /// </summary>
    public async Task HandleAsync(ClanApprovalGranted e, ISagaContext context)
    {
        Data.ClanApprovalReceived = true;
        Data.ClanApprovalStatus = ClanApprovalStatus.Approved;
        Data.ClanApprovalNotes = e.Notes;
        Data.ClanApprovalDecidedAt = e.ApprovedAt;
        Data.AssignedRegistrarId = e.RegistrarId;
        Data.CurrentStep = RegistrationStep.DIDIssuance;

        Data.AddAuditEntry("Clan approval granted",
            $"RegistrarId: {e.RegistrarId}, Notes: {e.Notes}");

        _logger.LogInformation("Clan approval granted for Identity: {IdentityId}. Proceeding to DID issuance.",
            Data.IdentityId);

        // Publish command to create DID
        var createDIDCommand = new Mamey.FWID.DIDs.Contracts.Commands.CreateDID
        {
            IdentityId = new Mamey.FWID.Identities.Domain.ValueObjects.IdentityId(Data.IdentityId)
        };

        await _publisher.PublishAsync(createDIDCommand);
    }

    /// <summary>
    /// Step 4b: Handles clan approval denied - fail the saga.
    /// </summary>
    public Task HandleAsync(ClanApprovalDenied e, ISagaContext context)
    {
        Data.ClanApprovalReceived = true;
        Data.ClanApprovalStatus = ClanApprovalStatus.Rejected;
        Data.ClanApprovalNotes = e.Reason;
        Data.ClanApprovalDecidedAt = e.DeniedAt;
        Data.AssignedRegistrarId = e.RegistrarId;
        Data.Status = "Rejected";
        Data.CurrentStep = RegistrationStep.Failed;
        Data.ErrorMessage = $"Clan approval denied: {e.Reason}";

        Data.AddAuditEntry("Clan approval denied",
            $"RegistrarId: {e.RegistrarId}, Reason: {e.Reason}");

        _logger.LogWarning("Clan approval denied for Identity: {IdentityId}. Reason: {Reason}",
            Data.IdentityId, e.Reason);

        // Trigger compensation
        Reject();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Step 4c: Handles clan approval timeout - escalate or fail.
    /// </summary>
    public Task HandleAsync(ClanApprovalTimedOut e, ISagaContext context)
    {
        Data.ClanApprovalStatus = ClanApprovalStatus.TimedOut;
        Data.CurrentStep = RegistrationStep.Failed;
        Data.Status = "Failed";
        Data.ErrorMessage = "Clan approval timed out";

        Data.AddAuditEntry("Clan approval timed out",
            $"SubmittedAt: {e.SubmittedAt}, TimedOutAt: {e.TimedOutAt}");

        _logger.LogWarning("Clan approval timed out for Identity: {IdentityId}. Submitted: {SubmittedAt}, Timed out: {TimedOutAt}",
            Data.IdentityId, e.SubmittedAt, e.TimedOutAt);

        Reject();

        return Task.CompletedTask;
    }

    public Task CompensateAsync(ClanApprovalGranted message, ISagaContext context)
    {
        _logger.LogWarning("Compensating Clan Approval for ApprovalId: {ApprovalId}", message.ApprovalId);
        Data.AddAuditEntry("Clan approval compensation initiated");
        // Revoke approval record
        return Task.CompletedTask;
    }

    public Task CompensateAsync(ClanApprovalDenied message, ISagaContext context) => Task.CompletedTask;
    public Task CompensateAsync(ClanApprovalTimedOut message, ISagaContext context) => Task.CompletedTask;
    
    #endregion

    #region Step 5: DID Issuance
    
    /// <summary>
    /// Step 5: Handles DIDCreated event - proceeds to zone access grant.
    /// </summary>
    public async Task HandleAsync(DIDCreated e, ISagaContext context)
    {
        Data.DIDCreated = true;
        Data.DIDId = e.DIDId.Value;
        Data.DIDString = e.DID?.Value;
        Data.DIDCreatedAt = DateTime.UtcNow;
        Data.CurrentStep = RegistrationStep.ZoneAccessGrant;

        Data.AddAuditEntry("DID created",
            $"DIDId: {e.DIDId.Value}, DID: {e.DID?.Value}");

        _logger.LogInformation("DID created: {DIDId} ({DID}) for Identity: {IdentityId}. Proceeding to grant zone access.",
            Data.DIDId, Data.DIDString, Data.IdentityId);

        // Grant zone access based on zone
        var zoneId = Guid.NewGuid(); // TODO: Resolve actual zone ID from zone name
        var grantAccessCommand = new Mamey.FWID.AccessControls.Contracts.Commands.GrantZoneAccess
        {
            IdentityId = e.IdentityId,
            ZoneId = new Mamey.FWID.AccessControls.Domain.ValueObjects.ZoneId(zoneId),
            Permission = Mamey.FWID.AccessControls.Domain.ValueObjects.AccessPermission.Read
        };

        await _publisher.PublishAsync(grantAccessCommand);
    }

    public async Task CompensateAsync(DIDCreated message, ISagaContext context)
    {
        _logger.LogWarning("Compensating DID Creation for DIDId: {DIDId}", message.DIDId);
        Data.AddAuditEntry("DID creation compensation initiated");
        
        // Deactivate DID
        // await _publisher.PublishAsync(new DeactivateDID { DIDId = message.DIDId.Value });
        await Task.CompletedTask;
    }
    
    #endregion

    #region Step 6: Zone Access Grant
    
    /// <summary>
    /// Step 6: Handles ZoneAccessGranted event - proceeds to credential issuance.
    /// </summary>
    public async Task HandleAsync(ZoneAccessGranted e, ISagaContext context)
    {
        Data.ZoneAccessGranted = true;
        Data.AccessControlId = e.AccessControlId.Value;
        Data.CurrentStep = RegistrationStep.CredentialGeneration;

        Data.AddAuditEntry("Zone access granted",
            $"AccessControlId: {e.AccessControlId.Value}, ZoneId: {e.ZoneId.Value}");

        _logger.LogInformation("Zone access granted: {AccessControlId} for Identity: {IdentityId}. Proceeding to issue credential.",
            Data.AccessControlId, Data.IdentityId);

        // Issue initial identity credential
        var issueCredentialCommand = new Mamey.FWID.Credentials.Contracts.Commands.IssueCredential
        {
            Id = Guid.NewGuid(),
            IdentityId = e.IdentityId,
            CredentialType = "IdentityCredential",
            Claims = new Dictionary<string, object>
            {
                { "identityId", Data.IdentityId.ToString() },
                { "firstName", Data.FirstName ?? "" },
                { "lastName", Data.LastName ?? "" },
                { "zone", Data.Zone ?? "general" },
                { "clan", Data.Clan ?? "" },
                { "didId", Data.DIDId?.ToString() ?? "" },
                { "did", Data.DIDString ?? "" },
                { "clanApprovalId", Data.ClanApprovalId?.ToString() ?? "" },
                { "registeredAt", Data.StartedAt.ToString("O") }
            },
            IssuerId = Data.AssignedRegistrarId ?? Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddYears(1)
        };

        await _publisher.PublishAsync(issueCredentialCommand);
    }

    public async Task CompensateAsync(ZoneAccessGranted message, ISagaContext context)
    {
        _logger.LogWarning("Compensating Zone Access Grant for AccessControlId: {AccessControlId}", message.AccessControlId);
        Data.AddAuditEntry("Zone access grant compensation initiated");
        
        // Revoke zone access
        // await _publisher.PublishAsync(new RevokeZoneAccess { AccessControlId = message.AccessControlId.Value });
        await Task.CompletedTask;
    }
    
    #endregion

    #region Step 7: Credential Issuance (Final Step)
    
    /// <summary>
    /// Step 7 (Final): Handles CredentialIssued event - completes the saga.
    /// </summary>
    public async Task HandleAsync(CredentialIssued e, ISagaContext context)
    {
        Data.CredentialIssued = true;
        Data.CredentialId = e.CredentialId.Value;
        Data.CredentialIssuedAt = DateTime.UtcNow;
        Data.Status = "Completed";
        Data.CurrentStep = RegistrationStep.Completed;
        Data.CompletedAt = DateTime.UtcNow;

        Data.AddAuditEntry("Credential issued - Saga completed",
            $"CredentialId: {e.CredentialId.Value}, ElapsedTime: {Data.ElapsedTime}");

        _logger.LogInformation(
            "Identity Registration saga COMPLETED for Identity: {IdentityId}. " +
            "DID: {DID}, CredentialId: {CredentialId}, ElapsedTime: {ElapsedTime}",
            Data.IdentityId, Data.DIDString, Data.CredentialId, Data.ElapsedTime);

        // Publish saga completed event
        await _publisher.PublishAsync(new IdentityRegistrationCompleted
        {
            IdentityId = Data.IdentityId,
            DIDId = Data.DIDId!.Value,
            DID = Data.DIDString!,
            CredentialId = Data.CredentialId!.Value,
            Zone = Data.Zone!,
            Clan = Data.Clan,
            CompletedAt = Data.CompletedAt.Value,
            ElapsedTime = Data.ElapsedTime
        });

        // Mark saga as complete
        Complete();
    }

    public async Task CompensateAsync(CredentialIssued message, ISagaContext context)
    {
        _logger.LogWarning("Compensating Credential Issuance for CredentialId: {CredentialId}", message.CredentialId);
        Data.AddAuditEntry("Credential issuance compensation initiated");
        
        // Revoke credential
        // await _publisher.PublishAsync(new RevokeCredential { CredentialId = message.CredentialId.Value });
        await Task.CompletedTask;
    }
    
    #endregion

    #region Registration Cancellation
    
    /// <summary>
    /// Handles user-initiated registration cancellation.
    /// </summary>
    public Task HandleAsync(RegistrationCancelled e, ISagaContext context)
    {
        Data.Status = "Cancelled";
        Data.CurrentStep = RegistrationStep.Cancelled;
        Data.ErrorMessage = e.Reason;
        Data.CompletedAt = e.CancelledAt;

        Data.AddAuditEntry("Registration cancelled by user", e.Reason);

        _logger.LogInformation("Identity Registration cancelled for Identity: {IdentityId}. Reason: {Reason}",
            Data.IdentityId, e.Reason);

        // Trigger compensation for any completed steps
        Reject();

        return Task.CompletedTask;
    }

    public Task CompensateAsync(RegistrationCancelled message, ISagaContext context) => Task.CompletedTask;
    
    #endregion
}

/// <summary>
/// Event to request clan approval (sent to clan registrar service).
/// </summary>
public record ClanApprovalRequested
{
    public Guid IdentityId { get; init; }
    public Guid ApprovalId { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Zone { get; init; } = null!;
    public string? Clan { get; init; }
    public Guid BiometricTemplateId { get; init; }
    public DateTime RequestedAt { get; init; }
    public DateTime TimeoutAt { get; init; }
}

/// <summary>
/// Event published when identity registration saga completes successfully.
/// </summary>
public record IdentityRegistrationCompleted
{
    public Guid IdentityId { get; init; }
    public Guid DIDId { get; init; }
    public string DID { get; init; } = null!;
    public Guid CredentialId { get; init; }
    public string Zone { get; init; } = null!;
    public string? Clan { get; init; }
    public DateTime CompletedAt { get; init; }
    public TimeSpan ElapsedTime { get; init; }
}
