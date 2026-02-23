using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class TwoFactorAuthStatisticsDto
{
    public int TotalTwoFactorAuth { get; set; }
    public int ActiveTwoFactorAuth { get; set; }
    public int PendingTwoFactorAuth { get; set; }
    public int DisabledTwoFactorAuth { get; set; }
}

