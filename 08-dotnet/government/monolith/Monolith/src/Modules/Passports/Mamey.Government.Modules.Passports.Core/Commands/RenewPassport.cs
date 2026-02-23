using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Passports.Core.Commands;

public record RenewPassport(Guid PassportId, int ValidityYears = 10) : ICommand;
