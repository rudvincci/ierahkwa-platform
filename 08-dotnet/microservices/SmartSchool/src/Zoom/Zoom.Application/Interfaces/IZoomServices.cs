using Common.Application.DTOs;
using Zoom.Application.DTOs;
using Zoom.Domain.Entities;

namespace Zoom.Application.Interfaces;

public interface ILiveClassService
{
    Task<LiveClassDto?> GetByIdAsync(int id);
    Task<PagedResult<LiveClassDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<LiveClassDto>> GetByClassRoomAsync(int classRoomId);
    Task<IEnumerable<LiveClassDto>> GetByTeacherAsync(int teacherId);
    Task<IEnumerable<LiveClassDto>> GetUpcomingAsync();
    Task<IEnumerable<LiveClassDto>> GetActiveAsync();
    Task<LiveClassDto> CreateAsync(int teacherId, CreateLiveClassDto dto);
    Task<LiveClassDto> UpdateAsync(UpdateLiveClassDto dto);
    Task<bool> DeleteAsync(int id);
    Task<LiveClassDto> StartClassAsync(int id);
    Task<LiveClassDto> EndClassAsync(int id);
    Task<bool> CancelClassAsync(int id);
    Task<IEnumerable<LiveClassAttendanceDto>> GetAttendanceAsync(int liveClassId);
    Task<LiveClassAttendanceDto> JoinClassAsync(int liveClassId, int studentId);
    Task<LiveClassAttendanceDto> LeaveClassAsync(int liveClassId, int studentId);
}

public interface IZoomService
{
    Task<ZoomMeetingResponse> CreateMeetingAsync(string topic, DateTime startTime, int durationMinutes);
    Task<bool> DeleteMeetingAsync(string meetingId);
    Task<bool> EndMeetingAsync(string meetingId);
    Task<string?> GetRecordingUrlAsync(string meetingId);
}

public interface IZoomSettingsService
{
    Task<ZoomSettingsDto?> GetSettingsAsync();
    Task<ZoomSettingsDto> SaveSettingsAsync(CreateZoomSettingsDto dto);
    Task<bool> TestConnectionAsync();
}
