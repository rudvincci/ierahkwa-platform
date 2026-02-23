using Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

namespace Mamey.ApplicationName.Modules.Raffles.Application.Services;

/// <summary>
/// A "Hybrid (Live + Online)" Raffle integrates both in-person/physical ticket sales 
/// and online/digital ticket sales into a single pool. 
/// The drawing often occurs live, possibly broadcast online,
/// but the fundamental logic for picking a winner is the same (random from the combined pool).
/// </summary>
internal class HybridLiveRaffleVariant : IRaffleVariant
{
    private const string PrizeKey = "HybridMainPrize";

    /// <summary>
    /// Computes the participant's probability of winning the main prize. 
    /// It's still (# participant tickets / total tickets),
    /// even though those tickets might come from offline or online sources.
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
    /// Draws a single winner from the entire ticket pool 
    /// (combining offline and online tickets).
    /// 
    /// In a real hybrid scenario, you might print out stubs for online tickets 
    /// and physically draw from a drum, or do a purely digital random draw 
    /// projected at the live event.
    /// Here we model the digital approach for demonstration.
    /// </summary>
    public List<Ticket> DrawWinners(Raffle raffle)
    {
        // If no tickets, no winner
        if (raffle.Tickets.Count == 0)
        {
            return new List<Ticket>();
        }

        // Randomly pick from the entire combined ticket list
        var rnd = new Random();
        int index = rnd.Next(raffle.Tickets.Count);

        var winningTicket = raffle.Tickets[index];
        winningTicket.IsWinner = true;
        winningTicket.PrizeAwarded = PrizeKey;

        return new List<Ticket> { winningTicket };
    }
}