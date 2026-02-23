using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.CMS.Core.Commands;

public record UpdateContent(
    Guid ContentId,
    string? Title = null,
    string? Body = null,
    string? Excerpt = null) : ICommand;
