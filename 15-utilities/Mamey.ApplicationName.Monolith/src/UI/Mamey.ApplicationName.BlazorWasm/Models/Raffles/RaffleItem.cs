namespace Mamey.ApplicationName.BlazorWasm.Models.Raffles;

public class RaffleItem
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public decimal TicketPrice { get; set; }

    public DateTime RaffleDate { get; set; }

    public int TotalTickets { get; set; }

    public int MaxTicketsPerOrder { get; set; }
}