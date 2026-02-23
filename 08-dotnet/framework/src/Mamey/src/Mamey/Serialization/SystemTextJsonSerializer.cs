using System.Text.Json;

namespace Mamey.Serialization;

public sealed class SystemTextJsonSerializer : ISerializer
{
    public T? Deserialize<T>(string value) where T : class
        => JsonSerializer.Deserialize<T>(value, JsonExtensions.SerializerOptions);

    public string Serialize<T>(T value) where T : class
        => JsonSerializer.Serialize(value, JsonExtensions.SerializerOptions);
        
    public object Deserialize(string value, Type type) => JsonSerializer.Deserialize(value, type, JsonExtensions.SerializerOptions);
}

