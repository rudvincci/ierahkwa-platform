using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;
using Pupitre.Users.Application.Exceptions;
using Pupitre.Users.Contracts.Commands;
using Pupitre.Users.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.Users.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateUserHandler : ICommandHandler<UpdateUser>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public UpdateUserHandler(
        IUserRepository userRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _userRepository = userRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(UpdateUser command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(command.Id);

        if (user is null)
        {
            throw new UserNotFoundException(command.Id);
        }

        user.Update(command.Name, command.Tags, command.ProgramCode,
            command.CredentialType, command.CompletionDate, command.Metadata, command.Nationality);
        await _userRepository.UpdateAsync(user, cancellationToken);

        if (command.ReissueCredential)
        {
            var payload = BuildPayload(user, command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);
            if (!string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                user.AttachBlockchainReceipt(
                    receipt.IdentityId!,
                    receipt.BlockchainAccount ?? user.BlockchainAccount,
                    receipt.DocumentId,
                    receipt.DocumentHash,
                    receipt.LedgerTransactionId,
                    receipt.CredentialIssuedAt,
                    receipt.PublishedToLedger ? "issued" : "pending");
                await _userRepository.UpdateAsync(user, cancellationToken);
            }
        }

        await _eventProcessor.ProcessAsync(user.Events);
    }

    private static EducationLedgerPayload BuildPayload(Pupitre.Users.Domain.Entities.User user, UpdateUser command)
        => new()
        {
            MinistryDataId = command.Id,
            CitizenId = user.CitizenId,
            IdentityId = user.GovernmentIdentityId,
            BlockchainAccount = user.BlockchainAccount,
            ProgramCode = user.ProgramCode,
            CredentialType = user.CredentialType,
            CompletionDate = command.CompletionDate ?? user.CompletionDate,
            CredentialDocumentBase64 = command.CredentialDocumentBase64,
            CredentialMimeType = command.CredentialMimeType,
            Metadata = ToReadOnly(command.Metadata),
            TransactionId = $"user-{command.Id:N}",
            SourceAccount = user.BlockchainAccount,
            TargetAccount = user.BlockchainAccount,
            Nationality = command.Nationality ?? user.Nationality
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}
