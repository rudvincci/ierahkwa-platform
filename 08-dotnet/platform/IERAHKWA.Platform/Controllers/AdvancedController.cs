using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Services;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Controllers;

// ============================================================================
//                    IERAHKWA ADVANCED API CONTROLLERS
//                    Banco BDET + Mamey Node Technology  
// ============================================================================

// ==================== ANALYTICS ====================

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analytics;

    public AnalyticsController(IAnalyticsService analytics) => _analytics = analytics;

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var dashboard = await _analytics.GetDashboardAsync();
        return Ok(new { success = true, data = dashboard });
    }

    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics()
    {
        var metrics = await _analytics.GetMetricsAsync();
        return Ok(new { success = true, data = metrics });
    }

    [HttpGet("charts/{metric}")]
    public async Task<IActionResult> GetChartData(string metric, [FromQuery] string timeframe = "24h")
    {
        var data = await _analytics.GetChartDataAsync(metric, timeframe);
        return Ok(new { success = true, data });
    }

    [HttpGet("services/status")]
    public async Task<IActionResult> GetServicesStatus()
    {
        var services = await _analytics.GetServicesStatusAsync();
        return Ok(new { success = true, data = services });
    }
}

// ==================== SECURITY ====================

[ApiController]
[Route("api/security")]
public class SecurityController : ControllerBase
{
    private readonly ISecurityService _security;

    public SecurityController(ISecurityService security) => _security = security;

    [HttpPost("2fa/enable")]
    public async Task<IActionResult> EnableTwoFactor([FromBody] Enable2FARequest request)
    {
        var result = await _security.EnableTwoFactorAsync(request.UserId, request.Method);
        return Ok(new { success = true, data = result });
    }

    [HttpPost("2fa/verify")]
    public async Task<IActionResult> VerifyTwoFactor([FromBody] Verify2FARequest request)
    {
        var isValid = await _security.VerifyTwoFactorAsync(request.UserId, request.Code);
        return Ok(new { success = isValid, verified = isValid });
    }

    [HttpGet("audit/{userId}")]
    public async Task<IActionResult> GetAuditLogs(string userId, [FromQuery] int limit = 50)
    {
        var logs = await _security.GetAuditLogsAsync(userId, limit);
        return Ok(new { success = true, data = logs });
    }
}

public class Enable2FARequest { public string UserId { get; set; } = ""; public string Method { get; set; } = "totp"; }
public class Verify2FARequest { public string UserId { get; set; } = ""; public string Code { get; set; } = ""; }

// ==================== MULTI-CHAIN BRIDGE ====================

[ApiController]
[Route("api/bridge")]
public class BridgeController : ControllerBase
{
    private readonly IBridgeService _bridge;

    public BridgeController(IBridgeService bridge) => _bridge = bridge;

    [HttpGet("chains")]
    public async Task<IActionResult> GetSupportedChains()
    {
        var chains = await _bridge.GetSupportedChainsAsync();
        return Ok(new { success = true, data = chains });
    }

    [HttpGet("fee")]
    public async Task<IActionResult> GetBridgeFee([FromQuery] string from, [FromQuery] string to, [FromQuery] string token)
    {
        var fee = await _bridge.GetBridgeFeeAsync(from, to, token);
        return Ok(new { success = true, data = new { fee } });
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> InitiateBridge([FromBody] BridgeRequest request)
    {
        try
        {
            var tx = await _bridge.InitiateBridgeAsync(request.UserId, request.FromChain, request.ToChain, request.Token, request.Amount, request.DestAddress);
            return Ok(new { success = true, data = tx });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpGet("transaction/{id}")]
    public async Task<IActionResult> GetTransaction(string id)
    {
        var tx = await _bridge.GetBridgeTransactionAsync(id);
        return tx != null ? Ok(new { success = true, data = tx }) : NotFound(new { success = false });
    }

    [HttpGet("history/{userId}")]
    public async Task<IActionResult> GetUserHistory(string userId)
    {
        var history = await _bridge.GetUserBridgeHistoryAsync(userId);
        return Ok(new { success = true, data = history });
    }
}

public class BridgeRequest
{
    public string UserId { get; set; } = "";
    public string FromChain { get; set; } = "";
    public string ToChain { get; set; } = "";
    public string Token { get; set; } = "";
    public decimal Amount { get; set; }
    public string DestAddress { get; set; } = "";
}

// ==================== TRADING PRO ====================

[ApiController]
[Route("api/trading")]
public class TradingController : ControllerBase
{
    private readonly ITradingService _trading;

    public TradingController(ITradingService trading) => _trading = trading;

    [HttpGet("pairs")]
    public async Task<IActionResult> GetTradingPairs()
    {
        var pairs = await _trading.GetTradingPairsAsync();
        return Ok(new { success = true, data = pairs });
    }

    [HttpGet("orderbook/{pairId}")]
    public async Task<IActionResult> GetOrderBook(string pairId)
    {
        var orderbook = await _trading.GetOrderBookAsync(pairId);
        return Ok(new { success = true, data = orderbook });
    }

    [HttpPost("order")]
    public async Task<IActionResult> PlaceOrder([FromBody] TradeOrder order)
    {
        var result = await _trading.PlaceOrderAsync(order);
        return Ok(new { success = true, data = result });
    }

    [HttpDelete("order/{orderId}")]
    public async Task<IActionResult> CancelOrder(string orderId)
    {
        var order = await _trading.CancelOrderAsync(orderId);
        return order != null ? Ok(new { success = true, data = order }) : NotFound(new { success = false });
    }

    [HttpGet("orders/{userId}")]
    public async Task<IActionResult> GetUserOrders(string userId, [FromQuery] bool openOnly = false)
    {
        var orders = await _trading.GetUserOrdersAsync(userId, openOnly);
        return Ok(new { success = true, data = orders });
    }

    [HttpGet("trades/{pairId}")]
    public async Task<IActionResult> GetRecentTrades(string pairId, [FromQuery] int limit = 50)
    {
        var trades = await _trading.GetRecentTradesAsync(pairId, limit);
        return Ok(new { success = true, data = trades });
    }

    [HttpPost("bot")]
    public async Task<IActionResult> CreateBot([FromBody] TradingBot bot)
    {
        var result = await _trading.CreateBotAsync(bot);
        return Ok(new { success = true, data = result });
    }

    [HttpPost("bot/{botId}/start")]
    public async Task<IActionResult> StartBot(string botId)
    {
        var bot = await _trading.StartBotAsync(botId);
        return bot != null ? Ok(new { success = true, data = bot }) : NotFound(new { success = false });
    }

    [HttpPost("bot/{botId}/stop")]
    public async Task<IActionResult> StopBot(string botId)
    {
        var bot = await _trading.StopBotAsync(botId);
        return bot != null ? Ok(new { success = true, data = bot }) : NotFound(new { success = false });
    }
}

// ==================== BDET BANK ====================

[ApiController]
[Route("api/bdet")]
public class BDETBankController : ControllerBase
{
    private readonly IBDETBankService _bank;

    public BDETBankController(IBDETBankService bank) => _bank = bank;

    [HttpPost("account")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateBDETAccountRequest request)
    {
        var account = await _bank.CreateAccountAsync(request.UserId, request.AccountType, request.Currency);
        return Ok(new { success = true, data = account });
    }

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetAccount(string accountId)
    {
        var account = await _bank.GetAccountAsync(accountId);
        return account != null ? Ok(new { success = true, data = account }) : NotFound(new { success = false });
    }

    [HttpGet("accounts/{userId}")]
    public async Task<IActionResult> GetUserAccounts(string userId)
    {
        var accounts = await _bank.GetUserAccountsAsync(userId);
        return Ok(new { success = true, data = accounts });
    }

    [HttpPost("payment")]
    public async Task<IActionResult> ProcessPayment([FromBody] BDETPaymentRequest request)
    {
        var result = await _bank.ProcessPaymentAsync(request);
        return result.Status == "completed" 
            ? Ok(new { success = true, data = result }) 
            : BadRequest(new { success = false, data = result });
    }

    [HttpPost("card")]
    public async Task<IActionResult> IssueCard([FromBody] IssueCardRequest request)
    {
        var card = await _bank.IssueCardAsync(request.AccountId, request.CardType, request.CardTier);
        return Ok(new { success = true, data = card });
    }

    [HttpGet("services")]
    public async Task<IActionResult> GetPaymentServices()
    {
        var services = await _bank.GetPaymentServicesAsync();
        return Ok(new { success = true, data = services });
    }
}

public class CreateBDETAccountRequest { public string UserId { get; set; } = ""; public string AccountType { get; set; } = "checking"; public string Currency { get; set; } = "WPM"; }
public class IssueCardRequest { public string AccountId { get; set; } = ""; public string CardType { get; set; } = "debit"; public string CardTier { get; set; } = "standard"; }

// ==================== METAVERSE ====================

[ApiController]
[Route("api/metaverse")]
public class MetaverseController : ControllerBase
{
    private readonly IMetaverseService _metaverse;

    public MetaverseController(IMetaverseService metaverse) => _metaverse = metaverse;

    [HttpPost("world")]
    public async Task<IActionResult> CreateWorld([FromBody] MetaverseWorld world)
    {
        var result = await _metaverse.CreateWorldAsync(world);
        return Ok(new { success = true, data = result });
    }

    [HttpGet("world/{worldId}")]
    public async Task<IActionResult> GetWorld(string worldId)
    {
        var world = await _metaverse.GetWorldAsync(worldId);
        return world != null ? Ok(new { success = true, data = world }) : NotFound(new { success = false });
    }

    [HttpGet("worlds")]
    public async Task<IActionResult> GetPublicWorlds()
    {
        var worlds = await _metaverse.GetPublicWorldsAsync();
        return Ok(new { success = true, data = worlds });
    }

    [HttpPost("avatar")]
    public async Task<IActionResult> CreateAvatar([FromBody] MetaverseAvatar avatar)
    {
        var result = await _metaverse.CreateAvatarAsync(avatar);
        return Ok(new { success = true, data = result });
    }

    [HttpPost("gallery")]
    public async Task<IActionResult> CreateGallery([FromBody] NFTGallery gallery)
    {
        var result = await _metaverse.CreateGalleryAsync(gallery);
        return Ok(new { success = true, data = result });
    }

    [HttpGet("gallery/{galleryId}")]
    public async Task<IActionResult> GetGallery(string galleryId)
    {
        var gallery = await _metaverse.GetGalleryAsync(galleryId);
        return gallery != null ? Ok(new { success = true, data = gallery }) : NotFound(new { success = false });
    }
}

// ==================== TOKENS ====================

[ApiController]
[Route("api/tokens")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokens;

    public TokenController(ITokenService tokens) => _tokens = tokens;

    [HttpGet]
    public async Task<IActionResult> GetAllTokens()
    {
        var tokens = await _tokens.GetAllTokensAsync();
        return Ok(new { success = true, data = tokens });
    }

    [HttpGet("{tokenId}")]
    public async Task<IActionResult> GetToken(string tokenId)
    {
        var token = await _tokens.GetTokenAsync(tokenId);
        return token != null ? Ok(new { success = true, data = token }) : NotFound(new { success = false });
    }

    [HttpPost]
    public async Task<IActionResult> CreateToken([FromBody] IerahkwaToken token)
    {
        var result = await _tokens.CreateTokenAsync(token);
        return Ok(new { success = true, data = result });
    }

    [HttpPost("launch")]
    public async Task<IActionResult> CreateLaunch([FromBody] TokenLaunch launch)
    {
        var result = await _tokens.CreateLaunchAsync(launch);
        return Ok(new { success = true, data = result });
    }

    [HttpPost("launch/{launchId}/contribute")]
    public async Task<IActionResult> Contribute(string launchId, [FromBody] ContributeRequest request)
    {
        try
        {
            var contribution = await _tokens.ContributeAsync(launchId, request.UserId, request.Amount);
            return Ok(new { success = true, data = contribution });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }
}

public class ContributeRequest { public string UserId { get; set; } = ""; public decimal Amount { get; set; } }
