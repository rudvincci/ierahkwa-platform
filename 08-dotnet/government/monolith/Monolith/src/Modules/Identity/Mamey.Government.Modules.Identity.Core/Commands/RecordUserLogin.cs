using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Identity.Core.Commands;

public record RecordUserLogin(Guid UserId) : ICommand;
