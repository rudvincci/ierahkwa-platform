using System.Security.Claims;

namespace Mamey.Graph.Services;

public interface IJwtService
{
    List<Claim?> DecodeJwt(string jwt);
}