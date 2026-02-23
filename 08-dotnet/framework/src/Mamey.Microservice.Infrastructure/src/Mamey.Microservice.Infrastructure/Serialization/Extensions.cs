using System.Text.Json;

namespace Mamey.Microservice.Infrastructure.Serialization
{
    internal static class Extensions
    {
        public static IServiceCollection AddSerialization(this IServiceCollection services)
            => services
                .AddTransient<ISerializer, SystemTextJsonSerializer>();
    }

    internal interface ISerializer
    {
        string Serialize<T>(T value) where T : class;
        T? Deserialize<T>(string value) where T : class;
    }

    // TODO: Protobuf insteead?
    internal sealed class SystemTextJsonSerializer : ISerializer
    {
        public T? Deserialize<T>(string value) where T : class
            => JsonSerializer.Deserialize<T>(value, JsonExtensions.SerializerOptions);

        public string Serialize<T>(T value) where T : class
            => JsonSerializer.Serialize(value, JsonExtensions.SerializerOptions);

    }
}

