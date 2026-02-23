using Dapper;
using InventoryManager.Models;

namespace InventoryManager.Data
{
    /// <summary>
    /// Repository for supplier operations
    /// </summary>
    public class SupplierRepository
    {
        /// <summary>
        /// Get all suppliers
        /// </summary>
        public List<Supplier> GetAll(bool includeInactive = false)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var sql = @"
                SELECT s.*,
                    (SELECT COUNT(*) FROM Products WHERE SupplierId = s.Id AND IsActive = 1) as ProductCount
                FROM Suppliers s
                " + (includeInactive ? "" : "WHERE s.IsActive = 1") + @"
                ORDER BY s.Name";
            
            return connection.Query<Supplier>(sql).ToList();
        }

        /// <summary>
        /// Search suppliers
        /// </summary>
        public List<Supplier> Search(string searchTerm)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.Query<Supplier>(@"
                SELECT s.*,
                    (SELECT COUNT(*) FROM Products WHERE SupplierId = s.Id AND IsActive = 1) as ProductCount
                FROM Suppliers s
                WHERE s.IsActive = 1 AND (
                    s.Code LIKE @Search 
                    OR s.Name LIKE @Search 
                    OR s.ContactPerson LIKE @Search
                    OR s.Email LIKE @Search)
                ORDER BY s.Name",
                new { Search = $"%{searchTerm}%" }).ToList();
        }

        /// <summary>
        /// Get supplier by ID
        /// </summary>
        public Supplier? GetById(int id)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.QueryFirstOrDefault<Supplier>(@"
                SELECT s.*,
                    (SELECT COUNT(*) FROM Products WHERE SupplierId = s.Id AND IsActive = 1) as ProductCount
                FROM Suppliers s
                WHERE s.Id = @Id", new { Id = id });
        }

        /// <summary>
        /// Create new supplier
        /// </summary>
        public int Create(Supplier supplier)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.ExecuteScalar<int>(@"
                INSERT INTO Suppliers (
                    Code, Name, ContactPerson, Phone, Email, Address,
                    City, Country, TaxId, Website, PaymentTerms, IsActive, Notes)
                VALUES (
                    @Code, @Name, @ContactPerson, @Phone, @Email, @Address,
                    @City, @Country, @TaxId, @Website, @PaymentTerms, @IsActive, @Notes);
                SELECT last_insert_rowid();", supplier);
        }

        /// <summary>
        /// Update supplier
        /// </summary>
        public bool Update(Supplier supplier)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var result = connection.Execute(@"
                UPDATE Suppliers SET
                    Code = @Code,
                    Name = @Name,
                    ContactPerson = @ContactPerson,
                    Phone = @Phone,
                    Email = @Email,
                    Address = @Address,
                    City = @City,
                    Country = @Country,
                    TaxId = @TaxId,
                    Website = @Website,
                    PaymentTerms = @PaymentTerms,
                    IsActive = @IsActive,
                    Notes = @Notes
                WHERE Id = @Id", supplier);
            
            return result > 0;
        }

        /// <summary>
        /// Delete supplier (soft delete)
        /// </summary>
        public bool Delete(int id)
        {
            using var connection = DatabaseManager.GetConnection();
            var result = connection.Execute(
                "UPDATE Suppliers SET IsActive = 0 WHERE Id = @Id", new { Id = id });
            return result > 0;
        }

        /// <summary>
        /// Generate next supplier code
        /// </summary>
        public string GenerateNextCode(string prefix = "SUP")
        {
            using var connection = DatabaseManager.GetConnection();
            
            var lastCode = connection.ExecuteScalar<string?>(@"
                SELECT Code FROM Suppliers 
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
