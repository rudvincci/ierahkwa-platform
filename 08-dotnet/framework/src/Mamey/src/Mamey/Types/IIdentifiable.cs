namespace Mamey.Types;

public interface IIdentifiable<out T>
{
    public T Id { get; }
}