using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.CMS.Core.Commands;

public record PublishContent(Guid ContentId) : ICommand;
