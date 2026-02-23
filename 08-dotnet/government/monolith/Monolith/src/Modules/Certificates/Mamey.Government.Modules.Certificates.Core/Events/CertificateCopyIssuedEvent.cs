using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Certificates.Core.Events;

public record CertificateCopyIssuedEvent(
    Guid CopyId, 
    Guid OriginalCertificateId,
    Guid? CitizenId, 
    string CopyNumber) : IEvent;
