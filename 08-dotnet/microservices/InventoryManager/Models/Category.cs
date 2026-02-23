namespace InventoryManager.Models
{
    /// <summary>
    /// Category model for product classification
    /// </summary>
    public class Category
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int ProductCount { get; set; }
    }
}
