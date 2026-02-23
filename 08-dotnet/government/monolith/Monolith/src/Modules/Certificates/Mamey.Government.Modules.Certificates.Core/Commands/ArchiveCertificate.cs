using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Certificates.Core.Commands;

public record ArchiveCertificate(Guid CertificateId, string ArchivedBy) : ICommand;
