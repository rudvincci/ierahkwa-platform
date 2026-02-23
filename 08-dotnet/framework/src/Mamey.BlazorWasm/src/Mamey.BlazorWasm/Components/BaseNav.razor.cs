using Mamey.BlazorWasm.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;

namespace Mamey.BlazorWasm.Components;

public partial class BaseNav : ComponentBase, IBrowserViewportObserver, IAsyncDisposable
{
    [Inject]
    private IBrowserViewportService BrowserViewportService { get; set; } = default!;

    [Inject]
    public NavigationManager? NavigationManager { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    [Parameter]
    public List<Route> Routes { get; set; } = [];

    [Parameter]
    public bool DrawerOpen
    {
        get => _drawerOpen;
        set
        {
            if (_drawerOpen != value)
            {
                _drawerOpen = value;
                DrawerOpenChanged.InvokeAsync(value);
            }
        }
    }

    [Parameter]
    public EventCallback<bool> DrawerOpenChanged { get; set; }

    [Parameter]
    public bool DrawerMiniEnabled { get; set; }

    private bool _drawerOpen;
    private readonly Dictionary<string, bool> _expandedStates = new();
    private AuthenticationState? _authState;

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationStateTask is not null)
        {
            _authState = await AuthenticationStateTask;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await BrowserViewportService.SubscribeAsync(this, fireImmediately: true);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    // ── Role-based filtering ──

    /// <summary>
    /// Filter top-level routes by RequiredRoles.
    /// Routes with empty RequiredRoles are always visible.
    /// </summary>
    private List<Route> FilterRoutes(List<Route> routes)
    {
        return routes.Where(IsVisible).ToList();
    }

    /// <summary>
    /// Get visible children of a route, filtered by RequiredRoles.
    /// </summary>
    private List<Route> GetVisibleChildren(Route route)
    {
        return route.ChildRoutes.Where(IsVisible).ToList();
    }

    /// <summary>
    /// A route is visible if:
    /// - No RequiredRoles and no AuthenticationRequired → always visible (public)
    /// - AuthenticationRequired but no RequiredRoles → visible to any authenticated user
    /// - RequiredRoles → visible only if user has at least one of them
    /// </summary>
    private bool IsVisible(Route route)
    {
        var isAuthenticated = _authState?.User?.Identity?.IsAuthenticated == true;

        // Auth required but not logged in → hide
        if (route.AuthenticationRequired && !isAuthenticated)
            return false;

        // No role restriction → visible (public or just auth-gated)
        if (route.RequiredRoles.Count == 0)
            return true;

        // Has role restriction but not authenticated → hide
        if (!isAuthenticated)
            return false;

        // Check if user has any of the required roles
        return route.RequiredRoles.Any(role => _authState!.User.IsInRole(role));
    }

    // ── Navigation ──

    private void NavigateToRoute(Route route)
    {
        if (!string.IsNullOrEmpty(route.Page) && NavigationManager != null)
        {
            NavigationManager.NavigateTo(route.Page);
        }
    }

    private void OnExpandedChanged(string? pagePrefix, bool expanded)
    {
        if (!string.IsNullOrEmpty(pagePrefix))
        {
            _expandedStates[pagePrefix] = expanded;
        }
    }

    public bool IsNavGroupExpanded(string? pagePrefix)
    {
        if (string.IsNullOrEmpty(pagePrefix))
            return false;

        if (NavigationManager is null)
            return false;

        if (_expandedStates.TryGetValue(pagePrefix, out bool manualState))
            return manualState;

        var relativePath = NavigationManager.Uri
            .Replace(NavigationManager.BaseUri.TrimEnd('/'), string.Empty);

        return relativePath.Contains(pagePrefix);
    }

    // ── Viewport observer ──

    public Task NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs browserViewportEventArgs)
    {
        return InvokeAsync(StateHasChanged);
    }

    ResizeOptions IBrowserViewportObserver.ResizeOptions { get; } = new()
    {
        NotifyOnBreakpointOnly = true,
    };

    Guid IBrowserViewportObserver.Id { get; } = Guid.NewGuid();

    public async ValueTask DisposeAsync() => await BrowserViewportService.UnsubscribeAsync(this);
}
