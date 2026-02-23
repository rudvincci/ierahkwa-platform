using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Mamey;

public static class JsonExtensions
{
    public static readonly JsonSerializerOptions? SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = {
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase), // Single converter for enum
        },
        //WriteIndented = true,
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        // Uncomment if circular references are expected
        ReferenceHandler = ReferenceHandler.Preserve,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        IncludeFields = true,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers = { AddNestedDerivedTypes }
        }
    };

    /// <summary>
    /// Serializer options for Redis cache operations.
    /// Does not use ReferenceHandler.Preserve to avoid issues with constructor parameter deserialization.
    /// </summary>
    public static readonly JsonSerializerOptions? CacheSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = {
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase), // Single converter for enum
        },
        //WriteIndented = true,
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        // Use IgnoreCycles instead of Preserve to avoid reference metadata issues with constructor parameters
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        IncludeFields = true,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers = { AddNestedDerivedTypes }
        }
    };
    public static void AddNestedDerivedTypes(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.PolymorphismOptions is null) return;

        var derivedTypes = jsonTypeInfo.PolymorphismOptions.DerivedTypes
            .Where(t => Attribute.IsDefined(t.DerivedType, typeof(JsonDerivedTypeAttribute)))
            .Select(t => t.DerivedType)
            .ToList();
        var hashset = new HashSet<Type>(derivedTypes);
        var queue = new Queue<Type>(derivedTypes);
        while (queue.TryDequeue(out var derived))
        {
            if (!hashset.Contains(derived))
            {
                // Todo: handle discriminators
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(derived, derived.FullName));
                hashset.Add(derived);
            }

            var attribute = derived.GetCustomAttributes<JsonDerivedTypeAttribute>();
            foreach (var jsonDerivedTypeAttribute in attribute) queue.Enqueue(jsonDerivedTypeAttribute.DerivedType);
        }
    }
    public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
    {
        // Property names are case insensitive
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy
            {
                ProcessDictionaryKeys = true,
                OverrideSpecifiedNames = true
            }
        },

        // Handle enums as strings
        Converters = new List<Newtonsoft.Json.JsonConverter>()
        {
            new StringEnumConverter(),
            new StringEnumConverter(new CamelCaseNamingStrategy())
        },

        // Indent JSON output (uncomment if needed)
        //Formatting = Formatting.Indented,

        // Preserve references (uncomment if needed)
        //PreserveReferencesHandling = PreserveReferencesHandling.Objects,

        // Allow trailing commas in JSON
        MissingMemberHandling = MissingMemberHandling.Ignore,
        
        // Skip comments in JSON
        //CommentHandling = CommentHandling.Ignore,

        // Include fields (Note: Newtonsoft.Json does not have a direct equivalent setting)
        // You will need to add fields to serialization manually if needed
    };

    public static async Task<T?> ReadAndDeserializeJsonAsync<T>(this string filePath)
    {
        try
        {

            if (File.Exists(filePath))
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (stream is null)
                    {
                        throw new FileNotFoundException("File not found", filePath);
                    }
                    return await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream, SerializerOptions);
                }
            }
            else
            {
                return default;
            }
        }
        catch (Exception ex)
        {
            throw;
        }
        
    }
}
