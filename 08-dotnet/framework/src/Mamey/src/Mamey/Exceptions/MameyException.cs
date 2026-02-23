namespace Mamey.Exceptions;

public class MameyException : Exception
{
    public MameyException()
        : base("Mamey Exception thrown")
    {
    }
    public MameyException(string message)
        : base(message)
    {
    }
    
    [JsonConstructor]
    public MameyException(string code, string reason) : base(String.Empty)
        => (Code, Reason) = (code, reason);
    public MameyException(string message, string code, string reason) : base(message)
        => (Code, Reason) = (code, reason);
    public MameyException(string message, string code, string reason, Exception innerException) 
        : base(message, innerException) 
        => (Code, Reason) = (code, reason);

    public virtual string? Code { get; }
    public virtual string? Reason { get; }
}
