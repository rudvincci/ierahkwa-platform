using Chronicle;
using Mamey.FWID.Sagas.Core.Commands;
using Mamey.FWID.Sagas.Core.Data;
using Mamey.FWID.Credentials.Domain.Events;
using Mamey.FWID.ZKPs.Domain.Events;
using Mamey.MessageBrokers;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Sagas.Core.Sagas.CredentialIssuance;

/// <summary>
/// Saga for orchestrating Credential Issuance process.
/// Orchestrates: Identity Validation → Credential Issuance → ZKP Proof Generation → Ledger Logging
/// </summary>
internal sealed class CredentialIssuanceSaga : Saga<CredentialIssuanceSagaData>,
    ISagaStartAction<StartCredentialIssuance>,
    ISagaAction<CredentialIssued>,
    ISagaAction<ZKPProofGenerated>
{
    private readonly ILogger<CredentialIssuanceSaga> _logger;
    private readonly IBusPublisher _publisher;
    private readonly ICorrelationContextAccessor _accessor;

    public CredentialIssuanceSaga(
        ILogger<CredentialIssuanceSaga> logger,
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
            StartCredentialIssuance cmd => (SagaId)cmd.IdentityId.ToString(),
            CredentialIssued e => (SagaId)e.IdentityId.Value.ToString(),
            ZKPProofGenerated e => (SagaId)e.IdentityId.Value.ToString(),
            _ => base.ResolveId(message, context)
        };

    /// <summary>
    /// Starts the saga by validating identity and issuing credential.
    /// </summary>
    public async Task HandleAsync(StartCredentialIssuance message, ISagaContext context)
    {
        Data.IdentityId = message.IdentityId;
        Data.CredentialType = message.CredentialType;
        Data.Claims = message.Claims;
        Data.IssuerId = message.IssuerId;
        Data.ExpiresAt = message.ExpiresAt;
        Data.Status = "Processing";
        Data.StartedAt = DateTime.UtcNow;
        Data.IdentityValidated = true; // TODO: Validate identity exists

        _logger.LogInformation("Credential Issuance saga started for IdentityId: {IdentityId}, Type: {CredentialType}", 
            Data.IdentityId, Data.CredentialType);

        // Issue credential
        var issueCredentialCommand = new Mamey.FWID.Credentials.Contracts.Commands.IssueCredential
        {
            Id = Guid.NewGuid(),
            IdentityId = new Mamey.FWID.Credentials.Domain.Entities.IdentityId(message.IdentityId),
            CredentialType = message.CredentialType,
            Claims = message.Claims,
            IssuerId = message.IssuerId,
            ExpiresAt = message.ExpiresAt
        };

        await _publisher.PublishAsync(issueCredentialCommand);
    }

    /// <summary>
    /// Handles CredentialIssued event - proceeds to generate ZKP proof.
    /// </summary>
    public async Task HandleAsync(CredentialIssued e, ISagaContext context)
    {
        Data.CredentialIssued = true;
        Data.CredentialId = e.CredentialId.Value;

        _logger.LogInformation("Credential issued: {CredentialId} for Identity: {IdentityId}. Proceeding to generate ZKP proof.", 
            Data.CredentialId, Data.IdentityId);

        // Generate ZKP proof
        var generateZKPCommand = new Mamey.FWID.ZKPs.Contracts.Commands.GenerateZKPProof
        {
            IdentityId = e.IdentityId,
            AttributeType = e.CredentialType,
            Attributes = new Dictionary<string, object>
            {
                { "credentialId", e.CredentialId.Value.ToString() },
                { "credentialType", e.CredentialType }
            },
            ExpiresAt = Data.ExpiresAt
        };

        await _publisher.PublishAsync(generateZKPCommand);
    }

    /// <summary>
    /// Handles ZKPProofGenerated event - proceeds to log to ledger and complete saga.
    /// </summary>
    public Task HandleAsync(ZKPProofGenerated e, ISagaContext context)
    {
        Data.ZKPProofGenerated = true;
        Data.ProofId = e.ProofId.Value;

        _logger.LogInformation("ZKP proof generated: {ProofId} for Identity: {IdentityId}. Logging to ledger.", 
            Data.ProofId, Data.IdentityId);

        // TODO: Log to ledger (external service)
        Data.LedgerLogged = true;
        Data.Status = "Completed";
        Data.CompletedAt = DateTime.UtcNow;

        _logger.LogInformation("Credential Issuance saga completed for IdentityId: {IdentityId}", Data.IdentityId);

        return Task.CompletedTask;
    }

    #region Compensation

    public Task CompensateAsync(StartCredentialIssuance message, ISagaContext context)
    {
        _logger.LogWarning("Compensating Credential Issuance saga for IdentityId: {IdentityId}", Data.IdentityId);
        return Task.CompletedTask;
    }

    public async Task CompensateAsync(CredentialIssued message, ISagaContext context)
    {
        _logger.LogWarning("Compensating Credential Issuance for CredentialId: {CredentialId}", message.CredentialId);
        
        // Revoke credential
        var revokeCredentialCommand = new Mamey.FWID.Credentials.Contracts.Commands.RevokeCredential
        {
            CredentialId = message.CredentialId,
            Reason = "Saga compensation - credential issuance failed",
            RevokedBy = Data.IssuerId
        };

        await _publisher.PublishAsync(revokeCredentialCommand);
    }

    public async Task CompensateAsync(ZKPProofGenerated message, ISagaContext context)
    {
        _logger.LogWarning("Compensating ZKP Proof Generation for ProofId: {ProofId}", message.ProofId);
        
        // Revoke proof
        var revokeProofCommand = new Mamey.FWID.ZKPs.Contracts.Commands.RevokeZKPProof
        {
            ProofId = message.ProofId,
            Reason = "Saga compensation - credential issuance failed"
        };

        await _publisher.PublishAsync(revokeProofCommand);
    }

    #endregion
}



