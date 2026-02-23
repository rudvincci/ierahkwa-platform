using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Mamey.BlazorWasm.Components;

public partial class ProfileMenu
{
    [Inject]
    private NavigationManager _navigationManager { get; set; }

    [Inject]
    public AuthenticationStateProvider _stateProvider { get; set; }

    public event Action<AuthenticationStateProvider>? OnSignOut;

    private async Task SignOutAsync()
    {
        OnSignOut?.Invoke(_stateProvider);
        Console.WriteLine("Invoked SignedOut Action");
        
        await Task.CompletedTask;
    }
}

