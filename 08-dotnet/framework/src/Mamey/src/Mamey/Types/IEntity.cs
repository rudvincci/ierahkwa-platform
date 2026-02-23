namespace Mamey.Types;

public interface IEntity<T>
{
    [Key]
    T Id { get; }
}