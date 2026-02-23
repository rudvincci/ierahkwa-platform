using Mamey.ApplicationName.BlazorWasm.Models.Raffles;

namespace Mamey.ApplicationName.BlazorWasm.Mock;

public class MockRaffleService
{
    private readonly List<RaffleItem> _raffleItems;
    private readonly List<Ticket> _tickets = new();

    public MockRaffleService()
    {
        _raffleItems = RaffleMockData.GetMockRaffleItems();
    }

    public List<RaffleItem> GetRaffleItems() => _raffleItems;

    public RaffleItem? GetRaffleItemById(int id) =>
        _raffleItems.FirstOrDefault(item => item.Id == id);

    public void BuyTicket(int raffleItemId, string userId, int ticketCount)
    {
        for (int i = 0; i < ticketCount; i++)
        {
            _tickets.Add(new Ticket
            {
                Id = new Random().Next(1, 10000),
                RaffleItemId = raffleItemId,
                UserId = userId,
                PurchaseDate = DateTime.Now
            });
        }
    }

    public List<Ticket> GetTicketsByUser(string userId) =>
        _tickets.Where(ticket => ticket.UserId == userId).ToList();
}