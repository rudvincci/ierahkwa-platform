using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Mamey.Persistence.SQL;

public class DictionaryValueComparer : ValueComparer<Dictionary<string, object>>
{
    public DictionaryValueComparer()
        : base(
            (d1, d2) => true, // Dummy expressions to satisfy base
            d => 0,
            d => d)
    {
    }

    public override bool Equals(object? left, object? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;

        var d1 = (Dictionary<string, object>)left;
        var d2 = (Dictionary<string, object>)right;

        if (d1.Count != d2.Count) return false;

        foreach (var kvp in d1)
        {
            if (!d2.TryGetValue(kvp.Key, out var value2)) return false;
            if (!Equals(kvp.Value, value2)) return false;
        }

        return true;
    }

    public override int GetHashCode(object? obj)
    {
        if (obj == null) return 0;

        var dict = (Dictionary<string, object>)obj;
        var hash = new HashCode();

        foreach (var kvp in dict.OrderBy(k => k.Key))
        {
            hash.Add(kvp.Key);
            hash.Add(kvp.Value?.GetHashCode() ?? 0);
        }

        return hash.ToHashCode();
    }

    public override object? Snapshot(object? obj)
    {
        if (obj is Dictionary<string, object> dict)
        {
            return new Dictionary<string, object>(dict);
        }

        return null;
    }
}