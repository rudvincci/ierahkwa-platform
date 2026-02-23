namespace Mamey.ApplicationName.BlazorWasm.Models.Raffles;

public class Ticket
{
    public int Id { get; set; }
    public int RaffleItemId { get; set; }
    public string UserId { get; set; }
    public DateTime PurchaseDate { get; set; }
}