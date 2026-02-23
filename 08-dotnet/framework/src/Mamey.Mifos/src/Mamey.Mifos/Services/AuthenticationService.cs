using Mamey.Mifos.Commands.Authentication;
using Mamey.Mifos.Results;

namespace Mamey.Mifos.Services;

public class AuthenticationService : IAuthenticationService
{
    private string _url;
    private readonly IMifosApiClient _mifosApiClient;
    public AuthenticationService(MifosOptions options, IMifosApiClient mifosApiClient)
    {
        _url = $"{options.HostUrl}/api/v1/authentication";
        _mifosApiClient = mifosApiClient;
    }

    public async Task<IMifosResult<AuthenticateResponse>> AuthenticateAsync(Authenticate command)
        => await _mifosApiClient.SendAsync<Authenticate, AuthenticateResponse>(new MifosRequest<Authenticate>(_url, command));
}
public interface IAuthenticationService
{
    Task<IMifosResult<AuthenticateResponse>> AuthenticateAsync(Authenticate command);
}

