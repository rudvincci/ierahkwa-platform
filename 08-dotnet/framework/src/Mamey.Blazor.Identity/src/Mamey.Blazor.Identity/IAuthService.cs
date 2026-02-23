using Mamey.Auth.Identity;
using Mamey.CQRS.Commands;

namespace Mamey.Blazor.Identity;

public interface IAuthService<in TLoginCommand, in TLogoutCommand> 
    where TLoginCommand : class, ICommand
    where TLogoutCommand : class, ICommand
{
    Task ClearAuthenticationAsync(CancellationToken none);
    Task<MameySignInResult> LoginAsync(TLoginCommand command, CancellationToken cancellationToken = default);
    Task LogoutAsync(TLogoutCommand command, CancellationToken cancellationToken = default);
}