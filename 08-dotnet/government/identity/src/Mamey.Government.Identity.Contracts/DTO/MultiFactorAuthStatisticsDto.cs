using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class MultiFactorAuthStatisticsDto
{
    public int TotalMultiFactorAuth { get; set; }
    public int ActiveMultiFactorAuth { get; set; }
    public int InactiveMultiFactorAuth { get; set; }
    public int TotalChallenges { get; set; }
    public int PendingChallenges { get; set; }
}
