using MeetingHub.Core.Interfaces;
using MeetingHub.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeetingHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeetingsController : ControllerBase
{
    private readonly IMeetingService _meetingService;
    public MeetingsController(IMeetingService meetingService) => _meetingService = meetingService;

    [HttpPost]
    public async Task<ActionResult<Meeting>> Create([FromBody] Meeting meeting)
    {
        var created = await _meetingService.CreateMeetingAsync(meeting);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Meeting>> GetById(Guid id)
    {
        var meeting = await _meetingService.GetMeetingByIdAsync(id);
        return meeting == null ? NotFound() : meeting;
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<Meeting>> GetByCode(string code)
    {
        var meeting = await _meetingService.GetMeetingByCodeAsync(code);
        return meeting == null ? NotFound() : meeting;
    }

    [HttpGet("organizer/{organizerId}")]
    public async Task<ActionResult<IEnumerable<Meeting>>> GetByOrganizer(Guid organizerId) =>
        Ok(await _meetingService.GetMeetingsByOrganizerAsync(organizerId));

    [HttpGet("upcoming/{userId}")]
    public async Task<ActionResult<IEnumerable<Meeting>>> GetUpcoming(Guid userId, [FromQuery] int days = 7) =>
        Ok(await _meetingService.GetUpcomingMeetingsAsync(userId, days));

    [HttpPut("{id}")]
    public async Task<ActionResult<Meeting>> Update(Guid id, [FromBody] Meeting meeting)
    {
        if (id != meeting.Id) return BadRequest();
        return await _meetingService.UpdateMeetingAsync(meeting);
    }

    [HttpPost("{id}/start")]
    public async Task<ActionResult<Meeting>> Start(Guid id) => await _meetingService.StartMeetingAsync(id);

    [HttpPost("{id}/end")]
    public async Task<ActionResult<Meeting>> End(Guid id) => await _meetingService.EndMeetingAsync(id);

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<Meeting>> Cancel(Guid id, [FromBody] string? reason) =>
        await _meetingService.CancelMeetingAsync(id, reason);

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id) { await _meetingService.DeleteMeetingAsync(id); return NoContent(); }

    [HttpPost("{id}/participants")]
    public async Task<ActionResult<MeetingParticipant>> AddParticipant(Guid id, [FromBody] MeetingParticipant participant) =>
        await _meetingService.AddParticipantAsync(id, participant);

    [HttpGet("{id}/participants")]
    public async Task<ActionResult<IEnumerable<MeetingParticipant>>> GetParticipants(Guid id) =>
        Ok(await _meetingService.GetParticipantsAsync(id));

    [HttpPost("{id}/join/{userId}")]
    public async Task<ActionResult<MeetingParticipant>> Join(Guid id, Guid userId, [FromQuery] string? device) =>
        await _meetingService.JoinMeetingAsync(id, userId, device);

    [HttpPost("{id}/leave/{userId}")]
    public async Task<ActionResult<MeetingParticipant>> Leave(Guid id, Guid userId) =>
        await _meetingService.LeaveMeetingAsync(id, userId);

    [HttpPost("{id}/recording/start")]
    public async Task<ActionResult<string>> StartRecording(Guid id) => Ok(await _meetingService.StartRecordingAsync(id));

    [HttpPost("{id}/recording/stop")]
    public async Task<ActionResult<string>> StopRecording(Guid id) => Ok(await _meetingService.StopRecordingAsync(id));

    [HttpGet("statistics")]
    public async Task<ActionResult<MeetingStatistics>> GetStatistics([FromQuery] Guid? userId, [FromQuery] string? department) =>
        await _meetingService.GetStatisticsAsync(userId, department);
}
