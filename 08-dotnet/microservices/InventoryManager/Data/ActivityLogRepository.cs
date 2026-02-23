using Dapper;
using Newtonsoft.Json;
using InventoryManager.Models;

namespace InventoryManager.Data
{
    /// <summary>
    /// Repository for activity log operations
    /// </summary>
    public class ActivityLogRepository
    {
        /// <summary>
        /// Log an activity
        /// </summary>
        public void Log(int userId, string action, string tableName, int? recordId = null, 
            object? oldValues = null, object? newValues = null)
        {
            using var connection = DatabaseManager.GetConnection();
            
            connection.Execute(@"
                INSERT INTO ActivityLogs (
                    UserId, Action, TableName, RecordId, OldValues, NewValues, IpAddress, ComputerName)
                VALUES (
                    @UserId, @Action, @TableName, @RecordId, @OldValues, @NewValues, @IpAddress, @ComputerName)",
                new
                {
                    UserId = userId,
                    Action = action,
                    TableName = tableName,
                    RecordId = recordId,
                    OldValues = oldValues != null ? JsonConvert.SerializeObject(oldValues) : null,
                    NewValues = newValues != null ? JsonConvert.SerializeObject(newValues) : null,
                    IpAddress = GetLocalIpAddress(),
                    ComputerName = Environment.MachineName
                });
        }

        /// <summary>
        /// Get activity logs
        /// </summary>
        public List<ActivityLog> GetAll(DateTime? startDate = null, DateTime? endDate = null, 
            int? userId = null, string? tableName = null)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var sql = @"
                SELECT l.*, u.FullName as UserName
                FROM ActivityLogs l
                INNER JOIN Users u ON l.UserId = u.Id
                WHERE 1=1";
            
            if (startDate.HasValue)
                sql += " AND l.CreatedAt >= @StartDate";
            if (endDate.HasValue)
                sql += " AND l.CreatedAt <= @EndDate";
            if (userId.HasValue)
                sql += " AND l.UserId = @UserId";
            if (!string.IsNullOrEmpty(tableName))
                sql += " AND l.TableName = @TableName";
            
            sql += " ORDER BY l.CreatedAt DESC LIMIT 1000";
            
            return connection.Query<ActivityLog>(sql, new 
            { 
                StartDate = startDate?.ToString("yyyy-MM-dd"), 
                EndDate = endDate?.ToString("yyyy-MM-dd 23:59:59"),
                UserId = userId,
                TableName = tableName
            }).ToList();
        }

        /// <summary>
        /// Get logs by record
        /// </summary>
        public List<ActivityLog> GetByRecord(string tableName, int recordId)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.Query<ActivityLog>(@"
                SELECT l.*, u.FullName as UserName
                FROM ActivityLogs l
                INNER JOIN Users u ON l.UserId = u.Id
                WHERE l.TableName = @TableName AND l.RecordId = @RecordId
                ORDER BY l.CreatedAt DESC",
                new { TableName = tableName, RecordId = recordId }).ToList();
        }

        /// <summary>
        /// Clear old logs (keep last N days)
        /// </summary>
        public int ClearOldLogs(int keepDays = 90)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.Execute(@"
                DELETE FROM ActivityLogs 
                WHERE CreatedAt < datetime('now', @Days)",
                new { Days = $"-{keepDays} days" });
        }

        private string GetLocalIpAddress()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return ip.ToString();
                }
            }
            catch { }
            return "127.0.0.1";
        }
    }
}
