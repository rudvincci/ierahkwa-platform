using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Certificates.Core.Commands;

public record IssueCertificateCopy(
    Guid OriginalCertificateId,
    string RequestedBy,
    string? Notes = null) : ICommand
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
