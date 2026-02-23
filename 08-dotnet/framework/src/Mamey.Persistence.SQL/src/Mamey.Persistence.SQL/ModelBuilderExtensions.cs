using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mamey.Persistence.SQL;

/// <summary>
/// EF Core model-building helpers for snake_case, UTC, strong IDs, and query filters.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Converts all table & column names to snake_case.
    /// Call last in <see cref="DbContext.OnModelCreating"/> (after configs).
    /// </summary>
    public static void UseSnakeCaseNamingConvention(this ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()!.ToSnakeCase());
            foreach (var prop in entity.GetProperties())
                prop.SetColumnName(prop.GetColumnName()!.ToSnakeCase());
            foreach (var key in entity.GetKeys())
                key.SetName(key.GetName()!.ToSnakeCase());
            foreach (var fk in entity.GetForeignKeys())
                fk.SetConstraintName(fk.GetConstraintName()!.ToSnakeCase());
            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(index.GetDatabaseName()!.ToSnakeCase());
        }
    }

    /// <summary>
    /// Converts <see cref="DateTime"/> and nullable DateTime to UTC in database.
    /// </summary>
    public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
    {
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v == null ? v : (v.Value.Kind == DateTimeKind.Utc ? v : v.Value.ToUniversalTime()),
            v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

        foreach (var prop in builder.Model.GetEntityTypes().SelectMany(t => t.GetProperties()))
        {
            if (prop.ClrType == typeof(DateTime))
                prop.SetValueConverter(dateTimeConverter);
            else if (prop.ClrType == typeof(DateTime?))
                prop.SetValueConverter(nullableDateTimeConverter);
        }
    }

    /// <summary>
    /// Adds a global query filter for any entity implementing <typeparamref name="TInterface"/>.
    /// Expression param is a lambda using a captured value (e.g., tenantId).
    /// </summary>
    public static void AddGlobalFilter<TInterface>(this ModelBuilder mb, Expression<Func<TInterface, bool>> filterExpression)
    {
        foreach (var entityType in mb.Model.GetEntityTypes().Where(et => typeof(TInterface).IsAssignableFrom(et.ClrType)))
        {
            var param = Expression.Parameter(entityType.ClrType, "e");
            var body = ReplacingExpressionVisitor.Replace(filterExpression.Parameters[0], param, filterExpression.Body);
            entityType.SetQueryFilter(Expression.Lambda(body, param));
        }
    }

    private static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        var sb = new System.Text.StringBuilder(input.Length + 10);
        var previousCategory = default(System.Globalization.UnicodeCategory?);

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsWhiteSpace(c))
                continue;

            var currentCategory = char.GetUnicodeCategory(c);
            if (currentCategory == System.Globalization.UnicodeCategory.UppercaseLetter &&
                previousCategory != System.Globalization.UnicodeCategory.SpaceSeparator &&
                previousCategory != null &&
                previousCategory != System.Globalization.UnicodeCategory.UppercaseLetter)
            {
                sb.Append('_');
            }

            sb.Append(char.ToLowerInvariant(c));
            previousCategory = currentCategory;
        }

        return sb.ToString();
    }public static ModelBuilder SetStrongIdsGuidClass(this ModelBuilder mb, params Type[] strongIdTypes)
    {
        foreach (var et in mb.Model.GetEntityTypes())
        {
            foreach (var p in et.GetProperties())
            {
                var t = p.ClrType;
                var match = strongIdTypes.FirstOrDefault(x => x == t);
                if (match is null) continue;

                var (conv, comp) = BuildConverterAndComparer(match);
                p.SetValueConverter(conv);
                p.SetValueComparer(comp);
            }
        }
        return mb;
    }

    private static (ValueConverter, ValueComparer) BuildConverterAndComparer(Type strongType)
    {
        // v => v.Value
        var valueProp = strongType.GetProperty("Value") 
                        ?? throw new InvalidOperationException($"{strongType.Name}.Value missing");
        var ctor = strongType.GetConstructor(new[] { typeof(Guid) }) 
                   ?? throw new InvalidOperationException($"{strongType.Name} needs ctor(Guid)");

        var vParam = Expression.Parameter(strongType, "v");
        var toBody = Expression.Property(vParam, valueProp);
        var toExpr = Expression.Lambda(toBody, vParam); // NOT statement body

        var gParam = Expression.Parameter(typeof(Guid), "g");
        var fromBody = Expression.New(ctor, gParam);
        var fromExpr = Expression.Lambda(fromBody, gParam);

        // Create generic ValueConverter<TStrong, Guid>
        var convType = typeof(ValueConverter<,>).MakeGenericType(strongType, typeof(Guid));
        var converter = (ValueConverter)Activator.CreateInstance(convType, toExpr, fromExpr)!;

        // Build comparer using compiled delegates (delegates are fine here)
        var to = (Func<object, Guid>)(obj => (Guid)toExpr.Compile().DynamicInvoke(obj)!);
        var from = (Func<Guid, object>)(g => fromExpr.Compile().DynamicInvoke(g)!);

        var comparerType = typeof(ValueComparer<>).MakeGenericType(strongType);
        var comparer = (ValueComparer)Activator.CreateInstance(
            comparerType,
            (Func<object?, object?, bool>)((l, r) =>
            {
                if (ReferenceEquals(l, r)) return true;
                if (l is null || r is null) return false;
                return to(l!).Equals(to(r!));
            }),
            (Func<object?, int>)(v => v is null ? 0 : to(v!).GetHashCode()),
            (Func<object?, object?>)(v => v is null ? null : from(to(v!)))
        )!;

        return (converter, comparer);
    }
}
/// <summary>
/// Auto-fills audit fields on entities implementing IAuditable&lt;UserId&gt;.
/// </summary>
