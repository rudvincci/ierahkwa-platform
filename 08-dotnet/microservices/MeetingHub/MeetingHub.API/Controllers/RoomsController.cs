using MeetingHub.Core.Interfaces;
using MeetingHub.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeetingHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IMeetingService _meetingService;
    public RoomsController(IMeetingService meetingService) => _meetingService = meetingService;

    [HttpPost]
    public async Task<ActionResult<MeetingRoom>> Create([FromBody] MeetingRoom room) =>
        CreatedAtAction(nameof(GetById), new { id = (await _meetingService.CreateRoomAsync(room)).Id }, room);

    [HttpGet("{id}")]
    public async Task<ActionResult<MeetingRoom>> GetById(Guid id)
    {
        var room = await _meetingService.GetRoomByIdAsync(id);
        return room == null ? NotFound() : room;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MeetingRoom>>> GetAll([FromQuery] string? location) =>
        Ok(await _meetingService.GetRoomsAsync(location));

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<MeetingRoom>>> GetAvailable([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] int? minCapacity) =>
        Ok(await _meetingService.GetAvailableRoomsAsync(start, end, minCapacity));

    [HttpPut("{id}")]
    public async Task<ActionResult<MeetingRoom>> Update(Guid id, [FromBody] MeetingRoom room) =>
        id != room.Id ? BadRequest() : await _meetingService.UpdateRoomAsync(room);

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id) { await _meetingService.DeleteRoomAsync(id); return NoContent(); }

    [HttpPost("{id}/book")]
    public async Task<ActionResult<RoomBooking>> Book(Guid id, [FromBody] RoomBooking booking)
    {
        booking.RoomId = id;
        return await _meetingService.BookRoomAsync(booking);
    }

    [HttpGet("{id}/bookings")]
    public async Task<ActionResult<IEnumerable<RoomBooking>>> GetBookings(Guid id, [FromQuery] DateTime? from, [FromQuery] DateTime? to) =>
        Ok(await _meetingService.GetRoomBookingsAsync(id, from, to));

    [HttpGet("{id}/availability")]
    public async Task<ActionResult<bool>> CheckAvailability(Guid id, [FromQuery] DateTime start, [FromQuery] DateTime end) =>
        Ok(await _meetingService.CheckRoomAvailabilityAsync(id, start, end));
}
