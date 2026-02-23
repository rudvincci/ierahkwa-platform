using System;
using Mamey.MicroMonolith.Abstractions.Exceptions;

namespace Mamey.MicroMonolith.Infrastructure.Exceptions;

public interface IExceptionCompositionRoot
{
    ExceptionResponse Map(Exception exception);
}