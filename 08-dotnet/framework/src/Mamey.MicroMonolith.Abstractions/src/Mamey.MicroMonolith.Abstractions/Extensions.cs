using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;


namespace Mamey.MicroMonolith.Abstractions;

public static class Extensions
{
    public static string RemoveWhitespace(this string value)
        => string.IsNullOrWhiteSpace(value) ? value : Regex.Replace(value, @"\s+", string.Empty);
        
    public static async Task<T> NotNull<T>(this Task<T> task, Func<Exception> exception = null)
    {
        if (task is null)
        {
            throw new InvalidOperationException("Task cannot be null.");
        }
            
        var result = await task;
        if (result is not null)
        {
            return result;
        }

        if (exception is not null)
        {
            throw exception();

        }

        throw new InvalidOperationException("Returned result is null.");
    }
    public static DateTime GetDate(this long timestamp) => DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
    public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();

    public static string Underscore(this string value)
        => string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
                .ToLowerInvariant();

    public static string GetRandomHexColor()
        => $"#{new Random().Next(0x1000000):X6}";

    public static string GetExceptionCode(this Exception exception)
        => exception.GetType().Name.Underscore().Replace("_exception", string.Empty);

    public static T ToEnum<T>(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return default(T);
        return (T)Enum.Parse(typeof(T), value, true);
    }
    public static string GetEnumDisplayName(this Enum enumType)
    {
        return enumType.GetType().GetMember(enumType.ToString())
                       .First()
                       .GetCustomAttribute<DisplayAttribute>()
                       .Name;
    }

    public static string ToCamelCase(this string str)
    {
        var words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
        var leadWord = Regex.Replace(words[0], @"([A-Z])([A-Z]+|[a-z0-9]+)($|[A-Z]\w*)",
            m =>
            {
                return m.Groups[1].Value.ToLower() + m.Groups[2].Value.ToLower() + m.Groups[3].Value;
            });
        var tailWords = words.Skip(1)
            .Select(word => char.ToUpper(word[0]) + word.Substring(1))
            .ToArray();
        return $"{leadWord}{string.Join(string.Empty, tailWords)}";
    }

    
}