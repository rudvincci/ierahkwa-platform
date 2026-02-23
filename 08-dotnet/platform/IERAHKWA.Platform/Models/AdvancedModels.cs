namespace IERAHKWA.Platform.Models;

// ============================================================================
//                    IERAHKWA ADVANCED PLATFORM MODELS
//                    Banco BDET + Mamey Node Technology
// ============================================================================

// ==================== ANALYTICS & MONITORING ====================

public class AnalyticsDashboard
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public PlatformMetrics Metrics { get; set; } = new();
    public List<LiveChart> Charts { get; set; } = new();
    public List<AlertRule> Alerts { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class PlatformMetrics
{
    // Users
    public long TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int OnlineNow { get; set; }
    public int NewUsersToday { get; set; }
    
    // Transactions
    public long TotalTransactions { get; set; }
    public decimal TotalVolume { get; set; }
    public decimal Volume24h { get; set; }
    public int TransactionsPerSecond { get; set; }
    
    // Blockchain
    public long BlockHeight { get; set; }
    public int PeerCount { get; set; }
    public double NetworkHashrate { get; set; }
    public decimal GasPrice { get; set; }
    
    // Services
    public Dictionary<string, ServiceStatus> Services { get; set; } = new();
    public double UptimePercent { get; set; } = 99.99;
    public double AverageResponseMs { get; set; }
    public int ErrorsLast24h { get; set; }
    
    // Financial
    public decimal TotalValueLocked { get; set; }
    public decimal WampumCirculating { get; set; }
    public decimal WampumPrice { get; set; }
    public decimal MarketCap { get; set; }
}

public class ServiceStatus
{
    public string Name { get; set; } = "";
    public string Status { get; set; } = "healthy"; // healthy, degraded, down
    public double ResponseTimeMs { get; set; }
    public int RequestsPerMinute { get; set; }
    public double ErrorRate { get; set; }
    public DateTime LastCheck { get; set; } = DateTime.UtcNow;
}

public class LiveChart
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Type { get; set; } = "line"; // line, bar, pie, area, candlestick
    public List<ChartDataPoint> Data { get; set; } = new();
    public string TimeFrame { get; set; } = "1h"; // 1m, 5m, 15m, 1h, 4h, 1d
}

public class ChartDataPoint
{
    public DateTime Timestamp { get; set; }
    public decimal Value { get; set; }
    public decimal? Open { get; set; }
    public decimal? High { get; set; }
    public decimal? Low { get; set; }
    public decimal? Close { get; set; }
    public decimal? Volume { get; set; }
}

public class AlertRule
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Metric { get; set; } = "";
    public string Condition { get; set; } = "gt"; // gt, lt, eq, gte, lte
    public decimal Threshold { get; set; }
    public string Severity { get; set; } = "warning"; // info, warning, critical
    public List<string> NotifyChannels { get; set; } = new(); // email, sms, push, webhook
    public bool IsActive { get; set; } = true;
}

// ==================== SECURITY SUITE ====================

public class SecurityConfig
{
    public bool TwoFactorEnabled { get; set; } = true;
    public bool BiometricEnabled { get; set; } = true;
    public int MaxLoginAttempts { get; set; } = 5;
    public int LockoutMinutes { get; set; } = 30;
    public int SessionTimeoutMinutes { get; set; } = 60;
    public bool RequireStrongPassword { get; set; } = true;
    public List<string> WhitelistedIPs { get; set; } = new();
    public List<string> BlacklistedIPs { get; set; } = new();
}

public class TwoFactorAuth
{
    public string UserId { get; set; } = "";
    public string Method { get; set; } = "totp"; // totp, sms, email, biometric
    public string Secret { get; set; } = "";
    public List<string> BackupCodes { get; set; } = new();
    public bool IsVerified { get; set; } = false;
    public DateTime? LastUsed { get; set; }
}

public class RateLimitRule
{
    public string Endpoint { get; set; } = "";
    public int RequestsPerMinute { get; set; } = 60;
    public int RequestsPerHour { get; set; } = 1000;
    public int BurstLimit { get; set; } = 10;
    public List<string> ExemptRoles { get; set; } = new();
}

public class SecurityAuditLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string Action { get; set; } = "";
    public string Resource { get; set; } = "";
    public string IpAddress { get; set; } = "";
    public string UserAgent { get; set; } = "";
    public string Result { get; set; } = "success"; // success, failure, blocked
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// ==================== REAL-TIME WEBSOCKETS ====================

public class WebSocketConnection
{
    public string ConnectionId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public List<string> Subscriptions { get; set; } = new();
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
}

public class RealtimeMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Channel { get; set; } = "";
    public string Event { get; set; } = "";
    public object Data { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class PushNotification
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Action { get; set; } = "";
    public Dictionary<string, string> Data { get; set; } = new();
    public bool IsRead { get; set; } = false;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}

public class LiveChat
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RoomId { get; set; } = "";
    public List<string> Participants { get; set; } = new();
    public List<ChatMessage> Messages { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SenderId { get; set; } = "";
    public string SenderName { get; set; } = "";
    public string Content { get; set; } = "";
    public string Type { get; set; } = "text"; // text, image, file, system
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// ==================== MULTI-CHAIN BRIDGE ====================

public class ChainConfig
{
    public string ChainId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Symbol { get; set; } = "";
    public string RpcUrl { get; set; } = "";
    public string ExplorerUrl { get; set; } = "";
    public string BridgeContract { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public int Confirmations { get; set; } = 12;
}

public class SupportedChain
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Symbol { get; set; } = "";
    public string Logo { get; set; } = "";
    public int ChainId { get; set; }
    public bool IsEVM { get; set; } = true;
    public decimal GasPrice { get; set; }
    public List<BridgeToken> SupportedTokens { get; set; } = new();
}

public class BridgeToken
{
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
    public string ContractAddress { get; set; } = "";
    public int Decimals { get; set; } = 18;
    public string Logo { get; set; } = "";
    public decimal MinBridge { get; set; }
    public decimal MaxBridge { get; set; }
    public decimal BridgeFee { get; set; }
}

public class BridgeTransaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    
    // Source
    public string SourceChain { get; set; } = "";
    public string SourceTxHash { get; set; } = "";
    public string SourceAddress { get; set; } = "";
    
    // Destination
    public string DestChain { get; set; } = "";
    public string DestTxHash { get; set; } = "";
    public string DestAddress { get; set; } = "";
    
    // Amount
    public string Token { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public decimal AmountReceived { get; set; }
    
    // Status
    public string Status { get; set; } = "pending"; // pending, confirming, bridging, completed, failed
    public int Confirmations { get; set; }
    public int RequiredConfirmations { get; set; }
    public string? ErrorMessage { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}

// ==================== TRADING PRO ====================

public class TradingPair
{
    public string Id { get; set; } = "";
    public string BaseToken { get; set; } = ""; // WAMPUM
    public string QuoteToken { get; set; } = ""; // USDT
    public string Symbol { get; set; } = ""; // WAMPUM/USDT
    public decimal LastPrice { get; set; }
    public decimal PriceChange24h { get; set; }
    public decimal PriceChangePercent24h { get; set; }
    public decimal High24h { get; set; }
    public decimal Low24h { get; set; }
    public decimal Volume24h { get; set; }
    public decimal VolumeQuote24h { get; set; }
    public int Decimals { get; set; } = 8;
    public decimal MinOrderSize { get; set; }
    public decimal MaxOrderSize { get; set; }
    public decimal MakerFee { get; set; } = 0.001m;
    public decimal TakerFee { get; set; } = 0.002m;
    public bool IsActive { get; set; } = true;
}

public class OrderBook
{
    public string PairId { get; set; } = "";
    public List<OrderBookLevel> Bids { get; set; } = new();
    public List<OrderBookLevel> Asks { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class OrderBookLevel
{
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public int OrderCount { get; set; }
}

public class TradeOrder
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string PairId { get; set; } = "";
    public string Side { get; set; } = "buy"; // buy, sell
    public string Type { get; set; } = "limit"; // market, limit, stop_limit, stop_market
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public decimal FilledQuantity { get; set; }
    public decimal AveragePrice { get; set; }
    public string Status { get; set; } = "open"; // open, partial, filled, cancelled, expired
    public decimal? StopPrice { get; set; }
    public string TimeInForce { get; set; } = "GTC"; // GTC, IOC, FOK
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FilledAt { get; set; }
}

public class Trade
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PairId { get; set; } = "";
    public string BuyOrderId { get; set; } = "";
    public string SellOrderId { get; set; } = "";
    public string BuyerId { get; set; } = "";
    public string SellerId { get; set; } = "";
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public decimal BuyerFee { get; set; }
    public decimal SellerFee { get; set; }
    public string Side { get; set; } = ""; // buy or sell (taker side)
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class TradingBot
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Strategy { get; set; } = "grid"; // grid, dca, arbitrage, market_maker
    public string PairId { get; set; } = "";
    public TradingBotConfig Config { get; set; } = new();
    public TradingBotStats Stats { get; set; } = new();
    public bool IsActive { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class TradingBotConfig
{
    public decimal Investment { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public int GridLevels { get; set; } = 10;
    public decimal TakeProfit { get; set; }
    public decimal StopLoss { get; set; }
    public bool AutoRestart { get; set; } = true;
}

public class TradingBotStats
{
    public decimal TotalProfit { get; set; }
    public decimal ProfitPercent { get; set; }
    public int TotalTrades { get; set; }
    public int WinningTrades { get; set; }
    public decimal MaxDrawdown { get; set; }
    public DateTime? LastTradeAt { get; set; }
}

// ==================== METAVERSE ====================

public class MetaverseWorld
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string OwnerId { get; set; } = "";
    public string Theme { get; set; } = "modern"; // modern, futuristic, nature, fantasy
    public WorldSettings Settings { get; set; } = new();
    public List<MetaverseSpace> Spaces { get; set; } = new();
    public int VisitorCount { get; set; }
    public bool IsPublic { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class WorldSettings
{
    public string Skybox { get; set; } = "day";
    public string Music { get; set; } = "";
    public bool AllowBuilding { get; set; } = false;
    public bool AllowTrading { get; set; } = true;
    public int MaxVisitors { get; set; } = 100;
    public decimal EntryFee { get; set; } = 0;
}

public class MetaverseSpace
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string WorldId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = "plaza"; // plaza, bank, casino, gallery, shop, office
    public Vector3D Position { get; set; } = new();
    public Vector3D Size { get; set; } = new();
    public List<MetaverseObject> Objects { get; set; } = new();
    public string? NftGalleryId { get; set; }
}

public class Vector3D
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}

public class MetaverseObject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = ""; // model, nft, screen, portal, npc
    public string ModelUrl { get; set; } = "";
    public Vector3D Position { get; set; } = new();
    public Vector3D Rotation { get; set; } = new();
    public Vector3D Scale { get; set; } = new() { X = 1, Y = 1, Z = 1 };
    public Dictionary<string, object> Properties { get; set; } = new();
    public bool IsInteractive { get; set; } = false;
    public string? OnClickAction { get; set; }
}

public class MetaverseAvatar
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string Name { get; set; } = "";
    public string ModelUrl { get; set; } = "";
    public AvatarAppearance Appearance { get; set; } = new();
    public List<string> OwnedWearables { get; set; } = new();
    public List<string> EquippedWearables { get; set; } = new();
}

public class AvatarAppearance
{
    public string SkinTone { get; set; } = "#FFD5B4";
    public string HairColor { get; set; } = "#2C1810";
    public string HairStyle { get; set; } = "default";
    public string EyeColor { get; set; } = "#4A90D9";
    public string Outfit { get; set; } = "casual";
}

public class NFTGallery
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OwnerId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public List<NFTDisplay> Displays { get; set; } = new();
    public bool IsPublic { get; set; } = true;
    public int VisitorCount { get; set; }
}

public class NFTDisplay
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string NftId { get; set; } = "";
    public string NftContract { get; set; } = "";
    public string NftChain { get; set; } = "MAMEY";
    public string ImageUrl { get; set; } = "";
    public string Name { get; set; } = "";
    public string Artist { get; set; } = "";
    public decimal? Price { get; set; }
    public bool IsForSale { get; set; } = false;
    public Vector3D Position { get; set; } = new();
}

// ==================== BDET BANK SERVICES ====================

public class BDETBankAccount
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string AccountNumber { get; set; } = "";
    public string AccountType { get; set; } = "checking"; // checking, savings, business, treasury
    public string Currency { get; set; } = "WPM";
    public decimal Balance { get; set; }
    public decimal AvailableBalance { get; set; }
    public decimal HoldBalance { get; set; }
    public decimal InterestRate { get; set; }
    public string Status { get; set; } = "active";
    public BDETAccountLimits Limits { get; set; } = new();
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
}

public class BDETAccountLimits
{
    public decimal DailyTransferLimit { get; set; } = 100000m;
    public decimal SingleTransferLimit { get; set; } = 50000m;
    public decimal DailyWithdrawLimit { get; set; } = 10000m;
    public decimal MonthlyTransferLimit { get; set; } = 1000000m;
}

public class BDETCard
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AccountId { get; set; } = "";
    public string CardNumber { get; set; } = ""; // Masked
    public string CardType { get; set; } = "debit"; // debit, credit, virtual
    public string CardTier { get; set; } = "standard"; // standard, gold, platinum, black
    public string Network { get; set; } = "IERAHKWA"; // IERAHKWA, VISA, Mastercard
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public string Status { get; set; } = "active";
    public DateTime ExpiryDate { get; set; }
    public CardSettings Settings { get; set; } = new();
}

public class CardSettings
{
    public bool OnlinePayments { get; set; } = true;
    public bool InternationalPayments { get; set; } = true;
    public bool ContactlessPayments { get; set; } = true;
    public bool AtmWithdrawals { get; set; } = true;
    public decimal DailySpendLimit { get; set; } = 5000m;
    public decimal AtmLimit { get; set; } = 1000m;
}

public class BDETPaymentService
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Type { get; set; } = ""; // p2p, merchant, international, recurring
    public string Provider { get; set; } = "BDET";
    public decimal Fee { get; set; }
    public string FeeType { get; set; } = "percent"; // fixed, percent
    public bool IsInstant { get; set; } = true;
    public List<string> SupportedCurrencies { get; set; } = new();
}

public class BDETPaymentRequest
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FromAccountId { get; set; } = "";
    public string ToAccountId { get; set; } = "";
    public string? ToAddress { get; set; } // For crypto
    public string? ToEmail { get; set; } // For P2P
    public string? ToPhone { get; set; } // For mobile
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "WPM";
    public string PaymentMethod { get; set; } = "internal"; // internal, wire, swift, crypto, card
    public string? Reference { get; set; }
    public string? Memo { get; set; }
    public PaymentSchedule? Schedule { get; set; }
}

public class PaymentSchedule
{
    public string Frequency { get; set; } = "once"; // once, daily, weekly, monthly
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MaxOccurrences { get; set; }
}

public class PaymentResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RequestId { get; set; } = "";
    public string Status { get; set; } = "pending"; // pending, processing, completed, failed
    public string? TransactionHash { get; set; }
    public decimal AmountSent { get; set; }
    public decimal Fee { get; set; }
    public decimal AmountReceived { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

// ==================== TOKEN MANAGEMENT ====================

public class IerahkwaToken
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ContractAddress { get; set; } = "";
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
    public int Decimals { get; set; } = 9;
    public decimal TotalSupply { get; set; }
    public decimal CirculatingSupply { get; set; }
    public decimal MaxSupply { get; set; }
    public string LogoUrl { get; set; } = "";
    public string Description { get; set; } = "";
    
    // Tokenomics
    public TokenDistribution Distribution { get; set; } = new();
    public TokenFeatures Features { get; set; } = new();
    
    // Market Data
    public decimal Price { get; set; }
    public decimal PriceChange24h { get; set; }
    public decimal MarketCap { get; set; }
    public decimal Volume24h { get; set; }
    
    public string OwnerId { get; set; } = "";
    public bool IsVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class TokenDistribution
{
    public decimal TeamPercent { get; set; } = 10;
    public decimal TreasuryPercent { get; set; } = 20;
    public decimal PublicSalePercent { get; set; } = 30;
    public decimal LiquidityPercent { get; set; } = 20;
    public decimal StakingRewardsPercent { get; set; } = 15;
    public decimal AirdropPercent { get; set; } = 5;
}

public class TokenFeatures
{
    public bool IsMintable { get; set; } = false;
    public bool IsBurnable { get; set; } = true;
    public bool IsPausable { get; set; } = false;
    public bool HasTransferFee { get; set; } = false;
    public decimal TransferFeePercent { get; set; } = 0;
    public bool HasReflection { get; set; } = false;
    public decimal ReflectionPercent { get; set; } = 0;
    public bool IsStakeable { get; set; } = true;
}

public class TokenLaunch
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TokenId { get; set; } = "";
    public string LaunchType { get; set; } = "fair"; // fair, presale, ido, ilo
    public decimal SoftCap { get; set; }
    public decimal HardCap { get; set; }
    public decimal MinContribution { get; set; }
    public decimal MaxContribution { get; set; }
    public decimal TokenPrice { get; set; }
    public decimal TotalRaised { get; set; }
    public int Participants { get; set; }
    public string Status { get; set; } = "upcoming"; // upcoming, live, ended, cancelled
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime? ListingTime { get; set; }
    public List<LaunchContribution> Contributions { get; set; } = new();
}

public class LaunchContribution
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string LaunchId { get; set; } = "";
    public string UserId { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal TokensAllocated { get; set; }
    public bool IsClaimed { get; set; } = false;
    public DateTime ContributedAt { get; set; } = DateTime.UtcNow;
}

// ==================== AI ADVANCED ====================

public class AIAgent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Type { get; set; } = "assistant"; // assistant, trader, analyst, moderator
    public string Model { get; set; } = "ierahkwa-gpt-4";
    public AIAgentConfig Config { get; set; } = new();
    public AIAgentStats Stats { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AIAgentConfig
{
    public string SystemPrompt { get; set; } = "";
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 4096;
    public List<string> Tools { get; set; } = new();
    public List<string> AllowedActions { get; set; } = new();
    public Dictionary<string, decimal> TradingLimits { get; set; } = new();
}

public class AIAgentStats
{
    public int TotalInteractions { get; set; }
    public int SuccessfulActions { get; set; }
    public decimal TotalVolumeProcessed { get; set; }
    public double AverageResponseTime { get; set; }
    public double SatisfactionScore { get; set; }
}

public class MLModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Type { get; set; } = ""; // price_prediction, sentiment, fraud_detection, recommendation
    public string Framework { get; set; } = ""; // tensorflow, pytorch, onnx
    public string Version { get; set; } = "1.0.0";
    public ModelMetrics Metrics { get; set; } = new();
    public bool IsDeployed { get; set; } = false;
    public DateTime TrainedAt { get; set; }
}

public class ModelMetrics
{
    public double Accuracy { get; set; }
    public double Precision { get; set; }
    public double Recall { get; set; }
    public double F1Score { get; set; }
    public double MAE { get; set; }
    public double RMSE { get; set; }
}
