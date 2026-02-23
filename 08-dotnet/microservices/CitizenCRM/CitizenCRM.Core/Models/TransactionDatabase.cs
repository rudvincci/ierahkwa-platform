using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CitizenCRM.Core.Models
{
    /// <summary>
    /// Base de Datos de Transacciones VIP
    /// Sovereign Government of Ierahkwa Ne Kanienke
    /// </summary>
    public class TransactionDatabase
    {
        private static List<VipTransactionRecord> _transactions = new();
        private static int _nextId = 1;

        /// <summary>
        /// Agregar nueva transacción
        /// </summary>
        public static VipTransactionRecord Add(VipTransactionRecord tx)
        {
            tx.Id = _nextId++;
            tx.CreatedAt = DateTime.UtcNow;
            tx.UpdatedAt = DateTime.UtcNow;
            _transactions.Add(tx);
            return tx;
        }

        /// <summary>
        /// Obtener todas las transacciones
        /// </summary>
        public static List<VipTransactionRecord> GetAll() => _transactions.ToList();

        /// <summary>
        /// Buscar por código Ierahkwa
        /// </summary>
        public static VipTransactionRecord? GetByIerahkwaCode(string code) =>
            _transactions.FirstOrDefault(t => t.IerahkwaCode == code);

        /// <summary>
        /// Buscar por código original
        /// </summary>
        public static VipTransactionRecord? GetByOriginalCode(string code) =>
            _transactions.FirstOrDefault(t => t.OriginalCode == code);

        /// <summary>
        /// Filtrar por tipo
        /// </summary>
        public static List<VipTransactionRecord> GetByType(string type) =>
            _transactions.Where(t => t.TransactionType == type).ToList();

        /// <summary>
        /// Filtrar por banco
        /// </summary>
        public static List<VipTransactionRecord> GetByBank(string bankCode) =>
            _transactions.Where(t => t.BankCode == bankCode).ToList();

        /// <summary>
        /// Filtrar por estado
        /// </summary>
        public static List<VipTransactionRecord> GetByStatus(string status) =>
            _transactions.Where(t => t.Status == status).ToList();

        /// <summary>
        /// Actualizar transacción
        /// </summary>
        public static VipTransactionRecord? Update(int id, VipTransactionRecord updated)
        {
            var existing = _transactions.FirstOrDefault(t => t.Id == id);
            if (existing == null) return null;

            existing.Name = updated.Name ?? existing.Name;
            existing.Description = updated.Description ?? existing.Description;
            existing.Status = updated.Status ?? existing.Status;
            existing.CompletionPercentage = updated.CompletionPercentage;
            existing.Amount = updated.Amount ?? existing.Amount;
            existing.OriginalCode = updated.OriginalCode ?? existing.OriginalCode;
            existing.UETR = updated.UETR ?? existing.UETR;
            existing.UpdatedAt = DateTime.UtcNow;

            return existing;
        }

        /// <summary>
        /// Obtener estadísticas
        /// </summary>
        public static TransactionStats GetStats() => new()
        {
            Total = _transactions.Count,
            Incoming = _transactions.Count(t => t.TransactionType == "INCOMING"),
            Local = _transactions.Count(t => t.TransactionType == "LOCAL"),
            Outgoing = _transactions.Count(t => t.TransactionType == "OUTGOING"),
            Pending = _transactions.Count(t => t.Status == "Pending"),
            InProgress = _transactions.Count(t => t.Status == "In Progress"),
            Completed = _transactions.Count(t => t.Status == "Completed"),
            TotalValue = _transactions.Where(t => t.Amount.HasValue).Sum(t => t.Amount!.Value),
            AverageCompletion = _transactions.Any() 
                ? (int)_transactions.Average(t => t.CompletionPercentage) 
                : 0,
            ByBank = _transactions.GroupBy(t => t.BankCode)
                .ToDictionary(g => g.Key ?? "Unknown", g => g.Count())
        };

        /// <summary>
        /// Inicializar con datos de ejemplo
        /// </summary>
        public static void Initialize()
        {
            if (_transactions.Any()) return;

            // ENTRANTES (código del emisor)
            Add(new VipTransactionRecord
            {
                TransactionType = "INCOMING",
                OriginalCode = "PENDING-TRN-MT103",
                Name = "IBAN TO IBAN MT103 CASH TRANSFER STP",
                Description = "Deutsche Bank MT103 - Extraer TRN del PDF",
                BankCode = "WAMPUM",
                Amount = 1_000_000_000M,
                Currency = "USD",
                Status = "In Progress",
                CompletionPercentage = 90,
                Priority = "Critical",
                FolderPath = "IBAN TO IBAN MT103 CASH TRANSFER STP"
            });

            Add(new VipTransactionRecord
            {
                TransactionType = "INCOMING",
                OriginalCode = "PENDING-TRN-CELOS-1B",
                Name = "STP MT103 CELOS INVEST - 1B",
                Description = "CELOS INVEST $1B - Extraer TRN de 16 PDFs",
                BankCode = "WAMPUM",
                Amount = 1_000_000_000M,
                Currency = "USD",
                Status = "In Progress",
                CompletionPercentage = 85,
                Priority = "Critical",
                FolderPath = "STP MT103/1B"
            });

            Add(new VipTransactionRecord
            {
                TransactionType = "INCOMING",
                OriginalCode = "PENDING-TRN-CELOS-5B",
                Name = "STP MT103 CELOS INVEST - 5B",
                Description = "CELOS INVEST $5B - Extraer TRN de 14 PDFs",
                BankCode = "WAMPUM",
                Amount = 5_000_000_000M,
                Currency = "USD",
                Status = "In Progress",
                CompletionPercentage = 85,
                Priority = "Critical",
                FolderPath = "STP MT103/5B"
            });

            Add(new VipTransactionRecord
            {
                TransactionType = "INCOMING",
                OriginalCode = "PENDING-SWIFT-UBS",
                Name = "INTERNATIONAL SWIFT ACKS",
                Description = "UBS London → Deutsche Bank",
                BankCode = "AGUILA",
                Status = "In Progress",
                CompletionPercentage = 80,
                Priority = "High",
                FolderPath = "INTERNATIONAL SWIFT ACKS"
            });

            Add(new VipTransactionRecord
            {
                TransactionType = "INCOMING",
                OriginalCode = "PENDING-BCV-REF",
                Name = "VENEZUELA BCV EUROCLEAR",
                Description = "Banco Central de Venezuela $4.2B",
                BankCode = "CONDOR",
                Amount = 4_200_000_000M,
                Currency = "USD",
                Status = "Pending",
                CompletionPercentage = 60,
                Priority = "High",
                FolderPath = "venezuea trasacion"
            });

            Add(new VipTransactionRecord
            {
                TransactionType = "INCOMING",
                OriginalCode = "PENDING-ISIN-CUSIP",
                Name = "BONOS HISTÓRICOS",
                Description = "1,632 documentos de bonos",
                BankCode = "KANATA",
                Status = "Pending",
                CompletionPercentage = 50,
                Priority = "Medium",
                FolderPath = "bonos historicos"
            });

            // LOCALES (código Ierahkwa)
            Add(new VipTransactionRecord
            {
                TransactionType = "LOCAL",
                IerahkwaCode = "OHWISTA-LOC-2601-0001",
                Name = "RUBÍ 3",
                Description = "Rubí - Piedra preciosa en custodia",
                BankCode = "WAMPUM",
                Status = "In Progress",
                CompletionPercentage = 75,
                Priority = "High",
                FolderPath = "rubi 3"
            });

            Add(new VipTransactionRecord
            {
                TransactionType = "LOCAL",
                IerahkwaCode = "OHWISTA-LOC-2601-0002",
                Name = "ALEXANDRITE",
                Description = "Alexandrita - Piedra preciosa Brasil",
                BankCode = "WAMPUM",
                Status = "Pending",
                CompletionPercentage = 40,
                Priority = "Medium",
                FolderPath = "Alexandrite"
            });
        }
    }

    /// <summary>
    /// Registro de transacción VIP
    /// </summary>
    public class VipTransactionRecord
    {
        public int Id { get; set; }
        public string? IerahkwaCode { get; set; }      // Código nuestro (local/saliente)
        public string? OriginalCode { get; set; }      // Código del emisor (entrante)
        public string? UETR { get; set; }              // SWIFT GPI UETR
        public string TransactionType { get; set; } = "INCOMING"; // INCOMING, LOCAL, OUTGOING
        public string? BankCode { get; set; }          // WAMPUM, AGUILA, CONDOR, etc.
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Status { get; set; } = "Pending";
        public int CompletionPercentage { get; set; }
        public string Priority { get; set; } = "Medium";
        public string? FolderPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Estadísticas de transacciones
    /// </summary>
    public class TransactionStats
    {
        public int Total { get; set; }
        public int Incoming { get; set; }
        public int Local { get; set; }
        public int Outgoing { get; set; }
        public int Pending { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public decimal TotalValue { get; set; }
        public int AverageCompletion { get; set; }
        public Dictionary<string, int> ByBank { get; set; } = new();
    }
}
