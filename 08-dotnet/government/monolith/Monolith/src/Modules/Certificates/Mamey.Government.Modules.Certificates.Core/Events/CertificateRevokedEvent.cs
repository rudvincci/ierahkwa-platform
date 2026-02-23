using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Certificates.Core.Events;

public record CertificateRevokedEvent(
    Guid CertificateId, 
    Guid? CitizenId, 
    string Reason) : IEvent;
