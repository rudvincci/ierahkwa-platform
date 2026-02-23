using VotingSystem.Core.Models;

namespace VotingSystem.Core.Interfaces;

public interface IElectionService
{
    Task<IEnumerable<Election>> GetAllAsync();
    Task<IEnumerable<Election>> GetActiveAsync();
    Task<Election?> GetByIdAsync(string id);
    Task<Election> CreateAsync(Election election);
    Task UpdateAsync(Election election);
    Task<ElectionResults> GetResultsAsync(string electionId);
    Task<VotingStats> GetStatsAsync();
}

public interface IVoteService
{
    Task<Vote> CastVoteAsync(string electionId, string voterId, string? candidateId, string? choice);
    Task<IEnumerable<Vote>> GetVotesByElectionAsync(string electionId);
    Task<IEnumerable<Vote>> GetVotesByVoterAsync(string voterId);
    Task<bool> HasVotedAsync(string electionId, string voterId);
}
