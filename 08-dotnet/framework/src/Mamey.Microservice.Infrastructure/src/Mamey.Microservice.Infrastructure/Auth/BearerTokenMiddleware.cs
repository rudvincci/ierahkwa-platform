using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace Mamey.Microservice.Infrastructure.Auth;
public class BearerTokenMiddleware : IMiddleware
{
    public BearerTokenMiddleware()
    {

    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
         // Get the authorization header from the request
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader))
        {
            var result = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            if(result.Principal is not null)
            {
                context.User = result.Principal;
            }           
        }
     
        // Get the device information from the request
        var deviceInfo = new
        {
            UserAgent = context.Request.Headers["User-Agent"],
            RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),
            RemotePort = context.Connection.RemotePort,
            LocalIpAddress = context.Connection.LocalIpAddress?.ToString(),
            LocalPort = context.Connection.LocalPort,
            OriginatingCountry = context.Request.Headers["CF-IPCountry"],
            MacAddress = "TODO: implement MAC address retrieval",
            OperatingSystem = "TODO: implement operating system retrieval"
        };

        // Add clickstream data to the context
        context.Items["ClickstreamData"] = new
        {
            Url = context.Request.Path.Value,
            Method = context.Request.Method,
            DeviceInfo = deviceInfo
        };

        // Call the next middleware in the pipeline
        await next(context);
    }
}
