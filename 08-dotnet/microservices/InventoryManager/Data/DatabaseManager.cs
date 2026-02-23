using System.Data.SQLite;
using Dapper;
using BCrypt.Net;

namespace InventoryManager.Data
{
    /// <summary>
    /// Database manager for SQLite operations with multi-user support
    /// </summary>
    public static class DatabaseManager
    {
        private static string _connectionString = string.Empty;
        private static readonly string DatabasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "InventoryManager", "inventory.db");

        public static string ConnectionString => _connectionString;

        /// <summary>
        /// Initialize the database
        /// </summary>
        public static void Initialize()
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(DatabasePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Build connection string with WAL mode for multi-user support
            _connectionString = new SQLiteConnectionStringBuilder
            {
                DataSource = DatabasePath,
                Version = 3,
                JournalMode = SQLiteJournalModeEnum.Wal,  // Write-Ahead Logging for concurrent access
                SyncMode = SynchronizationModes.Normal,
                CacheSize = 10000,
                PageSize = 4096,
                Pooling = true,
                MaxPoolSize = 100,
                BusyTimeout = 30000  // 30 seconds timeout for locked database
            }.ToString();

            // Create tables if they don't exist
            CreateTables();
            
            // Seed default data
            SeedDefaultData();
        }

        /// <summary>
        /// Get a new database connection
        /// </summary>
        public static SQLiteConnection GetConnection()
        {
            var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Create all database tables
        /// </summary>
        private static void CreateTables()
        {
            using var connection = GetConnection();
            
            // Users table
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE COLLATE NOCASE,
                    PasswordHash TEXT NOT NULL,
                    FullName TEXT NOT NULL,
                    Email TEXT,
                    Role INTEGER NOT NULL DEFAULT 3,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
                    LastLoginAt TEXT,
                    ComputerName TEXT,
                    SessionId TEXT
                )");

            // Categories table
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Categories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Code TEXT NOT NULL UNIQUE COLLATE NOCASE,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    ParentId INTEGER,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
                    FOREIGN KEY (ParentId) REFERENCES Categories(Id)
                )");

            // Suppliers table
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Suppliers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Code TEXT NOT NULL UNIQUE COLLATE NOCASE,
                    Name TEXT NOT NULL,
                    ContactPerson TEXT,
                    Phone TEXT,
                    Email TEXT,
                    Address TEXT,
                    City TEXT,
                    Country TEXT,
                    TaxId TEXT,
                    Website TEXT,
                    PaymentTerms TEXT,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
                    Notes TEXT
                )");

            // Products table
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Code TEXT NOT NULL UNIQUE COLLATE NOCASE,
                    Barcode TEXT,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    CategoryId INTEGER,
                    SupplierId INTEGER,
                    PurchasePrice REAL NOT NULL DEFAULT 0,
                    SalePrice REAL NOT NULL DEFAULT 0,
                    CurrentStock INTEGER NOT NULL DEFAULT 0,
                    MinimumStock INTEGER NOT NULL DEFAULT 0,
                    MaximumStock INTEGER NOT NULL DEFAULT 0,
                    Unit TEXT DEFAULT 'PCS',
                    Location TEXT,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
                    CreatedBy INTEGER,
                    UpdatedBy INTEGER,
                    Image BLOB,
                    Notes TEXT,
                    FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
                    FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id),
                    FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
                    FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
                )");

            // Stock Movements table
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS StockMovements (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductId INTEGER NOT NULL,
                    Type INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    PreviousStock INTEGER NOT NULL,
                    NewStock INTEGER NOT NULL,
                    UnitPrice REAL NOT NULL DEFAULT 0,
                    TotalValue REAL NOT NULL DEFAULT 0,
                    Reference TEXT,
                    Notes TEXT,
                    MovementDate TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
                    UserId INTEGER NOT NULL,
                    DocumentNumber TEXT,
                    FOREIGN KEY (ProductId) REFERENCES Products(Id),
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                )");

            // Activity Log table
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS ActivityLogs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Action TEXT NOT NULL,
                    TableName TEXT NOT NULL,
                    RecordId INTEGER,
                    OldValues TEXT,
                    NewValues TEXT,
                    IpAddress TEXT,
                    ComputerName TEXT,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                )");

            // Settings table for application configuration
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Settings (
                    Key TEXT PRIMARY KEY NOT NULL,
                    Value TEXT,
                    Description TEXT,
                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime'))
                )");

            // Create indexes for better performance
            connection.Execute(@"
                CREATE INDEX IF NOT EXISTS idx_products_code ON Products(Code);
                CREATE INDEX IF NOT EXISTS idx_products_barcode ON Products(Barcode);
                CREATE INDEX IF NOT EXISTS idx_products_name ON Products(Name);
                CREATE INDEX IF NOT EXISTS idx_products_category ON Products(CategoryId);
                CREATE INDEX IF NOT EXISTS idx_products_supplier ON Products(SupplierId);
                CREATE INDEX IF NOT EXISTS idx_stockmovements_product ON StockMovements(ProductId);
                CREATE INDEX IF NOT EXISTS idx_stockmovements_date ON StockMovements(MovementDate);
                CREATE INDEX IF NOT EXISTS idx_activitylogs_user ON ActivityLogs(UserId);
                CREATE INDEX IF NOT EXISTS idx_activitylogs_date ON ActivityLogs(CreatedAt);
            ");
        }

        /// <summary>
        /// Seed default data
        /// </summary>
        private static void SeedDefaultData()
        {
            using var connection = GetConnection();

            // Check if admin user exists
            var adminExists = connection.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM Users WHERE Username = @Username", 
                new { Username = "admin" }) > 0;

            if (!adminExists)
            {
                // Create default admin user (password: admin123)
                var passwordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
                connection.Execute(@"
                    INSERT INTO Users (Username, PasswordHash, FullName, Email, Role, IsActive)
                    VALUES (@Username, @PasswordHash, @FullName, @Email, @Role, 1)",
                    new
                    {
                        Username = "admin",
                        PasswordHash = passwordHash,
                        FullName = "System Administrator",
                        Email = "admin@inventory.local",
                        Role = (int)Models.UserRole.Admin
                    });

                // Create default categories
                var categories = new[]
                {
                    ("CAT001", "Electronics", "Electronic devices and components"),
                    ("CAT002", "Office Supplies", "Office materials and supplies"),
                    ("CAT003", "Furniture", "Office and home furniture"),
                    ("CAT004", "Raw Materials", "Raw materials for production"),
                    ("CAT005", "Finished Goods", "Ready for sale products")
                };

                foreach (var (code, name, description) in categories)
                {
                    connection.Execute(@"
                        INSERT OR IGNORE INTO Categories (Code, Name, Description)
                        VALUES (@Code, @Name, @Description)",
                        new { Code = code, Name = name, Description = description });
                }

                // Insert default settings
                var settings = new Dictionary<string, (string Value, string Description)>
                {
                    { "CompanyName", ("Sovereign Akwesasne Government", "Company name for reports") },
                    { "CompanyAddress", ("", "Company address for reports") },
                    { "CompanyPhone", ("", "Company phone number") },
                    { "CompanyEmail", ("", "Company email address") },
                    { "Currency", ("USD", "Default currency") },
                    { "DateFormat", ("yyyy-MM-dd", "Date format for display") },
                    { "BackupPath", (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "InventoryBackups"), "Default backup path") },
                    { "AutoBackup", ("true", "Enable automatic backups") },
                    { "AutoBackupInterval", ("24", "Auto backup interval in hours") },
                    { "LowStockAlert", ("true", "Enable low stock alerts") },
                    { "Theme", ("Office2019Colorful", "Application theme") }
                };

                foreach (var setting in settings)
                {
                    connection.Execute(@"
                        INSERT OR IGNORE INTO Settings (Key, Value, Description)
                        VALUES (@Key, @Value, @Description)",
                        new { Key = setting.Key, Value = setting.Value.Value, Description = setting.Value.Description });
                }
            }
        }

        /// <summary>
        /// Get database file path
        /// </summary>
        public static string GetDatabasePath() => DatabasePath;

        /// <summary>
        /// Backup database to specified path
        /// </summary>
        public static bool BackupDatabase(string backupPath)
        {
            try
            {
                using var sourceConnection = GetConnection();
                using var destConnection = new SQLiteConnection($"Data Source={backupPath};Version=3;");
                destConnection.Open();
                sourceConnection.BackupDatabase(destConnection, "main", "main", -1, null, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Restore database from backup
        /// </summary>
        public static bool RestoreDatabase(string backupPath)
        {
            try
            {
                if (!File.Exists(backupPath))
                    return false;

                // Close all connections
                SQLiteConnection.ClearAllPools();

                // Copy backup file
                File.Copy(backupPath, DatabasePath, true);
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
