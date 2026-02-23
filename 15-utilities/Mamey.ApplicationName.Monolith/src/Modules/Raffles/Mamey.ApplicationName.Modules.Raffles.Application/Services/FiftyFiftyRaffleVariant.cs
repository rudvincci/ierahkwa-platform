using Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

namespace Mamey.ApplicationName.Modules.Raffles.Application.Services;

/// <summary>
/// A 50/50 raffle is essentially a single-winner draw 
/// where the total pot is split between the winner and the organizer. 
/// Probability is the same as a standard raffle (ownedTickets / totalTickets).
/// The main difference is how the "prize" is calculated (a share of total revenue).
/// </summary>
internal class FiftyFiftyRaffleVariant : IRaffleVariant
{
    /// <summary>
    /// In a typical 50/50, the participant gets 50% of the pot. 
    /// If you want a 60/40 or 70/30, adjust this ratio.
    /// </summary>
    private const decimal WinnerShareRatio = 0.50m;

    /// <summary>
    /// The dictionary key for the prize name in the probability distribution.
    /// </summary>
    private const string PrizeKey = "FiftyFiftyJackpot";

    /// <summary>
    /// Computes the probability distribution for this variant. 
    /// There's only one winner in a typical 50/50, so we return a single key.
    /// Probability = participantTickets / totalTickets.
    /// </summary>
    public Dictionary<string, decimal> ComputeProbabilityDistribution(Raffle raffle, Participant participant)
    {
        int totalTickets = raffle.Tickets.Count;
        if (totalTickets == 0)
        {
            // No tickets means no chance of winning
            return new Dictionary<string, decimal>
            {
                [PrizeKey] = 0m
            };
        }

        int ownedTickets = raffle.Tickets.Count(t => t.OwnerId == participant.Id);

        // Probability of winning
        decimal probability = (decimal)ownedTickets / totalTickets;

        return new Dictionary<string, decimal>
        {
            [PrizeKey] = probability
        };
    }

    /// <summary>
    /// Draws the winning ticket from the entire pool. 
    /// Then you would presumably calculate the pot based on ticket revenue
    /// and give the winner 50% of that pot. 
    /// We'll just mark the winning ticket and let the calling service handle the payout.
    /// </summary>
    public List<Ticket> DrawWinners(Raffle raffle)
    {
        // If no tickets are sold, we can't pick a winner
        if (raffle.Tickets.Count == 0)
        {
            return new List<Ticket>();
        }

        // Let's pick a random winner
        var rnd = new Random();
        int winnerIndex = rnd.Next(raffle.Tickets.Count);

        var winningTicket = raffle.Tickets[winnerIndex];
        winningTicket.IsWinner = true;
        winningTicket.PrizeAwarded = PrizeKey;

        // (Optional) You could store the pot in raffle or recalc from ticket prices:
        // decimal totalRevenue = raffle.Tickets.Sum(t => t.Price);
        // decimal winnerAmount = totalRevenue * WinnerShareRatio;
        // 
        // In a real system, you'd record that in a Payment or Payout entity, 
        // or log it in some "JackpotAwarded" field. 
        //
        // For example:
        // raffle.JackpotAwarded = winnerAmount;
        // raffle.Status = RaffleStatus.Completed; 
        // etc.

        // Return the single winning ticket
        return new List<Ticket> { winningTicket };
    }
}