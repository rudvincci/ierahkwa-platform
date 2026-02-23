using MeetingHub.Core.Interfaces;
using MeetingHub.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeetingHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalendarsController : ControllerBase
{
    private readonly IMeetingService _meetingService;
    public CalendarsController(IMeetingService meetingService) => _meetingService = meetingService;

    [HttpPost]
    public async Task<ActionResult<Calendar>> Create([FromBody] Calendar calendar) =>
        CreatedAtAction(nameof(GetById), new { id = (await _meetingService.CreateCalendarAsync(calendar)).Id }, calendar);

    [HttpGet("{id}")]
    public async Task<ActionResult<Calendar>> GetById(Guid id)
    {
        var cal = await _meetingService.GetCalendarByIdAsync(id);
        return cal == null ? NotFound() : cal;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Calendar>>> GetByUser(Guid userId) =>
        Ok(await _meetingService.GetUserCalendarsAsync(userId));

    [HttpPut("{id}")]
    public async Task<ActionResult<Calendar>> Update(Guid id, [FromBody] Calendar calendar) =>
        id != calendar.Id ? BadRequest() : await _meetingService.UpdateCalendarAsync(calendar);

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id) { await _meetingService.DeleteCalendarAsync(id); return NoContent(); }

    [HttpPost("events")]
    public async Task<ActionResult<CalendarEvent>> CreateEvent([FromBody] CalendarEvent ev) =>
        CreatedAtAction(nameof(GetEvent), new { id = (await _meetingService.CreateEventAsync(ev)).Id }, ev);

    [HttpGet("events/{id}")]
    public async Task<ActionResult<CalendarEvent>> GetEvent(Guid id)
    {
        var ev = await _meetingService.GetEventByIdAsync(id);
        return ev == null ? NotFound() : ev;
    }

    [HttpGet("{id}/events")]
    public async Task<ActionResult<IEnumerable<CalendarEvent>>> GetEvents(Guid id, [FromQuery] DateTime from, [FromQuery] DateTime to) =>
        Ok(await _meetingService.GetEventsAsync(id, from, to));

    [HttpGet("user/{userId}/events")]
    public async Task<ActionResult<IEnumerable<CalendarEvent>>> GetUserEvents(Guid userId, [FromQuery] DateTime from, [FromQuery] DateTime to) =>
        Ok(await _meetingService.GetUserEventsAsync(userId, from, to));

    [HttpPut("events/{id}")]
    public async Task<ActionResult<CalendarEvent>> UpdateEvent(Guid id, [FromBody] CalendarEvent ev) =>
        id != ev.Id ? BadRequest() : await _meetingService.UpdateEventAsync(ev);

    [HttpDelete("events/{id}")]
    public async Task<ActionResult> DeleteEvent(Guid id) { await _meetingService.DeleteEventAsync(id); return NoContent(); }

    [HttpPost("events/{id}/respond")]
    public async Task<ActionResult<EventAttendee>> Respond(Guid id, [FromQuery] Guid userId, [FromBody] AttendeeResponse response) =>
        await _meetingService.RespondToEventAsync(id, userId, response);
}
