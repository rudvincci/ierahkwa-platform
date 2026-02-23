using System.Reactive.Linq;
using Mamey.ApplicationName.Modules.Identity.Blazor.ViewModels.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Identity.Blazor.ViewModels.Providers;

public class LoginViewModelProvider
{
    public LoginViewModel? Instance { get; private set; }

    public async Task<LoginViewModel> GetAsync(IServiceProvider sp)
    {
        if (Instance == null)
        {
            Instance = ActivatorUtilities.CreateInstance<LoginViewModel>(sp);
        }
        return Instance;
    }
}