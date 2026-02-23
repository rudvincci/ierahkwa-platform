using Microsoft.AspNetCore.Mvc;
using VotingSystem.Core.Interfaces;
using VotingSystem.Core.Models;

namespace VotingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ElectionsController : ControllerBase
{
    private readonly IElectionService _electionService;
    private readonly IVoteService _voteService;

    public ElectionsController(IElectionService electionService, IVoteService voteService)
    {
        _electionService = electionService;
        _voteService = voteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Election>>> GetAll()
    {
        var elections = await _electionService.GetAllAsync();
        return Ok(elections);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<Election>>> GetActive()
    {
        var elections = await _electionService.GetActiveAsync();
        return Ok(elections);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Election>> GetById(string id)
    {
        var election = await _electionService.GetByIdAsync(id);
        if (election == null) return NotFound();
        return Ok(election);
    }

    [HttpPost]
    public async Task<ActionResult<Election>> Create([FromBody] Election election)
    {
        var created = await _electionService.CreateAsync(election);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("{electionId}/vote")]
    public async Task<ActionResult<Vote>> CastVote(string electionId, [FromBody] VoteRequest request)
    {
        var vote = await _voteService.CastVoteAsync(electionId, request.VoterId, request.CandidateId, request.Choice);
        return Ok(vote);
    }

    [HttpGet("{electionId}/results")]
    public async Task<ActionResult<ElectionResults>> GetResults(string electionId)
    {
        var results = await _electionService.GetResultsAsync(electionId);
        return Ok(results);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<VotingStats>> GetStats()
    {
        var stats = await _electionService.GetStatsAsync();
        return Ok(stats);
    }
}

public class VoteRequest
{
    public string VoterId { get; set; } = string.Empty;
    public string? CandidateId { get; set; }
    public string? Choice { get; set; } // yes, no, abstain for proposals
}
