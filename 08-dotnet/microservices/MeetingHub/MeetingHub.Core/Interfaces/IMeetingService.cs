using MeetingHub.Core.Models;

namespace MeetingHub.Core.Interfaces;

public interface IMeetingService
{
    // Meetings
    Task<Meeting> CreateMeetingAsync(Meeting meeting);
    Task<Meeting?> GetMeetingByIdAsync(Guid id);
    Task<Meeting?> GetMeetingByCodeAsync(string code);
    Task<IEnumerable<Meeting>> GetMeetingsByOrganizerAsync(Guid organizerId);
    Task<IEnumerable<Meeting>> GetMeetingsByParticipantAsync(Guid userId);
    Task<IEnumerable<Meeting>> GetUpcomingMeetingsAsync(Guid userId, int days = 7);
    Task<Meeting> UpdateMeetingAsync(Meeting meeting);
    Task<Meeting> StartMeetingAsync(Guid id);
    Task<Meeting> EndMeetingAsync(Guid id);
    Task<Meeting> CancelMeetingAsync(Guid id, string? reason);
    Task DeleteMeetingAsync(Guid id);
    Task<string> GenerateJoinUrlAsync(Guid meetingId, Guid userId);

    // Participants
    Task<MeetingParticipant> AddParticipantAsync(Guid meetingId, MeetingParticipant participant);
    Task<MeetingParticipant> JoinMeetingAsync(Guid meetingId, Guid userId, string? device);
    Task<MeetingParticipant> LeaveMeetingAsync(Guid meetingId, Guid userId);
    Task RemoveParticipantAsync(Guid participantId);
    Task<IEnumerable<MeetingParticipant>> GetParticipantsAsync(Guid meetingId);
    Task SendInvitationsAsync(Guid meetingId);

    // Rooms
    Task<MeetingRoom> CreateRoomAsync(MeetingRoom room);
    Task<MeetingRoom?> GetRoomByIdAsync(Guid id);
    Task<IEnumerable<MeetingRoom>> GetRoomsAsync(string? location = null);
    Task<IEnumerable<MeetingRoom>> GetAvailableRoomsAsync(DateTime start, DateTime end, int? minCapacity = null);
    Task<MeetingRoom> UpdateRoomAsync(MeetingRoom room);
    Task DeleteRoomAsync(Guid id);

    // Room Bookings
    Task<RoomBooking> BookRoomAsync(RoomBooking booking);
    Task<IEnumerable<RoomBooking>> GetRoomBookingsAsync(Guid roomId, DateTime? from = null, DateTime? to = null);
    Task<RoomBooking> ApproveBookingAsync(Guid bookingId, Guid approvedBy);
    Task<RoomBooking> CancelBookingAsync(Guid bookingId);
    Task<bool> CheckRoomAvailabilityAsync(Guid roomId, DateTime start, DateTime end);

    // Calendar
    Task<Calendar> CreateCalendarAsync(Calendar calendar);
    Task<Calendar?> GetCalendarByIdAsync(Guid id);
    Task<IEnumerable<Calendar>> GetUserCalendarsAsync(Guid userId);
    Task<Calendar> UpdateCalendarAsync(Calendar calendar);
    Task DeleteCalendarAsync(Guid id);

    // Events
    Task<CalendarEvent> CreateEventAsync(CalendarEvent calendarEvent);
    Task<CalendarEvent?> GetEventByIdAsync(Guid id);
    Task<IEnumerable<CalendarEvent>> GetEventsAsync(Guid calendarId, DateTime from, DateTime to);
    Task<IEnumerable<CalendarEvent>> GetUserEventsAsync(Guid userId, DateTime from, DateTime to);
    Task<CalendarEvent> UpdateEventAsync(CalendarEvent calendarEvent);
    Task DeleteEventAsync(Guid id);
    Task<EventAttendee> RespondToEventAsync(Guid eventId, Guid userId, AttendeeResponse response);

    // Recording & Transcription
    Task<string> StartRecordingAsync(Guid meetingId);
    Task<string> StopRecordingAsync(Guid meetingId);
    Task<string> GetRecordingUrlAsync(Guid meetingId);
    Task<string> GetTranscriptAsync(Guid meetingId);
    Task<Meeting> GenerateMinutesAsync(Guid meetingId);

    // Statistics
    Task<MeetingStatistics> GetStatisticsAsync(Guid? userId = null, string? department = null);
}

public class MeetingStatistics
{
    public int TotalMeetings { get; set; }
    public int CompletedMeetings { get; set; }
    public int CancelledMeetings { get; set; }
    public int TotalParticipants { get; set; }
    public double AverageDurationMinutes { get; set; }
    public double AverageParticipantsPerMeeting { get; set; }
    public int TotalMeetingMinutes { get; set; }
    public int MeetingsThisWeek { get; set; }
    public int MeetingsThisMonth { get; set; }
    public Dictionary<string, int> MeetingsByType { get; set; } = new();
    public Dictionary<string, int> MeetingsByDepartment { get; set; } = new();
}
