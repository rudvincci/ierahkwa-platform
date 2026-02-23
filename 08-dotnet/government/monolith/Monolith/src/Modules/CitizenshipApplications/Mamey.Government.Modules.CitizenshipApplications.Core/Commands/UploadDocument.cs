using System;
using System.IO;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Commands;

public record UploadDocument(
    Guid ApplicationId,
    string DocumentType,
    string FileName,
    string ContentType,
    Stream Content) : ICommand;
