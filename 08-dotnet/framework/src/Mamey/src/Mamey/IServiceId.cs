namespace Mamey;

public interface IServiceId
{
    string Id { get; }
}
internal class ServiceId : IServiceId
{
    public string Id { get; } = $"{Guid.NewGuid():N}";
}