using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class TwoFactorSetupDto
{
    public string SecretKey { get; set; } = string.Empty;
    public string QrCodeUrl { get; set; } = string.Empty;
}

