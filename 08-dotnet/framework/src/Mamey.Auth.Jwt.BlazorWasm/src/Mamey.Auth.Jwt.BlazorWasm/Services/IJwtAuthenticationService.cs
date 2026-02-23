// MyApp.Client/Services/ReactiveAuthenticationService.cs


namespace Mamey.Auth.Jwt.BlazorWasm.Services;

public interface IJwtAuthenticationService : IAuthenticationService
{
    public AuthenticatedUser AuthenticatedUser { get;  }
}