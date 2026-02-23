using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Citizens.Core.Commands;

public record DeactivateCitizen(
    Guid CitizenId,
    string Reason,
    string DeactivatedBy) : ICommand;
