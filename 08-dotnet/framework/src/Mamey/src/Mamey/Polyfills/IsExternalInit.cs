// Polyfill for records and init-only properties in netstandard2.1
// See: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/init
#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
#endif
