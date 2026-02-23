using Microsoft.AspNetCore.Http;

namespace Mamey.Identity.AspNetCore.Managers;

public interface IIdentityRedirectManager
{
    void RedirectTo(string? uri, bool forceLoad = false);
    void RedirectTo(string uri, Dictionary<string, object?> queryParameters);
    void RedirectToWithStatus(string uri, string message, HttpContext context);
    void RedirectToCurrentPage();
    void RedirectToCurrentPageWithStatus(string message, HttpContext context);
}

