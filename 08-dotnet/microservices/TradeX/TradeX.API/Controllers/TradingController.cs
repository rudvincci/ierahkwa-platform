using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeX.Core.Interfaces;
using TradeX.Core.Models;

namespace TradeX.API.Controllers;

/// <summary>
/// Trading API - Ierahkwa TradeX Exchange
/// Spot trading with order matching engine
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TradingController : ControllerBase
{
    private readonly ITradingService _tradingService;
    private readonly ILogger<TradingController> _logger;
    
    public TradingController(ITradingService tradingService, ILogger<TradingController> logger)
    {
        _tradingService = tradingService;
        _logger = logger;
    }
    
    /// <summary>
    /// Get all trading pairs
    /// </summary>
    [HttpGet("pairs")]
    public async Task<ActionResult<IEnumerable<TradingPair>>> GetTradingPairs()
    {
        var pairs = await _tradingService.GetTradingPairsAsync();
        return Ok(pairs);
    }
    
    /// <summary>
    /// Get specific trading pair
    /// </summary>
    [HttpGet("pairs/{id}")]
    public async Task<ActionResult<TradingPair>> GetTradingPair(Guid id)
    {
        var pair = await _tradingService.GetTradingPairAsync(id);
        if (pair == null) return NotFound();
        return Ok(pair);
    }
    
    /// <summary>
    /// Place a new order
    /// </summary>
    [HttpPost("orders")]
    public async Task<ActionResult<Order>> PlaceOrder([FromBody] PlaceOrderRequest request)
    {
        try
        {
            var order = await _tradingService.PlaceOrderAsync(
                request.UserId,
                request.TradingPairId,
                request.Side,
                request.Type,
                request.Amount,
                request.Price,
                request.StopPrice
            );
            
            _logger.LogInformation("Order placed: {OrderId} by user {UserId}", order.Id, request.UserId);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("orders/{id}")]
    public async Task<ActionResult<Order>> GetOrder(Guid id)
    {
        var orders = await _tradingService.GetOrderHistoryAsync(Guid.Empty);
        var order = orders.FirstOrDefault(o => o.Id == id);
        if (order == null) return NotFound();
        return Ok(order);
    }
    
    /// <summary>
    /// Cancel an order
    /// </summary>
    [HttpDelete("orders/{id}")]
    public async Task<ActionResult<Order>> CancelOrder(Guid id, [FromQuery] Guid userId)
    {
        var order = await _tradingService.CancelOrderAsync(id, userId);
        if (order == null) return NotFound();
        return Ok(order);
    }
    
    /// <summary>
    /// Get user's open orders
    /// </summary>
    [HttpGet("orders/open/{userId}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOpenOrders(Guid userId)
    {
        var orders = await _tradingService.GetOpenOrdersAsync(userId);
        return Ok(orders);
    }
    
    /// <summary>
    /// Get user's order history
    /// </summary>
    [HttpGet("orders/history/{userId}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrderHistory(
        Guid userId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 50)
    {
        var orders = await _tradingService.GetOrderHistoryAsync(userId, page, pageSize);
        return Ok(orders);
    }
    
    /// <summary>
    /// Get user's trade history
    /// </summary>
    [HttpGet("trades/{userId}")]
    public async Task<ActionResult<IEnumerable<Trade>>> GetTradeHistory(
        Guid userId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 50)
    {
        var trades = await _tradingService.GetTradeHistoryAsync(userId, page, pageSize);
        return Ok(trades);
    }
}

public record PlaceOrderRequest(
    Guid UserId,
    Guid TradingPairId,
    OrderSide Side,
    OrderType Type,
    decimal Amount,
    decimal Price,
    decimal? StopPrice = null
);
