using System.Net;
using Microsoft.AspNetCore.Http;

namespace Mamey.BlazorWasm.Middleware;

public class IPMiddleware : IMiddleware
{
    public IPMiddleware()
    {
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var hostName = Dns.GetHostName();
        IPHostEntry clientIp = Dns.GetHostEntry(hostName);
        IPAddress[] address = clientIp.AddressList;
        for (int i = 0; i < address.Length; i++)
        {
            Console.WriteLine($"IP Address${i}: {address[i]}");
            // Call fn: ProcessIpAddress(address, userid?, organizationId?)
 
        }
        
        throw new NotImplementedException();
    }
}

