namespace OnlineSchool.Application.DTOs;

public class HomeworkAnswerDto
{
    public int Id { get; set; }
    public int HomeworkId { get; set; }
    public string? HomeworkTitle { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public DateTime SubmittedAt { get; set; }
    public decimal? Score { get; set; }
    public string? Feedback { get; set; }
    public bool IsGraded { get; set; }
    public DateTime? GradedAt { get; set; }
    public IEnumerable<QuestionAnswerDto> QuestionAnswers { get; set; } = new List<QuestionAnswerDto>();
}

public class QuestionAnswerDto
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string? QuestionText { get; set; }
    public string? Answer { get; set; }
    public string? FilePath { get; set; }
    public decimal? Points { get; set; }
}

public class SubmitHomeworkDto
{
    public int HomeworkId { get; set; }
    public IEnumerable<SubmitQuestionAnswerDto> Answers { get; set; } = new List<SubmitQuestionAnswerDto>();
}

public class SubmitQuestionAnswerDto
{
    public int QuestionId { get; set; }
    public string? Answer { get; set; }
    public string? FilePath { get; set; }
}

public class GradeHomeworkDto
{
    public int AnswerId { get; set; }
    public decimal Score { get; set; }
    public string? Feedback { get; set; }
    public IEnumerable<GradeQuestionDto>? QuestionGrades { get; set; }
}

public class GradeQuestionDto
{
    public int QuestionAnswerId { get; set; }
    public decimal Points { get; set; }
}

public class StudentProgressDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int TotalHomeworks { get; set; }
    public int CompletedHomeworks { get; set; }
    public decimal AverageScore { get; set; }
    public IEnumerable<MaterialProgressDto> MaterialProgress { get; set; } = new List<MaterialProgressDto>();
}

public class MaterialProgressDto
{
    public int MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public int TotalHomeworks { get; set; }
    public int CompletedHomeworks { get; set; }
    public decimal AverageScore { get; set; }
}
