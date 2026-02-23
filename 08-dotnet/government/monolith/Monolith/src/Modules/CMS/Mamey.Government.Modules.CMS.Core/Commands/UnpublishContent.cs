using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.CMS.Core.Commands;

public record UnpublishContent(Guid ContentId) : ICommand;
