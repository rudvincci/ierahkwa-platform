using Mamey.ApplicationName.BlazorWasm.Models.Raffles;

namespace Mamey.ApplicationName.BlazorWasm.Mock;

public static class RaffleMockData
{
    public static List<RaffleItem> GetMockRaffleItems() => new()
    {
        new RaffleItem
        {
            Id = 1,
            Title = "Gaming Laptop",
            Description = "Win a high-performance gaming laptop!",
            ImageUrl = "https://via.placeholder.com/300",
            TicketPrice = 10.00m,
            RaffleDate = DateTime.Now.AddMinutes(10),
            TotalTickets = 5000,
        },
        new RaffleItem
        {
            Id = 2,
            Title = "Smartphone",
            Description = "A brand new smartphone with the latest features.",
            ImageUrl = "https://via.placeholder.com/300",
            TicketPrice = 5.00m,
            RaffleDate = DateTime.Now.AddHours(1),
            TotalTickets = 1000,
        },
        new RaffleItem
        {
            Id = 3,
            Title = "Smartwatch",
            Description = "Stylish smartwatch with health tracking features.",
            ImageUrl = "https://via.placeholder.com/300",
            TicketPrice = 3.00m,
            RaffleDate = DateTime.Now.AddDays(1),
            TotalTickets = 100,
        },
    };
}
