using System;
using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;
using Pupitre.Curricula.Application.Exceptions;
using Pupitre.Curricula.Contracts.Commands;
using Pupitre.Curricula.Domain.Entities;
using Pupitre.Curricula.Domain.Repositories;

namespace Pupitre.Curricula.Application.Commands.Handlers;

internal sealed class AddCurriculumHandler : ICommandHandler<AddCurriculum>
{
    private readonly ICurriculumRepository _curriculumRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public AddCurriculumHandler(ICurriculumRepository curriculumRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _curriculumRepository = curriculumRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(AddCurriculum command, CancellationToken cancellationToken = default)
    {
        var curriculum = await _curriculumRepository.GetAsync(command.Id);

        if (curriculum is not null)
        {
            throw new CurriculumAlreadyExistsException(command.Id);
        }

        curriculum = Curriculum.Create(
            command.Id,
            command.Name ?? string.Empty,
            command.Tags,
            citizenId: command.CitizenId,
            nationality: command.Nationality,
            programCode: command.ProgramCode,
            credentialType: command.CredentialType,
            completionDate: command.CompletionDate,
            metadata: command.Metadata);

        await _curriculumRepository.AddAsync(curriculum, cancellationToken);

        if (command.PublishToLedger)
        {
            var payload = BuildLedgerPayload(command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);

            if (string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                throw new InvalidOperationException("Blockchain identity could not be resolved for curriculum credential.");
            }

            curriculum.AttachBlockchainReceipt(
                receipt.IdentityId,
                receipt.BlockchainAccount ?? command.BlockchainAccount,
                receipt.DocumentId,
                receipt.DocumentHash,
                receipt.LedgerTransactionId,
                receipt.CredentialIssuedAt,
                receipt.PublishedToLedger ? "issued" : "pending");

            await _curriculumRepository.UpdateAsync(curriculum, cancellationToken);
        }

        await _eventProcessor.ProcessAsync(curriculum.Events);
    }

    private static EducationLedgerPayload BuildLedgerPayload(AddCurriculum command)
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
            TransactionId = $"curriculum-{command.Id:N}",
            SourceAccount = command.BlockchainAccount,
            TargetAccount = command.BlockchainAccount
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}

