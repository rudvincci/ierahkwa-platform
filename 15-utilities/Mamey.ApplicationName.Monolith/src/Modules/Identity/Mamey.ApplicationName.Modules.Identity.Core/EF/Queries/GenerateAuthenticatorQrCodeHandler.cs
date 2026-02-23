using System.Text.Json;
using System.Web;
using Mamey.ApplicationName.Modules.Identity.Core.Queries;
using Mamey.Auth.Identity.Entities;
using Mamey.Barcode;
using Mamey.CQRS.Queries;
using Mamey.Persistence.Redis;
using Mamey.Types;
using Microsoft.AspNetCore.Identity;

namespace Mamey.ApplicationName.Modules.Identity.Core.EF.Queries;

internal sealed class GenerateAuthenticatorQrCodeHandler : IQueryHandler<GenerateAuthenticatorQrCode, byte[]?>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBarcodeService _barcodeService;
    private readonly ICache _cache;
    private readonly AppOptions _options;
    public async Task<byte[]?> HandleAsync(GenerateAuthenticatorQrCode command, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"user:{command.UserId}";
        var cachedUser = await _cache.GetAsync<string>(cacheKey);
        // TODO: decrypt user
        var user = JsonSerializer.Deserialize<ApplicationUser>(cachedUser);
        if (user == null)
        {
            user = await _userManager.FindByIdAsync(command.UserId.ToString());
            if (user == null)
            {
                return null;
            }
        }
        
        var key = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(key)) return null;

        // Format: otpauth://totp/AppName:Email?secret=KEY&issuer=AppName&digits=6
        var issuer = HttpUtility.UrlEncode(_options.Domain);
        var email = HttpUtility.UrlEncode(user.Email);
        var uri = $"otpauth://totp/{issuer}:{email}?secret={key}&issuer={issuer}&digits=6";
        return await _barcodeService.GenerateQRCodeAsync(uri);
    }
}