using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.TravelIdentities.Core.Commands;

public record RenewTravelIdentity(Guid TravelIdentityId, int ValidityYears = 8) : ICommand;
