using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Certificates.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Certificates.Core.Commands.Handlers;

internal sealed class ArchiveCertificateHandler : ICommandHandler<ArchiveCertificate>
{
    private readonly ICertificateRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<ArchiveCertificateHandler> _logger;

    public ArchiveCertificateHandler(
        ICertificateRepository repository,
        IMessageBroker messageBroker,
        ILogger<ArchiveCertificateHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(ArchiveCertificate command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Archiving certificate {CertificateId}", command.CertificateId);

        var certificate = await _repository.GetAsync(new CertificateId(command.CertificateId), cancellationToken);
        if (certificate is null)
        {
            throw new InvalidOperationException($"Certificate with ID {command.CertificateId} not found");
        }

        certificate.Revoke($"Archived by {command.ArchivedBy}");

        await _repository.UpdateAsync(certificate, cancellationToken);

        // Publish integration event
        await _messageBroker.PublishAsync(
            new CertificateArchivedEvent(certificate.Id.Value, certificate.CitizenId),
            cancellationToken);

        _logger.LogInformation("Certificate {CertificateId} archived", command.CertificateId);
    }
}
