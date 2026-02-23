using System.Collections.Concurrent;

namespace AdvocateOffice.Services;

public static class TokenStore
{
    private static readonly ConcurrentDictionary<string, int> Tokens = new();

    public static string Create(int userId)
    {
        var t = Guid.NewGuid().ToString("N");
        Tokens[t] = userId;
        return t;
    }

    public static bool Validate(string? token, out int userId)
    {
        userId = 0;
        if (string.IsNullOrWhiteSpace(token)) return false;
        return Tokens.TryGetValue(token.Trim(), out userId);
    }

    public static void Remove(string? token)
    {
        if (!string.IsNullOrWhiteSpace(token))
            Tokens.TryRemove(token.Trim(), out _);
    }
}
