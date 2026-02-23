using Common.Domain.Entities;

namespace OnlineSchool.Domain.Entities;

public class Homework : TenantEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TeacherId { get; set; }
    public int MaterialId { get; set; }
    public int ClassRoomId { get; set; }
    public DateTime DueDate { get; set; }
    public decimal MaxScore { get; set; } = 100;
    public bool IsActive { get; set; } = true;
    
    public virtual Teacher? Teacher { get; set; }
    public virtual Material? Material { get; set; }
    public virtual ClassRoom? ClassRoom { get; set; }
    public virtual ICollection<HomeworkContent> Contents { get; set; } = new List<HomeworkContent>();
    public virtual ICollection<HomeworkQuestion> Questions { get; set; } = new List<HomeworkQuestion>();
    public virtual ICollection<HomeworkAnswer> Answers { get; set; } = new List<HomeworkAnswer>();
}

public enum ContentType
{
    Video,
    PDF,
    Word,
    Image,
    Link
}

public class HomeworkContent : TenantEntity
{
    public int HomeworkId { get; set; }
    public string Title { get; set; } = string.Empty;
    public ContentType Type { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    
    public virtual Homework? Homework { get; set; }
}

public class HomeworkQuestion : TenantEntity
{
    public int HomeworkId { get; set; }
    public string Question { get; set; } = string.Empty;
    public QuestionType Type { get; set; } = QuestionType.Text;
    public string? Options { get; set; } // JSON for multiple choice
    public string? CorrectAnswer { get; set; }
    public decimal Points { get; set; } = 10;
    public int OrderIndex { get; set; }
    
    public virtual Homework? Homework { get; set; }
}

public enum QuestionType
{
    Text,
    MultipleChoice,
    TrueFalse,
    FileUpload
}
