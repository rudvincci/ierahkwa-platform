using System;
using System.Collections.Generic;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.Citizens.Core.Queries;

internal class CountCitizensByStatus : IQuery<Dictionary<CitizenshipStatus, int>>
{
    public Guid TenantId { get; set; }
}
