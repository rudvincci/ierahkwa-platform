namespace Mamey.Net;

internal class ServiceId : IServiceId
{
    public string Id { get; } = $"{Guid.NewGuid():N}";
}