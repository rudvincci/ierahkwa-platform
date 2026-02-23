using System.Net;
using Mamey.WebApi.Exceptions;

namespace INKG.CitizenPortal.ApiGataway.Infrastructure;

internal sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
{
    public ExceptionResponse Map(Exception exception)
        => exception switch
        {
            _ => new ExceptionResponse(new { code = "error", reason = "There was an error." },
                HttpStatusCode.BadRequest)
        };
}
