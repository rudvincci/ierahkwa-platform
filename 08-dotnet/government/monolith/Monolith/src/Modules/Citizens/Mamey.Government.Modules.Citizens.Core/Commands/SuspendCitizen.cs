using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Citizens.Core.Commands;

public record SuspendCitizen(
    Guid CitizenId,
    string Reason,
    string SuspendedBy) : ICommand;
