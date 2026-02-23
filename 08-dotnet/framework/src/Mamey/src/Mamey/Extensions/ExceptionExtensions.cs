namespace Mamey;

public static class ExceptionExtensions
{
    public static string GetExceptionCode(this Exception exception)
        => exception.GetType().Name.Underscore().Replace("_exception", string.Empty);
}
