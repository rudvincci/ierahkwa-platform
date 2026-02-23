using System.Reactive.Linq;
using Mamey.Auth.Identity.Managers;
using Mamey.Government.Identity.BlazorWasm.Services;
using Mamey.Government.Identity.BlazorWasm.ViewModels.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.BlazorWasm.Components;

public partial class LoginPanel : ComponentBase
{
    private string? errorMessage;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;


    // private LoginInputModel SignInCommandInput { get; set; }

    [SupplyParameterFromQuery()]
    private string? ReturnUrl { get; set; }
    // [Inject] MameySignInManager SignInManager { get; set; }
    [Inject] ILogger<LoginPanel> Logger { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }
    [Inject] IIdentityRedirectManager RedirectManager { get; set; }
    [Inject] IAuthService AuthService { get; set; }
    [Inject] LoginViewModel LoginViewModel { get; set; }
    
    private EditContext _ctx;
    protected override async Task OnInitializedAsync()
    {
        _ctx = new EditContext(LoginViewModel.Input);
        _ctx.OnFieldChanged += (s, e) =>
            Logger?.LogInformation("Changed: {Field} -> User={User}, PassLen={Len}",
                e.FieldIdentifier.FieldName,
                LoginViewModel.Input.Username,
                LoginViewModel.Input.Password?.Length ?? 0);
        
        if (HttpContext is not null && HttpMethods.IsGet(HttpContext.Request.Method))
        {
            // Clear the existing external cookie to ensure a clean login process
            // await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoginViewModel.InitializeAsync.Execute();
        }
    }

    public async Task LoginUser()
    {
        // This doesn't count login failures towards account lockout
        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        await LoginViewModel.LoginAsync.Execute();
    }
}