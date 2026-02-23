using OnlineSchool.Domain.Entities;

namespace OnlineSchool.Application.DTOs;

public class HomeworkDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int MaterialId { get; set; }
    public string? MaterialName { get; set; }
    public int ClassRoomId { get; set; }
    public string? ClassRoomName { get; set; }
    public DateTime DueDate { get; set; }
    public decimal MaxScore { get; set; }
    public bool IsActive { get; set; }
    public int SubmissionCount { get; set; }
    public int GradedCount { get; set; }
    public IEnumerable<HomeworkContentDto> Contents { get; set; } = new List<HomeworkContentDto>();
    public IEnumerable<HomeworkQuestionDto> Questions { get; set; } = new List<HomeworkQuestionDto>();
}

public class HomeworkContentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ContentType Type { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
}

public class HomeworkQuestionDto
{
    public int Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public string? Options { get; set; }
    public decimal Points { get; set; }
    public int OrderIndex { get; set; }
}

public class CreateHomeworkDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaterialId { get; set; }
    public int ClassRoomId { get; set; }
    public DateTime DueDate { get; set; }
    public decimal MaxScore { get; set; } = 100;
    public IEnumerable<CreateHomeworkContentDto>? Contents { get; set; }
    public IEnumerable<CreateHomeworkQuestionDto>? Questions { get; set; }
}

public class CreateHomeworkContentDto
{
    public string Title { get; set; } = string.Empty;
    public ContentType Type { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
}

public class CreateHomeworkQuestionDto
{
    public string Question { get; set; } = string.Empty;
    public QuestionType Type { get; set; } = QuestionType.Text;
    public string? Options { get; set; }
    public string? CorrectAnswer { get; set; }
    public decimal Points { get; set; } = 10;
    public int OrderIndex { get; set; }
}
