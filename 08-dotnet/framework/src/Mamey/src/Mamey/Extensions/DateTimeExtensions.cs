namespace Mamey;

public static class DateTimeExtensions
{
    public static int GetQuarterOfYear(this DateTime date)
    {
        int quarter = (date.Month - 1) / 3 + 1;
        return quarter;
    }
    // Function to get the pay period for a given month
    public static TimePeriod GetCurrentTimePeriod(int? month = null)
    {
        if (month is null)
        {
            month = DateTime.Today.Month;
        }
        int year = DateTime.Today.Year; // Get the current year
        return new TimePeriod
        {
            FirstDay = GetFirstDayOfMonth(year, month.Value),
            LastDay = GetLastDayOfMonth(year, month.Value)
        };
    }

    // Function to get the first day of the month
    public static DateTime GetFirstDayOfMonth(int year, int month)
    {
        return new DateTime(year, month, 1);
    }

    // Function to get the last day of the month
    public static DateTime GetLastDayOfMonth(int year, int month)
    {
        return new DateTime(year, month, DateTime.DaysInMonth(year, month));
    }
    public static DateTime GetDate(this long timestamp)
        => DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
    public static int CalculateAge(this DateTime dateOfBirth)
    {
        DateTime today = DateTime.UtcNow.Date;

        // Return 0 if the date of birth is in the future
        if (dateOfBirth.Date > today)
        {
            return 0;
        }

        int age = today.Year - dateOfBirth.Year;

        // Adjust if the birthday hasn't occurred yet this year
        if (dateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
    public static long ToUnixTimeMilliseconds(this DateTime dateTime)
            => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();

    public static bool InThePast(this DateTimeOffset dateTimeOffset)
    {
        // Provides a small leeway to account for time discrepancies
        return dateTimeOffset.AddSeconds(-30) < DateTimeOffset.Now;
    }
}
public struct TimePeriod
{
    public DateTime FirstDay;
    public DateTime LastDay;
}