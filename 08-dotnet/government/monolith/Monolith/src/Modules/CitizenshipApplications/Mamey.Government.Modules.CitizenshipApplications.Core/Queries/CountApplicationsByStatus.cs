using System;
using System.Collections.Generic;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Queries;

internal class CountApplicationsByStatus : IQuery<Dictionary<ApplicationStatus, int>>
{
    public Guid TenantId { get; set; }
}
