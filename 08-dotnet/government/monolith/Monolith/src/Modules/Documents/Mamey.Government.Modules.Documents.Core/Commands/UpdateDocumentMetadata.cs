using System;
using System.Collections.Generic;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Documents.Core.Commands;

public record UpdateDocumentMetadata(
    Guid DocumentId,
    string? Description = null,
    Dictionary<string, string>? Tags = null) : ICommand;
