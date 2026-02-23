using System;
using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;
using Pupitre.Lessons.Application.Exceptions;
using Pupitre.Lessons.Contracts.Commands;
using Pupitre.Lessons.Domain.Entities;
using Pupitre.Lessons.Domain.Repositories;

namespace Pupitre.Lessons.Application.Commands.Handlers;

internal sealed class AddLessonHandler : ICommandHandler<AddLesson>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public AddLessonHandler(ILessonRepository lessonRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _lessonRepository = lessonRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(AddLesson command, CancellationToken cancellationToken = default)
    {
        
        var lesson = await _lessonRepository.GetAsync(command.Id);
        
        if(lesson is not null)
        {
            throw new LessonAlreadyExistsException(command.Id);
        }

        lesson = Lesson.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags,
            citizenId: command.CitizenId, nationality: command.Nationality, programCode: command.ProgramCode,
            credentialType: command.CredentialType, completionDate: command.CompletionDate, metadata: command.Metadata);
        await _lessonRepository.AddAsync(lesson, cancellationToken);

        if (command.PublishToLedger)
        {
            var payload = BuildLedgerPayload(command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);

            if (string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                throw new InvalidOperationException("Blockchain identity could not be resolved for lesson credential.");
            }

            lesson.AttachBlockchainReceipt(
                receipt.IdentityId,
                receipt.BlockchainAccount ?? command.BlockchainAccount,
                receipt.DocumentId,
                receipt.DocumentHash,
                receipt.LedgerTransactionId,
                receipt.CredentialIssuedAt,
                receipt.PublishedToLedger ? "issued" : "pending");

            await _lessonRepository.UpdateAsync(lesson, cancellationToken);
        }

        await _eventProcessor.ProcessAsync(lesson.Events);
    }

    private static EducationLedgerPayload BuildLedgerPayload(AddLesson command)
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
            TransactionId = $"lesson-{command.Id:N}",
            SourceAccount = command.BlockchainAccount,
            TargetAccount = command.BlockchainAccount
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}

