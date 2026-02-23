using Common.Application.DTOs;
using OnlineSchool.Application.DTOs;

namespace OnlineSchool.Application.Interfaces;

public interface IHomeworkService
{
    Task<HomeworkDto?> GetByIdAsync(int id);
    Task<PagedResult<HomeworkDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<HomeworkDto>> GetByClassRoomAsync(int classRoomId);
    Task<IEnumerable<HomeworkDto>> GetByTeacherAsync(int teacherId);
    Task<IEnumerable<HomeworkDto>> GetByMaterialAsync(int materialId);
    Task<HomeworkDto> CreateAsync(int teacherId, CreateHomeworkDto dto);
    Task<HomeworkDto> UpdateAsync(UpdateHomeworkDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> AddContentAsync(int homeworkId, CreateHomeworkContentDto dto);
    Task<bool> AddQuestionAsync(int homeworkId, CreateHomeworkQuestionDto dto);
    Task<bool> RemoveContentAsync(int contentId);
    Task<bool> RemoveQuestionAsync(int questionId);
}

public class UpdateHomeworkDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public decimal MaxScore { get; set; }
    public bool IsActive { get; set; }
}

public interface IHomeworkAnswerService
{
    Task<HomeworkAnswerDto?> GetByIdAsync(int id);
    Task<HomeworkAnswerDto?> GetByHomeworkAndStudentAsync(int homeworkId, int studentId);
    Task<IEnumerable<HomeworkAnswerDto>> GetByHomeworkAsync(int homeworkId);
    Task<IEnumerable<HomeworkAnswerDto>> GetByStudentAsync(int studentId);
    Task<HomeworkAnswerDto> SubmitAsync(int studentId, SubmitHomeworkDto dto);
    Task<HomeworkAnswerDto> GradeAsync(int teacherId, GradeHomeworkDto dto);
    Task<StudentProgressDto> GetStudentProgressAsync(int studentId);
}

public interface IScheduleService
{
    Task<ScheduleDto?> GetByIdAsync(int id);
    Task<PagedResult<ScheduleDto>> GetAllAsync(QueryParameters parameters);
    Task<WeeklyScheduleDto> GetWeeklyScheduleByClassRoomAsync(int classRoomId);
    Task<IEnumerable<ScheduleDto>> GetByTeacherAsync(int teacherId);
    Task<ScheduleDto> CreateAsync(CreateScheduleDto dto);
    Task<ScheduleDto> UpdateAsync(UpdateScheduleDto dto);
    Task<bool> DeleteAsync(int id);
    Task<ScheduleSettingsDto> GetSettingsAsync();
    Task<ScheduleSettingsDto> UpdateSettingsAsync(ScheduleSettingsDto dto);
}
