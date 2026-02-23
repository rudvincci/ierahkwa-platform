using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Services;
using System.Collections.Concurrent;

namespace IERAHKWA.Platform.Controllers;

// ============================================================================
//                    GLOBAL UNIFIED CONTROLLER
//          Connects ALL platform modules with consistent APIs
// ============================================================================

[ApiController]
[Route("api")]
public class GlobalController : ControllerBase
{
    private readonly ILogger<GlobalController> _logger;
    private static readonly Random _random = new();

    public GlobalController(ILogger<GlobalController> logger) => _logger = logger;

    // ==================== FOREX ====================
    [HttpGet("forex/pairs")]
    public IActionResult GetForexPairs() => Ok(new { success = true, data = ForexData.GetPairs() });

    [HttpGet("forex/rates")]
    public IActionResult GetForexRates() => Ok(new { success = true, data = ForexData.GetRates() });

    [HttpPost("forex/trade")]
    public IActionResult ExecuteForexTrade([FromBody] ForexTradeRequest req) =>
        Ok(new { success = true, data = ForexData.ExecuteTrade(req) });

    // ==================== IDO FACTORY ====================
    [HttpGet("ido/projects")]
    public IActionResult GetIDOProjects() => Ok(new { success = true, data = IDOData.GetProjects() });

    [HttpGet("ido/project/{id}")]
    public IActionResult GetIDOProject(string id) => Ok(new { success = true, data = IDOData.GetProject(id) });

    [HttpPost("ido/participate")]
    public IActionResult ParticipateIDO([FromBody] IDOParticipateRequest req) =>
        Ok(new { success = true, data = IDOData.Participate(req) });

    // ==================== TOKEN FACTORY ====================
    [HttpGet("token-factory/templates")]
    public IActionResult GetTokenTemplates() => Ok(new { success = true, data = TokenFactoryData.GetTemplates() });

    [HttpPost("token-factory/create")]
    public IActionResult CreateToken([FromBody] CreateTokenRequest req) =>
        Ok(new { success = true, data = TokenFactoryData.CreateToken(req) });

    [HttpGet("token-factory/deployed")]
    public IActionResult GetDeployedTokens([FromQuery] string userId) =>
        Ok(new { success = true, data = TokenFactoryData.GetDeployedTokens(userId) });

    // ==================== FARM FACTORY ====================
    [HttpGet("farm/pools")]
    public IActionResult GetFarmPools() => Ok(new { success = true, data = FarmData.GetPools() });

    [HttpGet("farm/user/{userId}")]
    public IActionResult GetUserFarms(string userId) => Ok(new { success = true, data = FarmData.GetUserFarms(userId) });

    [HttpPost("farm/deposit")]
    public IActionResult FarmDeposit([FromBody] FarmRequest req) => Ok(new { success = true, data = FarmData.Deposit(req) });

    [HttpPost("farm/harvest")]
    public IActionResult FarmHarvest([FromBody] FarmRequest req) => Ok(new { success = true, data = FarmData.Harvest(req) });

    // ==================== SMART SCHOOL ====================
    [HttpGet("school/courses")]
    public IActionResult GetCourses() => Ok(new { success = true, data = SchoolData.GetCourses() });

    [HttpGet("school/course/{id}")]
    public IActionResult GetCourse(string id) => Ok(new { success = true, data = SchoolData.GetCourse(id) });

    [HttpPost("school/enroll")]
    public IActionResult EnrollCourse([FromBody] EnrollRequest req) => Ok(new { success = true, data = SchoolData.Enroll(req) });

    [HttpGet("school/progress/{userId}")]
    public IActionResult GetProgress(string userId) => Ok(new { success = true, data = SchoolData.GetProgress(userId) });

    // ==================== INSURANCE ====================
    [HttpGet("insurance/plans")]
    public IActionResult GetInsurancePlans() => Ok(new { success = true, data = InsuranceData.GetPlans() });

    [HttpPost("insurance/subscribe")]
    public IActionResult SubscribeInsurance([FromBody] InsuranceRequest req) =>
        Ok(new { success = true, data = InsuranceData.Subscribe(req) });

    [HttpGet("insurance/policies/{userId}")]
    public IActionResult GetPolicies(string userId) => Ok(new { success = true, data = InsuranceData.GetPolicies(userId) });

    // ==================== HEALTH ====================
    [HttpGet("health/services")]
    public IActionResult GetHealthServices() => Ok(new { success = true, data = HealthData.GetServices() });

    [HttpPost("health/appointment")]
    public IActionResult BookAppointment([FromBody] AppointmentRequest req) =>
        Ok(new { success = true, data = HealthData.BookAppointment(req) });

    [HttpGet("health/records/{userId}")]
    public IActionResult GetHealthRecords(string userId) => Ok(new { success = true, data = HealthData.GetRecords(userId) });

    // ==================== CITIZEN CRM ====================
    [HttpGet("citizen/profile/{userId}")]
    public IActionResult GetCitizenProfile(string userId) => Ok(new { success = true, data = CitizenData.GetProfile(userId) });

    [HttpGet("citizen/services")]
    public IActionResult GetCitizenServices() => Ok(new { success = true, data = CitizenData.GetServices() });

    [HttpPost("citizen/request")]
    public IActionResult SubmitRequest([FromBody] CitizenRequest req) =>
        Ok(new { success = true, data = CitizenData.SubmitRequest(req) });

    // ==================== VOTING / DAO ====================
    [HttpGet("governance/proposals")]
    public IActionResult GetProposals() => Ok(new { success = true, data = GovernanceData.GetProposals() });

    [HttpGet("governance/proposal/{id}")]
    public IActionResult GetProposal(string id) => Ok(new { success = true, data = GovernanceData.GetProposal(id) });

    [HttpPost("governance/vote")]
    public IActionResult Vote([FromBody] VoteRequest req) => Ok(new { success = true, data = GovernanceData.Vote(req) });

    [HttpPost("governance/propose")]
    public IActionResult CreateProposal([FromBody] ProposalRequest req) =>
        Ok(new { success = true, data = GovernanceData.CreateProposal(req) });

    [HttpGet("governance/stats")]
    public IActionResult GetGovernanceStats() => Ok(new { success = true, data = GovernanceData.GetStats() });

    // ==================== GLOBAL NAVIGATION ====================
    [HttpGet("navigation")]
    public IActionResult GetNavigation() => Ok(new { success = true, data = GetAllModules() });

    private object GetAllModules() => new[]
    {
        new { id = "dashboard", name = "Dashboard", icon = "bi-speedometer2", url = "/dashboard.html", category = "main" },
        new { id = "wallet", name = "Wallet", icon = "bi-wallet2", url = "/wallet.html", category = "main" },
        new { id = "cryptohost", name = "CryptoHost Exchange", icon = "bi-currency-exchange", url = "/cryptohost.html", category = "trading" },
        new { id = "trading", name = "TradeX Pro", icon = "bi-graph-up-arrow", url = "/tradex.html", category = "trading" },
        new { id = "forex", name = "Forex", icon = "bi-cash-coin", url = "/forex.html", category = "trading" },
        new { id = "bridge", name = "Multi-Chain Bridge", icon = "bi-link-45deg", url = "/bridge.html", category = "trading" },
        new { id = "bdet-bank", name = "BDET Bank", icon = "bi-bank2", url = "/bdet-bank.html", category = "finance" },
        new { id = "casino", name = "Casino", icon = "bi-dice-5", url = "/casino.html", category = "entertainment" },
        new { id = "social", name = "Social", icon = "bi-people", url = "/social-media.html", category = "social" },
        new { id = "ai-studio", name = "AI Studio", icon = "bi-robot", url = "/ai-platform.html", category = "tools" },
        new { id = "ido", name = "IDO Factory", icon = "bi-rocket-takeoff", url = "/ido-factory.html", category = "defi" },
        new { id = "token-factory", name = "Token Factory", icon = "bi-coin", url = "/token-factory.html", category = "defi" },
        new { id = "farmfactory", name = "Farm Factory", icon = "bi-flower1", url = "/farmfactory.html", category = "defi" },
        new { id = "school", name = "Smart School", icon = "bi-mortarboard", url = "/smartschool.html", category = "services" },
        new { id = "health", name = "Health Platform", icon = "bi-heart-pulse", url = "/health-platform.html", category = "services" },
        new { id = "insurance", name = "Insurance", icon = "bi-shield-check", url = "/insurance-platform.html", category = "services" },
        new { id = "citizen", name = "Citizen CRM", icon = "bi-person-badge", url = "/citizen-crm.html", category = "government" },
        new { id = "voting", name = "Voting", icon = "bi-check2-square", url = "/voting.html", category = "government" },
        new { id = "dao", name = "DAO Governance", icon = "bi-diagram-3", url = "/dao-governance.html", category = "government" },
        new { id = "metaverse", name = "Metaverse", icon = "bi-globe2", url = "/quantum-platform.html", category = "entertainment" }
    };
}

// ==================== DATA CLASSES ====================

public static class ForexData
{
    private static readonly Random _r = new();
    public static object GetPairs() => new[]
    {
        new { pair = "EUR/USD", bid = 1.0852m + (decimal)(_r.NextDouble() * 0.001), ask = 1.0854m, change = 0.15m, volume = "5.2B" },
        new { pair = "GBP/USD", bid = 1.2685m + (decimal)(_r.NextDouble() * 0.001), ask = 1.2687m, change = -0.08m, volume = "3.1B" },
        new { pair = "USD/JPY", bid = 149.25m + (decimal)(_r.NextDouble() * 0.1), ask = 149.27m, change = 0.22m, volume = "4.8B" },
        new { pair = "WPM/USD", bid = 0.0125m, ask = 0.0126m, change = 3.5m, volume = "250M" },
        new { pair = "IGT/USD", bid = 0.85m, ask = 0.851m, change = 5.8m, volume = "120M" }
    };
    public static object GetRates() => new { baseCurrency = "USD", rates = new Dictionary<string, decimal> { ["EUR"] = 0.92m, ["GBP"] = 0.79m, ["JPY"] = 149.25m, ["WPM"] = 80m, ["IGT"] = 1.18m } };
    public static object ExecuteTrade(ForexTradeRequest req) => new { tradeId = Guid.NewGuid().ToString()[..8], pair = req.Pair, type = req.Type, amount = req.Amount, rate = req.Rate, status = "executed", timestamp = DateTime.UtcNow };
}

public static class IDOData
{
    public static object GetProjects() => new[]
    {
        new { id = "ido-1", name = "MameySwap", symbol = "MSWAP", raised = 850000m, goal = 1000000m, participants = 2500, status = "live", endDate = DateTime.UtcNow.AddDays(5), price = 0.05m },
        new { id = "ido-2", name = "NativeNFT", symbol = "NNFT", raised = 1200000m, goal = 1200000m, participants = 4200, status = "completed", endDate = DateTime.UtcNow.AddDays(-2), price = 0.12m },
        new { id = "ido-3", name = "ChainGuard", symbol = "CGRD", raised = 250000m, goal = 750000m, participants = 850, status = "live", endDate = DateTime.UtcNow.AddDays(12), price = 0.025m },
        new { id = "ido-4", name = "SovereignAI", symbol = "SAI", raised = 0m, goal = 2000000m, participants = 0, status = "upcoming", endDate = DateTime.UtcNow.AddDays(30), price = 0.08m }
    };
    public static object GetProject(string id) => new { id, name = "MameySwap", symbol = "MSWAP", description = "Decentralized exchange on MAMEY chain", raised = 850000m, goal = 1000000m, tokenomics = new { total = 100000000, sale = 30, team = 15, liquidity = 25, ecosystem = 30 } };
    public static object Participate(IDOParticipateRequest req) => new { participationId = Guid.NewGuid().ToString()[..8], projectId = req.ProjectId, amount = req.Amount, tokens = req.Amount / 0.05m, status = "confirmed" };
}

public static class TokenFactoryData
{
    public static object GetTemplates() => new[]
    {
        new { id = "standard", name = "Standard Token", features = new[] { "Transfer", "Approve", "Burn" }, fee = 100m },
        new { id = "governance", name = "Governance Token", features = new[] { "Voting", "Delegation", "Timelock" }, fee = 250m },
        new { id = "reflection", name = "Reflection Token", features = new[] { "Auto-rewards", "Anti-whale" }, fee = 500m },
        new { id = "nft", name = "NFT Collection", features = new[] { "ERC721", "Metadata", "Royalties" }, fee = 350m }
    };
    public static object CreateToken(CreateTokenRequest req) => new { contractAddress = $"0x{Guid.NewGuid():N}"[..42], name = req.Name, symbol = req.Symbol, supply = req.TotalSupply, template = req.Template, deployed = true, txHash = $"0x{Guid.NewGuid():N}" };
    public static object GetDeployedTokens(string userId) => new[] { new { address = "0x1234...abcd", name = "MyToken", symbol = "MTK", supply = 1000000000, holders = 150 } };
}

public static class FarmData
{
    public static object GetPools() => new[]
    {
        new { id = "wpm-usdt", name = "WPM/USDT", apr = 125m, tvl = 15000000m, rewards = "WPM", multiplier = "2x" },
        new { id = "igt-eth", name = "IGT/ETH", apr = 85m, tvl = 8500000m, rewards = "IGT", multiplier = "1.5x" },
        new { id = "wpm-single", name = "WPM Stake", apr = 45m, tvl = 25000000m, rewards = "WPM", multiplier = "1x" },
        new { id = "igt-single", name = "IGT Stake", apr = 65m, tvl = 12000000m, rewards = "IGT", multiplier = "1x" }
    };
    public static object GetUserFarms(string userId) => new[] { new { pool = "wpm-usdt", deposited = 5000m, rewards = 125.5m, apr = 125m } };
    public static object Deposit(FarmRequest req) => new { txHash = $"0x{Guid.NewGuid():N}", pool = req.PoolId, amount = req.Amount, status = "deposited" };
    public static object Harvest(FarmRequest req) => new { txHash = $"0x{Guid.NewGuid():N}", pool = req.PoolId, harvested = 125.5m, status = "harvested" };
}

public static class SchoolData
{
    public static object GetCourses() => new[]
    {
        new { id = "blockchain-101", name = "Blockchain Fundamentals", instructor = "Dr. Nakamoto", duration = "8 weeks", students = 1250, rating = 4.8m, price = 0m, level = "beginner" },
        new { id = "defi-mastery", name = "DeFi Mastery", instructor = "Prof. Vitalik", duration = "12 weeks", students = 850, rating = 4.9m, price = 500m, level = "advanced" },
        new { id = "smart-contracts", name = "Smart Contract Development", instructor = "Solidity Expert", duration = "10 weeks", students = 680, rating = 4.7m, price = 750m, level = "intermediate" },
        new { id = "trading-pro", name = "Professional Trading", instructor = "Wall Street Trader", duration = "6 weeks", students = 2100, rating = 4.6m, price = 350m, level = "intermediate" }
    };
    public static object GetCourse(string id) => new { id, name = "Blockchain Fundamentals", modules = new[] { "Introduction", "Cryptography", "Consensus", "Smart Contracts", "DeFi", "NFTs", "DAOs", "Final Project" } };
    public static object Enroll(EnrollRequest req) => new { enrollmentId = Guid.NewGuid().ToString()[..8], courseId = req.CourseId, status = "enrolled", startDate = DateTime.UtcNow };
    public static object GetProgress(string userId) => new[] { new { course = "blockchain-101", progress = 65, completed = 5, total = 8 } };
}

public static class InsuranceData
{
    public static object GetPlans() => new[]
    {
        new { id = "basic", name = "Basic Coverage", coverage = 10000m, premium = 25m, features = new[] { "Wallet protection", "Transaction insurance" } },
        new { id = "pro", name = "Pro Coverage", coverage = 100000m, premium = 100m, features = new[] { "Smart contract coverage", "DeFi protection", "24/7 support" } },
        new { id = "enterprise", name = "Enterprise", coverage = 1000000m, premium = 500m, features = new[] { "Full coverage", "Legal support", "Priority claims" } }
    };
    public static object Subscribe(InsuranceRequest req) => new { policyId = $"POL-{Guid.NewGuid().ToString()[..8]}", plan = req.PlanId, coverage = 100000m, validUntil = DateTime.UtcNow.AddYears(1) };
    public static object GetPolicies(string userId) => new[] { new { policyId = "POL-12345678", plan = "pro", coverage = 100000m, status = "active" } };
}

public static class HealthData
{
    public static object GetServices() => new[]
    {
        new { id = "telemedicine", name = "Telemedicine", description = "Video consultations", price = 50m },
        new { id = "lab-tests", name = "Lab Tests", description = "Blood work, diagnostics", price = 150m },
        new { id = "mental-health", name = "Mental Health", description = "Therapy sessions", price = 100m },
        new { id = "wellness", name = "Wellness Program", description = "Fitness & nutrition", price = 75m }
    };
    public static object BookAppointment(AppointmentRequest req) => new { appointmentId = $"APT-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}", service = req.ServiceId, date = req.Date, status = "confirmed" };
    public static object GetRecords(string userId) => new[] { new { date = DateTime.UtcNow.AddDays(-30), type = "checkup", status = "completed", notes = "All vitals normal" } };
}

public static class CitizenData
{
    public static object GetProfile(string userId) => new { id = userId, citizenId = $"CIT-{userId[..8].ToUpper()}", status = "verified", memberSince = DateTime.UtcNow.AddYears(-2), tier = "Gold", benefits = new[] { "Voting rights", "Tax benefits", "Priority services" } };
    public static object GetServices() => new[]
    {
        new { id = "id-card", name = "Digital ID Card", description = "Sovereign identity document", processingTime = "24h" },
        new { id = "passport", name = "Digital Passport", description = "Travel document", processingTime = "5 days" },
        new { id = "business-license", name = "Business License", description = "Commercial permit", processingTime = "3 days" },
        new { id = "tax-filing", name = "Tax Filing", description = "Annual tax submission", processingTime = "Instant" }
    };
    public static object SubmitRequest(CitizenRequest req) => new { requestId = $"REQ-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(10000, 99999)}", service = req.ServiceId, status = "processing", estimatedCompletion = DateTime.UtcNow.AddDays(3) };
}

public static class GovernanceData
{
    private static readonly ConcurrentDictionary<string, ProposalData> _proposals = new();
    
    static GovernanceData()
    {
        _proposals["prop-1"] = new ProposalData { Id = "prop-1", Title = "Increase Staking Rewards", Description = "Proposal to increase WPM staking APY from 12% to 15%", Status = "active", ForVotes = 2500000, AgainstVotes = 800000, EndDate = DateTime.UtcNow.AddDays(3) };
        _proposals["prop-2"] = new ProposalData { Id = "prop-2", Title = "New Bridge Integration", Description = "Add support for Arbitrum bridge", Status = "active", ForVotes = 1800000, AgainstVotes = 200000, EndDate = DateTime.UtcNow.AddDays(7) };
        _proposals["prop-3"] = new ProposalData { Id = "prop-3", Title = "Treasury Allocation", Description = "Allocate 5M WPM for ecosystem grants", Status = "passed", ForVotes = 5000000, AgainstVotes = 500000, EndDate = DateTime.UtcNow.AddDays(-5) };
    }

    public static object GetProposals() => _proposals.Values.Select(p => new { p.Id, p.Title, p.Status, p.ForVotes, p.AgainstVotes, p.EndDate, participation = (p.ForVotes + p.AgainstVotes) / 10000000m * 100 });
    public static object GetProposal(string id) => _proposals.TryGetValue(id, out var p) ? p : new ProposalData { Title = "Not Found" };
    public static object Vote(VoteRequest req)
    {
        if (_proposals.TryGetValue(req.ProposalId, out var p))
        {
            if (req.Support) p.ForVotes += req.Amount;
            else p.AgainstVotes += req.Amount;
        }
        return new { proposalId = req.ProposalId, vote = req.Support ? "for" : "against", power = req.Amount, status = "recorded", txHash = $"0x{Guid.NewGuid():N}" };
    }
    public static object CreateProposal(ProposalRequest req)
    {
        var id = $"prop-{_proposals.Count + 1}";
        _proposals[id] = new ProposalData { Id = id, Title = req.Title, Description = req.Description, Status = "active", EndDate = DateTime.UtcNow.AddDays(14) };
        return new { proposalId = id, title = req.Title, status = "created", votingEnds = DateTime.UtcNow.AddDays(14) };
    }
    public static object GetStats() => new { totalProposals = _proposals.Count, activeProposals = _proposals.Values.Count(p => p.Status == "active"), totalVotes = _proposals.Values.Sum(p => p.ForVotes + p.AgainstVotes), participation = 45.5m };

    private class ProposalData { public string Id { get; set; } = ""; public string Title { get; set; } = ""; public string Description { get; set; } = ""; public string Status { get; set; } = ""; public decimal ForVotes { get; set; } public decimal AgainstVotes { get; set; } public DateTime EndDate { get; set; } }
}

// ==================== REQUEST MODELS ====================
public class ForexTradeRequest { public string Pair { get; set; } = ""; public string Type { get; set; } = "buy"; public decimal Amount { get; set; } public decimal Rate { get; set; } }
public class IDOParticipateRequest { public string UserId { get; set; } = ""; public string ProjectId { get; set; } = ""; public decimal Amount { get; set; } }
public class CreateTokenRequest { public string Name { get; set; } = ""; public string Symbol { get; set; } = ""; public decimal TotalSupply { get; set; } public string Template { get; set; } = "standard"; }
public class FarmRequest { public string UserId { get; set; } = ""; public string PoolId { get; set; } = ""; public decimal Amount { get; set; } }
public class EnrollRequest { public string UserId { get; set; } = ""; public string CourseId { get; set; } = ""; }
public class InsuranceRequest { public string UserId { get; set; } = ""; public string PlanId { get; set; } = ""; }
public class AppointmentRequest { public string UserId { get; set; } = ""; public string ServiceId { get; set; } = ""; public DateTime Date { get; set; } }
public class CitizenRequest { public string UserId { get; set; } = ""; public string ServiceId { get; set; } = ""; public string Details { get; set; } = ""; }
public class VoteRequest { public string UserId { get; set; } = ""; public string ProposalId { get; set; } = ""; public bool Support { get; set; } public decimal Amount { get; set; } }
public class ProposalRequest { public string UserId { get; set; } = ""; public string Title { get; set; } = ""; public string Description { get; set; } = ""; }
