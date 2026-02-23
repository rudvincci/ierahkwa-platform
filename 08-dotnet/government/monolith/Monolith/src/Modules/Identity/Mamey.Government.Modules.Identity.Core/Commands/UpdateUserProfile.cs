using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Identity.Core.Commands;

public record UpdateUserProfile(
    Guid UserId,
    string? Email = null,
    string? DisplayName = null) : ICommand;
