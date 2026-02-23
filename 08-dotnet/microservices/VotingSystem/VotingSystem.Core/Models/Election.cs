namespace VotingSystem.Core.Models;

public class Election
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ElectionType Type { get; set; } = ElectionType.General;
    public string District { get; set; } = string.Empty;
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ElectionStatus Status { get; set; } = ElectionStatus.Upcoming;
    
    public List<Candidate> Candidates { get; set; } = new();
    public List<string> Options { get; set; } = new(); // For proposals: yes, no, abstain
    
    public int TotalVotes { get; set; }
    public int EligibleVoters { get; set; }
    public decimal ParticipationRate => EligibleVoters > 0 ? (decimal)TotalVotes / EligibleVoters * 100 : 0;
    
    public string BlockchainContract { get; set; } = string.Empty;
    public string CurrentBlock { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum ElectionType
{
    General,
    Council,
    Referendum,
    Proposal,
    Budget
}

public enum ElectionStatus
{
    Upcoming,
    Active,
    Closed,
    Certified
}

public class Candidate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Party { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public int VoteCount { get; set; }
    public decimal VotePercentage { get; set; }
}

public class Vote
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ElectionId { get; set; } = string.Empty;
    public string VoterId { get; set; } = string.Empty;
    public string? CandidateId { get; set; }
    public string? Choice { get; set; }
    public string TransactionHash { get; set; } = string.Empty;
    public string BlockNumber { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool Verified { get; set; } = true;
}

public class ElectionResults
{
    public string ElectionId { get; set; } = string.Empty;
    public string ElectionTitle { get; set; } = string.Empty;
    public int TotalVotes { get; set; }
    public decimal ParticipationRate { get; set; }
    public List<CandidateResult> Results { get; set; } = new();
    public string? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public bool IsCertified { get; set; }
}

public class CandidateResult
{
    public string CandidateId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Party { get; set; } = string.Empty;
    public int Votes { get; set; }
    public decimal Percentage { get; set; }
}

public class VotingStats
{
    public int ActiveElections { get; set; }
    public int RegisteredVoters { get; set; }
    public decimal OverallParticipation { get; set; }
    public int CompletedElections { get; set; }
}
