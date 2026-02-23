namespace Mamey.MicroMonolith.Abstractions.Contracts;

public interface IContract
{
    Type Type { get; }
    public IEnumerable<string> Required { get; }
}