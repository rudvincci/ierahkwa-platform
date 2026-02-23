

using Mamey.Auth;
using Mamey.MicroMonolith.Abstractions.Auth;

namespace Mamey.ApplicationName.Modules.Identity.Core.Storage;

internal interface IUserRequestStorage
{
    void SetToken(string email, JsonWebToken jwt);
    JsonWebToken GetToken(string email);
}