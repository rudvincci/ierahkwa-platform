namespace InventoryManager.Models
{
    /// <summary>
    /// Stock movement model for tracking inventory changes
    /// </summary>
    public class StockMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public MovementType Type { get; set; }
        public int Quantity { get; set; }
        public int PreviousStock { get; set; }
        public int NewStock { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime MovementDate { get; set; } = DateTime.Now;
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? DocumentNumber { get; set; }
    }

    public enum MovementType
    {
        Purchase = 1,      // Stock In - Purchase
        Sale = 2,          // Stock Out - Sale
        Return = 3,        // Stock In - Customer Return
        Adjustment = 4,    // Stock Adjustment
        Transfer = 5,      // Transfer between locations
        Damage = 6,        // Stock Out - Damaged goods
        Initial = 7        // Initial stock entry
    }
}
