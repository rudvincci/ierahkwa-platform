using System.Security.Claims;

namespace Mamey.BlazorWasm;

public interface IJwtService
{
    List<Claim?> DecodeJwt(string jwt);
}
