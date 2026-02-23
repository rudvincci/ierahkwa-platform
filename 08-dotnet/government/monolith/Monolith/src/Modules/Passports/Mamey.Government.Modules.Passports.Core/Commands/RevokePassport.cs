using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Passports.Core.Commands;

public record RevokePassport(Guid PassportId, string Reason, string RevokedBy) : ICommand;
