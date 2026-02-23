using Microsoft.AspNetCore.Mvc;

namespace NET10.API.Controllers;

/// <summary>
/// Governance Controller - Sovereign DAO governance and voting
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GovernanceController : ControllerBase
{
    private static readonly List<Proposal> _proposals = InitializeProposals();
    private static readonly List<Vote> _votes = new();
    private static readonly List<Delegate> _delegates = InitializeDelegates();
    
    /// <summary>
    /// Get all proposals
    /// </summary>
    [HttpGet("proposals")]
    public ActionResult<ProposalListResponse> GetProposals(
        [FromQuery] ProposalStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _proposals.AsQueryable();
        
        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);
        
        var total = query.Count();
        var items = query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        return Ok(new ProposalListResponse
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
            ActiveCount = _proposals.Count(p => p.Status == ProposalStatus.Active),
            PendingCount = _proposals.Count(p => p.Status == ProposalStatus.Pending),
            PassedCount = _proposals.Count(p => p.Status == ProposalStatus.Passed)
        });
    }
    
    /// <summary>
    /// Get proposal by ID
    /// </summary>
    [HttpGet("proposals/{id}")]
    public ActionResult<Proposal> GetProposal(Guid id)
    {
        var proposal = _proposals.FirstOrDefault(p => p.Id == id);
        if (proposal == null) return NotFound();
        return Ok(proposal);
    }
    
    /// <summary>
    /// Create new proposal
    /// </summary>
    [HttpPost("proposals")]
    public ActionResult<Proposal> CreateProposal([FromBody] CreateProposalRequest request)
    {
        var proposal = new Proposal
        {
            Id = Guid.NewGuid(),
            Number = _proposals.Count + 1,
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            Proposer = request.Proposer,
            ProposerName = request.ProposerName,
            Status = ProposalStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            VotingStartsAt = DateTime.UtcNow.AddDays(2),
            VotingEndsAt = DateTime.UtcNow.AddDays(9),
            QuorumRequired = 1_000_000,
            Actions = request.Actions
        };
        
        _proposals.Insert(0, proposal);
        return CreatedAtAction(nameof(GetProposal), new { id = proposal.Id }, proposal);
    }
    
    /// <summary>
    /// Vote on proposal
    /// </summary>
    [HttpPost("proposals/{id}/vote")]
    public ActionResult<Vote> CastVote(Guid id, [FromBody] CastVoteRequest request)
    {
        var proposal = _proposals.FirstOrDefault(p => p.Id == id);
        if (proposal == null) return NotFound();
        
        if (proposal.Status != ProposalStatus.Active)
            return BadRequest(new { error = "Proposal is not active for voting" });
        
        // Check if already voted
        var existingVote = _votes.FirstOrDefault(v => v.ProposalId == id && v.Voter == request.Voter);
        if (existingVote != null)
            return BadRequest(new { error = "Already voted on this proposal" });
        
        var vote = new Vote
        {
            Id = Guid.NewGuid(),
            ProposalId = id,
            Voter = request.Voter,
            VoterName = request.VoterName,
            Support = request.Support,
            VotingPower = request.VotingPower,
            Reason = request.Reason,
            Timestamp = DateTime.UtcNow
        };
        
        _votes.Add(vote);
        
        // Update proposal vote counts
        switch (request.Support)
        {
            case VoteType.For:
                proposal.VotesFor += request.VotingPower;
                break;
            case VoteType.Against:
                proposal.VotesAgainst += request.VotingPower;
                break;
            case VoteType.Abstain:
                proposal.VotesAbstain += request.VotingPower;
                break;
        }
        
        return Ok(vote);
    }
    
    /// <summary>
    /// Get votes for proposal
    /// </summary>
    [HttpGet("proposals/{id}/votes")]
    public ActionResult<List<Vote>> GetProposalVotes(Guid id)
    {
        var votes = _votes.Where(v => v.ProposalId == id).OrderByDescending(v => v.Timestamp).ToList();
        return Ok(votes);
    }
    
    /// <summary>
    /// Get delegates
    /// </summary>
    [HttpGet("delegates")]
    public ActionResult<List<Delegate>> GetDelegates([FromQuery] int limit = 20)
    {
        return Ok(_delegates.OrderByDescending(d => d.VotingPower).Take(limit).ToList());
    }
    
    /// <summary>
    /// Get delegate by address
    /// </summary>
    [HttpGet("delegates/{address}")]
    public ActionResult<Delegate> GetDelegate(string address)
    {
        var del = _delegates.FirstOrDefault(d => d.Address.Equals(address, StringComparison.OrdinalIgnoreCase));
        if (del == null) return NotFound();
        return Ok(del);
    }
    
    /// <summary>
    /// Delegate voting power
    /// </summary>
    [HttpPost("delegate")]
    public ActionResult DelegateVotes([FromBody] DelegateRequest request)
    {
        var del = _delegates.FirstOrDefault(d => d.Address.Equals(request.DelegateTo, StringComparison.OrdinalIgnoreCase));
        if (del == null)
        {
            _delegates.Add(new Delegate
            {
                Address = request.DelegateTo,
                Name = "New Delegate",
                VotingPower = request.Amount,
                DelegatorCount = 1
            });
        }
        else
        {
            del.VotingPower += request.Amount;
            del.DelegatorCount++;
        }
        
        return Ok(new { message = "Voting power delegated successfully" });
    }
    
    /// <summary>
    /// Get governance stats
    /// </summary>
    [HttpGet("stats")]
    public ActionResult<GovernanceStats> GetStats()
    {
        return Ok(new GovernanceStats
        {
            TotalProposals = _proposals.Count,
            ActiveProposals = _proposals.Count(p => p.Status == ProposalStatus.Active),
            PassedProposals = _proposals.Count(p => p.Status == ProposalStatus.Passed),
            RejectedProposals = _proposals.Count(p => p.Status == ProposalStatus.Rejected),
            TotalVotes = _votes.Count,
            TotalVotingPower = 500_000_000m,
            DelegatedPower = 125_000_000m,
            UniqueVoters = _votes.Select(v => v.Voter).Distinct().Count(),
            AverageParticipation = 35.5m,
            QuorumThreshold = 4m
        });
    }
    
    /// <summary>
    /// Get treasury info
    /// </summary>
    [HttpGet("treasury")]
    public ActionResult<TreasuryInfo> GetTreasury()
    {
        return Ok(new TreasuryInfo
        {
            TotalBalance = 250_000_000m,
            Balances = new List<TreasuryBalance>
            {
                new() { Token = "IGT-PM", Balance = 100_000_000m, UsdValue = 250_000_000m },
                new() { Token = "USDT", Balance = 50_000_000m, UsdValue = 50_000_000m },
                new() { Token = "ETH", Balance = 10_000m, UsdValue = 25_000_000m }
            },
            PendingOutflows = 5_000_000m,
            RecentTransactions = new List<TreasuryTransaction>
            {
                new() { Type = "outflow", Description = "Infrastructure Development Grant", Amount = 1_000_000m, Date = DateTime.UtcNow.AddDays(-5) },
                new() { Type = "inflow", Description = "Protocol Fees", Amount = 250_000m, Date = DateTime.UtcNow.AddDays(-3) },
                new() { Type = "outflow", Description = "Community Rewards Distribution", Amount = 500_000m, Date = DateTime.UtcNow.AddDays(-1) }
            }
        });
    }
    
    private static List<Proposal> InitializeProposals()
    {
        return new List<Proposal>
        {
            new()
            {
                Number = 15,
                Title = "Ierahkwa Education Fund Allocation",
                Description = "Allocate 5M IGT-PM from treasury for sovereign education initiatives including language preservation and cultural programs.",
                Category = ProposalCategory.Treasury,
                Proposer = "0x1234...5678",
                ProposerName = "Ierahkwa Foundation",
                Status = ProposalStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                VotingStartsAt = DateTime.UtcNow.AddDays(-3),
                VotingEndsAt = DateTime.UtcNow.AddDays(4),
                VotesFor = 15_000_000,
                VotesAgainst = 2_000_000,
                VotesAbstain = 500_000,
                QuorumRequired = 10_000_000
            },
            new()
            {
                Number = 14,
                Title = "Protocol Fee Adjustment",
                Description = "Reduce swap fees from 0.3% to 0.25% to increase trading volume and competitiveness.",
                Category = ProposalCategory.Protocol,
                Proposer = "0xabcd...ef01",
                ProposerName = "DeFi Committee",
                Status = ProposalStatus.Passed,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                VotingStartsAt = DateTime.UtcNow.AddDays(-13),
                VotingEndsAt = DateTime.UtcNow.AddDays(-6),
                VotesFor = 25_000_000,
                VotesAgainst = 5_000_000,
                VotesAbstain = 1_000_000,
                QuorumRequired = 10_000_000
            },
            new()
            {
                Number = 13,
                Title = "New Validator Requirements",
                Description = "Increase minimum stake for validators from 1M to 2M IGT-PM to improve network security.",
                Category = ProposalCategory.Governance,
                Proposer = "0x9876...5432",
                ProposerName = "Security Council",
                Status = ProposalStatus.Rejected,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                VotingStartsAt = DateTime.UtcNow.AddDays(-28),
                VotingEndsAt = DateTime.UtcNow.AddDays(-21),
                VotesFor = 8_000_000,
                VotesAgainst = 20_000_000,
                VotesAbstain = 2_000_000,
                QuorumRequired = 10_000_000
            },
            new()
            {
                Number = 12,
                Title = "Cross-Chain Bridge to Polygon",
                Description = "Deploy official Ierahkwa bridge to Polygon network for expanded interoperability.",
                Category = ProposalCategory.Infrastructure,
                Proposer = "0xfedc...ba98",
                ProposerName = "Tech Team",
                Status = ProposalStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                VotingStartsAt = DateTime.UtcNow.AddDays(1),
                VotingEndsAt = DateTime.UtcNow.AddDays(8),
                QuorumRequired = 10_000_000
            }
        };
    }
    
    private static List<Delegate> InitializeDelegates()
    {
        return new List<Delegate>
        {
            new() { Address = "0x1234...5678", Name = "Ierahkwa Foundation", VotingPower = 50_000_000, DelegatorCount = 1250, ProposalsVoted = 15 },
            new() { Address = "0xabcd...ef01", Name = "Community DAO", VotingPower = 25_000_000, DelegatorCount = 890, ProposalsVoted = 14 },
            new() { Address = "0x9876...5432", Name = "DeFi Alliance", VotingPower = 15_000_000, DelegatorCount = 456, ProposalsVoted = 12 },
            new() { Address = "0xfedc...ba98", Name = "Staking Pool", VotingPower = 10_000_000, DelegatorCount = 234, ProposalsVoted = 10 },
            new() { Address = "0x2468...1357", Name = "Treasury Council", VotingPower = 8_000_000, DelegatorCount = 123, ProposalsVoted = 15 }
        };
    }
}

// ═══════════════════════════════════════════════════════════════
// GOVERNANCE MODELS
// ═══════════════════════════════════════════════════════════════

public class Proposal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProposalCategory Category { get; set; }
    public string Proposer { get; set; } = string.Empty;
    public string ProposerName { get; set; } = string.Empty;
    public ProposalStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime VotingStartsAt { get; set; }
    public DateTime VotingEndsAt { get; set; }
    public decimal VotesFor { get; set; }
    public decimal VotesAgainst { get; set; }
    public decimal VotesAbstain { get; set; }
    public decimal QuorumRequired { get; set; }
    public List<ProposalAction>? Actions { get; set; }
    
    public decimal TotalVotes => VotesFor + VotesAgainst + VotesAbstain;
    public decimal ForPercentage => TotalVotes > 0 ? (VotesFor / TotalVotes) * 100 : 0;
    public bool QuorumReached => TotalVotes >= QuorumRequired;
}

public enum ProposalStatus
{
    Pending,
    Active,
    Passed,
    Rejected,
    Cancelled,
    Executed
}

public enum ProposalCategory
{
    Treasury,
    Protocol,
    Governance,
    Infrastructure,
    Community,
    Emergency
}

public class ProposalAction
{
    public string Target { get; set; } = string.Empty;
    public string Function { get; set; } = string.Empty;
    public string[] Args { get; set; } = Array.Empty<string>();
    public decimal Value { get; set; }
}

public class ProposalListResponse
{
    public List<Proposal> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int ActiveCount { get; set; }
    public int PendingCount { get; set; }
    public int PassedCount { get; set; }
}

public class CreateProposalRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProposalCategory Category { get; set; }
    public string Proposer { get; set; } = string.Empty;
    public string ProposerName { get; set; } = string.Empty;
    public List<ProposalAction>? Actions { get; set; }
}

public class Vote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProposalId { get; set; }
    public string Voter { get; set; } = string.Empty;
    public string VoterName { get; set; } = string.Empty;
    public VoteType Support { get; set; }
    public decimal VotingPower { get; set; }
    public string? Reason { get; set; }
    public DateTime Timestamp { get; set; }
}

public enum VoteType
{
    Against,
    For,
    Abstain
}

public class CastVoteRequest
{
    public string Voter { get; set; } = string.Empty;
    public string VoterName { get; set; } = string.Empty;
    public VoteType Support { get; set; }
    public decimal VotingPower { get; set; }
    public string? Reason { get; set; }
}

public class Delegate
{
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal VotingPower { get; set; }
    public int DelegatorCount { get; set; }
    public int ProposalsVoted { get; set; }
    public string? Bio { get; set; }
    public string? Website { get; set; }
}

public class DelegateRequest
{
    public string From { get; set; } = string.Empty;
    public string DelegateTo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class GovernanceStats
{
    public int TotalProposals { get; set; }
    public int ActiveProposals { get; set; }
    public int PassedProposals { get; set; }
    public int RejectedProposals { get; set; }
    public int TotalVotes { get; set; }
    public decimal TotalVotingPower { get; set; }
    public decimal DelegatedPower { get; set; }
    public int UniqueVoters { get; set; }
    public decimal AverageParticipation { get; set; }
    public decimal QuorumThreshold { get; set; }
}

public class TreasuryInfo
{
    public decimal TotalBalance { get; set; }
    public List<TreasuryBalance> Balances { get; set; } = new();
    public decimal PendingOutflows { get; set; }
    public List<TreasuryTransaction> RecentTransactions { get; set; } = new();
}

public class TreasuryBalance
{
    public string Token { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal UsdValue { get; set; }
}

public class TreasuryTransaction
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
