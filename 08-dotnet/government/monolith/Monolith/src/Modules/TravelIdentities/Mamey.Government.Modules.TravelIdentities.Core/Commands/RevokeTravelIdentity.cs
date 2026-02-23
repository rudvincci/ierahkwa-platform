using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.TravelIdentities.Core.Commands;

public record RevokeTravelIdentity(Guid TravelIdentityId, string Reason, string RevokedBy) : ICommand;
