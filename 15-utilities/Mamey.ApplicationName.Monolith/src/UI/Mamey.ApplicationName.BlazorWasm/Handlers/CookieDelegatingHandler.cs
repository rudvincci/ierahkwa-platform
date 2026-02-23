using Mamey.ApplicationName.BlazorWasm.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
namespace Mamey.ApplicationName.BlazorWasm.Handlers;

public class CookieDelegatingHandler : DelegatingHandler
{
    private readonly ILogger<CookieDelegatingHandler> _logger;
    private readonly CookieService _cookieService;
    public CookieDelegatingHandler(ILogger<CookieDelegatingHandler> logger, CookieService cookieService)
    {
        _logger = logger;
        _cookieService = cookieService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        var cookie = await _cookieService.GetCookieAsync("__accessToken");
        Console.WriteLine($"Retrieved cookie: {cookie}");
        return await base.SendAsync(request, cancellationToken);
    }
}
