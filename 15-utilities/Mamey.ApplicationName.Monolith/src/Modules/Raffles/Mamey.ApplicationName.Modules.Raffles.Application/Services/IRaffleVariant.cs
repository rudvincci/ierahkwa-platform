using Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

namespace Mamey.ApplicationName.Modules.Raffles.Application.Services;

internal interface IRaffleVariant
{
    // For each distinct prize or outcome, returns the probability
    // that the given participant wins that prize/outcome.
    Dictionary<string, decimal> ComputeProbabilityDistribution(Raffle raffle, Participant participant);

    // Performs the drawing event, returning the winning ticket(s).
    // In the standard raffle, we typically return exactly one winner.
    List<Ticket> DrawWinners(Raffle raffle);
}