using System.Collections.Concurrent;

namespace Mamey.Portal.Web.Auth;

public sealed class MockSessionStore
{
    private readonly ConcurrentDictionary<string, MockSessionData> _sessions = new(StringComparer.Ordinal);

    public MockSessionData GetOrCreate(string sessionId)
        => _sessions.GetOrAdd(sessionId, _ => new MockSessionData());

    public void Clear(string sessionId)
        => _sessions.TryRemove(sessionId, out _);
}

public sealed class MockSessionData
{
    public string Tenant { get; set; } = "default";
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}




