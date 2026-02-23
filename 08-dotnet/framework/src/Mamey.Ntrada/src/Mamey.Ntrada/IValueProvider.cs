using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Mamey.Ntrada
{
    internal interface IValueProvider
    {
        IEnumerable<string> Tokens { get; }
        string Get(string value, HttpRequest request, RouteData data);
    }
}