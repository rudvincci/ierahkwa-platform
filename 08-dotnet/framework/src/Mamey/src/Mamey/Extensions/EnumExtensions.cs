using System.Reflection;

namespace Mamey;

public static class EnumExtensions
{
    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
    public static T ToEnumByShortName<T>(this string shortName) where T : Enum
    {
        var type = typeof(T);
        if (!type.IsEnum) throw new ArgumentException("Type must be an enum");

        foreach (var field in type.GetFields())
        {
            var attribute = field.GetCustomAttribute<DisplayAttribute>();
            if (attribute != null && attribute.ShortName == shortName)
            {
                return (T)field.GetValue(null);
            }
        }
        throw new ArgumentException($"No matching enum value found for short name: {shortName}");
    }
    public static string? GetEnumDisplayName(this Enum enumType)
    {
        return enumType.GetType().GetMember(enumType.ToString())
                       .First()
                       ?.GetCustomAttribute<DisplayAttribute>()
                       ?.Name;
    }
    public static string? GetEnumShortNameDisplayAttribute(this Enum enumType)
    {
        return enumType.GetType().GetMember(enumType.ToString())
                       .First()
                       ?.GetCustomAttribute<DisplayAttribute>()
                       ?.ShortName;
    }
    public static string? GetEnumDescriptionAttribute(this Enum enumType)
    {
        return enumType.GetType().GetMember(enumType.ToString())
                       .First()
                       ?.GetCustomAttribute<DescriptionAttribute>()
                       ?.Description;
    }
}
