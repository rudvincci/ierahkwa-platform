//using System;
//using System.ComponentModel.DataAnnotations;
//using System.Reflection;

//namespace Mamey.Microservice.Abstractions
//{
//    public static class Extensions
//    {
//        public static DateTime GetDate(this long timestamp) => DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
//        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
//            => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();

//        //public static string Underscore(this string value)
//        //    => string.IsNullOrWhiteSpace(value)
//        //        ? string.Empty
//        //        : string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
//        //            .ToLowerInvariant();

//        //public static string GetRandomHexColor()
//        //    => $"#{new Random().Next(0x1000000):X6}";

//        //public static string GetExceptionCode(this Exception exception)
//        //    => exception.GetType().Name.Underscore().Replace("_exception", string.Empty);

//        public static T ToEnum<T>(this string value)
//        {
//            return (T)Enum.Parse(typeof(T), value, true);
//        }
//        public static string? GetEnumDisplayName(this Enum enumType)
//        {
//            return enumType.GetType().GetMember(enumType.ToString())
//                           .First()
//                           .GetCustomAttribute<DisplayAttribute>()
//                           ?.Name;
//        }
//    }
//}

