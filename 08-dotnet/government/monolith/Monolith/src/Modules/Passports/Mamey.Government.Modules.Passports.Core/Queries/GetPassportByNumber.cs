using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Passports.Core.DTO;

namespace Mamey.Government.Modules.Passports.Core.Queries;

internal class GetPassportByNumber : IQuery<PassportDto?>
{
    public string PassportNumber { get; set; } = string.Empty;
}
