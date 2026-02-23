using Mamey.WebApi;
using Mamey.WebApi.Exceptions;

namespace Mamey.FWID.Notifications.Infrastructure.Exceptions;

internal class ExceptionToResponseMapper : IExceptionToResponseMapper
{
    public ExceptionResponse? Map(Exception exception)
        => exception switch
        {
            _ => null
        };
}







