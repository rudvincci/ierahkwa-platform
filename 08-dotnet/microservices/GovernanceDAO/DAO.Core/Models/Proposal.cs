namespace DAO.Core.Models;

public class Proposal
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProposerId { get; set; } = string.Empty;
    public string ProposerName { get; set; } = string.Empty;
    public ProposalType Type { get; set; }
    public ProposalStatus Status { get; set; } = ProposalStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime VotingEndsAt { get; set; }
    public int VotesFor { get; set; }
    public int VotesAgainst { get; set; }
    public int VotesAbstain { get; set; }
    public decimal QuorumRequired { get; set; } = 0.5m;
    public string ContractAddress { get; set; } = string.Empty;
}

public enum ProposalType { Treasury, Parameter, Membership, Constitutional, Emergency }
public enum ProposalStatus { Draft, Active, Passed, Rejected, Executed, Cancelled }

public class DAOStats
{
    public int TotalProposals { get; set; }
    public int ActiveProposals { get; set; }
    public int TotalMembers { get; set; }
    public decimal TotalVotingPower { get; set; }
    public decimal AverageParticipation { get; set; }
}
