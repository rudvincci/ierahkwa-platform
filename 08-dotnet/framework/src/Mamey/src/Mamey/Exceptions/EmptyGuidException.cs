namespace Mamey.Exceptions;

public class EmptyGuidException : MameyException
{
    public EmptyGuidException(string message, string paramName) 
        : base(message, "empty_guid", $"{paramName} is empty.")
    {
        ParamName = paramName;
    }
    public EmptyGuidException(string paramName) 
        : base($"Parameter Name: '{paramName}' is empty.", "empty_guid", $"{paramName} is empty.")
    {
        ParamName = paramName;
    }
    public string ParamName { get; }
}