using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class RoleStatisticsDto
{
    public int TotalRoles { get; set; }
    public int ActiveRoles { get; set; }
    public int InactiveRoles { get; set; }
    public int DeprecatedRoles { get; set; }
}

