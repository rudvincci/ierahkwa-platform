namespace InventoryManager.Models
{
    /// <summary>
    /// Supplier model for vendor management
    /// </summary>
    public class Supplier
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string PaymentTerms { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? Notes { get; set; }
        public int ProductCount { get; set; }
    }
}
