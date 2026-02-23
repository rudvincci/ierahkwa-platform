using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class PermissionStatisticsDto
{
    public int TotalPermissions { get; set; }
    public int ActivePermissions { get; set; }
    public int InactivePermissions { get; set; }
    public int UniqueResources { get; set; }
    public int UniqueActions { get; set; }
}

