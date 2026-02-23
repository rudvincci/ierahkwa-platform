using System;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Certificates.Core.Domain.Entities;
using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Certificates.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Certificates.Core.Commands.Handlers;

internal sealed class IssueCertificateHandler : ICommandHandler<IssueCertificate>
{
    private readonly ICertificateRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<IssueCertificateHandler> _logger;

    public IssueCertificateHandler(
        ICertificateRepository repository,
        IMessageBroker messageBroker,
        ILogger<IssueCertificateHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(IssueCertificate command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Issuing {CertificateType} certificate", command.CertificateType);

        var certificateNumber = GenerateCertificateNumber(command.CertificateType);

        var certificate = new Certificate(
            new CertificateId(command.Id),
            new TenantId(command.TenantId),
            command.CitizenId,
            command.CertificateType,
            certificateNumber,
            DateTime.UtcNow);

        await _repository.AddAsync(certificate, cancellationToken);

        // Publish integration event
        await _messageBroker.PublishAsync(
            new CertificateIssuedEvent(
                certificate.Id.Value, 
                command.CitizenId, 
                command.CertificateType.ToString(), 
                certificateNumber),
            cancellationToken);

        _logger.LogInformation("Certificate {CertificateNumber} issued", certificateNumber);
    }

    private static string GenerateCertificateNumber(CertificateType type)
    {
        var prefix = type switch
        {
            CertificateType.BirthCertificate => "BC",
            CertificateType.MarriageCertificate => "MC",
            CertificateType.DeathCertificate => "DC",
            CertificateType.CitizenshipCertificate => "CC",
            _ => "OT"
        };
        var random = new Random();
        return $"{prefix}-{DateTime.UtcNow:yyyy}-{random.Next(100000, 999999)}";
    }
}
