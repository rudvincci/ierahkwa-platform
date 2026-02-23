namespace Mamey.Serialization;

public interface ISerializer
{
    string Serialize<T>(T value) where T : class;
    T? Deserialize<T>(string value) where T : class;
    object Deserialize(string value, Type type);
}