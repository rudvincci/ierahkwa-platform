using Common.Domain.Entities;

namespace OnlineSchool.Domain.Entities;

public class HomeworkAnswer : TenantEntity
{
    public int HomeworkId { get; set; }
    public int StudentId { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public decimal? Score { get; set; }
    public string? Feedback { get; set; }
    public bool IsGraded { get; set; } = false;
    public DateTime? GradedAt { get; set; }
    public int? GradedBy { get; set; }
    
    public virtual Homework? Homework { get; set; }
    public virtual Student? Student { get; set; }
    public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();
}

public class QuestionAnswer : TenantEntity
{
    public int HomeworkAnswerId { get; set; }
    public int QuestionId { get; set; }
    public string? Answer { get; set; }
    public string? FilePath { get; set; }
    public decimal? Points { get; set; }
    
    public virtual HomeworkAnswer? HomeworkAnswer { get; set; }
    public virtual HomeworkQuestion? Question { get; set; }
}
