using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace Mamey.Microservice.Infrastructure.Azure
{
    internal static class Extensions
    {
        private const string SectionName = "azureAdB2C";
        public static IServiceCollection AddAzureADB2C(this IServiceCollection services, IConfiguration configuration, string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = SectionName;
            }

            var validAudiences = configuration.GetSection($"{SectionName}:validAudiences").Get<string[]>() ?? Array.Empty<string>();
            var clientId = configuration[$"{SectionName}:clientId"];
            if (!string.IsNullOrEmpty(clientId))
            {
                validAudiences = validAudiences.Append(clientId).ToArray();
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                SaveSigninToken = false,
                ValidateActor = false,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = false,
                ValidateLifetime = true,
                ValidAudiences = validAudiences
            };

            // Adds Microsoft Identity platform (Azure AD B2C) support to protect this Api
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApi(options =>
                    {
                        configuration.Bind(sectionName, options);

                        options.TokenValidationParameters.NameClaimType = "name";
                    },
            options => { configuration.Bind(sectionName, options); });
            // End of the Microsoft Identity platform block


            services.AddControllers();
            return services;
        }
    }
}

