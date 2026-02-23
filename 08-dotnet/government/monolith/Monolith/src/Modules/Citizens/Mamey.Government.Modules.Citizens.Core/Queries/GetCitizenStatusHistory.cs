using System;
using System.Collections.Generic;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Citizens.Core.DTO;

namespace Mamey.Government.Modules.Citizens.Core.Queries;

public record GetCitizenStatusHistory(Guid CitizenId) : IQuery<IReadOnlyList<CitizenStatusHistoryDto>>;
