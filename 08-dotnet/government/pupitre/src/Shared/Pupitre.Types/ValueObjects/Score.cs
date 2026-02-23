namespace Pupitre.Types.ValueObjects;

/// <summary>
/// Represents an assessment score.
/// </summary>
public sealed record Score
{
    /// <summary>
    /// Points earned.
    /// </summary>
    public decimal PointsEarned { get; }

    /// <summary>
    /// Maximum possible points.
    /// </summary>
    public decimal MaxPoints { get; }

    /// <summary>
    /// Percentage score (0-100).
    /// </summary>
    public decimal Percentage => MaxPoints > 0 ? (PointsEarned / MaxPoints) * 100 : 0;

    /// <summary>
    /// Letter grade based on percentage.
    /// </summary>
    public string LetterGrade => Percentage switch
    {
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        >= 60 => "D",
        _ => "F"
    };

    public Score(decimal pointsEarned, decimal maxPoints)
    {
        if (pointsEarned < 0)
            throw new ArgumentException("Points earned cannot be negative", nameof(pointsEarned));
        if (maxPoints <= 0)
            throw new ArgumentException("Max points must be positive", nameof(maxPoints));
        if (pointsEarned > maxPoints)
            throw new ArgumentException("Points earned cannot exceed max points", nameof(pointsEarned));

        PointsEarned = pointsEarned;
        MaxPoints = maxPoints;
    }

    public static Score Perfect(decimal maxPoints) => new(maxPoints, maxPoints);
    public static Score Zero(decimal maxPoints) => new(0, maxPoints);

    public override string ToString() => $"{PointsEarned}/{MaxPoints} ({Percentage:F1}%)";
}
