using Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

namespace Mamey.ApplicationName.Modules.Raffles.Application.Services;

/// <summary>
/// An "Online-Only" raffle handles ticket sales, participant management,
/// and the drawing process entirely over the internet. 
/// From a logic standpoint, it's often like a standard single-winner raffle, 
/// but all tickets are digital.
/// </summary>
internal class OnlineOnlyRaffleVariant : IRaffleVariant
{
    private const string PrizeKey = "OnlineGrandPrize";

    /// <summary>
    /// Computes the participant's probability of winning (assuming a single grand prize).
    /// This is the same as standard: (# participant tickets) / (total tickets).
    /// </summary>
    public Dictionary<string, decimal> ComputeProbabilityDistribution(Raffle raffle, Participant participant)
    {
        int totalTickets = raffle.Tickets.Count;
        if (totalTickets == 0)
        {
            return new Dictionary<string, decimal>
            {
                [PrizeKey] = 0m
            };
        }

        int participantTickets = raffle.Tickets.Count(t => t.OwnerId == participant.Id);
        decimal probability = (decimal)participantTickets / totalTickets;

        return new Dictionary<string, decimal>
        {
            [PrizeKey] = probability
        };
    }

    /// <summary>
    /// Draws the single winner ticket from the online pool. 
    /// Typically triggered by an automated job or admin action at a specific time.
    /// </summary>
    public List<Ticket> DrawWinners(Raffle raffle)
    {
        // If no tickets, no winner
        if (raffle.Tickets.Count == 0)
        {
            return new List<Ticket>();
        }

        // Use a random approach to pick one winning ticket
        // (Consider cryptographic RNG for better fairness)
        var rnd = new Random();
        int index = rnd.Next(raffle.Tickets.Count);

        var winningTicket = raffle.Tickets[index];
        winningTicket.IsWinner = true;
        winningTicket.PrizeAwarded = PrizeKey;

        return new List<Ticket> { winningTicket };
    }
}