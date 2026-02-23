using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Documents.Core.Commands;

public record DeleteDocument(Guid DocumentId, string DeletedBy) : ICommand;
