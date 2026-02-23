using Mamey.Auth.Identity;
using Mamey.CQRS.Commands;

namespace Mamey.Blazor.Identity;

public interface IAuthenticationApiClient<in TLoginCommand> where TLoginCommand : class, ICommand
{
    Task<MameySignInResult> LoginAsync(TLoginCommand command, CancellationToken cancellationToken = default);
    Task ClearAuthenticationAsync(CancellationToken cancellationToken = default);
}