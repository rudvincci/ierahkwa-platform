using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class SessionStatisticsDto
{
    public int TotalSessions { get; set; }
    public int ActiveSessions { get; set; }
    public int ExpiredSessions { get; set; }
    public int RevokedSessions { get; set; }
}

