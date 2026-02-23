using System;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Certificates.Core.Domain.Entities;
using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Certificates.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Certificates.Core.Commands.Handlers;

internal sealed class IssueCertificateCopyHandler : ICommandHandler<IssueCertificateCopy>
{
    private readonly ICertificateRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<IssueCertificateCopyHandler> _logger;

    public IssueCertificateCopyHandler(
        ICertificateRepository repository,
        IMessageBroker messageBroker,
        ILogger<IssueCertificateCopyHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(IssueCertificateCopy command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Issuing copy of certificate {OriginalCertificateId}", command.OriginalCertificateId);

        var original = await _repository.GetAsync(new CertificateId(command.OriginalCertificateId), cancellationToken);
        if (original is null)
        {
            throw new InvalidOperationException($"Original certificate with ID {command.OriginalCertificateId} not found");
        }

        // Create a copy with a new certificate number
        var copyNumber = $"{original.CertificateNumber}-COPY-{DateTime.UtcNow:yyyyMMdd}";
        
        var copy = new Certificate(
            new CertificateId(command.Id),
            original.TenantId,
            original.CitizenId,
            original.CertificateType,
            copyNumber,
            DateTime.UtcNow);

        await _repository.AddAsync(copy, cancellationToken);

        // Publish integration event
        await _messageBroker.PublishAsync(
            new CertificateCopyIssuedEvent(
                copy.Id.Value, 
                command.OriginalCertificateId,
                original.CitizenId, 
                copyNumber),
            cancellationToken);

        _logger.LogInformation("Certificate copy {CopyNumber} issued from original {OriginalNumber}",
            copyNumber, original.CertificateNumber);
    }
}
