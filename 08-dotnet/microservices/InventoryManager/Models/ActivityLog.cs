namespace InventoryManager.Models
{
    /// <summary>
    /// Activity log for audit trail
    /// </summary>
    public class ActivityLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string Action { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public int? RecordId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string ComputerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
