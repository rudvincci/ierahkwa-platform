using System.Net;

namespace Mamey.MicroMonolith.Abstractions.Exceptions;

public record ExceptionResponse(object Response, HttpStatusCode StatusCode);