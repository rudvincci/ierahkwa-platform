using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Passports.Core.DTO;

namespace Mamey.Government.Modules.Passports.Core.Queries;

internal class GetPassport : IQuery<PassportDto?>
{
    public Guid PassportId { get; set; }
}
