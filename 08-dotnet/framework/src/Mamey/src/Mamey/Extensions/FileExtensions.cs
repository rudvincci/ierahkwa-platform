using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Xml.Serialization;
using Mamey.Types;

namespace Mamey;

public static class FileExtensions
{
    public static async Task SaveObjectToFileAsync<T>(this T obj, string filePath, SerializationFormat format = SerializationFormat.Json, bool append = false, JsonSerializerOptions jsonOptions = null)
    {
        try
        {
            string serializedData = SerializeObject(obj, format, jsonOptions);

            if (append)
            {
                await File.AppendAllTextAsync(filePath, serializedData + Environment.NewLine);
            }
            else
            {
                await File.WriteAllTextAsync(filePath, serializedData);
            }

            Console.WriteLine($"Object successfully saved to {filePath}.");
        }
        catch (IOException ioEx)
        {
            Console.WriteLine($"I/O error occurred while saving the object to file: {ioEx.Message}");
        }
        catch (UnauthorizedAccessException authEx)
        {
            Console.WriteLine($"Access error occurred while saving the object to file: {authEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred while saving the object to file: {ex.Message}");
        }
    }

    private static string SerializeObject<T>(this T obj, SerializationFormat format, JsonSerializerOptions jsonOptions)
    {
        switch (format)
        {
            case SerializationFormat.Xml:
                using (var stringWriter = new StringWriter())
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(stringWriter, obj);
                    return stringWriter.ToString();
                }

            case SerializationFormat.Binary:
                using (var memoryStream = new MemoryStream())
                {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, obj);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }

            case SerializationFormat.Json:
            default:
                return JsonSerializer.Serialize(obj, jsonOptions ?? new JsonSerializerOptions { WriteIndented = true });
        }
    }

    public static string GetFileNameWithTimestamp(string baseName, string extension)
    {
        return $"{baseName}_{DateTime.Now:yyyyMMdd_HHmmss}.{extension}";
    }

}
