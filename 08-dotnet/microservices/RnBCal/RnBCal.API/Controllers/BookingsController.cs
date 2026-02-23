using Microsoft.AspNetCore.Mvc;
using RnBCal.Core.Models;

namespace RnBCal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly ILogger<BookingsController> _logger;
    private static readonly List<Booking> _bookings = new();

    public BookingsController(ILogger<BookingsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all bookings
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_bookings.OrderByDescending(b => b.CreatedAt));
    }

    /// <summary>
    /// Get booking by ID
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var booking = _bookings.FirstOrDefault(b => b.Id == id);
        if (booking == null)
            return NotFound(new { error = "Booking not found" });
        
        return Ok(booking);
    }

    /// <summary>
    /// Create new booking
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] Booking booking)
    {
        booking.Id = Guid.NewGuid().ToString();
        booking.CreatedAt = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;
        
        _bookings.Add(booking);
        _logger.LogInformation($"Created booking {booking.Id}");
        
        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    /// <summary>
    /// Update booking
    /// </summary>
    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] Booking booking)
    {
        var existing = _bookings.FirstOrDefault(b => b.Id == id);
        if (existing == null)
            return NotFound(new { error = "Booking not found" });
        
        booking.Id = id;
        booking.CreatedAt = existing.CreatedAt;
        booking.UpdatedAt = DateTime.UtcNow;
        
        _bookings.Remove(existing);
        _bookings.Add(booking);
        
        _logger.LogInformation($"Updated booking {booking.Id}");
        
        return Ok(booking);
    }

    /// <summary>
    /// Delete booking
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var booking = _bookings.FirstOrDefault(b => b.Id == id);
        if (booking == null)
            return NotFound(new { error = "Booking not found" });
        
        _bookings.Remove(booking);
        _logger.LogInformation($"Deleted booking {id}");
        
        return NoContent();
    }

    /// <summary>
    /// Get booking statistics
    /// </summary>
    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        var stats = new
        {
            totalBookings = _bookings.Count,
            byStatus = _bookings.GroupBy(b => b.Status)
                .Select(g => new { status = g.Key.ToString(), count = g.Count() }),
            byType = _bookings.GroupBy(b => b.Type)
                .Select(g => new { type = g.Key.ToString(), count = g.Count() }),
            totalRevenue = _bookings.Sum(b => b.TotalAmount),
            recentBookings = _bookings.OrderByDescending(b => b.CreatedAt).Take(5)
        };
        
        return Ok(stats);
    }
}
