using Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

namespace Mamey.ApplicationName.Modules.Raffles.Application.Services;

/// <summary>
/// The "InstantWinRaffleVariant" simulates a scratch-off or spin-to-win scenario
/// where each ticket reveals if it is a winner the moment the participant "scratches" it.
/// There's no single drawing event for the entire raffle.
/// </summary>
internal class InstantWinRaffleVariant : IRaffleVariant
{
    /// <summary>
    /// Probability that any given ticket is a winner when revealed.
    /// For a 20% chance, set this to 0.20m.
    /// </summary>
    private const decimal WinChancePerTicket = 0.20m;

    /// <summary>
    /// A key for the "InstantWinPrize" to maintain consistency with the IRaffleVariant dictionary return.
    /// </summary>
    private const string PrizeKey = "InstantWinPrize";

    /// <summary>
    /// Computes the probability that a participant will have at least one winning ticket
    /// if they have purchased multiple unscratched tickets in this raffle.
    /// 
    /// Probability = 1 - (1 - p)^m, where m is # of that participant's tickets.
    /// </summary>
    public Dictionary<string, decimal> ComputeProbabilityDistribution(Raffle raffle, Participant participant)
    {
        // Count how many tickets the participant has in this raffle.
        // In a real scenario, we might only count unscratched tickets, or all tickets.
        int participantTickets = raffle.Tickets.Count(t => t.OwnerId == participant.Id);

        if (participantTickets == 0)
        {
            // If they have no tickets, no chance
            return new Dictionary<string, decimal>
            {
                [PrizeKey] = 0m
            };
        }

        // Probability that at least one is a winner:
        // pWinAny = 1 - (1 - WinChancePerTicket)^participantTickets
        double pLoseAll = Math.Pow((double)(1 - WinChancePerTicket), participantTickets);
        double pWinAny = 1.0 - pLoseAll;

        // Return as decimal
        return new Dictionary<string, decimal>
        {
            [PrizeKey] = (decimal)pWinAny
        };
    }

    /// <summary>
    /// For an instant-win raffle, there's typically no single moment to "draw winners."
    /// So this method doesn't truly apply in the standard sense. 
    /// We'll return an empty list to signify no centralized winners are drawn at once.
    /// </summary>
    public List<Ticket> DrawWinners(Raffle raffle)
    {
        // In a real system, you might not even call this for Instant-Win,
        // or you might implement a batch process that reveals unscratched tickets automatically (rare).
        return new List<Ticket>();
    }

    /// <summary>
    /// Example method (not part of IRaffleVariant) to "reveal" or "scratch" a specific ticket. 
    /// If your system allows it, you'd call this to determine if a ticket is a winner in real time.
    /// </summary>
    public bool RevealTicket(Ticket ticket)
    {
        // If the ticket is already scratched, return its existing result:
        if (ticket.IsWinner || !string.IsNullOrEmpty(ticket.PrizeAwarded))
        {
            // Means we already revealed it was a winner
            return true;
        }

        // Otherwise, do a random check:
        var rnd = new Random();
        double roll = rnd.NextDouble(); // [0..1)
        bool isWinner = (roll < (double)WinChancePerTicket);

        // Update ticket accordingly:
        if (isWinner)
        {
            ticket.IsWinner = true;
            ticket.PrizeAwarded = PrizeKey;
        }

        // We might also record the date/time of reveal, etc.

        return isWinner;
    }
}