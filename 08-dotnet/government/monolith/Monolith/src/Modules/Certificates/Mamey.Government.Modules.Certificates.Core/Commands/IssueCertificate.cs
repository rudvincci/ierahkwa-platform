using System;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.Certificates.Core.Commands;

public record IssueCertificate(
    Guid TenantId,
    Guid? CitizenId,
    CertificateType CertificateType,
    string? SubjectName = null,
    string? Notes = null) : ICommand
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
