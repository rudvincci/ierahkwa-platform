using Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

namespace Mamey.ApplicationName.Modules.Raffles.Application.Services;

/// <summary>
/// A "Door Prize" raffle typically gives each event attendee exactly 1 free ticket.
/// There's usually one or few prizes, and the draw is done from those tickets.
/// 
/// Probability for any participant = #TicketsParticipantHas / TotalTickets.
/// Often that's 1 / N if exactly one ticket per attendee.
/// </summary>
internal class DoorPrizeRaffleVariant : IRaffleVariant
{
    private const string PrizeKey = "DoorPrize";

    /// <summary>
    /// Computes the probability distribution for this raffle.
    /// Typically, there's just one prize, so the distribution has one key: "DoorPrize".
    /// Probability = (# participant tickets) / (total tickets).
    /// In many door prize scenarios, that's 1/N if each attendee has exactly 1 ticket.
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

        // Typically 1 ticket per attendee, but let's compute in case it's more.
        int participantTickets = raffle.Tickets.Count(t => t.OwnerId == participant.Id);

        decimal probability = (decimal)participantTickets / totalTickets;

        return new Dictionary<string, decimal>
        {
            [PrizeKey] = probability
        };
    }

    /// <summary>
    /// Single-winner draw from the entire ticket pool, awarding the "DoorPrize" to one random ticket.
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

        return new List<Ticket> { winningTicket };
    }
}