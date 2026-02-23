using Dapper;
using InventoryManager.Models;

namespace InventoryManager.Data
{
    /// <summary>
    /// Repository for category operations
    /// </summary>
    public class CategoryRepository
    {
        /// <summary>
        /// Get all categories
        /// </summary>
        public List<Category> GetAll(bool includeInactive = false)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var sql = @"
                SELECT c.*, 
                    p.Name as ParentName,
                    (SELECT COUNT(*) FROM Products WHERE CategoryId = c.Id AND IsActive = 1) as ProductCount
                FROM Categories c
                LEFT JOIN Categories p ON c.ParentId = p.Id
                " + (includeInactive ? "" : "WHERE c.IsActive = 1") + @"
                ORDER BY c.Name";
            
            return connection.Query<Category>(sql).ToList();
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        public Category? GetById(int id)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.QueryFirstOrDefault<Category>(@"
                SELECT c.*, p.Name as ParentName
                FROM Categories c
                LEFT JOIN Categories p ON c.ParentId = p.Id
                WHERE c.Id = @Id", new { Id = id });
        }

        /// <summary>
        /// Create new category
        /// </summary>
        public int Create(Category category)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.ExecuteScalar<int>(@"
                INSERT INTO Categories (Code, Name, Description, ParentId, IsActive)
                VALUES (@Code, @Name, @Description, @ParentId, @IsActive);
                SELECT last_insert_rowid();", category);
        }

        /// <summary>
        /// Update category
        /// </summary>
        public bool Update(Category category)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var result = connection.Execute(@"
                UPDATE Categories SET
                    Code = @Code,
                    Name = @Name,
                    Description = @Description,
                    ParentId = @ParentId,
                    IsActive = @IsActive
                WHERE Id = @Id", category);
            
            return result > 0;
        }

        /// <summary>
        /// Delete category (soft delete)
        /// </summary>
        public bool Delete(int id)
        {
            using var connection = DatabaseManager.GetConnection();
            var result = connection.Execute(
                "UPDATE Categories SET IsActive = 0 WHERE Id = @Id", new { Id = id });
            return result > 0;
        }

        /// <summary>
        /// Generate next category code
        /// </summary>
        public string GenerateNextCode(string prefix = "CAT")
        {
            using var connection = DatabaseManager.GetConnection();
            
            var lastCode = connection.ExecuteScalar<string?>(@"
                SELECT Code FROM Categories 
                WHERE Code LIKE @Prefix
                ORDER BY Code DESC LIMIT 1",
                new { Prefix = $"{prefix}%" });

            if (string.IsNullOrEmpty(lastCode))
                return $"{prefix}001";

            var numberPart = lastCode.Substring(prefix.Length);
            if (int.TryParse(numberPart, out int number))
                return $"{prefix}{(number + 1):D3}";

            return $"{prefix}001";
        }
    }
}
