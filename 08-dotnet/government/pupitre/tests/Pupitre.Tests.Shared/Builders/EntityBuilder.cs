namespace Pupitre.Tests.Shared.Builders;

public abstract class EntityBuilder<TEntity, TBuilder>
    where TEntity : class
    where TBuilder : EntityBuilder<TEntity, TBuilder>
{
    protected Guid Id = Guid.NewGuid();
    protected string Name = "Test Entity";
    protected DateTime CreatedAt = DateTime.UtcNow;

    public TBuilder WithId(Guid id)
    {
        Id = id;
        return (TBuilder)this;
    }

    public TBuilder WithName(string name)
    {
        Name = name;
        return (TBuilder)this;
    }

    public TBuilder WithCreatedAt(DateTime createdAt)
    {
        CreatedAt = createdAt;
        return (TBuilder)this;
    }

    public abstract TEntity Build();
}

public class TestDataGenerator
{
    private static readonly Random Random = new();
    
    public static string RandomString(int length = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    public static string RandomEmail() => $"{RandomString(8)}@test.pupitre.edu";
    
    public static int RandomInt(int min = 0, int max = 100) => Random.Next(min, max);
    
    public static decimal RandomDecimal(decimal min = 0, decimal max = 100) 
        => min + (decimal)Random.NextDouble() * (max - min);

    public static DateTime RandomDate(int daysBack = 365) 
        => DateTime.UtcNow.AddDays(-Random.Next(daysBack));

    public static T RandomEnum<T>() where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(Random.Next(values.Length))!;
    }
}
