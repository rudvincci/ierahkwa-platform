using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class MultiFactorSetupDto
{
    public IEnumerable<int> EnabledMethods { get; set; } = Enumerable.Empty<int>();
    public int RequiredMethods { get; set; }
}

