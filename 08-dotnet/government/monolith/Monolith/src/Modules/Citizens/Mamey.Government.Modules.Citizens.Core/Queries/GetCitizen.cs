using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Citizens.Core.DTO;

namespace Mamey.Government.Modules.Citizens.Core.Queries;

public record GetCitizen(Guid CitizenId) : IQuery<CitizenDto?>;
