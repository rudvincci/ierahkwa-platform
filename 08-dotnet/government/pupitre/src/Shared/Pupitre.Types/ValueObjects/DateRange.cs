namespace Pupitre.Types.ValueObjects;

/// <summary>
/// Represents a date range (e.g., semester, school year).
/// </summary>
public sealed record DateRange
{
    /// <summary>
    /// Start date of the range.
    /// </summary>
    public DateOnly StartDate { get; }

    /// <summary>
    /// End date of the range.
    /// </summary>
    public DateOnly EndDate { get; }

    /// <summary>
    /// Duration in days.
    /// </summary>
    public int DurationDays => EndDate.DayNumber - StartDate.DayNumber;

    /// <summary>
    /// Whether the current date falls within this range.
    /// </summary>
    public bool IsActive => DateOnly.FromDateTime(DateTime.UtcNow) >= StartDate 
                         && DateOnly.FromDateTime(DateTime.UtcNow) <= EndDate;

    public DateRange(DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));

        StartDate = startDate;
        EndDate = endDate;
    }

    public static DateRange CreateSchoolYear(int year)
    {
        return new DateRange(
            new DateOnly(year, 8, 15),
            new DateOnly(year + 1, 6, 15)
        );
    }

    public static DateRange CreateSemester(int year, int semester)
    {
        return semester switch
        {
            1 => new DateRange(new DateOnly(year, 8, 15), new DateOnly(year, 12, 20)),
            2 => new DateRange(new DateOnly(year + 1, 1, 10), new DateOnly(year + 1, 6, 15)),
            _ => throw new ArgumentException("Semester must be 1 or 2", nameof(semester))
        };
    }

    public bool Contains(DateOnly date) => date >= StartDate && date <= EndDate;

    public override string ToString() => $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
}
