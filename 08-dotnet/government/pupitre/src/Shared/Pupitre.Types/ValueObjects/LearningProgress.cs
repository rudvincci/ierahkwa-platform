namespace Pupitre.Types.ValueObjects;

/// <summary>
/// Represents a student's learning progress.
/// </summary>
public sealed record LearningProgress
{
    /// <summary>
    /// Number of completed items.
    /// </summary>
    public int CompletedCount { get; }

    /// <summary>
    /// Total number of items.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Completion percentage (0-100).
    /// </summary>
    public decimal CompletionPercentage => TotalCount > 0 
        ? (decimal)CompletedCount / TotalCount * 100 
        : 0;

    /// <summary>
    /// Whether all items are completed.
    /// </summary>
    public bool IsComplete => CompletedCount >= TotalCount;

    public LearningProgress(int completedCount, int totalCount)
    {
        if (completedCount < 0)
            throw new ArgumentException("Completed count cannot be negative", nameof(completedCount));
        if (totalCount < 0)
            throw new ArgumentException("Total count cannot be negative", nameof(totalCount));
        if (completedCount > totalCount)
            throw new ArgumentException("Completed count cannot exceed total count", nameof(completedCount));

        CompletedCount = completedCount;
        TotalCount = totalCount;
    }

    public static LearningProgress Empty => new(0, 0);
    public static LearningProgress Complete(int count) => new(count, count);
    public static LearningProgress NotStarted(int totalCount) => new(0, totalCount);

    public LearningProgress IncrementCompleted() => 
        new(Math.Min(CompletedCount + 1, TotalCount), TotalCount);

    public override string ToString() => $"{CompletedCount}/{TotalCount} ({CompletionPercentage:F1}%)";
}
