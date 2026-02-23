namespace Pupitre.FeatureFlags;

/// <summary>
/// Service for checking feature flag status.
/// </summary>
public interface IFeatureFlagService
{
    /// <summary>
    /// Checks if a feature is enabled globally.
    /// </summary>
    Task<bool> IsEnabledAsync(string featureName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a feature is enabled for a specific user.
    /// </summary>
    Task<bool> IsEnabledForUserAsync(string featureName, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a feature is enabled for a percentage of users.
    /// </summary>
    Task<bool> IsEnabledForPercentageAsync(string featureName, Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Known feature flags in Pupitre.
/// </summary>
public static class FeatureFlags
{
    // AI Features
    public const string AITutorEnabled = "ai.tutor.enabled";
    public const string AIAutoGrading = "ai.auto-grading.enabled";
    public const string AIContentGeneration = "ai.content-generation.enabled";
    public const string AISpeechToText = "ai.speech-to-text.enabled";
    public const string AIVisionAnalysis = "ai.vision-analysis.enabled";
    public const string AIRecommendations = "ai.recommendations.enabled";

    // Learning Features
    public const string AdaptiveLearning = "learning.adaptive.enabled";
    public const string GamificationEnabled = "learning.gamification.enabled";
    public const string BadgesEnabled = "learning.badges.enabled";
    public const string LeaderboardEnabled = "learning.leaderboard.enabled";

    // Assessment Features
    public const string TimedAssessments = "assessment.timed.enabled";
    public const string InstantFeedback = "assessment.instant-feedback.enabled";
    public const string RetakeEnabled = "assessment.retake.enabled";

    // Communication Features
    public const string ParentPortal = "communication.parent-portal.enabled";
    public const string RealTimeNotifications = "communication.realtime.enabled";
    public const string EmailDigest = "communication.email-digest.enabled";

    // Platform Features
    public const string OfflineMode = "platform.offline.enabled";
    public const string DarkMode = "platform.dark-mode.enabled";
    public const string MultiLanguage = "platform.multi-language.enabled";
    public const string AccessibilityMode = "platform.accessibility.enabled";
}
