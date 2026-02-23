// using Mamey.Contexts;
// using Mamey.Exceptions;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Primitives;
//
// namespace Mamey.Net.Middlewares;
//
// public class OrganizationMiddleware : IMiddleware
// {
//     public OrganizationMiddleware()
//     {
//     }
//
//     public async Task InvokeAsync(HttpContext context, RequestDelegate next)
//     {
//         Guid organizationId;
//
//         //ctx.Request.
//         StringValues orgHeaders;
//         var ictx = context.RequestServices.GetRequiredService<IContext>();
//         if (!ictx.Identity.OrganizationId.Equals(Guid.Empty))
//         {
//             if (!context.Request.Headers.TryGetValue("X-ORG", out orgHeaders) && orgHeaders.Count > 0)
//             {
//                 throw new MameyException("Invalid header 'X-ORG' ");
//             }
//             if (Guid.TryParse(orgHeaders.FirstOrDefault(), out organizationId))
//             {
//                 ictx.Identity.SetOrganizationId(organizationId);
//             }
//         }
//         await next(context);
//     }
// }
//
//
