
using Mamey.Auth;
using Mamey.MicroMonolith.Abstractions.Auth;
using Mamey.MicroMonolith.Abstractions.Storage;
using Mamey.Persistence.Redis;

namespace Mamey.ApplicationName.Modules.Identity.Core.Storage;

internal sealed class UserRequestStorage : IUserRequestStorage
{
    private readonly IRequestStorage _requestStorage;

    public UserRequestStorage(IRequestStorage requestStorage)
    {
        _requestStorage = requestStorage;
    }

    public void SetToken(string email, JsonWebToken jwt)
        => _requestStorage.Set(GetKey(email), jwt);

    public JsonWebToken GetToken(string email)
        => _requestStorage.Get<JsonWebToken>(GetKey(email));

    private static string GetKey(string email) => $"jwt:{email}";
}

