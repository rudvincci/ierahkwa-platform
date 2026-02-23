using System.Diagnostics.CodeAnalysis;
using Mamey.Auth.Identity.Managers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Mamey.Blazor.Identity;

public sealed class IdentityRedirectManager : IIdentityRedirectManager
{
    public const string StatusCookieName = "Identity.StatusMessage";

    private static readonly CookieBuilder StatusCookieBuilder = new()
    {
        SameSite   = SameSiteMode.Strict,
        HttpOnly   = true,
        IsEssential= true,
        MaxAge     = TimeSpan.FromSeconds(5),
    };

    private readonly NavigationManager _navigationManager;

    public IdentityRedirectManager(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    /// <summary>
    /// Blazor‐only redirect. Throws after calling NavigateTo so the prerenderer
    /// will treat it as a redirect instead of completely rendering the component.
    /// </summary>
    [DoesNotReturn]
    public void RedirectTo(string? uri, bool forceLoad = false)
    {
        uri ??= "";

        // prevent open‐redirect
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            uri = "/" + _navigationManager.ToBaseRelativePath(uri);
        }

        // interactive client redirect
        _navigationManager.NavigateTo(uri, forceLoad);
        // throw new InvalidOperationException($"{nameof(IdentityRedirectManager)} can only be used during static rendering.");
    }

    /// <summary>
    /// Blazor‐only redirect with added query params.
    /// </summary>
    [DoesNotReturn]
    public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
    {
        var baseUri = _navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri  = _navigationManager.GetUriWithQueryParameters(baseUri, queryParameters);
        RedirectTo(newUri);
    }

    /// <summary>
    /// *Server‐side* redirect that also sets a temporary status‐message cookie.
    /// </summary>
    [DoesNotReturn]
    public void RedirectToWithStatus(string uri, string message, HttpContext context)
    {
        // set the status‐cookie
        context.Response.Cookies.Append(
            StatusCookieName,
            message,
            StatusCookieBuilder.Build(context));

        // do a true HTTP 302 redirect
        context.Response.Redirect(uri);
        throw new InvalidOperationException($"{nameof(IdentityRedirectManager)} can only be used during server‐side rendering.");
    }

    public string CurrentPath
        => _navigationManager.ToAbsoluteUri(_navigationManager.Uri)
                             .GetLeftPart(UriPartial.Path);

    /// <summary>Blazor‐only redirect to the same page.</summary>
    [DoesNotReturn]
    public void RedirectToCurrentPage() 
        => RedirectTo(CurrentPath);

    /// <summary>Server‐side redirect back to current page, with status.</summary>
    [DoesNotReturn]
    public void RedirectToCurrentPageWithStatus(string message, HttpContext context)
        => RedirectToWithStatus(CurrentPath, message, context);
}
