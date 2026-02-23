namespace Mamey.Templates;

internal static class Guard
{
    public static void NotNull(object? o, string name)
    {
        if (o is null) throw new ArgumentNullException(name);
    }

    public static void NotNullOrWhiteSpace(string? s, string name)
    {
        if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException($"'{name}' cannot be null/empty.", name);
    }

    public static void Within(long actual, long max, string name)
    {
        if (actual > max) throw new ArgumentOutOfRangeException(name, $"Size {actual} exceeds limit {max} bytes.");
    }
}