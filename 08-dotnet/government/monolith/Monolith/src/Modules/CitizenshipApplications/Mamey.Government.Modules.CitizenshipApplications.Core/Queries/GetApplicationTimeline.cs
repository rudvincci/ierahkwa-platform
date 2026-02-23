using System;
using System.Collections.Generic;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Queries;

internal class GetApplicationTimeline : IQuery<IReadOnlyList<ApplicationTimelineEntryDto>>
{
    public Guid ApplicationId { get; set; }
}
