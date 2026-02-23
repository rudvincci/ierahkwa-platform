using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Certificates.Core.Commands;

public record RevokeCertificate(Guid CertificateId, string Reason, string RevokedBy) : ICommand;
