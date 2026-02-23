using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Curricula.Application.Exceptions;
using Pupitre.Curricula.Contracts.Commands;
using Pupitre.Curricula.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;

namespace Pupitre.Curricula.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateCurriculumHandler : ICommandHandler<UpdateCurriculum>
{
    private readonly ICurriculumRepository _curriculumRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public UpdateCurriculumHandler(
        ICurriculumRepository curriculumRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _curriculumRepository = curriculumRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(UpdateCurriculum command, CancellationToken cancellationToken = default)
    {
        var curriculum = await _curriculumRepository.GetAsync(command.Id);

        if(curriculum is null)
        {
            throw new CurriculumNotFoundException(command.Id);
        }

        curriculum.Update(command.Name, command.Tags, command.ProgramCode, command.CredentialType,
            command.CompletionDate, command.Metadata, command.Nationality);
        await _curriculumRepository.UpdateAsync(curriculum, cancellationToken);

        if (command.ReissueCredential)
        {
            var payload = BuildPayload(curriculum, command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);
            if (!string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                curriculum.AttachBlockchainReceipt(
                    receipt.IdentityId!,
                    receipt.BlockchainAccount ?? curriculum.BlockchainAccount,
                    receipt.DocumentId,
                    receipt.DocumentHash,
                    receipt.LedgerTransactionId,
                    receipt.CredentialIssuedAt,
                    receipt.PublishedToLedger ? "issued" : "pending");
                await _curriculumRepository.UpdateAsync(curriculum, cancellationToken);
            }
        }

        await _eventProcessor.ProcessAsync(curriculum.Events);
    }

    private static EducationLedgerPayload BuildPayload(Domain.Entities.Curriculum curriculum, UpdateCurriculum command)
        => new()
        {
            MinistryDataId = command.Id,
            CitizenId = curriculum.CitizenId,
            IdentityId = curriculum.GovernmentIdentityId,
            BlockchainAccount = curriculum.BlockchainAccount,
            ProgramCode = curriculum.ProgramCode,
            CredentialType = curriculum.CredentialType,
            CompletionDate = command.CompletionDate ?? curriculum.CompletionDate,
            CredentialDocumentBase64 = command.CredentialDocumentBase64,
            CredentialMimeType = command.CredentialMimeType,
            Metadata = ToReadOnly(command.Metadata),
            TransactionId = $"curriculum-{command.Id:N}",
            SourceAccount = curriculum.BlockchainAccount,
            TargetAccount = curriculum.BlockchainAccount,
            Nationality = command.Nationality ?? curriculum.Nationality
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}


