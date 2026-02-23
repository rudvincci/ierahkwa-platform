using MeetingHub.Core.Interfaces;
using MeetingHub.Core.Models;

namespace MeetingHub.Infrastructure.Services;

public class MeetingService : IMeetingService
{
    private readonly List<Meeting> _meetings = new();
    private readonly List<MeetingParticipant> _participants = new();
    private readonly List<MeetingRoom> _rooms = new();
    private readonly List<RoomBooking> _bookings = new();
    private readonly List<Calendar> _calendars = new();
    private readonly List<CalendarEvent> _events = new();

    public Task<Meeting> CreateMeetingAsync(Meeting meeting)
    {
        meeting.Id = Guid.NewGuid();
        meeting.MeetingCode = GenerateMeetingCode();
        meeting.Status = MeetingStatus.Scheduled;
        meeting.JoinUrl = $"https://meet.ierahkwa.gov/{meeting.MeetingCode}";
        meeting.HostUrl = $"https://meet.ierahkwa.gov/{meeting.MeetingCode}/host";
        meeting.CreatedAt = DateTime.UtcNow;
        _meetings.Add(meeting);
        return Task.FromResult(meeting);
    }

    public Task<Meeting?> GetMeetingByIdAsync(Guid id) =>
        Task.FromResult(_meetings.FirstOrDefault(m => m.Id == id));

    public Task<Meeting?> GetMeetingByCodeAsync(string code) =>
        Task.FromResult(_meetings.FirstOrDefault(m => m.MeetingCode == code));

    public Task<IEnumerable<Meeting>> GetMeetingsByOrganizerAsync(Guid organizerId) =>
        Task.FromResult(_meetings.Where(m => m.OrganizerId == organizerId));

    public Task<IEnumerable<Meeting>> GetMeetingsByParticipantAsync(Guid userId)
    {
        var meetingIds = _participants.Where(p => p.UserId == userId).Select(p => p.MeetingId);
        return Task.FromResult(_meetings.Where(m => meetingIds.Contains(m.Id)));
    }

    public Task<IEnumerable<Meeting>> GetUpcomingMeetingsAsync(Guid userId, int days = 7)
    {
        var endDate = DateTime.UtcNow.AddDays(days);
        var meetingIds = _participants.Where(p => p.UserId == userId).Select(p => p.MeetingId).ToHashSet();
        return Task.FromResult(_meetings.Where(m => 
            (meetingIds.Contains(m.Id) || m.OrganizerId == userId) &&
            m.StartTime >= DateTime.UtcNow && m.StartTime <= endDate &&
            m.Status == MeetingStatus.Scheduled));
    }

    public Task<Meeting> UpdateMeetingAsync(Meeting meeting)
    {
        var existing = _meetings.FirstOrDefault(m => m.Id == meeting.Id);
        if (existing != null)
        {
            existing.Title = meeting.Title;
            existing.Description = meeting.Description;
            existing.StartTime = meeting.StartTime;
            existing.EndTime = meeting.EndTime;
            existing.Agenda = meeting.Agenda;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        return Task.FromResult(existing ?? meeting);
    }

    public Task<Meeting> StartMeetingAsync(Guid id)
    {
        var meeting = _meetings.FirstOrDefault(m => m.Id == id);
        if (meeting != null)
        {
            meeting.Status = MeetingStatus.InProgress;
            meeting.StartedAt = DateTime.UtcNow;
        }
        return Task.FromResult(meeting!);
    }

    public Task<Meeting> EndMeetingAsync(Guid id)
    {
        var meeting = _meetings.FirstOrDefault(m => m.Id == id);
        if (meeting != null)
        {
            meeting.Status = MeetingStatus.Completed;
            meeting.EndedAt = DateTime.UtcNow;
            meeting.Duration = (int)(meeting.EndedAt.Value - meeting.StartedAt!.Value).TotalMinutes;
        }
        return Task.FromResult(meeting!);
    }

    public Task<Meeting> CancelMeetingAsync(Guid id, string? reason)
    {
        var meeting = _meetings.FirstOrDefault(m => m.Id == id);
        if (meeting != null) meeting.Status = MeetingStatus.Cancelled;
        return Task.FromResult(meeting!);
    }

    public Task DeleteMeetingAsync(Guid id)
    {
        var meeting = _meetings.FirstOrDefault(m => m.Id == id);
        if (meeting != null) _meetings.Remove(meeting);
        return Task.CompletedTask;
    }

    public Task<string> GenerateJoinUrlAsync(Guid meetingId, Guid userId)
    {
        var meeting = _meetings.FirstOrDefault(m => m.Id == meetingId);
        return Task.FromResult($"{meeting?.JoinUrl}?token={Guid.NewGuid():N}");
    }

    public Task<MeetingParticipant> AddParticipantAsync(Guid meetingId, MeetingParticipant participant)
    {
        participant.Id = Guid.NewGuid();
        participant.MeetingId = meetingId;
        participant.Status = ParticipantStatus.Invited;
        _participants.Add(participant);
        return Task.FromResult(participant);
    }

    public Task<MeetingParticipant> JoinMeetingAsync(Guid meetingId, Guid userId, string? device)
    {
        var participant = _participants.FirstOrDefault(p => p.MeetingId == meetingId && p.UserId == userId);
        if (participant != null)
        {
            participant.Status = ParticipantStatus.Joined;
            participant.JoinedAt = DateTime.UtcNow;
            participant.Device = device;
            var meeting = _meetings.FirstOrDefault(m => m.Id == meetingId);
            if (meeting != null) meeting.CurrentParticipants++;
        }
        return Task.FromResult(participant!);
    }

    public Task<MeetingParticipant> LeaveMeetingAsync(Guid meetingId, Guid userId)
    {
        var participant = _participants.FirstOrDefault(p => p.MeetingId == meetingId && p.UserId == userId);
        if (participant != null)
        {
            participant.Status = ParticipantStatus.Left;
            participant.LeftAt = DateTime.UtcNow;
            if (participant.JoinedAt.HasValue)
                participant.TotalMinutes = (int)(participant.LeftAt.Value - participant.JoinedAt.Value).TotalMinutes;
            var meeting = _meetings.FirstOrDefault(m => m.Id == meetingId);
            if (meeting != null) meeting.CurrentParticipants--;
        }
        return Task.FromResult(participant!);
    }

    public Task RemoveParticipantAsync(Guid participantId)
    {
        var participant = _participants.FirstOrDefault(p => p.Id == participantId);
        if (participant != null) _participants.Remove(participant);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<MeetingParticipant>> GetParticipantsAsync(Guid meetingId) =>
        Task.FromResult(_participants.Where(p => p.MeetingId == meetingId));

    public Task SendInvitationsAsync(Guid meetingId)
    {
        // In production: Send email invitations
        return Task.CompletedTask;
    }

    public Task<MeetingRoom> CreateRoomAsync(MeetingRoom room)
    {
        room.Id = Guid.NewGuid();
        _rooms.Add(room);
        return Task.FromResult(room);
    }

    public Task<MeetingRoom?> GetRoomByIdAsync(Guid id) =>
        Task.FromResult(_rooms.FirstOrDefault(r => r.Id == id));

    public Task<IEnumerable<MeetingRoom>> GetRoomsAsync(string? location = null)
    {
        var rooms = _rooms.Where(r => r.IsActive);
        if (!string.IsNullOrEmpty(location)) rooms = rooms.Where(r => r.Location == location);
        return Task.FromResult(rooms);
    }

    public async Task<IEnumerable<MeetingRoom>> GetAvailableRoomsAsync(DateTime start, DateTime end, int? minCapacity = null)
    {
        var bookedRoomIds = _bookings
            .Where(b => b.Status == BookingStatus.Confirmed && b.StartTime < end && b.EndTime > start)
            .Select(b => b.RoomId).ToHashSet();
        var rooms = _rooms.Where(r => r.IsActive && !bookedRoomIds.Contains(r.Id));
        if (minCapacity.HasValue) rooms = rooms.Where(r => r.Capacity >= minCapacity.Value);
        return await Task.FromResult(rooms);
    }

    public Task<MeetingRoom> UpdateRoomAsync(MeetingRoom room)
    {
        var existing = _rooms.FirstOrDefault(r => r.Id == room.Id);
        if (existing != null)
        {
            existing.Name = room.Name;
            existing.Capacity = room.Capacity;
            existing.Equipment = room.Equipment;
        }
        return Task.FromResult(existing ?? room);
    }

    public Task DeleteRoomAsync(Guid id)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == id);
        if (room != null) room.IsActive = false;
        return Task.CompletedTask;
    }

    public Task<RoomBooking> BookRoomAsync(RoomBooking booking)
    {
        booking.Id = Guid.NewGuid();
        booking.Status = booking.RequiresApproval ? BookingStatus.Pending : BookingStatus.Confirmed;
        booking.CreatedAt = DateTime.UtcNow;
        _bookings.Add(booking);
        return Task.FromResult(booking);
    }

    public Task<IEnumerable<RoomBooking>> GetRoomBookingsAsync(Guid roomId, DateTime? from = null, DateTime? to = null)
    {
        var bookings = _bookings.Where(b => b.RoomId == roomId);
        if (from.HasValue) bookings = bookings.Where(b => b.EndTime >= from.Value);
        if (to.HasValue) bookings = bookings.Where(b => b.StartTime <= to.Value);
        return Task.FromResult(bookings);
    }

    public Task<RoomBooking> ApproveBookingAsync(Guid bookingId, Guid approvedBy)
    {
        var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
        if (booking != null)
        {
            booking.Status = BookingStatus.Confirmed;
            booking.ApprovedBy = approvedBy;
            booking.ApprovedAt = DateTime.UtcNow;
        }
        return Task.FromResult(booking!);
    }

    public Task<RoomBooking> CancelBookingAsync(Guid bookingId)
    {
        var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
        if (booking != null) booking.Status = BookingStatus.Cancelled;
        return Task.FromResult(booking!);
    }

    public Task<bool> CheckRoomAvailabilityAsync(Guid roomId, DateTime start, DateTime end)
    {
        var hasConflict = _bookings.Any(b => 
            b.RoomId == roomId && 
            b.Status == BookingStatus.Confirmed &&
            b.StartTime < end && b.EndTime > start);
        return Task.FromResult(!hasConflict);
    }

    public Task<Calendar> CreateCalendarAsync(Calendar calendar)
    {
        calendar.Id = Guid.NewGuid();
        calendar.CreatedAt = DateTime.UtcNow;
        _calendars.Add(calendar);
        return Task.FromResult(calendar);
    }

    public Task<Calendar?> GetCalendarByIdAsync(Guid id) =>
        Task.FromResult(_calendars.FirstOrDefault(c => c.Id == id));

    public Task<IEnumerable<Calendar>> GetUserCalendarsAsync(Guid userId) =>
        Task.FromResult(_calendars.Where(c => c.OwnerId == userId));

    public Task<Calendar> UpdateCalendarAsync(Calendar calendar)
    {
        var existing = _calendars.FirstOrDefault(c => c.Id == calendar.Id);
        if (existing != null)
        {
            existing.Name = calendar.Name;
            existing.Color = calendar.Color;
            existing.IsShared = calendar.IsShared;
        }
        return Task.FromResult(existing ?? calendar);
    }

    public Task DeleteCalendarAsync(Guid id)
    {
        var calendar = _calendars.FirstOrDefault(c => c.Id == id);
        if (calendar != null) _calendars.Remove(calendar);
        return Task.CompletedTask;
    }

    public Task<CalendarEvent> CreateEventAsync(CalendarEvent calendarEvent)
    {
        calendarEvent.Id = Guid.NewGuid();
        calendarEvent.CreatedAt = DateTime.UtcNow;
        _events.Add(calendarEvent);
        return Task.FromResult(calendarEvent);
    }

    public Task<CalendarEvent?> GetEventByIdAsync(Guid id) =>
        Task.FromResult(_events.FirstOrDefault(e => e.Id == id));

    public Task<IEnumerable<CalendarEvent>> GetEventsAsync(Guid calendarId, DateTime from, DateTime to) =>
        Task.FromResult(_events.Where(e => e.CalendarId == calendarId && e.StartTime >= from && e.StartTime <= to));

    public Task<IEnumerable<CalendarEvent>> GetUserEventsAsync(Guid userId, DateTime from, DateTime to)
    {
        var userCalendarIds = _calendars.Where(c => c.OwnerId == userId).Select(c => c.Id).ToHashSet();
        return Task.FromResult(_events.Where(e => userCalendarIds.Contains(e.CalendarId) && e.StartTime >= from && e.StartTime <= to));
    }

    public Task<CalendarEvent> UpdateEventAsync(CalendarEvent calendarEvent)
    {
        var existing = _events.FirstOrDefault(e => e.Id == calendarEvent.Id);
        if (existing != null)
        {
            existing.Title = calendarEvent.Title;
            existing.StartTime = calendarEvent.StartTime;
            existing.EndTime = calendarEvent.EndTime;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        return Task.FromResult(existing ?? calendarEvent);
    }

    public Task DeleteEventAsync(Guid id)
    {
        var ev = _events.FirstOrDefault(e => e.Id == id);
        if (ev != null) _events.Remove(ev);
        return Task.CompletedTask;
    }

    public Task<EventAttendee> RespondToEventAsync(Guid eventId, Guid userId, AttendeeResponse response)
    {
        var attendee = new EventAttendee { Id = Guid.NewGuid(), EventId = eventId, UserId = userId, Response = response, RespondedAt = DateTime.UtcNow };
        return Task.FromResult(attendee);
    }

    public Task<string> StartRecordingAsync(Guid meetingId) => Task.FromResult($"rec_{meetingId:N}");
    public Task<string> StopRecordingAsync(Guid meetingId) => Task.FromResult($"https://recordings.ierahkwa.gov/{meetingId}");
    public Task<string> GetRecordingUrlAsync(Guid meetingId) => Task.FromResult($"https://recordings.ierahkwa.gov/{meetingId}");
    public Task<string> GetTranscriptAsync(Guid meetingId) => Task.FromResult("Meeting transcript...");
    public Task<Meeting> GenerateMinutesAsync(Guid meetingId)
    {
        var meeting = _meetings.FirstOrDefault(m => m.Id == meetingId);
        if (meeting != null) meeting.Minutes = "Auto-generated meeting minutes...";
        return Task.FromResult(meeting!);
    }

    public Task<MeetingStatistics> GetStatisticsAsync(Guid? userId = null, string? department = null)
    {
        var meetings = _meetings.AsEnumerable();
        if (userId.HasValue) meetings = meetings.Where(m => m.OrganizerId == userId.Value);
        if (!string.IsNullOrEmpty(department)) meetings = meetings.Where(m => m.Department == department);
        var list = meetings.ToList();

        return Task.FromResult(new MeetingStatistics
        {
            TotalMeetings = list.Count,
            CompletedMeetings = list.Count(m => m.Status == MeetingStatus.Completed),
            CancelledMeetings = list.Count(m => m.Status == MeetingStatus.Cancelled),
            TotalParticipants = _participants.Count,
            AverageDurationMinutes = list.Where(m => m.Duration > 0).DefaultIfEmpty().Average(m => m?.Duration ?? 0),
            MeetingsByType = list.GroupBy(m => m.Type.ToString()).ToDictionary(g => g.Key, g => g.Count())
        });
    }

    private string GenerateMeetingCode() => $"MTG-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
}
