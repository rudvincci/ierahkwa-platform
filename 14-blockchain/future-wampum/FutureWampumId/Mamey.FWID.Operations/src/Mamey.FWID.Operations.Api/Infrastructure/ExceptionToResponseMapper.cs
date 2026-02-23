using System.Net;
using Mamey.WebApi.Exceptions;

namespace Mamey.FWID.Operations.Api.Infrastructure;

internal sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
{
    public ExceptionResponse Map(Exception exception)
        => exception switch
        {
            _ => new ExceptionResponse(new { code = "error", reason = "There was an error." },
                HttpStatusCode.BadRequest)
        };
}



