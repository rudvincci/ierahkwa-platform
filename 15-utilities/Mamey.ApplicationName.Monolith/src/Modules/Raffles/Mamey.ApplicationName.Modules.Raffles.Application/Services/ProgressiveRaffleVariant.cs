using Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

namespace Mamey.ApplicationName.Modules.Raffles.Application.Services;

/// <summary>
/// A simplified Progressive Raffle:
/// In each "round," exactly one ticket is drawn. 
/// That ticket has a partial chance to claim the jackpot. If it fails, the pot rolls over.
/// 
/// This class focuses on just one round's logic. 
/// For multiple rounds, you'd call DrawWinners each time 
/// or store progressive data in a "RaffleRound" entity.
/// </summary>
internal class ProgressiveRaffleVariant : IRaffleVariant
{
    /// <summary>
    /// The chance that the drawn ticket actually wins the jackpot. 
    /// E.g., 0.5 = 50% chance each round.
    /// You can set this from config or the raffle's VariantConfiguration.
    /// </summary>
    private const decimal SecondaryWinChance = 0.50m;

    /// <summary>
    /// A key name for the "ProgressiveJackpot" in the probability dictionary.
    /// </summary>
    private const string JackpotKey = "ProgressiveJackpot";

    /// <summary>
    /// Returns the probability that the participant wins 
    /// the jackpot *this round*.
    /// 
    /// Probability = (# owned / total) * SecondaryWinChance
    /// 
    /// If you want to factor in multiple rounds, 
    /// you'd do so in your service logic, not here.
    /// </summary>
    public Dictionary<string, decimal> ComputeProbabilityDistribution(Raffle raffle, Participant participant)
    {
        int totalTickets = raffle.Tickets.Count;
        if (totalTickets == 0)
        {
            // No tickets => no chance
            return new Dictionary<string, decimal>
            {
                [JackpotKey] = 0m
            };
        }

        int ownedTickets = raffle.Tickets.Count(t => t.OwnerId == participant.Id);

        // Probability of being drawn
        decimal pDrawn = (decimal)ownedTickets / totalTickets;

        // Probability of also winning the jackpot once drawn
        decimal pWinJackpot = pDrawn * SecondaryWinChance;

        return new Dictionary<string, decimal>
        {
            [JackpotKey] = pWinJackpot
        };
    }

    /// <summary>
    /// Executes a single round:
    /// 1. Draw a random ticket from the pool (like standard).
    /// 2. Check if it wins the jackpot (random check vs. SecondaryWinChance).
    /// 3. If it does, mark the ticket as winner, otherwise no winner is selected.
    /// 
    /// In real usage, if no winner is found, the jackpot is not claimed 
    /// and you do another round at a later time.
    /// </summary>
    public List<Ticket> DrawWinners(Raffle raffle)
    {
        // If no tickets, no draw
        if (raffle.Tickets.Count == 0)
        {
            return new List<Ticket>();
        }

        // Randomly pick a ticket
        var rnd = new Random();
        int index = rnd.Next(raffle.Tickets.Count);
        var drawnTicket = raffle.Tickets[index];

        // Then do the "secondary" check to see if the pot is claimed
        double chanceCheck = rnd.NextDouble(); // in [0,1)

        if (chanceCheck < (double)SecondaryWinChance)
        {
            // The jackpot is won!
            drawnTicket.IsWinner = true;
            drawnTicket.PrizeAwarded = JackpotKey;

            return new List<Ticket> { drawnTicket };
        }
        else
        {
            // No winner this round; the jackpot is not claimed 
            // (so we return an empty list).
            return new List<Ticket>();
        }
    }
}