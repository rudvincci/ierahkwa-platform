namespace Mamey.Auth.Decentralized.Utilities;

/// <summary>
/// Extension methods for DateTime operations
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Converts a DateTime to ISO 8601 format
    /// </summary>
    /// <param name="dateTime">The DateTime to convert</param>
    /// <returns>ISO 8601 formatted string</returns>
    public static string ToIso8601String(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
    
    /// <summary>
    /// Converts a DateTime to ISO 8601 format with timezone
    /// </summary>
    /// <param name="dateTime">The DateTime to convert</param>
    /// <param name="timeZone">The timezone to use</param>
    /// <returns>ISO 8601 formatted string with timezone</returns>
    public static string ToIso8601String(this DateTime dateTime, TimeZoneInfo timeZone)
    {
        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
        return utcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
    
    /// <summary>
    /// Converts a DateTime to Unix timestamp
    /// </summary>
    /// <param name="dateTime">The DateTime to convert</param>
    /// <returns>Unix timestamp</returns>
    public static long ToUnixTimestamp(this DateTime dateTime)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long)(dateTime.ToUniversalTime() - epoch).TotalSeconds;
    }
    
    /// <summary>
    /// Converts a Unix timestamp to DateTime
    /// </summary>
    /// <param name="timestamp">The Unix timestamp</param>
    /// <returns>DateTime</returns>
    public static DateTime FromUnixTimestamp(long timestamp)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddSeconds(timestamp);
    }
    
    /// <summary>
    /// Converts a DateTime to RFC 3339 format
    /// </summary>
    /// <param name="dateTime">The DateTime to convert</param>
    /// <returns>RFC 3339 formatted string</returns>
    public static string ToRfc3339String(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
    
    /// <summary>
    /// Converts a DateTime to RFC 3339 format with timezone
    /// </summary>
    /// <param name="dateTime">The DateTime to convert</param>
    /// <param name="timeZone">The timezone to use</param>
    /// <returns>RFC 3339 formatted string with timezone</returns>
    public static string ToRfc3339String(this DateTime dateTime, TimeZoneInfo timeZone)
    {
        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
        return utcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
    
    /// <summary>
    /// Converts a DateTime to W3C date format
    /// </summary>
    /// <param name="dateTime">The DateTime to convert</param>
    /// <returns>W3C date formatted string</returns>
    public static string ToW3cDateString(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd");
    }
    
    /// <summary>
    /// Converts a DateTime to W3C datetime format
    /// </summary>
    /// <param name="dateTime">The DateTime to convert</param>
    /// <returns>W3C datetime formatted string</returns>
    public static string ToW3cDateTimeString(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
    
    /// <summary>
    /// Converts a DateTime to W3C datetime format with timezone
    /// </summary>
    /// <param name="dateTime">The DateTime to convert</param>
    /// <param name="timeZone">The timezone to use</param>
    /// <returns>W3C datetime formatted string with timezone</returns>
    public static string ToW3cDateTimeString(this DateTime dateTime, TimeZoneInfo timeZone)
    {
        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
        return utcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
    
    /// <summary>
    /// Checks if a DateTime is in the past
    /// </summary>
    /// <param name="dateTime">The DateTime to check</param>
    /// <returns>True if the DateTime is in the past</returns>
    public static bool IsInPast(this DateTime dateTime)
    {
        return dateTime < DateTime.UtcNow;
    }
    
    /// <summary>
    /// Checks if a DateTime is in the future
    /// </summary>
    /// <param name="dateTime">The DateTime to check</param>
    /// <returns>True if the DateTime is in the future</returns>
    public static bool IsInFuture(this DateTime dateTime)
    {
        return dateTime > DateTime.UtcNow;
    }
    
    /// <summary>
    /// Checks if a DateTime is expired (in the past)
    /// </summary>
    /// <param name="dateTime">The DateTime to check</param>
    /// <returns>True if the DateTime is expired</returns>
    public static bool IsExpired(this DateTime dateTime)
    {
        return dateTime.IsInPast();
    }
    
    /// <summary>
    /// Checks if a DateTime is valid (not expired)
    /// </summary>
    /// <param name="dateTime">The DateTime to check</param>
    /// <returns>True if the DateTime is valid</returns>
    public static bool IsValid(this DateTime dateTime)
    {
        return !dateTime.IsExpired();
    }
    
    /// <summary>
    /// Gets the time until a DateTime
    /// </summary>
    /// <param name="dateTime">The DateTime to check</param>
    /// <returns>TimeSpan until the DateTime</returns>
    public static TimeSpan TimeUntil(this DateTime dateTime)
    {
        return dateTime - DateTime.UtcNow;
    }
    
    /// <summary>
    /// Gets the time since a DateTime
    /// </summary>
    /// <param name="dateTime">The DateTime to check</param>
    /// <returns>TimeSpan since the DateTime</returns>
    public static TimeSpan TimeSince(this DateTime dateTime)
    {
        return DateTime.UtcNow - dateTime;
    }
}
