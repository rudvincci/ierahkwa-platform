using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Certificates.Core.Events;

public record CertificateArchivedEvent(
    Guid CertificateId, 
    Guid? CitizenId) : IEvent;
