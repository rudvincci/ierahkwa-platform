using Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

namespace Mamey.ApplicationName.Modules.Raffles.Application.Services;

/// <summary>
/// A "Standard" raffle is the simplest type: 
/// Single prize, single draw event. 
/// Probability of winning for a participant is (ticketsOwned / totalTickets).
/// The draw picks exactly one random ticket from the entire pool.
/// </summary>
internal class StandardRaffleVariant : IRaffleVariant
{
    /// <summary>
    /// Computes the probability distribution for each "prize" in this variant.
    /// Since a standard raffle typically has one grand prize,
    /// we return a dictionary with a single key: "MainPrize".
    /// </summary>
    /// <param name="raffle">The raffle object containing all tickets.</param>
    /// <param name="participant">The participant for whom we're computing odds.</param>
    /// <returns>A dictionary with a single entry: { "MainPrize" : probability }</returns>
    public Dictionary<string, decimal> ComputeProbabilityDistribution(Raffle raffle, Participant participant)
    {
        // Safety check: if no tickets exist, probability is 0
        var totalTickets = raffle.Tickets.Count;
        if (totalTickets == 0)
        {
            return new Dictionary<string, decimal>
            {
                ["MainPrize"] = 0m
            };
        }

        // Count how many tickets are owned by this participant
        var participantTickets = raffle.Tickets
            .Count(t => t.OwnerId == participant.Id);

        // Probability = (# tickets owned) / (total tickets)
        decimal probability = (decimal)participantTickets / totalTickets;

        // Return a single key "MainPrize" with that probability
        return new Dictionary<string, decimal>
        {
            ["MainPrize"] = probability
        };
    }

    /// <summary>
    /// Draws the winner(s). In a standard raffle, we have exactly one winner.
    /// Steps:
    ///   1. Check if there are any tickets. If not, return empty list.
    ///   2. Pick a random index from [0 .. ticketCount-1].
    ///   3. Mark that ticket as winner, assign a prize name, and return it as a list.
    /// </summary>
    /// <param name="raffle">The raffle object containing the tickets to draw from.</param>
    /// <returns>A list with exactly one winning Ticket (unless no tickets exist).</returns>
    public List<Ticket> DrawWinners(Raffle raffle)
    {
        // If no tickets, no winners
        if (raffle.Tickets.Count == 0)
        {
            return new List<Ticket>();
        }

        // Use a random generator. 
        // NOTE: For real-world fairness, consider a cryptographically secure RNG instead.
        var rnd = new Random();

        // Get a random index within the range of available tickets
        int winnerIndex = rnd.Next(raffle.Tickets.Count);

        // Retrieve the winning ticket
        var winningTicket = raffle.Tickets[winnerIndex];
        winningTicket.IsWinner = true;
        winningTicket.PrizeAwarded = "MainPrize";

        // Return it as a single-element list
        return new List<Ticket> { winningTicket };
    }
}