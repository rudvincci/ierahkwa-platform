namespace InventoryManager.Models
{
    /// <summary>
    /// User model for authentication and authorization
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLoginAt { get; set; }
        public string? ComputerName { get; set; }
        public string? SessionId { get; set; }
    }

    public enum UserRole
    {
        Admin = 1,
        Manager = 2,
        User = 3,
        ReadOnly = 4
    }
}
