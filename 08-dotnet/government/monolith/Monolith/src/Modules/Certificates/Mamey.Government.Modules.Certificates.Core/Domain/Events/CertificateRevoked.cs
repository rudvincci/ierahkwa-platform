using Mamey.CQRS;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.Certificates.Core.Domain.Events;

internal record CertificateRevoked(
    CertificateId CertificateId,
    string Reason) : IDomainEvent;
