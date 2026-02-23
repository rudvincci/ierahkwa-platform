using System.Data.SQLite;
using Dapper;
using BCrypt.Net;
using InventoryManager.Models;

namespace InventoryManager.Data
{
    /// <summary>
    /// Repository for user operations
    /// </summary>
    public class UserRepository
    {
        /// <summary>
        /// Authenticate user with username and password
        /// </summary>
        public User? Authenticate(string username, string password)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var user = connection.QueryFirstOrDefault<User>(@"
                SELECT * FROM Users WHERE Username = @Username AND IsActive = 1",
                new { Username = username });

            if (user == null)
                return null;

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            // Update last login
            connection.Execute(@"
                UPDATE Users SET 
                    LastLoginAt = datetime('now', 'localtime'),
                    ComputerName = @ComputerName,
                    SessionId = @SessionId
                WHERE Id = @Id",
                new 
                { 
                    Id = user.Id,
                    ComputerName = Environment.MachineName,
                    SessionId = Guid.NewGuid().ToString()
                });

            user.LastLoginAt = DateTime.Now;
            user.ComputerName = Environment.MachineName;

            return user;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        public List<User> GetAll()
        {
            using var connection = DatabaseManager.GetConnection();
            return connection.Query<User>("SELECT * FROM Users ORDER BY FullName").ToList();
        }

        /// <summary>
        /// Get active users (currently logged in)
        /// </summary>
        public List<User> GetActiveUsers()
        {
            using var connection = DatabaseManager.GetConnection();
            return connection.Query<User>(@"
                SELECT * FROM Users 
                WHERE IsActive = 1 
                AND SessionId IS NOT NULL 
                AND LastLoginAt > datetime('now', '-1 hour')
                ORDER BY LastLoginAt DESC").ToList();
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        public User? GetById(int id)
        {
            using var connection = DatabaseManager.GetConnection();
            return connection.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE Id = @Id", new { Id = id });
        }

        /// <summary>
        /// Create new user
        /// </summary>
        public int Create(User user, string password)
        {
            using var connection = DatabaseManager.GetConnection();
            
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            
            return connection.ExecuteScalar<int>(@"
                INSERT INTO Users (Username, PasswordHash, FullName, Email, Role, IsActive)
                VALUES (@Username, @PasswordHash, @FullName, @Email, @Role, @IsActive);
                SELECT last_insert_rowid();", user);
        }

        /// <summary>
        /// Update user
        /// </summary>
        public bool Update(User user)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var result = connection.Execute(@"
                UPDATE Users SET 
                    Username = @Username,
                    FullName = @FullName,
                    Email = @Email,
                    Role = @Role,
                    IsActive = @IsActive
                WHERE Id = @Id", user);
            
            return result > 0;
        }

        /// <summary>
        /// Change user password
        /// </summary>
        public bool ChangePassword(int userId, string newPassword)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            
            var result = connection.Execute(@"
                UPDATE Users SET PasswordHash = @PasswordHash WHERE Id = @Id",
                new { Id = userId, PasswordHash = passwordHash });
            
            return result > 0;
        }

        /// <summary>
        /// Delete user (soft delete)
        /// </summary>
        public bool Delete(int id)
        {
            using var connection = DatabaseManager.GetConnection();
            var result = connection.Execute(
                "UPDATE Users SET IsActive = 0 WHERE Id = @Id", new { Id = id });
            return result > 0;
        }

        /// <summary>
        /// Logout user
        /// </summary>
        public void Logout(int userId)
        {
            using var connection = DatabaseManager.GetConnection();
            connection.Execute(@"
                UPDATE Users SET SessionId = NULL WHERE Id = @Id",
                new { Id = userId });
        }
    }
}
