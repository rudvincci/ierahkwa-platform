using Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

namespace Mamey.ApplicationName.Modules.Raffles.Application.Services;

/// <summary>
/// A multi-tier raffle awards multiple prizes in sequence 
/// (e.g., 1st, 2nd, and 3rd place).
/// In the typical "without replacement" scenario, once a ticket wins,
/// it's removed from the pool for subsequent prizes.
/// </summary>
internal class MultiTierRaffleVariant : IRaffleVariant
{
    // For illustration, we'll use a fixed list of 3 tier names. 
    // In production, you might read these from Raffle.Prizes or config.
    private readonly List<string> _prizeNames = new List<string>
    {
        "FirstPrize",
        "SecondPrize",
        "ThirdPrize"
    };

    /// <summary>
    /// Computes the chance of winning each tier individually. 
    /// Then returns a dictionary mapping each tier name -> probability.
    /// 
    /// Probability of winning tier i depends on losing all previous tiers 
    /// (so that participant's tickets remain in the pool) 
    /// and then winning on the ith draw.
    /// 
    /// This method does NOT sum them for "overall chance" - 
    /// it gives the distinct chance of each tier. 
    /// The calling code can interpret or sum them as needed.
    /// </summary>
    public Dictionary<string, decimal> ComputeProbabilityDistribution(Raffle raffle, Participant participant)
    {
        int totalTickets = raffle.Tickets.Count;
        if (totalTickets == 0)
        {
            // No tickets => 0 probability for all tiers
            return _prizeNames.ToDictionary(name => name, _ => 0m);
        }

        int ownedTickets = raffle.Tickets.Count(t => t.OwnerId == participant.Id);

        var result = new Dictionary<string, decimal>();

        // We'll simulate the sequential approach for each tier:
        // Keep track of how many tickets remain, 
        // and how many participant tickets remain if they haven't won yet.
        int remainingTotal = totalTickets;
        int remainingOwned = ownedTickets;

        // Probability that participant has NOT won any previous tier
        // i.e., hasn't been removed from the pool yet
        decimal cumulativeLoseProb = 1m;

        for (int i = 0; i < _prizeNames.Count; i++)
        {
            if (remainingTotal <= 0 || remainingOwned <= 0)
            {
                // If no tickets remain or the participant no longer has any tickets, 
                // participant's chance for further prizes is 0
                result[_prizeNames[i]] = 0m;
                continue;
            }

            // Probability of winning this particular tier 
            // given that the participant hasn't won a previous tier
            decimal pWinThisTier = (decimal)remainingOwned / remainingTotal;

            // Net probability = cumulativeLoseProb * pWinThisTier
            decimal finalProbForThisTier = cumulativeLoseProb * pWinThisTier;

            result[_prizeNames[i]] = finalProbForThisTier;

            // Next, update the chance that participant is STILL losing after this tier:
            // They either lose this tier with probability (1 - pWinThisTier).
            cumulativeLoseProb *= (1 - pWinThisTier);

            // Also, if the participant didn't lose, they'd be removed from the pool. 
            // But we only remove them if they actually won. 
            // For the next tier, we assume "without replacement" for the winner.
            // So the total pool is reduced by 1, 
            // and if the participant won, their tickets are effectively 0 for future tiers.
            remainingTotal -= 1;

            // If participant won, remainingOwned should go to 0 
            // because they can't win subsequent tiers (if that's the rule).
            // But for probability calculations, we proceed conceptually:
            // If they "won," the probability that they remain in the pool is 0 for the next draw.
            // The "cumulativeLoseProb" factor accounts for the scenario where they didn't win.
        }

        return result;
    }

    /// <summary>
    /// Draws winners for each tier in order.
    /// 1. Shuffle or pick a random ticket for the first prize, mark it.
    /// 2. Remove that ticket.
    /// 3. Repeat for the second prize, etc.
    /// 4. Return all winners as a list (in order of drawing).
    /// </summary>
    public List<Ticket> DrawWinners(Raffle raffle)
    {
        // We'll store each winner in this list
        var winners = new List<Ticket>();

        // If there are no tickets, we can't draw anything
        if (raffle.Tickets.Count == 0)
        {
            return winners;
        }

        // Convert tickets to a mutable list
        var ticketPool = raffle.Tickets.ToList();
        var rnd = new Random();

        // For each tier in our list, pick one winner if possible
        foreach (var prizeName in _prizeNames)
        {
            if (ticketPool.Count == 0)
            {
                break; // No more tickets to draw from
            }

            int index = rnd.Next(ticketPool.Count);
            var winningTicket = ticketPool[index];

            // Mark as winner
            winningTicket.IsWinner = true;
            winningTicket.PrizeAwarded = prizeName;

            winners.Add(winningTicket);

            // Remove from pool so they can't win subsequent prizes
            ticketPool.RemoveAt(index);
        }

        return winners;
    }
}