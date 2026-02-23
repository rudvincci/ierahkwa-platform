using Mamey.WebApi;
using Mamey.WebApi.Exceptions;

namespace Mamey.FWID.Sagas.Core.Exceptions;

internal sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
{
    public ExceptionResponse Map(Exception exception)
        => exception switch
        {
            _ => new ExceptionResponse(exception.Message, System.Net.HttpStatusCode.InternalServerError)
        };
}



