using VotingSystem.Core.Interfaces;
using VotingSystem.Core.Models;

namespace VotingSystem.Infrastructure.Services;

public class ElectionService : IElectionService
{
    private static readonly List<Election> _elections = new()
    {
        new Election
        {
            Id = "ELEC-2026-001",
            Title = "Council Representative Election 2026",
            Description = "Election for District 3 Council Representative",
            Type = ElectionType.Council,
            District = "District 3",
            StartDate = DateTime.UtcNow.AddDays(-5),
            EndDate = DateTime.UtcNow.AddDays(2),
            Status = ElectionStatus.Active,
            TotalVotes = 789,
            EligibleVoters = 1234,
            BlockchainContract = "0x7f4a2b9c...9c3d",
            CurrentBlock = "#4,256,789",
            Candidates = new List<Candidate>
            {
                new() { Id = "C1", Name = "Carlos Mendoza", Party = "Progressive Party", VoteCount = 285, VotePercentage = 36 },
                new() { Id = "C2", Name = "María García", Party = "Unity Coalition", VoteCount = 312, VotePercentage = 40 },
                new() { Id = "C3", Name = "Roberto Chen", Party = "Independent", VoteCount = 142, VotePercentage = 18 },
                new() { Id = "C4", Name = "Ana Torres", Party = "Community First", VoteCount = 50, VotePercentage = 6 }
            }
        },
        new Election
        {
            Id = "PROP-2026-042",
            Title = "Proposal #42: Community Fund Allocation",
            Description = "Allocate $500,000 from the Community Fund towards infrastructure improvements",
            Type = ElectionType.Proposal,
            StartDate = DateTime.UtcNow.AddDays(-3),
            EndDate = DateTime.UtcNow.AddDays(1),
            Status = ElectionStatus.Active,
            TotalVotes = 800,
            EligibleVoters = 1234,
            Options = new List<string> { "Yes", "No", "Abstain" }
        }
    };

    public Task<IEnumerable<Election>> GetAllAsync() => Task.FromResult(_elections.AsEnumerable());

    public Task<IEnumerable<Election>> GetActiveAsync() => 
        Task.FromResult(_elections.Where(e => e.Status == ElectionStatus.Active));

    public Task<Election?> GetByIdAsync(string id) => 
        Task.FromResult(_elections.FirstOrDefault(e => e.Id == id));

    public Task<Election> CreateAsync(Election election)
    {
        election.Id = $"ELEC-{DateTime.Now.Year}-{(_elections.Count + 1):D3}";
        _elections.Add(election);
        return Task.FromResult(election);
    }

    public Task UpdateAsync(Election election)
    {
        var index = _elections.FindIndex(e => e.Id == election.Id);
        if (index >= 0) _elections[index] = election;
        return Task.CompletedTask;
    }

    public Task<ElectionResults> GetResultsAsync(string electionId)
    {
        var election = _elections.FirstOrDefault(e => e.Id == electionId);
        if (election == null) return Task.FromResult(new ElectionResults());

        var winner = election.Candidates.OrderByDescending(c => c.VoteCount).FirstOrDefault();
        
        return Task.FromResult(new ElectionResults
        {
            ElectionId = election.Id,
            ElectionTitle = election.Title,
            TotalVotes = election.TotalVotes,
            ParticipationRate = election.ParticipationRate,
            WinnerId = winner?.Id,
            WinnerName = winner?.Name,
            Results = election.Candidates.Select(c => new CandidateResult
            {
                CandidateId = c.Id,
                Name = c.Name,
                Party = c.Party,
                Votes = c.VoteCount,
                Percentage = c.VotePercentage
            }).ToList()
        });
    }

    public Task<VotingStats> GetStatsAsync()
    {
        return Task.FromResult(new VotingStats
        {
            ActiveElections = _elections.Count(e => e.Status == ElectionStatus.Active),
            RegisteredVoters = 1234,
            OverallParticipation = 87,
            CompletedElections = 12
        });
    }
}

public class VoteService : IVoteService
{
    private static readonly List<Vote> _votes = new();

    public Task<Vote> CastVoteAsync(string electionId, string voterId, string? candidateId, string? choice)
    {
        var vote = new Vote
        {
            ElectionId = electionId,
            VoterId = voterId,
            CandidateId = candidateId,
            Choice = choice,
            TransactionHash = $"0x{Guid.NewGuid():N}".Substring(0, 18),
            BlockNumber = $"#{new Random().Next(4000000, 5000000)}"
        };
        _votes.Add(vote);
        return Task.FromResult(vote);
    }

    public Task<IEnumerable<Vote>> GetVotesByElectionAsync(string electionId) =>
        Task.FromResult(_votes.Where(v => v.ElectionId == electionId));

    public Task<IEnumerable<Vote>> GetVotesByVoterAsync(string voterId) =>
        Task.FromResult(_votes.Where(v => v.VoterId == voterId));

    public Task<bool> HasVotedAsync(string electionId, string voterId) =>
        Task.FromResult(_votes.Any(v => v.ElectionId == electionId && v.VoterId == voterId));
}
