using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Certificates.Core.Events;

public record CertificateIssuedEvent(
    Guid CertificateId, 
    Guid? CitizenId, 
    string CertificateType,
    string CertificateNumber) : IEvent;
