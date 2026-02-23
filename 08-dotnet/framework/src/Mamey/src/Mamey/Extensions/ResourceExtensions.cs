using System.Globalization;
using System.IO;
using System.Reflection;
//using System.Text.Json;
using System.Xml.Serialization;
using CsvHelper;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Security.Cryptography;
using Google.Protobuf;
using System.Text.Json;
//using Newtonsoft.Json;

namespace Mamey;

public static class ResourceExtensions
{
    public static async Task<T> GetJsonFromEmbeddedResourceAsync<T>(string resourcePath, string assemblyName)
    {
        // Validate input
        if (string.IsNullOrEmpty(resourcePath))
            throw new ArgumentException("Resource path cannot be null or empty.", nameof(resourcePath));

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var assembly = assemblies.SingleOrDefault(c => c.GetName().Name == assemblyName);
        var resourceNames = assembly.GetManifestResourceNames();
        //var assembly = Assembly.Load(assemblyName);

        //var resourceNames = assembly.GetManifestResourceNames();

        
        
        // Find the embedded resource stream
        try
        {

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                    throw new InvalidOperationException($"Resource {resourcePath} not found.");

                // Read the stream asynchronously into a string
                using (StreamReader reader = new StreamReader(stream))
                {
                    string jsonText = await reader.ReadToEndAsync();

                    // Deserialize the JSON string into the specified type T
                    //T result = JsonConvert.DeserializeObject<T>(jsonText);
                   T result = JsonSerializer.Deserialize<T>(jsonText, JsonExtensions.SerializerOptions);

                    if (result == null)
                        throw new InvalidOperationException("Failed to deserialize the JSON content.");

                    return result;
                }
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    public static async Task<T> GetXmlFromEmbeddedResourceAsync<T>(string resourcePath)
    {
        var assembly =  Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
        {

            if (stream == null)
                throw new InvalidOperationException($"Resource {resourcePath} not found.");

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }
    }
    public static async Task<string> GetTextFromEmbeddedResourceAsync(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
        {
            if (stream == null)
                throw new InvalidOperationException($"Resource {resourcePath} not found.");

            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
    public static async Task<byte[]> GetBinaryFromEmbeddedResourceAsync(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
        {
            if (stream == null)
                throw new InvalidOperationException($"Resource {resourcePath} not found.");

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
    public static async Task<List<T>> GetCsvFromEmbeddedResourceAsync<T>(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
        {
            if (stream == null)
                throw new InvalidOperationException($"Resource {resourcePath} not found.");

            using (StreamReader reader = new StreamReader(stream))
            using (CsvReader csvReader = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                var records = new List<T>();
                await csvReader.ReadAsync();
                csvReader.ReadHeader();
                while (await csvReader.ReadAsync())
                {
                    records.Add(csvReader.GetRecord<T>());
                }
                return records;
            }
        }
    }
    // Previous methods for JSON, XML, Text, Binary, and CSV are assumed to be included

    public static async Task<Dictionary<string, string>> GetPropertiesFromEmbeddedResourceAsync(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
        {
            if (stream == null)
                throw new InvalidOperationException($"Resource {resourcePath} not found.");


            using (StreamReader reader = new StreamReader(stream))
            {
                var properties = new Dictionary<string, string>();
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                    {
                        var splitIndex = line.IndexOf('=');
                        if (splitIndex > 0)
                        {
                            string key = line.Substring(0, splitIndex).Trim();
                            string value = line.Substring(splitIndex + 1).Trim();
                            properties[key] = value;
                        }
                    }
                }
                return properties;
            }
        }
    }

    public static async Task<T> GetYamlFromEmbeddedResourceAsync<T>(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
        {
            if (stream == null)
                throw new InvalidOperationException($"Resource {resourcePath} not found.");

            using (StreamReader reader = new StreamReader(stream))
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                    .Build();

                var yamlObject = deserializer.Deserialize<T>(reader);
                return yamlObject;
            }
        }
    }
    public static async Task<byte[]> GetEncryptedResourceAsync(string resourcePath, byte[] key, byte[] iv)
    {
        var assembly = Assembly.GetExecutingAssembly();
        if (key == null || iv == null)
            throw new ArgumentNullException("Key and IV must not be null.");

        if (key.Length != 16 || iv.Length != 16)
            throw new ArgumentException("Key and IV must be 16 bytes long.");

        using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
        {
            if (stream == null)
                throw new FileNotFoundException($"The resource '{resourcePath}' was not found in the assembly.");

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                try
                {
                    using (CryptoStream cryptoStream = new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        var result = new MemoryStream();
                        await cryptoStream.CopyToAsync(result);
                        return result.ToArray();
                    }
                }
                catch (CryptographicException e)
                {
                    throw new InvalidOperationException("Failed to decrypt the resource.", e);
                }
            }
        }
    }
    public static async Task<T> GetProtoBufFromEmbeddedResourceAsync<T>(string resourcePath) where T : class, IMessage<T>, new()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream rawStream = assembly.GetManifestResourceStream(resourcePath))
        {
            if (rawStream == null)
                throw new FileNotFoundException($"The resource '{resourcePath}' was not found in the assembly.");

            using (BufferedStream bufferedStream = new BufferedStream(rawStream))
            {
                T result = new T();
                result.MergeFrom(bufferedStream);
                return result;
            }
        }
    }
    public static async Task<IDictionary<string, object>> GetJsonLdFromEmbeddedResourceAsync(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
        {
            if (stream == null)
            {
                throw new FileNotFoundException($"The resource '{resourcePath}' was not found in the assembly.");

            }
            using (StreamReader reader = new StreamReader(stream))
            {
                var jsonText = await reader.ReadToEndAsync();
                //return JsonConvert.DeserializeObject<IDictionary<string, object>>(jsonText, JsonExtensions.SerializerSettings);
                return JsonSerializer.Deserialize<IDictionary<string, object>>(jsonText, JsonExtensions.SerializerOptions);
            }
        }
    }
    public static async Task<string> GetMarkdownFromEmbeddedResourceAsync(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
        {
            if (stream == null)
                throw new FileNotFoundException($"The resource '{resourcePath}' was not found in the assembly.");

            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
    public static async Task<IDictionary<string, string>> GetEnvironmentVariablesFromEmbeddedResourceAsync(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
        {
            if (stream == null)
                throw new FileNotFoundException($"The resource '{resourcePath}' was not found in the assembly.");

            using (StreamReader reader = new StreamReader(stream))
            {
                var envVars = new Dictionary<string, string>();
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line) && line.Contains("="))
                    {
                        var split = line.Split(new char[] { '=' }, 2);
                        if (split.Length == 2)
                        {
                            envVars[split[0].Trim()] = split[1].Trim();
                        }
                    }
                }
                return envVars;
            }
        }
    }

}
