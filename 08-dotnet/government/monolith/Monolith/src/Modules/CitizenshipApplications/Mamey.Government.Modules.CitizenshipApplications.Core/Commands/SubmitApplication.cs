using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Commands;

public record SubmitApplication(Guid ApplicationId) : ICommand;
