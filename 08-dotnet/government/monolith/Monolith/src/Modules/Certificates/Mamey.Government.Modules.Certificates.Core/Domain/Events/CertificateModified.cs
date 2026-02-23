using Mamey.CQRS;
using Mamey.Government.Modules.Certificates.Core.Domain.Entities;

namespace Mamey.Government.Modules.Certificates.Core.Domain.Events;

internal record CertificateModified(Certificate Certificate) : IDomainEvent;
