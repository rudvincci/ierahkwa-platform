using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Certificates.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Certificates.Core.Commands.Handlers;

internal sealed class RevokeCertificateHandler : ICommandHandler<RevokeCertificate>
{
    private readonly ICertificateRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<RevokeCertificateHandler> _logger;

    public RevokeCertificateHandler(
        ICertificateRepository repository,
        IMessageBroker messageBroker,
        ILogger<RevokeCertificateHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(RevokeCertificate command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Revoking certificate {CertificateId}", command.CertificateId);

        var certificate = await _repository.GetAsync(new CertificateId(command.CertificateId), cancellationToken);
        if (certificate is null)
        {
            throw new InvalidOperationException($"Certificate with ID {command.CertificateId} not found");
        }

        if (!certificate.IsActive)
        {
            throw new InvalidOperationException($"Certificate {command.CertificateId} is already revoked");
        }

        certificate.Revoke(command.Reason);

        await _repository.UpdateAsync(certificate, cancellationToken);

        // Publish integration event
        await _messageBroker.PublishAsync(
            new CertificateRevokedEvent(certificate.Id.Value, certificate.CitizenId, command.Reason),
            cancellationToken);

        _logger.LogInformation("Certificate {CertificateId} revoked. Reason: {Reason}",
            command.CertificateId, command.Reason);
    }
}
