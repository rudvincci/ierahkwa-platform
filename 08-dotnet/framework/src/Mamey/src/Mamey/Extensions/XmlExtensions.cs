using System.IO;
using System.Xml.Serialization;

namespace Mamey;

public static class XmlExtensions
{
    public static T Deserialize<T>(string xml)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (StringReader reader = new StringReader(xml))
        {
            return (T)serializer.Deserialize(reader);
        }
    }

    public static string Serialize<T>(T obj)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (StringWriter writer = new StringWriter())
        {
            serializer.Serialize(writer, obj);
            return writer.ToString();
        }
    }
}
