using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.CMS.Core.Commands;

public record DeleteContent(Guid ContentId, string DeletedBy) : ICommand;
