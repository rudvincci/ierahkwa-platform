using System;
using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Users.Application.Exceptions;
using Pupitre.Users.Contracts.Commands;
using Pupitre.Users.Domain.Entities;
using Pupitre.Users.Domain.Repositories;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;

namespace Pupitre.Users.Application.Commands.Handlers;

internal sealed class AddUserHandler : ICommandHandler<AddUser>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public AddUserHandler(IUserRepository userRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _userRepository = userRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(AddUser command, CancellationToken cancellationToken = default)
    {
        
        var user = await _userRepository.GetAsync(command.Id);
        
        if (user is not null)
        {
            throw new UserAlreadyExistsException(command.Id);
        }

        user = User.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags,
            citizenId: command.CitizenId, nationality: command.Nationality, programCode: command.ProgramCode,
            credentialType: command.CredentialType, completionDate: command.CompletionDate, metadata: command.Metadata);
        await _userRepository.AddAsync(user, cancellationToken);

        if (command.PublishToLedger)
        {
            var payload = BuildLedgerPayload(command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);

            if (string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                throw new InvalidOperationException("Blockchain identity could not be resolved for user credential.");
            }

            user.AttachBlockchainReceipt(
                receipt.IdentityId,
                receipt.BlockchainAccount ?? command.BlockchainAccount,
                receipt.DocumentId,
                receipt.DocumentHash,
                receipt.LedgerTransactionId,
                receipt.CredentialIssuedAt,
                receipt.PublishedToLedger ? "issued" : "pending");

            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        await _eventProcessor.ProcessAsync(user.Events);
    }

    private static EducationLedgerPayload BuildLedgerPayload(AddUser command)
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
            TransactionId = $"user-{command.Id:N}",
            SourceAccount = command.BlockchainAccount,
            TargetAccount = command.BlockchainAccount
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}

