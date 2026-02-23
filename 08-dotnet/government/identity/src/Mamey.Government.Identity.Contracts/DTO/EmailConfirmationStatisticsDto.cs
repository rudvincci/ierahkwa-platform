using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class EmailConfirmationStatisticsDto
{
    public int TotalConfirmations { get; set; }
    public int PendingConfirmations { get; set; }
    public int ConfirmedConfirmations { get; set; }
    public int ExpiredConfirmations { get; set; }
}

