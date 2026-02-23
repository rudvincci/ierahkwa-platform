// StrongIds.cs

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mamey.Identity.AspNetCore.Data;

public sealed class StrongIdConverter<TStrong> : ValueConverter<TStrong, Guid>
    where TStrong : notnull
{
    public StrongIdConverter()
        : base(ToProvider(), FromProvider()) { }

    private static Expression<Func<TStrong, Guid>> ToProvider()
    {
        var t   = typeof(TStrong);
        var val = t.GetProperty("Value") ?? throw new InvalidOperationException($"{t.Name}.Value missing");
        var p   = Expression.Parameter(t, "v");
        return Expression.Lambda<Func<TStrong, Guid>>(Expression.Property(p, val), p);
    }

    private static Expression<Func<Guid, TStrong>> FromProvider()
    {
        var t    = typeof(TStrong);
        var ctor = t.GetConstructor(new[] { typeof(Guid) }) ?? throw new InvalidOperationException($"{t.Name} needs ctor(Guid)");
        var p    = Expression.Parameter(typeof(Guid), "g");
        return Expression.Lambda<Func<Guid, TStrong>>(Expression.New(ctor, p), p);
    }
}

public sealed class StrongIdComparer<TStrong> : ValueComparer<TStrong>
    where TStrong : notnull
{
    public StrongIdComparer()
        : base(
            (l, r) => GuidEquals(l, r),
            v => GuidHash(v),
            v => v) { } // immutable, shallow copy OK

    private static Guid ToGuid(TStrong v)
    {
        var value = (string?)((StrongIdConverter<TStrong>)Converter).ConvertToProvider(v);
        if (value != null) 
            return Guid.Parse(value);
        return Guid.Empty;
    }

    private static bool GuidEquals(TStrong? l, TStrong? r)
    {
        if (ReferenceEquals(l, r)) return true;
        if (l is null || r is null) return false;
        return ToGuid(l).Equals(ToGuid(r));
    }

    private static int GuidHash(TStrong? v)
        => v is null ? 0 : ToGuid(v).GetHashCode();

    private static readonly ValueConverter<TStrong, Guid> Converter = new StrongIdConverter<TStrong>();
}

