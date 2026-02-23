namespace Pupitre.Types.Enums;

/// <summary>
/// Types of notifications.
/// </summary>
public enum NotificationType
{
    /// <summary>New lesson available</summary>
    NewLesson = 1,
    
    /// <summary>Assessment due soon</summary>
    AssessmentDue = 2,
    
    /// <summary>Assessment graded</summary>
    AssessmentGraded = 3,
    
    /// <summary>Achievement unlocked</summary>
    Achievement = 4,
    
    /// <summary>Message from educator</summary>
    EducatorMessage = 5,
    
    /// <summary>Message from parent</summary>
    ParentMessage = 6,
    
    /// <summary>System announcement</summary>
    SystemAnnouncement = 7,
    
    /// <summary>Reminder notification</summary>
    Reminder = 8,
    
    /// <summary>Progress milestone reached</summary>
    ProgressMilestone = 9,
    
    /// <summary>IEP update</summary>
    IEPUpdate = 10,
    
    /// <summary>Credential issued</summary>
    CredentialIssued = 11
}
