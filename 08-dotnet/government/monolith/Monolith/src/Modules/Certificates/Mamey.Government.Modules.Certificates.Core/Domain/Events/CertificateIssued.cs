using Mamey.CQRS;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.Certificates.Core.Domain.Events;

internal record CertificateIssued(
    CertificateId CertificateId,
    CitizenId? CitizenId,
    CertificateType CertificateType,
    string CertificateNumber) : IDomainEvent;
