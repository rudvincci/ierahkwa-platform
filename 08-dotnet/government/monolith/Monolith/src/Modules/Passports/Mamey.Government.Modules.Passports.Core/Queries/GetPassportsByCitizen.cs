using System;
using System.Collections.Generic;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Passports.Core.DTO;

namespace Mamey.Government.Modules.Passports.Core.Queries;

internal class GetPassportsByCitizen : IQuery<IEnumerable<PassportSummaryDto>>
{
    public Guid CitizenId { get; set; }
}
