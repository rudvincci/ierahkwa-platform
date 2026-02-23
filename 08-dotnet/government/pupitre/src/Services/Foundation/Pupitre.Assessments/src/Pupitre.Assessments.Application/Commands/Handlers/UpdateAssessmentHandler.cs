using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Assessments.Application.Exceptions;
using Pupitre.Assessments.Contracts.Commands;
using Pupitre.Assessments.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;

namespace Pupitre.Assessments.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateAssessmentHandler : ICommandHandler<UpdateAssessment>
{
    private readonly IAssessmentRepository _assessmentRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public UpdateAssessmentHandler(
        IAssessmentRepository assessmentRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _assessmentRepository = assessmentRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(UpdateAssessment command, CancellationToken cancellationToken = default)
    {
        var assessment = await _assessmentRepository.GetAsync(command.Id);

        if(assessment is null)
        {
            throw new AssessmentNotFoundException(command.Id);
        }

        assessment.Update(command.Name, command.Tags, command.ProgramCode,
            command.CredentialType, command.CompletionDate, command.Metadata, command.Nationality);
        await _assessmentRepository.UpdateAsync(assessment, cancellationToken);

        if (command.ReissueCredential)
        {
            var payload = BuildPayload(assessment, command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);
            if (!string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                assessment.AttachBlockchainReceipt(
                    receipt.IdentityId!,
                    receipt.BlockchainAccount ?? assessment.BlockchainAccount,
                    receipt.DocumentId,
                    receipt.DocumentHash,
                    receipt.LedgerTransactionId,
                    receipt.CredentialIssuedAt,
                    receipt.PublishedToLedger ? "issued" : "pending");
                await _assessmentRepository.UpdateAsync(assessment, cancellationToken);
            }
        }

        await _eventProcessor.ProcessAsync(assessment.Events);
    }

    private static EducationLedgerPayload BuildPayload(Domain.Entities.Assessment assessment, UpdateAssessment command)
        => new()
        {
            MinistryDataId = command.Id,
            CitizenId = assessment.CitizenId,
            IdentityId = assessment.GovernmentIdentityId,
            BlockchainAccount = assessment.BlockchainAccount,
            ProgramCode = assessment.ProgramCode,
            CredentialType = assessment.CredentialType,
            CompletionDate = command.CompletionDate ?? assessment.CompletionDate,
            CredentialDocumentBase64 = command.CredentialDocumentBase64,
            CredentialMimeType = command.CredentialMimeType,
            Metadata = ToReadOnly(command.Metadata),
            TransactionId = $"assessment-{command.Id:N}",
            SourceAccount = assessment.BlockchainAccount,
            TargetAccount = assessment.BlockchainAccount,
            Nationality = command.Nationality ?? assessment.Nationality
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}


