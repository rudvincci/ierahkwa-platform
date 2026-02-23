using System;

namespace Mamey.MicroMonolith.Abstractions.Exceptions;

public abstract class MameyException : Exception
{
    protected MameyException(string message) : base(message)
    {
    }

    protected MameyException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
