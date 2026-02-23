// using Mamey.ApplicationName.Modules.Identity.Blazor.Clients;
// using Mamey.ApplicationName.Modules.Identity.Contracts.Commands;
// using Mamey.ApplicationName.Modules.Identity.Contracts.Dto;
// using Mamey.Auth.Identity.Managers;
// using Mamey.Blazor.Identity;
// using Mamey.CQRS.Commands;
// using Login = Mamey.ApplicationName.Modules.Identity.Contracts.Commands.Login;
//
// namespace Mamey.ApplicationName.Modules.Identity.Blazor.Services;
//
// internal class IdentityAuthService : IIdentityAuthService
// {
//     private readonly IAuthenticationApiClient<Login> _api;
//
//     public IdentityAuthService(IAuthenticationApiClient<Login> api)
//     {
//         _api = api;
//     }
//
//     public Task ClearAuthenticationAsync(CancellationToken cancellationToken = default)
//         => _api.ClearAuthenticationAsync(cancellationToken);
//
//     public Task<MameySignInResult> LoginAsync(Login command, CancellationToken cancellationToken = default)
//         => _api.LoginAsync(command, cancellationToken);
//
//     public Task LogoutAsync(Logout command, CancellationToken cancellationToken = default)
//     {
//         throw new NotImplementedException();
//     }
// }