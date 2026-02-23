using System;
using Microsoft.AspNetCore.Routing;

namespace Mamey.Ntrada
{
    internal interface IRouteProvider
    {
        Action<IEndpointRouteBuilder> Build();
    }
}