using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models.Hotel;

namespace NET10.API.Controllers;

[ApiController]
[Route("api/hotel")]
public class HotelController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly IReservationService _reservationService;
    private readonly IGuestService _guestService;
    private readonly IRoomService _roomService;
    private readonly IRealEstateService _realEstateService;
    private readonly IFutureheadTrustService _fhtService;
    private readonly IHotelReportsService _reportsService;
    
    public HotelController(
        IPropertyService propertyService,
        IReservationService reservationService,
        IGuestService guestService,
        IRoomService roomService,
        IRealEstateService realEstateService,
        IFutureheadTrustService fhtService,
        IHotelReportsService reportsService)
    {
        _propertyService = propertyService;
        _reservationService = reservationService;
        _guestService = guestService;
        _roomService = roomService;
        _realEstateService = realEstateService;
        _fhtService = fhtService;
        _reportsService = reportsService;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PROPERTIES
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("properties")]
    public async Task<ActionResult<List<Property>>> GetProperties()
    {
        return Ok(await _propertyService.GetAllAsync());
    }
    
    [HttpGet("properties/featured")]
    public async Task<ActionResult<List<Property>>> GetFeaturedProperties()
    {
        return Ok(await _propertyService.GetFeaturedAsync());
    }
    
    [HttpGet("properties/{id}")]
    public async Task<ActionResult<Property>> GetProperty(Guid id)
    {
        var property = await _propertyService.GetByIdAsync(id);
        return property != null ? Ok(property) : NotFound();
    }
    
    [HttpGet("properties/type/{type}")]
    public async Task<ActionResult<List<Property>>> GetPropertiesByType(PropertyType type)
    {
        return Ok(await _propertyService.GetByTypeAsync(type));
    }
    
    [HttpPost("properties")]
    public async Task<ActionResult<Property>> CreateProperty([FromBody] Property property)
    {
        return Ok(await _propertyService.CreateAsync(property));
    }
    
    [HttpPost("properties/search")]
    public async Task<ActionResult<List<Property>>> SearchProperties([FromBody] PropertySearchCriteria criteria)
    {
        return Ok(await _propertyService.SearchAsync(criteria));
    }
    
    // ═══════════════════════════════════════════════════════════════
    // ROOMS
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("properties/{propertyId}/rooms")]
    public async Task<ActionResult<List<Room>>> GetRooms(Guid propertyId)
    {
        return Ok(await _roomService.GetByPropertyAsync(propertyId));
    }
    
    [HttpGet("rooms/{id}")]
    public async Task<ActionResult<Room>> GetRoom(Guid id)
    {
        var room = await _roomService.GetByIdAsync(id);
        return room != null ? Ok(room) : NotFound();
    }
    
    [HttpGet("properties/{propertyId}/rooms/available")]
    public async Task<ActionResult<List<Room>>> GetAvailableRooms(
        Guid propertyId, 
        [FromQuery] DateTime checkIn, 
        [FromQuery] DateTime checkOut)
    {
        return Ok(await _roomService.GetAvailableAsync(propertyId, checkIn, checkOut));
    }
    
    [HttpPost("rooms")]
    public async Task<ActionResult<Room>> CreateRoom([FromBody] Room room)
    {
        return Ok(await _roomService.CreateAsync(room));
    }
    
    [HttpPut("rooms/{id}/status")]
    public async Task<ActionResult<Room>> UpdateRoomStatus(Guid id, [FromQuery] RoomStatus status)
    {
        return Ok(await _roomService.UpdateStatusAsync(id, status));
    }
    
    [HttpGet("properties/{propertyId}/room-types")]
    public async Task<ActionResult<List<RoomType>>> GetRoomTypes(Guid propertyId)
    {
        return Ok(await _roomService.GetRoomTypesAsync(propertyId));
    }
    
    // ═══════════════════════════════════════════════════════════════
    // GUESTS
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("guests")]
    public async Task<ActionResult<List<Guest>>> GetGuests()
    {
        return Ok(await _guestService.GetAllAsync());
    }
    
    [HttpGet("guests/{id}")]
    public async Task<ActionResult<Guest>> GetGuest(Guid id)
    {
        var guest = await _guestService.GetByIdAsync(id);
        return guest != null ? Ok(guest) : NotFound();
    }
    
    [HttpGet("guests/search")]
    public async Task<ActionResult<List<Guest>>> SearchGuests([FromQuery] string q)
    {
        return Ok(await _guestService.SearchAsync(q));
    }
    
    [HttpPost("guests")]
    public async Task<ActionResult<Guest>> CreateGuest([FromBody] Guest guest)
    {
        return Ok(await _guestService.CreateAsync(guest));
    }
    
    [HttpPut("guests/{id}")]
    public async Task<ActionResult<Guest>> UpdateGuest(Guid id, [FromBody] Guest guest)
    {
        guest.Id = id;
        return Ok(await _guestService.UpdateAsync(guest));
    }
    
    [HttpPost("guests/{id}/loyalty")]
    public async Task<ActionResult<Guest>> AddLoyaltyPoints(Guid id, [FromQuery] int points)
    {
        return Ok(await _guestService.AddLoyaltyPointsAsync(id, points));
    }
    
    // ═══════════════════════════════════════════════════════════════
    // RESERVATIONS
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("properties/{propertyId}/reservations")]
    public async Task<ActionResult<List<Reservation>>> GetReservations(Guid propertyId)
    {
        return Ok(await _reservationService.GetByPropertyAsync(propertyId));
    }
    
    [HttpGet("reservations/{id}")]
    public async Task<ActionResult<Reservation>> GetReservation(Guid id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);
        return reservation != null ? Ok(reservation) : NotFound();
    }
    
    [HttpGet("reservations/number/{number}")]
    public async Task<ActionResult<Reservation>> GetReservationByNumber(string number)
    {
        var reservation = await _reservationService.GetByNumberAsync(number);
        return reservation != null ? Ok(reservation) : NotFound();
    }
    
    [HttpPost("reservations")]
    public async Task<ActionResult<Reservation>> CreateReservation([FromBody] Reservation reservation)
    {
        return Ok(await _reservationService.CreateAsync(reservation));
    }
    
    [HttpPost("reservations/{id}/confirm")]
    public async Task<ActionResult<Reservation>> ConfirmReservation(Guid id)
    {
        return Ok(await _reservationService.ConfirmAsync(id));
    }
    
    [HttpPost("reservations/{id}/cancel")]
    public async Task<ActionResult> CancelReservation(Guid id, [FromQuery] string reason)
    {
        var result = await _reservationService.CancelAsync(id, reason);
        return result ? Ok() : NotFound();
    }
    
    [HttpPost("reservations/{id}/checkin")]
    public async Task<ActionResult<Booking>> CheckIn(Guid id)
    {
        return Ok(await _reservationService.CheckInAsync(id));
    }
    
    [HttpPost("bookings/{id}/checkout")]
    public async Task<ActionResult<Booking>> CheckOut(Guid id)
    {
        return Ok(await _reservationService.CheckOutAsync(id));
    }
    
    [HttpGet("properties/{propertyId}/today/checkins")]
    public async Task<ActionResult<List<Reservation>>> GetTodayCheckIns(Guid propertyId)
    {
        return Ok(await _reservationService.GetTodayCheckInsAsync(propertyId));
    }
    
    [HttpGet("properties/{propertyId}/today/checkouts")]
    public async Task<ActionResult<List<Reservation>>> GetTodayCheckOuts(Guid propertyId)
    {
        return Ok(await _reservationService.GetTodayCheckOutsAsync(propertyId));
    }
    
    [HttpGet("pricing")]
    public async Task<ActionResult<ReservationPricing>> CalculatePricing(
        [FromQuery] Guid roomId,
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut,
        [FromQuery] int guests = 2)
    {
        return Ok(await _reservationService.CalculatePricingAsync(roomId, checkIn, checkOut, guests));
    }
    
    // ═══════════════════════════════════════════════════════════════
    // REAL ESTATE
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("realestate")]
    public async Task<ActionResult<List<RealEstateListing>>> GetListings()
    {
        return Ok(await _realEstateService.GetActiveAsync());
    }
    
    [HttpGet("realestate/{id}")]
    public async Task<ActionResult<RealEstateListing>> GetListing(Guid id)
    {
        var listing = await _realEstateService.GetByIdAsync(id);
        if (listing != null)
        {
            await _realEstateService.IncrementViewsAsync(id);
        }
        return listing != null ? Ok(listing) : NotFound();
    }
    
    [HttpPost("realestate")]
    public async Task<ActionResult<RealEstateListing>> CreateListing([FromBody] RealEstateListing listing)
    {
        return Ok(await _realEstateService.CreateAsync(listing));
    }
    
    [HttpPost("realestate/search")]
    public async Task<ActionResult<List<RealEstateListing>>> SearchListings([FromBody] RealEstateSearchCriteria criteria)
    {
        return Ok(await _realEstateService.SearchAsync(criteria));
    }
    
    [HttpPut("realestate/{id}/status")]
    public async Task<ActionResult<RealEstateListing>> UpdateListingStatus(Guid id, [FromQuery] ListingStatus status)
    {
        return Ok(await _realEstateService.UpdateStatusAsync(id, status));
    }
    
    // ═══════════════════════════════════════════════════════════════
    // FUTUREHEAD TRUST COIN (FHT)
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("fht/rate")]
    public async Task<ActionResult<decimal>> GetFHTRate()
    {
        return Ok(await _fhtService.GetFHTExchangeRateAsync());
    }
    
    [HttpGet("fht/convert")]
    public async Task<ActionResult<object>> ConvertCurrency([FromQuery] decimal amount, [FromQuery] string from = "USD")
    {
        if (from.ToUpper() == "USD")
        {
            var fht = await _fhtService.ConvertUSDToFHTAsync(amount);
            return Ok(new { USD = amount, FHT = fht, Rate = await _fhtService.GetFHTExchangeRateAsync() });
        }
        else
        {
            var usd = await _fhtService.ConvertFHTToUSDAsync(amount);
            return Ok(new { FHT = amount, USD = usd, Rate = await _fhtService.GetFHTExchangeRateAsync() });
        }
    }
    
    [HttpPost("fht/transaction")]
    public async Task<ActionResult<FutureheadTrustTransaction>> CreateFHTTransaction([FromBody] FutureheadTrustTransaction tx)
    {
        return Ok(await _fhtService.CreateTransactionAsync(tx));
    }
    
    [HttpGet("fht/transaction/{txHash}")]
    public async Task<ActionResult<FutureheadTrustTransaction>> GetFHTTransaction(string txHash)
    {
        var tx = await _fhtService.GetByHashAsync(txHash);
        return tx != null ? Ok(tx) : NotFound();
    }
    
    [HttpPost("fht/transaction/{txHash}/confirm")]
    public async Task<ActionResult<FutureheadTrustTransaction>> ConfirmFHTTransaction(string txHash, [FromQuery] int confirmations)
    {
        return Ok(await _fhtService.ConfirmTransactionAsync(txHash, confirmations));
    }
    
    [HttpGet("fht/payment-address")]
    public async Task<ActionResult<string>> GeneratePaymentAddress()
    {
        return Ok(await _fhtService.GeneratePaymentAddressAsync());
    }
    
    // ═══════════════════════════════════════════════════════════════
    // REPORTS
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("properties/{propertyId}/dashboard")]
    public async Task<ActionResult<HotelDashboard>> GetDashboard(Guid propertyId)
    {
        return Ok(await _reportsService.GetDashboardAsync(propertyId));
    }
    
    [HttpGet("properties/{propertyId}/reports/occupancy")]
    public async Task<ActionResult<OccupancyReport>> GetOccupancyReport(
        Guid propertyId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow;
        return Ok(await _reportsService.GetOccupancyReportAsync(propertyId, fromDate, toDate));
    }
    
    [HttpGet("properties/{propertyId}/reports/revenue")]
    public async Task<ActionResult<RevenueReport>> GetRevenueReport(
        Guid propertyId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow;
        return Ok(await _reportsService.GetRevenueReportAsync(propertyId, fromDate, toDate));
    }
    
    [HttpGet("properties/{propertyId}/reports/transactions/daily")]
    public async Task<ActionResult<List<TransactionSummary>>> GetDailyTransactions(
        Guid propertyId,
        [FromQuery] DateTime? date)
    {
        return Ok(await _reportsService.GetDailyTransactionsAsync(propertyId, date ?? DateTime.UtcNow));
    }
    
    [HttpGet("properties/{propertyId}/reports/transactions/weekly")]
    public async Task<ActionResult<List<TransactionSummary>>> GetWeeklyTransactions(
        Guid propertyId,
        [FromQuery] DateTime? weekStart)
    {
        return Ok(await _reportsService.GetWeeklyTransactionsAsync(propertyId, weekStart ?? DateTime.UtcNow.AddDays(-7)));
    }
    
    [HttpGet("properties/{propertyId}/reports/transactions/monthly")]
    public async Task<ActionResult<List<TransactionSummary>>> GetMonthlyTransactions(
        Guid propertyId,
        [FromQuery] int? year,
        [FromQuery] int? month)
    {
        var y = year ?? DateTime.UtcNow.Year;
        var m = month ?? DateTime.UtcNow.Month;
        return Ok(await _reportsService.GetMonthlyTransactionsAsync(propertyId, y, m));
    }
}
