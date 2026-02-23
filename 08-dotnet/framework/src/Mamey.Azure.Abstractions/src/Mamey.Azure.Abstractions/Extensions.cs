using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Azure.Abstractions;

public static class Extensions
{
    public static IMameyBuilder AddAzure(this IMameyBuilder builder, string section = AzureOptions.APPSETTINGS_SECTION)
    {
        var options = builder.Services.GetOptions<AzureOptions>(section);
        builder.Services.AddSingleton(options);

        if (!options.Enabled)
        {
            return builder;
        }


        return builder;
    }
    public static IEnumerable<Claim> ParseClaimsFromJwt(this string jwt)
    {
        if (string.IsNullOrEmpty(jwt.Trim()))
        {
            throw new ArgumentException($"'{nameof(jwt)}' cannot be null or empty.", nameof(jwt));
        }

        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs?.Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString()));
    }
    private static byte[] ParseBase64WithoutPadding(this string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
