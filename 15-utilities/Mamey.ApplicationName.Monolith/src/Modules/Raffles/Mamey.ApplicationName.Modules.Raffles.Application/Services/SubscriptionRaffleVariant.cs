using Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

namespace Mamey.ApplicationName.Modules.Raffles.Application.Services;

/// <summary>
/// A Subscription (Ongoing) Raffle means participants subscribe (pay monthly or otherwise)
/// and are automatically entered into recurring draws (e.g., monthly).
/// Each "draw" can be handled like a standard single-winner selection from among active subscribers.
/// </summary>
internal class SubscriptionRaffleVariant : IRaffleVariant
{
    private const string PrizeKey = "SubscriptionDraw";

    /// <summary>
    /// Computes the probability distribution for this raffle in a single draw context.
    /// Typically there's just 1 winner each period. Probability = (# participant tickets) / (total tickets).
    /// 
    /// If a participant is subscribed, they have an entry each draw cycle. 
    /// You may store multiple tickets if they pay for multiple subscription entries, 
    /// or just 1 if a subscription yields a single entry.
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
    /// Draws a winner for the current period (a single iteration).
    /// In practice, you might call this method monthly or weekly via a scheduled job.
    /// It's the same random selection as a standard single-winner approach.
    /// </summary>
    public List<Ticket> DrawWinners(Raffle raffle)
    {
        if (raffle.Tickets.Count == 0)
        {
            return new List<Ticket>();
        }

        var rnd = new Random();
        int index = rnd.Next(raffle.Tickets.Count);

        var winningTicket = raffle.Tickets[index];
        winningTicket.IsWinner = true;
        winningTicket.PrizeAwarded = PrizeKey;

        // We might record the date/time, which month or round it was, etc.
        return new List<Ticket> { winningTicket };
    }
}