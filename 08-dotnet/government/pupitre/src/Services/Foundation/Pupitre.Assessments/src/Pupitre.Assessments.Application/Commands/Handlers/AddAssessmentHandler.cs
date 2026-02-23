using System;
using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Assessments.Application.Exceptions;
using Pupitre.Assessments.Contracts.Commands;
using Pupitre.Assessments.Domain.Entities;
using Pupitre.Assessments.Domain.Repositories;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;

namespace Pupitre.Assessments.Application.Commands.Handlers;

internal sealed class AddAssessmentHandler : ICommandHandler<AddAssessment>
{
    private readonly IAssessmentRepository _assessmentRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public AddAssessmentHandler(IAssessmentRepository assessmentRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _assessmentRepository = assessmentRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(AddAssessment command, CancellationToken cancellationToken = default)
    {
        
        var assessment = await _assessmentRepository.GetAsync(command.Id);
        
        if(assessment is not null)
        {
            throw new AssessmentAlreadyExistsException(command.Id);
        }

        assessment = Assessment.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags,
            citizenId: command.CitizenId, nationality: command.Nationality, programCode: command.ProgramCode,
            credentialType: command.CredentialType, completionDate: command.CompletionDate, metadata: command.Metadata);
        await _assessmentRepository.AddAsync(assessment, cancellationToken);

        if (command.PublishToLedger)
        {
            var payload = BuildLedgerPayload(command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);

            if (string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                throw new InvalidOperationException("Blockchain identity could not be resolved for assessment credential.");
            }

            assessment.AttachBlockchainReceipt(
                receipt.IdentityId,
                receipt.BlockchainAccount ?? command.BlockchainAccount,
                receipt.DocumentId,
                receipt.DocumentHash,
                receipt.LedgerTransactionId,
                receipt.CredentialIssuedAt,
                receipt.PublishedToLedger ? "issued" : "pending");

            await _assessmentRepository.UpdateAsync(assessment, cancellationToken);
        }

        await _eventProcessor.ProcessAsync(assessment.Events);
    }

    private static EducationLedgerPayload BuildLedgerPayload(AddAssessment command)
        => new()
        {
            MinistryDataId = command.Id,
            CitizenId = command.CitizenId,
            IdentityId = command.IdentityId,
            BlockchainAccount = command.BlockchainAccount,
            FirstName = command.FirstName,
            LastName = command.LastName,
            DateOfBirth = command.DateOfBirth,
            Nationality = command.Nationality,
            ProgramCode = command.ProgramCode,
            CredentialType = command.CredentialType,
            CompletionDate = command.CompletionDate,
            CredentialDocumentBase64 = command.CredentialDocumentBase64,
            CredentialMimeType = command.CredentialMimeType,
            Metadata = ToReadOnly(command.Metadata),
            TransactionId = $"assessment-{command.Id:N}",
            SourceAccount = command.BlockchainAccount,
            TargetAccount = command.BlockchainAccount
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}

