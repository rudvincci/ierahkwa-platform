using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Queries;

public record GetApplication(Guid ApplicationId) : IQuery<ApplicationDto?>;
