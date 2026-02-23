using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;
using Pupitre.Lessons.Application.Exceptions;
using Pupitre.Lessons.Contracts.Commands;
using Pupitre.Lessons.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.Lessons.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateLessonHandler : ICommandHandler<UpdateLesson>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public UpdateLessonHandler(
        ILessonRepository lessonRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _lessonRepository = lessonRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(UpdateLesson command, CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonRepository.GetAsync(command.Id);

        if(lesson is null)
        {
            throw new LessonNotFoundException(command.Id);
        }

        lesson.Update(command.Name, command.Tags, command.ProgramCode,
            command.CredentialType, command.CompletionDate, command.Metadata, command.Nationality);
        await _lessonRepository.UpdateAsync(lesson, cancellationToken);

        if (command.ReissueCredential)
        {
            var payload = BuildPayload(lesson, command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);
            if (!string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                lesson.AttachBlockchainReceipt(
                    receipt.IdentityId!,
                    receipt.BlockchainAccount ?? lesson.BlockchainAccount,
                    receipt.DocumentId,
                    receipt.DocumentHash,
                    receipt.LedgerTransactionId,
                    receipt.CredentialIssuedAt,
                    receipt.PublishedToLedger ? "issued" : "pending");
                await _lessonRepository.UpdateAsync(lesson, cancellationToken);
            }
        }

        await _eventProcessor.ProcessAsync(lesson.Events);
    }

    private static EducationLedgerPayload BuildPayload(Domain.Entities.Lesson lesson, UpdateLesson command)
        => new()
        {
            MinistryDataId = command.Id,
            CitizenId = lesson.CitizenId,
            IdentityId = lesson.GovernmentIdentityId,
            BlockchainAccount = lesson.BlockchainAccount,
            ProgramCode = lesson.ProgramCode,
            CredentialType = lesson.CredentialType,
            CompletionDate = command.CompletionDate ?? lesson.CompletionDate,
            CredentialDocumentBase64 = command.CredentialDocumentBase64,
            CredentialMimeType = command.CredentialMimeType,
            Metadata = ToReadOnly(command.Metadata),
            TransactionId = $"lesson-{command.Id:N}",
            SourceAccount = lesson.BlockchainAccount,
            TargetAccount = lesson.BlockchainAccount,
            Nationality = command.Nationality ?? lesson.Nationality
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}


