using Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

namespace Mamey.ApplicationName.Modules.Raffles.Application.Services;

/// <summary>
/// A "Reverse Raffle" eliminates tickets one by one until only one remains.
/// The last ticket standing is the winner.
/// From a pure probability standpoint, each ticket has equal chance of being last.
/// </summary>
internal class ReverseRaffleVariant : IRaffleVariant
{
    /// <summary>
    /// The dictionary key for the final winning prize.
    /// </summary>
    private const string FinalPrizeKey = "FinalWinner";

    /// <summary>
    /// Computes the probability distribution. 
    /// For a single-winner reverse raffle, there's just 1 final prize:
    /// Probability = (# tickets owned by participant) / (total tickets).
    /// </summary>
    public Dictionary<string, decimal> ComputeProbabilityDistribution(Raffle raffle, Participant participant)
    {
        int totalTickets = raffle.Tickets.Count;
        if (totalTickets == 0)
        {
            return new Dictionary<string, decimal>
            {
                [FinalPrizeKey] = 0m
            };
        }

        int participantTickets = raffle.Tickets.Count(t => t.OwnerId == participant.Id);

        // Probability = participantTickets / totalTickets
        decimal probability = (decimal)participantTickets / totalTickets;

        return new Dictionary<string, decimal>
        {
            [FinalPrizeKey] = probability
        };
    }

    /// <summary>
    /// Draws the winning ticket by simulating an elimination process:
    /// 1. Shuffle all tickets.
    /// 2. The last ticket in the shuffled sequence is declared the winner.
    /// 
    /// (In a live event, you'd remove tickets one-by-one, but 
    /// the final result is the same as picking a random ticket 
    /// to be last.)
    /// </summary>
    public List<Ticket> DrawWinners(Raffle raffle)
    {
        // If no tickets, no winner
        if (raffle.Tickets.Count == 0)
        {
            return new List<Ticket>();
        }

        // We take all tickets into a temporary list:
        var tickets = raffle.Tickets.ToList();

        // Shuffle them randomly:
        var rnd = new Random();
        for (int i = 0; i < tickets.Count; i++)
        {
            int swapIndex = rnd.Next(i, tickets.Count);
            // Swap tickets[i] and tickets[swapIndex]
            var temp = tickets[i];
            tickets[i] = tickets[swapIndex];
            tickets[swapIndex] = temp;
        }

        // The last ticket in the list is the final winner
        var winner = tickets[tickets.Count - 1];
        winner.IsWinner = true;
        winner.PrizeAwarded = FinalPrizeKey;

        return new List<Ticket> { winner };
    }
}