using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeX.Core.Interfaces;
using TradeX.Core.Models;

namespace TradeX.Infrastructure.Services;

/// <summary>
/// Trading Service - Ierahkwa TradeX Exchange
/// Spot trading with order matching engine
/// </summary>
public class TradingService : ITradingService
{
    private readonly IIerahkwaNodeService _nodeService;
    private readonly IWalletService _walletService;
    
    // In-memory order books (use Redis in production)
    private static readonly Dictionary<Guid, List<Order>> _buyOrders = new();
    private static readonly Dictionary<Guid, List<Order>> _sellOrders = new();
    private static readonly List<Order> _allOrders = new();
    private static readonly List<Trade> _allTrades = new();
    private static readonly List<TradingPair> _tradingPairs = InitializeTradingPairs();
    
    public TradingService(IIerahkwaNodeService nodeService, IWalletService walletService)
    {
        _nodeService = nodeService;
        _walletService = walletService;
    }
    
    public async Task<Order> PlaceOrderAsync(Guid userId, Guid tradingPairId, OrderSide side, 
        OrderType type, decimal amount, decimal price, decimal? stopPrice = null)
    {
        var pair = _tradingPairs.FirstOrDefault(p => p.Id == tradingPairId)
            ?? throw new Exception("Trading pair not found");
        
        // Validate order
        if (amount < pair.MinOrderAmount)
            throw new Exception($"Minimum order amount is {pair.MinOrderAmount}");
        if (amount > pair.MaxOrderAmount)
            throw new Exception($"Maximum order amount is {pair.MaxOrderAmount}");
        
        // For market orders, use current market price
        if (type == OrderType.Market)
        {
            price = pair.LastPrice;
        }
        
        // Calculate fee
        var fee = amount * price * pair.TakerFee;
        
        // Create order
        var order = new Order
        {
            UserId = userId,
            TradingPairId = tradingPairId,
            Side = side,
            Type = type,
            Price = price,
            Amount = amount,
            StopPrice = stopPrice,
            Fee = fee,
            Status = OrderStatus.Open
        };
        
        // Lock funds in wallet
        var assetToLock = side == OrderSide.Buy ? pair.QuoteAssetId : pair.BaseAssetId;
        var amountToLock = side == OrderSide.Buy ? amount * price : amount;
        
        // Add to order book
        lock (_allOrders)
        {
            _allOrders.Add(order);
            
            if (!_buyOrders.ContainsKey(tradingPairId))
                _buyOrders[tradingPairId] = new List<Order>();
            if (!_sellOrders.ContainsKey(tradingPairId))
                _sellOrders[tradingPairId] = new List<Order>();
            
            if (side == OrderSide.Buy)
                _buyOrders[tradingPairId].Add(order);
            else
                _sellOrders[tradingPairId].Add(order);
        }
        
        // Try to match orders immediately
        await MatchOrdersAsync(tradingPairId);
        
        return order;
    }
    
    public Task<Order?> CancelOrderAsync(Guid orderId, Guid userId)
    {
        lock (_allOrders)
        {
            var order = _allOrders.FirstOrDefault(o => o.Id == orderId && o.UserId == userId);
            if (order == null || order.Status != OrderStatus.Open)
                return Task.FromResult<Order?>(null);
            
            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;
            
            // Remove from order book
            if (order.Side == OrderSide.Buy && _buyOrders.ContainsKey(order.TradingPairId))
                _buyOrders[order.TradingPairId].Remove(order);
            else if (_sellOrders.ContainsKey(order.TradingPairId))
                _sellOrders[order.TradingPairId].Remove(order);
            
            return Task.FromResult<Order?>(order);
        }
    }
    
    public Task<IEnumerable<Order>> GetOpenOrdersAsync(Guid userId)
    {
        var orders = _allOrders
            .Where(o => o.UserId == userId && o.Status == OrderStatus.Open)
            .OrderByDescending(o => o.CreatedAt);
        return Task.FromResult<IEnumerable<Order>>(orders);
    }
    
    public Task<IEnumerable<Order>> GetOrderHistoryAsync(Guid userId, int page = 1, int pageSize = 50)
    {
        var orders = _allOrders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        return Task.FromResult<IEnumerable<Order>>(orders);
    }
    
    public Task<IEnumerable<Trade>> GetTradeHistoryAsync(Guid userId, int page = 1, int pageSize = 50)
    {
        var trades = _allTrades
            .Where(t => t.BuyerId == userId || t.SellerId == userId)
            .OrderByDescending(t => t.ExecutedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        return Task.FromResult<IEnumerable<Trade>>(trades);
    }
    
    public Task<IEnumerable<TradingPair>> GetTradingPairsAsync()
    {
        return Task.FromResult<IEnumerable<TradingPair>>(_tradingPairs.Where(p => p.IsActive));
    }
    
    public Task<TradingPair?> GetTradingPairAsync(Guid id)
    {
        return Task.FromResult(_tradingPairs.FirstOrDefault(p => p.Id == id));
    }
    
    public Task MatchOrdersAsync(Guid tradingPairId)
    {
        lock (_allOrders)
        {
            if (!_buyOrders.ContainsKey(tradingPairId) || !_sellOrders.ContainsKey(tradingPairId))
                return Task.CompletedTask;
            
            var buys = _buyOrders[tradingPairId]
                .Where(o => o.Status == OrderStatus.Open)
                .OrderByDescending(o => o.Price)
                .ThenBy(o => o.CreatedAt)
                .ToList();
            
            var sells = _sellOrders[tradingPairId]
                .Where(o => o.Status == OrderStatus.Open)
                .OrderBy(o => o.Price)
                .ThenBy(o => o.CreatedAt)
                .ToList();
            
            foreach (var buyOrder in buys)
            {
                foreach (var sellOrder in sells)
                {
                    if (buyOrder.Status != OrderStatus.Open || sellOrder.Status != OrderStatus.Open)
                        continue;
                    
                    // Check if prices match
                    if (buyOrder.Price < sellOrder.Price)
                        continue;
                    
                    // Calculate trade amount
                    var tradeAmount = Math.Min(buyOrder.RemainingAmount, sellOrder.RemainingAmount);
                    var tradePrice = sellOrder.Price; // Price at sell order level
                    
                    // Execute trade
                    var trade = new Trade
                    {
                        BuyOrderId = buyOrder.Id,
                        SellOrderId = sellOrder.Id,
                        TradingPairId = tradingPairId,
                        BuyerId = buyOrder.UserId,
                        SellerId = sellOrder.UserId,
                        Price = tradePrice,
                        Amount = tradeAmount,
                        BuyerFee = tradeAmount * tradePrice * 0.001m,
                        SellerFee = tradeAmount * tradePrice * 0.001m,
                        IsBotTrade = buyOrder.IsBot || sellOrder.IsBot
                    };
                    
                    _allTrades.Add(trade);
                    
                    // Update orders
                    buyOrder.FilledAmount += tradeAmount;
                    sellOrder.FilledAmount += tradeAmount;
                    
                    if (buyOrder.RemainingAmount == 0)
                    {
                        buyOrder.Status = OrderStatus.Filled;
                        buyOrder.FilledAt = DateTime.UtcNow;
                    }
                    else if (buyOrder.FilledAmount > 0)
                    {
                        buyOrder.Status = OrderStatus.PartiallyFilled;
                    }
                    
                    if (sellOrder.RemainingAmount == 0)
                    {
                        sellOrder.Status = OrderStatus.Filled;
                        sellOrder.FilledAt = DateTime.UtcNow;
                    }
                    else if (sellOrder.FilledAmount > 0)
                    {
                        sellOrder.Status = OrderStatus.PartiallyFilled;
                    }
                    
                    // Update pair last price
                    var pair = _tradingPairs.First(p => p.Id == tradingPairId);
                    pair.LastPrice = tradePrice;
                    pair.Volume24h += tradeAmount * tradePrice;
                }
            }
        }
        
        return Task.CompletedTask;
    }
    
    private static List<TradingPair> InitializeTradingPairs()
    {
        // Initialize with IGT token pairs
        return new List<TradingPair>
        {
            new() { Symbol = "IGT-PM/USDT", LastPrice = 1.0m, IsActive = true },
            new() { Symbol = "IGT-BDET/USDT", LastPrice = 1.0m, IsActive = true },
            new() { Symbol = "IGT-MFT/USDT", LastPrice = 1.0m, IsActive = true },
            new() { Symbol = "BTC/USDT", LastPrice = 65000m, IsActive = true },
            new() { Symbol = "ETH/USDT", LastPrice = 3500m, IsActive = true },
            new() { Symbol = "BNB/USDT", LastPrice = 550m, IsActive = true },
            new() { Symbol = "SOL/USDT", LastPrice = 150m, IsActive = true },
            new() { Symbol = "IGT-PM/BTC", LastPrice = 0.000015m, IsActive = true },
            new() { Symbol = "IGT-PM/ETH", LastPrice = 0.00029m, IsActive = true },
        };
    }
}
