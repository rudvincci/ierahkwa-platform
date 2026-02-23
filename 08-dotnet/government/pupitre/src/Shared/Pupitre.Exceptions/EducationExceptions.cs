namespace Pupitre.Exceptions;

/// <summary>
/// Exception thrown when a user is not found.
/// </summary>
public class UserNotFoundException : EntityNotFoundException
{
    public UserNotFoundException(Guid userId) : base("User", userId.ToString()) { }
}

/// <summary>
/// Exception thrown when a student is not found.
/// </summary>
public class StudentNotFoundException : EntityNotFoundException
{
    public StudentNotFoundException(Guid studentId) : base("Student", studentId.ToString()) { }
}

/// <summary>
/// Exception thrown when an educator is not found.
/// </summary>
public class EducatorNotFoundException : EntityNotFoundException
{
    public EducatorNotFoundException(Guid educatorId) : base("Educator", educatorId.ToString()) { }
}

/// <summary>
/// Exception thrown when a GLE (Grade Level Expectation) is not found.
/// </summary>
public class GLENotFoundException : EntityNotFoundException
{
    public GLENotFoundException(Guid gleId) : base("GLE", gleId.ToString()) { }
}

/// <summary>
/// Exception thrown when a curriculum is not found.
/// </summary>
public class CurriculumNotFoundException : EntityNotFoundException
{
    public CurriculumNotFoundException(Guid curriculumId) : base("Curriculum", curriculumId.ToString()) { }
}

/// <summary>
/// Exception thrown when a lesson is not found.
/// </summary>
public class LessonNotFoundException : EntityNotFoundException
{
    public LessonNotFoundException(Guid lessonId) : base("Lesson", lessonId.ToString()) { }
}

/// <summary>
/// Exception thrown when an assessment is not found.
/// </summary>
public class AssessmentNotFoundException : EntityNotFoundException
{
    public AssessmentNotFoundException(Guid assessmentId) : base("Assessment", assessmentId.ToString()) { }
}

/// <summary>
/// Exception thrown when an IEP is not found.
/// </summary>
public class IEPNotFoundException : EntityNotFoundException
{
    public IEPNotFoundException(Guid iepId) : base("IEP", iepId.ToString()) { }
}

/// <summary>
/// Exception thrown when a student has not completed prerequisites.
/// </summary>
public class PrerequisiteNotMetException : BusinessRuleException
{
    public PrerequisiteNotMetException(string lessonName, string prerequisiteName)
        : base("PREREQUISITE_NOT_MET", $"Cannot access '{lessonName}' without completing prerequisite '{prerequisiteName}'.")
    {
    }
}

/// <summary>
/// Exception thrown when a student's grade level doesn't match content.
/// </summary>
public class GradeLevelMismatchException : BusinessRuleException
{
    public GradeLevelMismatchException(int studentGrade, int contentGrade)
        : base("GRADE_LEVEL_MISMATCH", $"Student grade level ({studentGrade}) does not match content grade level ({contentGrade}).")
    {
    }
}

/// <summary>
/// Exception thrown when assessment submission deadline has passed.
/// </summary>
public class AssessmentDeadlinePassedException : BusinessRuleException
{
    public AssessmentDeadlinePassedException(DateTime deadline)
        : base("ASSESSMENT_DEADLINE_PASSED", $"Assessment deadline ({deadline:g}) has passed.")
    {
    }
}

/// <summary>
/// Exception thrown when maximum assessment attempts exceeded.
/// </summary>
public class MaxAttemptsExceededException : BusinessRuleException
{
    public MaxAttemptsExceededException(int maxAttempts)
        : base("MAX_ATTEMPTS_EXCEEDED", $"Maximum number of attempts ({maxAttempts}) has been exceeded.")
    {
    }
}

/// <summary>
/// Exception thrown when content is locked due to parental controls.
/// </summary>
public class ContentLockedException : ForbiddenException
{
    public ContentLockedException(string reason)
        : base($"Content is locked: {reason}")
    {
    }
}

/// <summary>
/// Exception thrown when AI tutor session has expired.
/// </summary>
public class TutorSessionExpiredException : BusinessRuleException
{
    public TutorSessionExpiredException(Guid sessionId)
        : base("TUTOR_SESSION_EXPIRED", $"Tutor session '{sessionId}' has expired.")
    {
    }
}

/// <summary>
/// Exception thrown when AI service is unavailable.
/// </summary>
public class AIServiceUnavailableException : PupitreException
{
    public AIServiceUnavailableException(string serviceName)
        : base("AI_SERVICE_UNAVAILABLE", $"AI service '{serviceName}' is currently unavailable.", 503)
    {
    }
}
