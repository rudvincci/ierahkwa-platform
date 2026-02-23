using Dapper;

namespace InventoryManager.Data
{
    /// <summary>
    /// Repository for application settings
    /// </summary>
    public class SettingsRepository
    {
        /// <summary>
        /// Get setting value
        /// </summary>
        public string? GetValue(string key)
        {
            using var connection = DatabaseManager.GetConnection();
            return connection.ExecuteScalar<string?>(
                "SELECT Value FROM Settings WHERE Key = @Key",
                new { Key = key });
        }

        /// <summary>
        /// Get setting value with default
        /// </summary>
        public string GetValue(string key, string defaultValue)
        {
            return GetValue(key) ?? defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        public void SetValue(string key, string value, string? description = null)
        {
            using var connection = DatabaseManager.GetConnection();
            
            connection.Execute(@"
                INSERT INTO Settings (Key, Value, Description, UpdatedAt)
                VALUES (@Key, @Value, @Description, datetime('now', 'localtime'))
                ON CONFLICT(Key) DO UPDATE SET 
                    Value = @Value,
                    Description = COALESCE(@Description, Description),
                    UpdatedAt = datetime('now', 'localtime')",
                new { Key = key, Value = value, Description = description });
        }

        /// <summary>
        /// Get all settings
        /// </summary>
        public Dictionary<string, string> GetAll()
        {
            using var connection = DatabaseManager.GetConnection();
            
            var settings = connection.Query<(string Key, string Value)>(
                "SELECT Key, Value FROM Settings");
            
            return settings.ToDictionary(s => s.Key, s => s.Value ?? "");
        }

        /// <summary>
        /// Delete setting
        /// </summary>
        public void Delete(string key)
        {
            using var connection = DatabaseManager.GetConnection();
            connection.Execute("DELETE FROM Settings WHERE Key = @Key", new { Key = key });
        }
    }
}
