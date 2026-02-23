using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Extensions.Primitives;

namespace Mamey;

public static class CollectionExtensions
{
    public static Dictionary<string, string?> ToDictionary<T>(this T obj) where T : class
            => obj.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(obj, null));

    public static IReadOnlyDictionary<string, object?> ToDictionary(this object obj)
        => (IReadOnlyDictionary<string, object?>)obj.GetType()
        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));

    public static IEnumerable<KeyValuePair<string, StringValues>> ToKeyValuePairList<T>(this T me) where T : class
    {
        List<KeyValuePair<string, StringValues>> result = new List<KeyValuePair<string, StringValues>>();
        foreach (var property in me.GetType().GetProperties())
        {
            result.Add(new KeyValuePair<string, StringValues>(property.Name.ToCamelCase(), property.GetValue(me)?.ToString()));
        }
        return result;
    }
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
        return new ObservableCollection<T>(source);
    }
}
